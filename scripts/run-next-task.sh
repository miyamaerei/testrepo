#!/bin/bash
# E-Kanban 自动化开发任务执行脚本
# 用于 GitHub Copilot CLI 编程模式自动执行开发任务

# 获取当前目录
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
cd "$PROJECT_ROOT" || exit 1

# 读取 implementation-plan.md 找到第一个待执行任务
get_next_task() {
    grep -n "⏱️ 待执行" docs/implementation-plan.md | head -1 | cut -d: -f1
}

# 获取任务文件路径
get_task_file() {
    local line_num=$1
    # 从 implementation-plan.md 中提取任务编号
    # 格式如 "1.1 | 创建 E-Kanban 领域实体"
    local task_line=$(sed -n "${line_num}p" docs/implementation-plan.md)
    local task_id=$(echo "$task_line" | cut -d'|' -f1 | xargs)
    echo "tasks/${task_id}-*.md"
}

# 执行当前任务
run_current_task() {
    local task_file=$(get_next_task 1)
    if [ ! -f $task_file ]; then
        echo "没有找到待执行的任务文件: $task_file"
        exit 1
    fi
    
    echo "=========================================="
    echo "开始执行任务: $task_file"
    echo "=========================================="
    
    # 读取任务内容作为 prompt
    local task_content=$(cat "$task_file")
    
    # 使用 GitHub Copilot CLI 编程模式执行
    copilot -p "$task_content" --allow-all-tools
}

# 显示当前任务
show_current_task() {
    echo "当前待执行任务："
    grep "⏱️ 待执行" docs/implementation-plan.md | head -5
}

# 主逻辑
case "$1" in
    "next")
        run_current_task
        ;;
    "list")
        show_current_task
        ;;
    *)
        echo "Usage: ./run-next-task.sh [next|list]"
        echo ""
        echo "next   - 执行下一个待执行任务"
        echo "list   - 列出待执行任务"
        exit 1
        ;;
esac
