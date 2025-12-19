using System;
using System.Threading.Tasks;
using Game.Tests.StressTests;
using Game.Tests.BalanceTests;
using Game.Tests.ExperienceTests;

namespace Game.Tests
{
    /// <summary>
    /// 综合测试运行器
    /// 运行所有测试并生成完整报告
    /// </summary>
    public class TestRunner
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  游戏测试与优化 - 综合测试套件                              ║");
            Console.WriteLine("║  测试目标：性能、平衡与体验提升                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

            var startTime = DateTime.Now;
            bool allTestsPassed = true;

            // 1. 网络压力测试
            Console.WriteLine("【1/3】网络压力与稳定性测试");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            var stressTest = new NetworkStressTest();
            var stressResult = await stressTest.RunStressTest();
            allTestsPassed &= stressResult.Passed;
            Console.WriteLine();

            // 2. 平衡性测试
            Console.WriteLine("【2/3】游戏平衡性测试");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            var balanceTest = new GameBalanceTest();
            var balanceResult = balanceTest.RunAllTests();
            allTestsPassed &= balanceResult.Passed;
            Console.WriteLine();

            // 3. 用户体验测试
            Console.WriteLine("【3/3】用户体验优化测试");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            var uxTest = new UserExperienceTest();
            var uxResult = uxTest.RunAllTests();
            allTestsPassed &= uxResult.Passed;
            Console.WriteLine();

            // 生成最终报告
            var endTime = DateTime.Now;
            var duration = endTime - startTime;

            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      最终测试报告                           ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"测试开始时间: {startTime:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"测试结束时间: {endTime:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"总计用时: {duration.TotalSeconds:F2} 秒");
            Console.WriteLine();

            // 详细结果
            Console.WriteLine("详细测试结果:");
            Console.WriteLine("─────────────────────────────────────────────────────────────");
            
            Console.WriteLine($"1. 网络压力测试: {(stressResult.Passed ? "✓ 通过" : "✗ 未通过")}");
            Console.WriteLine($"   - 连接成功率: {stressResult.Metrics.ConnectionSuccessRate:P2}");
            Console.WriteLine($"   - 平均延迟: {stressResult.Metrics.AverageLatencyMs}ms");
            Console.WriteLine($"   - 重连成功率: {stressResult.Metrics.ReconnectSuccessRate:P2}");
            Console.WriteLine();

            Console.WriteLine($"2. 平衡性测试: {(balanceResult.Passed ? "✓ 通过" : "✗ 未通过")}");
            Console.WriteLine($"   - 掉落率比例: {balanceResult.Results.DropRateRatio:F2}x");
            Console.WriteLine($"   - Buff测试通过率: {balanceResult.Results.BuffTestPassRate:P2}");
            Console.WriteLine($"   - 组队经验加成: +{balanceResult.Results.TeamExpBonus:F1}%");
            Console.WriteLine();

            Console.WriteLine($"3. 用户体验测试: {(uxResult.Passed ? "✓ 通过" : "✗ 未通过")}");
            Console.WriteLine($"   - 综合评分: {uxResult.Metrics.OverallScore:P2}");
            Console.WriteLine($"   - 流程效率: {uxResult.Metrics.WorkflowEfficiency:P2}");
            Console.WriteLine($"   - 邀请系统: {uxResult.Metrics.InviteSystemScore:P2}");
            Console.WriteLine();

            Console.WriteLine("─────────────────────────────────────────────────────────────");
            Console.WriteLine($"总体测试结果: {(allTestsPassed ? "✓✓✓ 全部通过 ✓✓✓" : "⚠⚠⚠ 需要优化 ⚠⚠⚠")}");
            Console.WriteLine();

            // 建议
            if (!allTestsPassed)
            {
                Console.WriteLine("优化建议:");
                Console.WriteLine("─────────────────────────────────────────────────────────────");
                
                if (!stressResult.Passed)
                {
                    Console.WriteLine("• 网络性能需要优化:");
                    if (stressResult.Metrics.AverageLatencyMs > 200)
                        Console.WriteLine("  - 考虑使用UDP进行位置同步以降低延迟");
                    if (stressResult.Metrics.ReconnectSuccessRate < 0.95f)
                        Console.WriteLine("  - 增强断线重连机制，增加重试次数");
                    if (stressResult.Metrics.ConnectionSuccessRate < 0.95f)
                        Console.WriteLine("  - 优化服务器连接池配置");
                }

                if (!balanceResult.Passed)
                {
                    Console.WriteLine("• 游戏平衡性需要调整:");
                    if (Math.Abs(balanceResult.Results.DropRateRatio - 1.5f) > 0.1f)
                        Console.WriteLine("  - 调整双人掉落倍率，使其更接近1.5倍");
                    if (balanceResult.Results.TeamExpBonus < 20 || balanceResult.Results.TeamExpBonus > 30)
                        Console.WriteLine("  - 调整组队经验加成至20-30%范围内");
                }

                if (!uxResult.Passed)
                {
                    Console.WriteLine("• 用户体验需要改进:");
                    if (uxResult.Metrics.WorkflowEfficiency < 0.8f)
                        Console.WriteLine("  - 简化操作流程，减少步骤");
                    if (uxResult.Metrics.InviteSystemScore < 0.8f)
                        Console.WriteLine("  - 完善一键邀请功能");
                }
                Console.WriteLine();
            }

            // 核心目标达成情况
            Console.WriteLine("核心目标达成情况:");
            Console.WriteLine("─────────────────────────────────────────────────────────────");
            Console.WriteLine($"✓ 200人并发测试: {(stressResult.Metrics.ConnectionSuccessRate >= 0.95f ? "达成" : "未达成")}");
            Console.WriteLine($"✓ 延迟≤200ms: {(stressResult.Metrics.AverageLatencyMs <= 200 ? "达成" : "未达成")}");
            Console.WriteLine($"✓ 重连≥95%: {(stressResult.Metrics.ReconnectSuccessRate >= 0.95f ? "达成" : "未达成")}");
            Console.WriteLine($"✓ 平衡性合理: {(balanceResult.Passed ? "达成" : "未达成")}");
            Console.WriteLine($"✓ 体验优化: {(uxResult.Passed ? "达成" : "未达成")}");
            Console.WriteLine();

            Console.WriteLine("测试完成！");
            Console.WriteLine("════════════════════════════════════════════════════════════");
        }
    }
}
