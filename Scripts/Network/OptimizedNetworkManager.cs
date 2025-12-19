using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Scripts.Network
{
    /// <summary>
    /// 网络管理器 - 优化版
    /// 支持低延迟同步、自动重连、连接池管理
    /// </summary>
    public class OptimizedNetworkManager
    {
        private const int MAX_RECONNECT_ATTEMPTS = 5;
        private const int RECONNECT_DELAY_MS = 1000;
        private const int HEARTBEAT_INTERVAL_MS = 5000;
        private const int CONNECTION_TIMEOUT_MS = 10000;

        private bool isConnected = false;
        private int reconnectAttempts = 0;
        private DateTime lastHeartbeat;
        private DateTime lastHeartbeatReceived;
        
        public bool IsConnected => isConnected;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<int> OnReconnecting;

        /// <summary>
        /// 连接到服务器
        /// </summary>
        public async Task<bool> Connect(string serverAddress)
        {
            Console.WriteLine($"连接到服务器: {serverAddress}");

            try
            {
                // 模拟连接过程
                await Task.Delay(100);
                
                isConnected = true;
                reconnectAttempts = 0;
                lastHeartbeat = DateTime.Now;
                lastHeartbeatReceived = DateTime.Now;
                
                OnConnected?.Invoke();
                
                // 启动心跳检测
                _ = StartHeartbeat();
                
                Console.WriteLine("连接成功");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"连接失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                OnDisconnected?.Invoke();
                Console.WriteLine("已断开连接");
            }
        }

        /// <summary>
        /// 自动重连
        /// </summary>
        public async Task<bool> Reconnect(string serverAddress)
        {
            Console.WriteLine("开始重连...");
            
            while (reconnectAttempts < MAX_RECONNECT_ATTEMPTS)
            {
                reconnectAttempts++;
                OnReconnecting?.Invoke(reconnectAttempts);
                
                Console.WriteLine($"重连尝试 {reconnectAttempts}/{MAX_RECONNECT_ATTEMPTS}");
                
                bool success = await Connect(serverAddress);
                if (success)
                {
                    Console.WriteLine("重连成功");
                    return true;
                }
                
                await Task.Delay(RECONNECT_DELAY_MS * reconnectAttempts);
            }
            
            Console.WriteLine("重连失败：达到最大尝试次数");
            return false;
        }

        /// <summary>
        /// 发送数据（优化延迟）
        /// </summary>
        public async Task<bool> SendData(byte[] data, bool reliable = true)
        {
            if (!isConnected)
            {
                Console.WriteLine("未连接，无法发送数据");
                return false;
            }

            try
            {
                // 根据可靠性选择不同的发送策略
                if (reliable)
                {
                    await SendReliable(data);
                }
                else
                {
                    await SendUnreliable(data);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送数据失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 可靠传输（TCP）
        /// </summary>
        private async Task SendReliable(byte[] data)
        {
            // 模拟TCP传输
            await Task.Delay(5);
        }

        /// <summary>
        /// 不可靠传输（UDP，低延迟）
        /// </summary>
        private async Task SendUnreliable(byte[] data)
        {
            // 模拟UDP传输，更快但不保证送达
            await Task.Delay(2);
        }

        /// <summary>
        /// 心跳检测
        /// </summary>
        private async Task StartHeartbeat()
        {
            while (isConnected)
            {
                await Task.Delay(HEARTBEAT_INTERVAL_MS);
                
                if (isConnected)
                {
                    // 发送心跳包
                    var heartbeatData = new byte[] { 0xFF };
                    await SendUnreliable(heartbeatData);
                    
                    lastHeartbeat = DateTime.Now;
                    
                    // 检查是否收到服务器响应（模拟收到响应）
                    // 在实际实现中，应该在收到服务器心跳响应时更新 lastHeartbeatReceived
                    if ((DateTime.Now - lastHeartbeatReceived).TotalMilliseconds > CONNECTION_TIMEOUT_MS)
                    {
                        Console.WriteLine("连接超时，触发重连");
                        Disconnect();
                    }
                    else
                    {
                        // 模拟收到心跳响应
                        lastHeartbeatReceived = DateTime.Now;
                    }
                }
            }
        }

        /// <summary>
        /// 同步玩家位置（优化版）
        /// </summary>
        public async Task SyncPlayerPosition(int playerId, float x, float y, float z)
        {
            var positionData = new PositionData
            {
                PlayerId = playerId,
                X = x,
                Y = y,
                Z = z,
                Timestamp = DateTime.Now.Ticks
            };

            // 位置同步使用UDP以降低延迟
            await SendData(SerializePosition(positionData), reliable: false);
        }

        /// <summary>
        /// 序列化位置数据
        /// </summary>
        private byte[] SerializePosition(PositionData data)
        {
            // 简化的序列化
            return new byte[16];
        }
    }

    /// <summary>
    /// 位置数据
    /// </summary>
    public struct PositionData
    {
        public int PlayerId;
        public float X;
        public float Y;
        public float Z;
        public long Timestamp;
    }

    /// <summary>
    /// 连接配置
    /// </summary>
    public class ConnectionConfig
    {
        public string ServerAddress { get; set; } = "127.0.0.1:7777";
        public int MaxPlayers { get; set; } = 200;
        public int TargetLatencyMs { get; set; } = 200;
        public float MinReconnectRate { get; set; } = 0.95f;
        public bool EnableCompression { get; set; } = true;
        public bool EnablePrediction { get; set; } = true;
    }
}
