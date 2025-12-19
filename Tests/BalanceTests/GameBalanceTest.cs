using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Tests.BalanceTests
{
    /// <summary>
    /// 游戏平衡性测试框架
    /// 测试副本难度、掉落率、协作Buff、组队奖励等
    /// </summary>
    public class GameBalanceTest
    {
        private BalanceConfig config;
        private TestResults results = new TestResults();

        public GameBalanceTest()
        {
            config = new BalanceConfig();
        }

        /// <summary>
        /// 运行所有平衡性测试
        /// </summary>
        public BalanceTestResult RunAllTests()
        {
            Console.WriteLine("=== 开始游戏平衡性测试 ===\n");

            TestDropRates();
            TestCooperationBuffs();
            TestTeamRewards();
            TestInstanceDifficulty();

            return GenerateReport();
        }

        /// <summary>
        /// 测试掉落率平衡
        /// </summary>
        private void TestDropRates()
        {
            Console.WriteLine("测试掉落率平衡...");

            // 单人掉落测试
            var soloDrops = SimulateDrops(1000, 1);
            var soloDropRate = (float)soloDrops.Count / 1000;

            // 双人掉落测试（目标：单人 x 1.5）
            var duoDrops = SimulateDrops(1000, 2);
            var duoDropRate = (float)duoDrops.Count / 1000;

            results.SoloDropRate = soloDropRate;
            results.DuoDropRate = duoDropRate;
            results.DropRateRatio = duoDropRate / soloDropRate;

            Console.WriteLine($"  单人掉落率: {soloDropRate:P2}");
            Console.WriteLine($"  双人掉落率: {duoDropRate:P2}");
            Console.WriteLine($"  双人/单人比率: {results.DropRateRatio:F2}x (目标: 1.5x)");
            
            bool passed = Math.Abs(results.DropRateRatio - 1.5f) < 0.1f;
            Console.WriteLine($"  结果: {(passed ? "✓ 通过" : "✗ 需要调整")}\n");
        }

        /// <summary>
        /// 测试协作Buff效果
        /// </summary>
        private void TestCooperationBuffs()
        {
            Console.WriteLine("测试协作Buff效果...");

            // 测试距离≤5格时的Buff触发
            var buffTests = new List<BuffTestCase>
            {
                new BuffTestCase { Distance = 3, ExpectBuff = true },
                new BuffTestCase { Distance = 5, ExpectBuff = true },
                new BuffTestCase { Distance = 6, ExpectBuff = false },
                new BuffTestCase { Distance = 10, ExpectBuff = false }
            };

            int passedTests = 0;
            foreach (var test in buffTests)
            {
                bool buffActive = CheckCooperationBuff(test.Distance);
                bool passed = buffActive == test.ExpectBuff;
                
                if (passed) passedTests++;
                
                Console.WriteLine($"  距离 {test.Distance} 格: Buff {(buffActive ? "激活" : "未激活")} " +
                                $"{(passed ? "✓" : "✗")}");
            }

            results.BuffTestPassRate = (float)passedTests / buffTests.Count;
            Console.WriteLine($"  Buff触发测试通过率: {results.BuffTestPassRate:P2}\n");
        }

        /// <summary>
        /// 测试组队奖励合理性
        /// </summary>
        private void TestTeamRewards()
        {
            Console.WriteLine("测试组队奖励平衡...");

            // 测试经验奖励
            int soloExp = CalculateReward(100, 1);
            int duoExp = CalculateReward(100, 2);
            
            float expBonus = ((float)duoExp / soloExp - 1) * 100;

            results.SoloExpReward = soloExp;
            results.DuoExpReward = duoExp;
            results.TeamExpBonus = expBonus;

            Console.WriteLine($"  单人经验: {soloExp}");
            Console.WriteLine($"  双人经验: {duoExp}");
            Console.WriteLine($"  组队加成: +{expBonus:F1}% (目标: +20-30%)");

            bool passed = expBonus >= 20 && expBonus <= 30;
            Console.WriteLine($"  结果: {(passed ? "✓ 通过" : "✗ 需要调整")}\n");
        }

        /// <summary>
        /// 测试副本难度平衡
        /// </summary>
        private void TestInstanceDifficulty()
        {
            Console.WriteLine("测试副本难度平衡...");

            var difficulties = new[] { "简单", "普通", "困难", "噩梦" };
            var targetClearRates = new[] { 0.90f, 0.70f, 0.40f, 0.15f };

            for (int i = 0; i < difficulties.Length; i++)
            {
                float clearRate = SimulateInstanceClearRate(i);
                float target = targetClearRates[i];
                bool balanced = Math.Abs(clearRate - target) < 0.10f;

                Console.WriteLine($"  {difficulties[i]}: 通关率 {clearRate:P2} " +
                                $"(目标: {target:P2}) {(balanced ? "✓" : "✗")}");

                results.InstanceDifficulties.Add(new DifficultyResult
                {
                    Level = difficulties[i],
                    ClearRate = clearRate,
                    Balanced = balanced
                });
            }
            Console.WriteLine();
        }

        /// <summary>
        /// 模拟掉落
        /// </summary>
        private List<Item> SimulateDrops(int attempts, int playerCount)
        {
            var drops = new List<Item>();
            var random = new Random();

            float baseDropRate = config.BaseDropRate;
            float teamMultiplier = playerCount == 2 ? config.DuoDropMultiplier : 1.0f;
            float finalDropRate = baseDropRate * teamMultiplier;

            for (int i = 0; i < attempts; i++)
            {
                if (random.NextDouble() < finalDropRate)
                {
                    drops.Add(new Item { Id = i, Rarity = "Common" });
                }
            }

            return drops;
        }

        /// <summary>
        /// 检查协作Buff
        /// </summary>
        private bool CheckCooperationBuff(int distance)
        {
            return distance <= config.CooperationBuffDistance;
        }

        /// <summary>
        /// 计算奖励
        /// </summary>
        private int CalculateReward(int baseReward, int playerCount)
        {
            float teamBonus = playerCount == 2 ? config.DuoExpBonus : 0f;
            return (int)(baseReward * (1 + teamBonus));
        }

        /// <summary>
        /// 模拟副本通关率
        /// </summary>
        private float SimulateInstanceClearRate(int difficultyLevel)
        {
            var random = new Random();
            int clears = 0;
            int attempts = 1000;

            // 难度越高，通关率越低
            float baseClearRate = 0.95f - (difficultyLevel * 0.25f);

            for (int i = 0; i < attempts; i++)
            {
                if (random.NextDouble() < baseClearRate)
                {
                    clears++;
                }
            }

            return (float)clears / attempts;
        }

        /// <summary>
        /// 生成测试报告
        /// </summary>
        private BalanceTestResult GenerateReport()
        {
            Console.WriteLine("=== 平衡性测试总结 ===");

            bool allPassed = 
                Math.Abs(results.DropRateRatio - 1.5f) < 0.1f &&
                results.BuffTestPassRate >= 0.9f &&
                results.TeamExpBonus >= 20 && results.TeamExpBonus <= 30 &&
                results.InstanceDifficulties.All(d => d.Balanced);

            Console.WriteLine($"总体结果: {(allPassed ? "✓ 平衡性良好" : "⚠ 需要调整")}");
            Console.WriteLine($"详细报告已生成\n");

            return new BalanceTestResult
            {
                Results = results,
                Passed = allPassed
            };
        }
    }

    /// <summary>
    /// 平衡配置
    /// </summary>
    public class BalanceConfig
    {
        public float BaseDropRate { get; set; } = 0.10f;
        public float DuoDropMultiplier { get; set; } = 1.5f;
        public float DuoExpBonus { get; set; } = 0.25f; // 25%
        public int CooperationBuffDistance { get; set; } = 5;
    }

    /// <summary>
    /// 测试结果
    /// </summary>
    public class TestResults
    {
        public float SoloDropRate { get; set; }
        public float DuoDropRate { get; set; }
        public float DropRateRatio { get; set; }
        public float BuffTestPassRate { get; set; }
        public int SoloExpReward { get; set; }
        public int DuoExpReward { get; set; }
        public float TeamExpBonus { get; set; }
        public List<DifficultyResult> InstanceDifficulties { get; set; } = new List<DifficultyResult>();
    }

    public class BuffTestCase
    {
        public int Distance { get; set; }
        public bool ExpectBuff { get; set; }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Rarity { get; set; }
    }

    public class DifficultyResult
    {
        public string Level { get; set; }
        public float ClearRate { get; set; }
        public bool Balanced { get; set; }
    }

    public class BalanceTestResult
    {
        public TestResults Results { get; set; }
        public bool Passed { get; set; }
    }
}
