#!/usr/bin/env bash
# 检查 PR 规模是否在合理范围内
# 用法：bash tools/scripts/check-pr-size.sh

MAX_LINES=500
MAX_FILES=15

CHANGED_LINES=$(git diff --stat HEAD~1 HEAD 2>/dev/null | tail -1 | grep -oE '[0-9]+ insertion' | grep -oE '[0-9]+' || echo "0")
CHANGED_FILES=$(git diff --name-only HEAD~1 HEAD 2>/dev/null | wc -l || echo "0")

echo "PR 规模统计："
echo "  改动行数：$CHANGED_LINES（建议 <$MAX_LINES）"
echo "  改动文件数：$CHANGED_FILES（建议 <$MAX_FILES）"

if [ "$CHANGED_LINES" -gt "$MAX_LINES" ] || [ "$CHANGED_FILES" -gt "$MAX_FILES" ]; then
  echo "⚠️  PR 规模超出建议范围，请在 PR 描述中说明原因"
else
  echo "✅ PR 规模在合理范围内"
fi
