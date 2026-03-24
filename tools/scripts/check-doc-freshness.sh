#!/usr/bin/env bash
# 检查文档是否标注了"最后评审"日期
# 用法：bash tools/scripts/check-doc-freshness.sh

set -e

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
WARNINGS=0

echo "正在检查文档新鲜度..."

find "$REPO_ROOT/docs" -name "*.md" | while read -r file; do
  if ! grep -q "最后评审" "$file" 2>/dev/null; then
    echo "⚠️  缺少评审日期：$file"
    WARNINGS=$((WARNINGS + 1))
  fi
done

echo "文档新鲜度检查完成（$WARNINGS 个文件缺少评审日期）"
