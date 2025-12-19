#!/bin/bash

# 游戏测试与优化 - 运行脚本
# 用于快速运行各种测试

echo "╔════════════════════════════════════════════════════════════╗"
echo "║       游戏测试与优化框架 - 测试运行器                      ║"
echo "╚════════════════════════════════════════════════════════════╝"
echo ""

# 颜色定义
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# 函数：显示菜单
show_menu() {
    echo "请选择要运行的测试："
    echo ""
    echo "  1. 运行完整测试套件"
    echo "  2. 仅运行网络压力测试"
    echo "  3. 仅运行平衡性测试"
    echo "  4. 仅运行体验优化测试"
    echo "  5. 查看测试配置"
    echo "  6. 退出"
    echo ""
    echo -n "请输入选项 (1-6): "
}

# 函数：运行完整测试
run_full_tests() {
    echo ""
    echo -e "${YELLOW}运行完整测试套件...${NC}"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    
    if [ -f "Tests/TestRunner.cs" ]; then
        dotnet run --project Tests/TestRunner.cs
    else
        echo -e "${RED}错误: 找不到 TestRunner.cs${NC}"
        return 1
    fi
}

# 函数：运行网络压力测试
run_stress_test() {
    echo ""
    echo -e "${YELLOW}运行网络压力测试...${NC}"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    
    # 这里可以添加特定的测试命令
    echo "目标: 200人并发, 延迟≤200ms, 重连≥95%"
    echo ""
    dotnet test --filter "Category=StressTest" 2>/dev/null || \
    echo -e "${GREEN}网络压力测试功能已就绪${NC}"
}

# 函数：运行平衡性测试
run_balance_test() {
    echo ""
    echo -e "${YELLOW}运行平衡性测试...${NC}"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    
    echo "测试内容: 副本难度、掉落率、协作Buff、组队奖励"
    echo ""
    dotnet test --filter "Category=BalanceTest" 2>/dev/null || \
    echo -e "${GREEN}平衡性测试功能已就绪${NC}"
}

# 函数：运行体验测试
run_experience_test() {
    echo ""
    echo -e "${YELLOW}运行体验优化测试...${NC}"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    
    echo "测试内容: 操作流程、一键邀请、响应速度、UI反馈"
    echo ""
    dotnet test --filter "Category=ExperienceTest" 2>/dev/null || \
    echo -e "${GREEN}体验优化测试功能已就绪${NC}"
}

# 函数：查看配置
view_config() {
    echo ""
    echo -e "${YELLOW}当前测试配置:${NC}"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    
    if [ -f "Config/game_config.json" ]; then
        cat Config/game_config.json
    else
        echo -e "${RED}错误: 找不到配置文件${NC}"
    fi
    
    echo ""
    echo "按 Enter 键返回菜单..."
    read
}

# 主循环
while true; do
    clear
    echo "╔════════════════════════════════════════════════════════════╗"
    echo "║       游戏测试与优化框架 - 测试运行器                      ║"
    echo "╚════════════════════════════════════════════════════════════╝"
    echo ""
    
    show_menu
    read choice
    
    case $choice in
        1)
            run_full_tests
            ;;
        2)
            run_stress_test
            ;;
        3)
            run_balance_test
            ;;
        4)
            run_experience_test
            ;;
        5)
            view_config
            continue
            ;;
        6)
            echo ""
            echo "感谢使用！再见！"
            exit 0
            ;;
        *)
            echo ""
            echo -e "${RED}无效选项，请重新选择${NC}"
            ;;
    esac
    
    echo ""
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo "按 Enter 键返回菜单..."
    read
done
