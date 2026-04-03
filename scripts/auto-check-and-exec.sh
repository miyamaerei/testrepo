#!/bin/bash
# E-Kanban 自动开发检查脚本
# 每 10 分钟运行一次，检查是否有：
# 1. 待执行的任务 → 如果有且没有 InProgress 任务，自动执行下一个
# 2. 超时的 InProgress AI 任务 → 触发重新执行

set -e

# 获取项目根目录
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
cd "$PROJECT_ROOT" || exit 1

# 日志函数
log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1"
}

log "=== E-Kanban 自动开发检查开始 ==="

# 1. 检查有多少待执行任务
PENDING_TASKS=$(grep -c "⏱️ 待执行" docs/implementation-plan.md)
COMPLETED_TASKS=$(grep -c "✅ 完成" docs/implementation-plan.md)
TOTAL_TASKS=23

log "当前任务统计: 待执行=$PENDING_TASKS, 已完成=$COMPLETED_TASKS, 总计=$TOTAL_TASKS"

# 2. 如果所有任务都完成了
if [ "$PENDING_TASKS" -eq 0 ]; then
    log "🎉 所有任务都已完成！检查结束。"
    exit 0
fi

# 3. 获取第一个待执行任务
FIRST_PENDING_LINE=$(grep -n "⏱️ 待执行" docs/implementation-plan.md | head -1)
LINE_NUM=$(echo "$FIRST_PENDING_LINE" | cut -d: -f1)
TASK_LINE=$(echo "$FIRST_PENDING_LINE" | cut -d: -f2-)
TASK_ID=$(echo "$TASK_LINE" | cut -d'|' -f1 | xargs)

log "找到第一个待执行任务: $TASK_ID"

# 4. 找到对应的任务文件
TASK_FILE=$(find "$PROJECT_ROOT/tasks" -name "$TASK_ID*.md")

if [ ! -f "$TASK_FILE" ]; then
    log "❌ 找不到任务文件: $TASK_ID*.md"
    exit 1
fi

log "任务文件位置: $TASK_FILE"

# 5. 检查是否存在 copilot 命令
if ! command -v copilot &> /dev/null; then
    log "❌ copilot 命令不可用，无法执行任务"
    exit 1
fi

log "✅ copilot CLI 可用，开始执行任务 $TASK_ID"

# 6. 执行任务
log "正在执行 GitHub Copilot CLI... (任务文件: $TASK_FILE)"
copilot -p "$(cat "$TASK_FILE")" --allow-all-tools

EXIT_CODE=$?
if [ $EXIT_CODE -eq 0 ]; then
    log "✅ 任务 $TASK_ID 执行完成"
else
    log "⚠️  copilot 执行返回非零退出码: $EXIT_CODE"
fi

log "=== E-Kanban 自动开发检查结束 ==="

# 输出新的进度统计
PENDING_AFTER=$(grep -c "⏱️ 待执行" docs/implementation-plan.md)
COMPLETED_AFTER=$(grep -c "✅ 完成" docs/implementation-plan.md)
log "执行后统计: 待执行=$PENDING_AFTER, 已完成=$COMPLETED_AFTER"

exit 0
