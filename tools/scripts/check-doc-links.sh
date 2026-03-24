#!/usr/bin/env bash
# 检查 Markdown 文档中的内部链接是否有效
# 用法：bash tools/scripts/check-doc-links.sh

set -e

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
ERRORS=0

echo "正在检查文档链接..."

find "$REPO_ROOT" -name "*.md" -not -path "*/node_modules/*" | while read -r file; do
  # 提取 Markdown 中的本地链接
  grep -oE '\[([^\]]+)\]\(([^)]+)\)' "$file" | grep -oE '\(([^)]+)\)' | tr -d '()' | while read -r link; do
    # 跳过 HTTP(S) 链接和锚点链接
    if [[ "$link" =~ ^https?:// ]] || [[ "$link" =~ ^# ]]; then
      continue
    fi
    # 处理相对路径
    dir=$(dirname "$file")
    target="$dir/$link"
    if [ ! -e "$target" ]; then
      echo "❌ 失效链接：$file -> $link"
      ERRORS=$((ERRORS + 1))
    fi
  done
done

if [ "$ERRORS" -gt 0 ]; then
  echo "发现 $ERRORS 个失效链接"
  exit 1
else
  echo "✅ 所有文档链接有效"
fi
