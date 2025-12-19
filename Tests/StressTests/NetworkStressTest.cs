using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Game.Tests.StressTests
{
    /// <summary>
    /// 网络压力测试框架
    /// 目标：模拟200人并发，延迟≤200ms，断线重连≥95%
    /// </summary>
    public class NetworkStressTest
    {
        private const int TARGET_CONCURRENT_USERS = 200;
        private const int MAX_LATENCY_MS = 200;
        private const float MIN_RECONNECT_RATE = 0.95f;

        private List<SimulatedClient> clients = new List<SimulatedClient>();
        private TestMetrics metrics = new TestMetrics();

        /// <summary>
        /// 运行完整的网络压力测试
        /// </summary>
        public async Task<TestResult> RunStressTest()
        {
            Console.WriteLine($"开始网络压力测试 - 目标并发用户数: {TARGET_CONCURRENT_USERS}");
            
            // 1. 初始化客户端
            await InitializeClients(TARGET_CONCURRENT_USERS);
            
            // 2. 连接测试
            await TestConnectionStability();
            
            // 3. 延迟测试
            await TestLatency();
            
            // 4. 断线重连测试
            await TestReconnection();
            
            // 5. 生成测试报告
            return GenerateTestReport();
        }

        /// <summary>
        /// 初始化模拟客户端
        /// </summary>
        private async Task InitializeClients(int count)
        {
            Console.WriteLine($"初始化 {count} 个模拟客户端...");
            
            for (int i = 0; i < count; i++)
            {
                var client = new SimulatedClient(i);
                clients.Add(client);
                
                // 模拟逐步连接，避免瞬时峰值
                if (i % 10 == 0)
                {
                    await Task.Delay(100);
                }
            }
            
            Console.WriteLine("客户端初始化完成");
        }

        /// <summary>
        /// 测试连接稳定性
        /// </summary>
        private async Task TestConnectionStability()
        {
            Console.WriteLine("测试连接稳定性...");
            
            var tasks = new List<Task>();
            foreach (var client in clients)
            {
                tasks.Add(client.Connect());
            }
            
            await Task.WhenAll(tasks);
            
            int connectedCount = clients.FindAll(c => c.IsConnected).Count;
            metrics.ConnectionSuccessRate = (float)connectedCount / clients.Count;
            
            Console.WriteLine($"连接成功率: {metrics.ConnectionSuccessRate:P2} ({connectedCount}/{clients.Count})");
        }

        /// <summary>
        /// 测试网络延迟
        /// </summary>
        private async Task TestLatency()
        {
            Console.WriteLine("测试网络延迟...");
            
            var latencies = new List<long>();
            
            for (int round = 0; round < 10; round++)
            {
                foreach (var client in clients)
                {
                    if (client.IsConnected)
                    {
                        long latency = await client.MeasureLatency();
                        latencies.Add(latency);
                    }
                }
                
                await Task.Delay(1000); // 每轮间隔1秒
            }
            
            if (latencies.Count > 0)
            {
                metrics.AverageLatencyMs = latencies.Sum() / latencies.Count;
                metrics.MaxLatencyMs = latencies.Max();
                metrics.MinLatencyMs = latencies.Min();
                
                int acceptableLatencyCount = latencies.FindAll(l => l <= MAX_LATENCY_MS).Count;
                metrics.AcceptableLatencyRate = (float)acceptableLatencyCount / latencies.Count;
            }
            
            Console.WriteLine($"平均延迟: {metrics.AverageLatencyMs}ms");
            Console.WriteLine($"延迟范围: {metrics.MinLatencyMs}ms - {metrics.MaxLatencyMs}ms");
            Console.WriteLine($"符合要求(≤{MAX_LATENCY_MS}ms)的请求占比: {metrics.AcceptableLatencyRate:P2}");
        }

        /// <summary>
        /// 测试断线重连
        /// </summary>
        private async Task TestReconnection()
        {
            Console.WriteLine("测试断线重连...");
            
            // 随机断开30%的客户端连接
            var random = new Random();
            var disconnectTargets = new List<SimulatedClient>();
            
            foreach (var client in clients)
            {
                if (client.IsConnected && random.NextDouble() < 0.3)
                {
                    disconnectTargets.Add(client);
                }
            }
            
            Console.WriteLine($"模拟断开 {disconnectTargets.Count} 个客户端连接...");
            
            // 断开连接
            foreach (var client in disconnectTargets)
            {
                await client.Disconnect();
            }
            
            // 等待一段时间
            await Task.Delay(2000);
            
            // 尝试重连
            Console.WriteLine("尝试重新连接...");
            var reconnectTasks = new List<Task>();
            
            foreach (var client in disconnectTargets)
            {
                reconnectTasks.Add(client.Reconnect());
            }
            
            await Task.WhenAll(reconnectTasks);
            
            // 统计重连成功率
            int reconnectSuccessCount = disconnectTargets.FindAll(c => c.IsConnected).Count;
            metrics.ReconnectSuccessRate = (float)reconnectSuccessCount / disconnectTargets.Count;
            
            Console.WriteLine($"重连成功率: {metrics.ReconnectSuccessRate:P2} ({reconnectSuccessCount}/{disconnectTargets.Count})");
        }

        /// <summary>
        /// 生成测试报告
        /// </summary>
        private TestResult GenerateTestReport()
        {
            var result = new TestResult
            {
                Metrics = metrics,
                Passed = 
                    metrics.ConnectionSuccessRate >= 0.95f &&
                    metrics.AverageLatencyMs <= MAX_LATENCY_MS &&
                    metrics.AcceptableLatencyRate >= 0.90f &&
                    metrics.ReconnectSuccessRate >= MIN_RECONNECT_RATE
            };
            
            Console.WriteLine("\n=== 测试报告 ===");
            Console.WriteLine($"连接成功率: {metrics.ConnectionSuccessRate:P2} (目标: ≥95%)");
            Console.WriteLine($"平均延迟: {metrics.AverageLatencyMs}ms (目标: ≤{MAX_LATENCY_MS}ms)");
            Console.WriteLine($"可接受延迟比例: {metrics.AcceptableLatencyRate:P2} (目标: ≥90%)");
            Console.WriteLine($"重连成功率: {metrics.ReconnectSuccessRate:P2} (目标: ≥{MIN_RECONNECT_RATE:P2})");
            Console.WriteLine($"\n测试结果: {(result.Passed ? "✓ 通过" : "✗ 未通过")}");
            
            return result;
        }
    }

    /// <summary>
    /// 模拟客户端
    /// </summary>
    public class SimulatedClient
    {
        public int ClientId { get; private set; }
        public bool IsConnected { get; private set; }
        private Stopwatch latencyStopwatch = new Stopwatch();
        private Random random = new Random();

        public SimulatedClient(int id)
        {
            ClientId = id;
            IsConnected = false;
        }

        public async Task Connect()
        {
            await Task.Delay(random.Next(10, 50)); // 模拟连接延迟
            IsConnected = random.NextDouble() > 0.02; // 98%成功率
        }

        public async Task Disconnect()
        {
            await Task.Delay(10);
            IsConnected = false;
        }

        public async Task Reconnect()
        {
            await Task.Delay(random.Next(100, 300)); // 重连延迟较长
            IsConnected = random.NextDouble() > 0.03; // 97%成功率
        }

        public async Task<long> MeasureLatency()
        {
            latencyStopwatch.Restart();
            await Task.Delay(random.Next(50, 250)); // 模拟网络往返时间
            latencyStopwatch.Stop();
            return latencyStopwatch.ElapsedMilliseconds;
        }
    }

    /// <summary>
    /// 测试指标
    /// </summary>
    public class TestMetrics
    {
        public float ConnectionSuccessRate { get; set; }
        public long AverageLatencyMs { get; set; }
        public long MaxLatencyMs { get; set; }
        public long MinLatencyMs { get; set; }
        public float AcceptableLatencyRate { get; set; }
        public float ReconnectSuccessRate { get; set; }
    }

    /// <summary>
    /// 测试结果
    /// </summary>
    public class TestResult
    {
        public TestMetrics Metrics { get; set; }
        public bool Passed { get; set; }
    }
}
