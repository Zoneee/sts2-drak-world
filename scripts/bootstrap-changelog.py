#!/usr/bin/env python3
"""
bootstrap-changelog.py  ─  One-time tool to generate CHANGELOG.md from exec-plans.

Run from repository root:
    python3 scripts/bootstrap-changelog.py

WARNING: Overwrites CHANGELOG.md.  Review and hand-edit the output before publishing.
The <!-- DRAFT --> marker at the top of the output is intentional — remove it after review.
"""

import re
from pathlib import Path
from collections import defaultdict

REPO_ROOT  = Path(__file__).parent.parent
PLANS_DIR  = REPO_ROOT / "docs" / "exec-plans" / "completed"
OUT_PATH   = REPO_ROOT / "CHANGELOG.md"

# File date prefix  →  (version, canonical date for that version header)
DATE_VERSION: dict[str, tuple[str, str]] = {
    "2026-03-24": ("v1.0.0", "2026-03-24"),
    "2026-03-25": ("v1.0.0", "2026-03-24"),   # grouped into v1.0.0
    "2026-03-26": ("v1.1.0", "2026-03-26"),
    "2026-03-27": ("v1.2.0", "2026-03-27"),
    "2026-03-28": ("v1.2.1", "2026-03-28"),
}

VERSION_ORDER = ["v1.2.1", "v1.2.0", "v1.1.0", "v1.0.0"]

VERSION_DATES: dict[str, str] = {
    "v1.0.0": "2026-03-24",
    "v1.1.0": "2026-03-26",
    "v1.2.0": "2026-03-27",
    "v1.2.1": "2026-03-28",
}


def extract_section_bullets(text: str, section: str) -> list[str]:
    """Return stripped '- ...' lines under a given ## section."""
    result: list[str] = []
    in_section = False
    for line in text.splitlines():
        if re.match(rf"^##\s+{re.escape(section)}\s*$", line):
            in_section = True
            continue
        if in_section:
            if re.match(r"^##\s+", line):
                break
            if line.strip().startswith("- "):
                result.append(line.strip())
    return result


def extract_section_subheadings(text: str, section: str) -> list[str]:
    """Return '### Title' lines under a ## section, converted to bullet items."""
    result: list[str] = []
    in_section = False
    for line in text.splitlines():
        if re.match(rf"^##\s+{re.escape(section)}\s*$", line):
            in_section = True
            continue
        if in_section:
            if re.match(r"^##\s+", line):
                break
            m = re.match(r"^###\s+(.+)", line)
            if m:
                result.append(f"- {m.group(1).strip()}")
    return result


def get_title(text: str, fallback: str) -> str:
    """Try '## 标题' section body, then first '# ' heading."""
    lines = text.splitlines()
    after_title = False
    for line in lines:
        if re.match(r"^##\s+标题\s*$", line):
            after_title = True
            continue
        if after_title:
            if line.strip() and not line.startswith("#"):
                return line.strip()
            if line.startswith("#"):
                break
    for line in lines:
        if re.match(r"^#\s+", line):
            return line.lstrip("# ").strip()
    return fallback


def collect_entries(path: Path) -> list[str]:
    """Extract most useful changelog lines from an exec-plan file (draft quality)."""
    text = path.read_text(encoding="utf-8")

    # Priority 1: 进展日志 bullets (strip leading date prefix)
    entries = extract_section_bullets(text, "进展日志")
    if entries:
        return [re.sub(r"^-\s+\d{4}-\d{2}-\d{2}[：:]\s*", "- ", e) for e in entries]

    # Priority 2: Bug 清单 subheadings
    entries = extract_section_subheadings(text, "Bug 清单")
    if entries:
        return entries

    # Priority 3: 验证结果 bullets
    entries = extract_section_bullets(text, "验证结果")
    if entries:
        return [re.sub(r"^-\s+已(完成|确认|验证)\w*[：:]\s*", "- ", e) for e in entries]

    # Priority 4: 范围 bullets (capped to avoid noise)
    entries = extract_section_bullets(text, "范围")
    return entries[:8]


def main() -> None:
    plan_files = sorted(
        p for p in PLANS_DIR.glob("*.md") if p.name != ".gitkeep"
    )

    # version → [(plan_title, [entry_lines])]
    data: dict[str, list[tuple[str, list[str]]]] = defaultdict(list)

    for plan in plan_files:
        m = re.match(r"^(\d{4}-\d{2}-\d{2})", plan.name)
        if not m or m.group(1) not in DATE_VERSION:
            continue
        version, _ = DATE_VERSION[m.group(1)]
        text    = plan.read_text(encoding="utf-8")
        title   = get_title(text, plan.stem)
        entries = collect_entries(plan)
        if entries:
            data[version].append((title, entries))

    header = [
        "<!-- DRAFT — 请开发者校对后移除此行 -->",
        "<!--",
        "  格式规范：",
        "    ## vX.Y.Z — YYYY-MM-DD",
        "    ### 新增 / 调整与平衡 / 修复 / 已知问题（可选）",
        "    - **卡牌中文名**（英文 ID）：说明",
        "",
        "  运行 python3 scripts/bootstrap-changelog.py 可重新生成草稿",
        "  （会覆盖现有文件，请注意备份手动编辑的内容）",
        "-->",
        "",
        "# CHANGELOG",
        "",
    ]

    body: list[str] = []
    for ver in VERSION_ORDER:
        date = VERSION_DATES[ver]
        body.append(f"## {ver} — {date}")
        body.append("")
        if ver in data:
            for title, entries in data[ver]:
                body.append(f"<!-- 来源：{title} -->")
                for entry in entries:
                    body.append(entry)
                body.append("")
        else:
            body.append("<!-- TODO: 填写本版本变更 -->")
            body.append("")

    OUT_PATH.write_text("\n".join(header + body), encoding="utf-8")
    print(f"✓  Generated {OUT_PATH.relative_to(REPO_ROOT)}")
    print("   Review the draft above, then remove the <!-- DRAFT --> first line.")


if __name__ == "__main__":
    main()
