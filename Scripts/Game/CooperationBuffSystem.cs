using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Scripts.Game
{
    /// <summary>
    /// 协作Buff系统
    /// 当玩家距离≤5格时触发协作增益
    /// </summary>
    public class CooperationBuffSystem
    {
        private const int COOPERATION_DISTANCE = 5;
        private const float DAMAGE_BUFF = 1.15f;       // +15% 伤害
        private const float DEFENSE_BUFF = 1.10f;      // +10% 防御
        private const float SPEED_BUFF = 1.05f;        // +5% 移动速度
        private const float EXP_BUFF = 1.25f;          // +25% 经验

        private Dictionary<int, PlayerState> players = new Dictionary<int, PlayerState>();
        private List<ActiveBuff> activeBuffs = new List<ActiveBuff>();

        /// <summary>
        /// 更新玩家位置并检查协作Buff
        /// </summary>
        public void UpdatePlayerPosition(int playerId, float x, float y, float z)
        {
            if (!players.ContainsKey(playerId))
            {
                players[playerId] = new PlayerState { Id = playerId };
            }

            var player = players[playerId];
            player.X = x;
            player.Y = y;
            player.Z = z;

            // 检查与其他玩家的距离
            CheckCooperationBuffs(playerId);
        }

        /// <summary>
        /// 检查协作Buff
        /// </summary>
        private void CheckCooperationBuffs(int playerId)
        {
            var player = players[playerId];
            var nearbyPlayers = new List<int>();

            // 找出附近的玩家
            foreach (var otherPlayer in players.Values)
            {
                if (otherPlayer.Id == playerId) continue;

                float distance = CalculateDistance(player, otherPlayer);
                if (distance <= COOPERATION_DISTANCE)
                {
                    nearbyPlayers.Add(otherPlayer.Id);
                }
            }

            // 更新Buff状态
            if (nearbyPlayers.Count > 0)
            {
                ActivateCooperationBuff(playerId);
            }
            else
            {
                DeactivateCooperationBuff(playerId);
            }
        }

        /// <summary>
        /// 激活协作Buff
        /// </summary>
        private void ActivateCooperationBuff(int playerId)
        {
            var existingBuff = activeBuffs.FirstOrDefault(b => b.PlayerId == playerId);
            if (existingBuff == null)
            {
                var buff = new ActiveBuff
                {
                    PlayerId = playerId,
                    DamageMultiplier = DAMAGE_BUFF,
                    DefenseMultiplier = DEFENSE_BUFF,
                    SpeedMultiplier = SPEED_BUFF,
                    ExpMultiplier = EXP_BUFF,
                    ActivatedAt = DateTime.Now
                };

                activeBuffs.Add(buff);
                Console.WriteLine($"玩家 {playerId} 获得协作Buff");
            }

            // 更新Buff时间
            var player = players[playerId];
            player.HasCooperationBuff = true;
        }

        /// <summary>
        /// 取消协作Buff
        /// </summary>
        private void DeactivateCooperationBuff(int playerId)
        {
            var buff = activeBuffs.FirstOrDefault(b => b.PlayerId == playerId);
            if (buff != null)
            {
                activeBuffs.Remove(buff);
                Console.WriteLine($"玩家 {playerId} 失去协作Buff");
            }

            var player = players[playerId];
            player.HasCooperationBuff = false;
        }

        /// <summary>
        /// 计算两个玩家之间的距离
        /// </summary>
        private float CalculateDistance(PlayerState p1, PlayerState p2)
        {
            float dx = p1.X - p2.X;
            float dy = p1.Y - p2.Y;
            float dz = p1.Z - p2.Z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// 获取玩家的伤害加成
        /// </summary>
        public float GetDamageMultiplier(int playerId)
        {
            var buff = activeBuffs.FirstOrDefault(b => b.PlayerId == playerId);
            return buff?.DamageMultiplier ?? 1.0f;
        }

        /// <summary>
        /// 获取玩家的防御加成
        /// </summary>
        public float GetDefenseMultiplier(int playerId)
        {
            var buff = activeBuffs.FirstOrDefault(b => b.PlayerId == playerId);
            return buff?.DefenseMultiplier ?? 1.0f;
        }

        /// <summary>
        /// 获取玩家的速度加成
        /// </summary>
        public float GetSpeedMultiplier(int playerId)
        {
            var buff = activeBuffs.FirstOrDefault(b => b.PlayerId == playerId);
            return buff?.SpeedMultiplier ?? 1.0f;
        }

        /// <summary>
        /// 获取玩家的经验加成
        /// </summary>
        public float GetExpMultiplier(int playerId)
        {
            var buff = activeBuffs.FirstOrDefault(b => b.PlayerId == playerId);
            return buff?.ExpMultiplier ?? 1.0f;
        }

        /// <summary>
        /// 检查玩家是否有协作Buff
        /// </summary>
        public bool HasCooperationBuff(int playerId)
        {
            return activeBuffs.Any(b => b.PlayerId == playerId);
        }

        /// <summary>
        /// 获取所有活跃的Buff
        /// </summary>
        public List<ActiveBuff> GetActiveBuffs()
        {
            return activeBuffs.ToList();
        }

        /// <summary>
        /// 清理过期Buff（如果需要）
        /// </summary>
        public void CleanupExpiredBuffs(TimeSpan maxDuration)
        {
            var now = DateTime.Now;
            activeBuffs.RemoveAll(b => (now - b.ActivatedAt) > maxDuration);
        }

        /// <summary>
        /// 显示协作状态
        /// </summary>
        public void DisplayCooperationStatus()
        {
            Console.WriteLine("\n=== 协作Buff状态 ===");
            foreach (var player in players.Values)
            {
                string status = player.HasCooperationBuff ? "✓ 激活" : "✗ 未激活";
                Console.WriteLine($"玩家 {player.Id}: {status}");
            }
            Console.WriteLine($"总计活跃Buff: {activeBuffs.Count}\n");
        }
    }

    /// <summary>
    /// 玩家状态
    /// </summary>
    public class PlayerState
    {
        public int Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public bool HasCooperationBuff { get; set; }
    }

    /// <summary>
    /// 活跃的Buff
    /// </summary>
    public class ActiveBuff
    {
        public int PlayerId { get; set; }
        public float DamageMultiplier { get; set; }
        public float DefenseMultiplier { get; set; }
        public float SpeedMultiplier { get; set; }
        public float ExpMultiplier { get; set; }
        public DateTime ActivatedAt { get; set; }
    }

    /// <summary>
    /// 团队奖励系统
    /// </summary>
    public class TeamRewardSystem
    {
        private const float DUO_REWARD_MULTIPLIER = 1.5f; // 双人掉落 = 单人 x 1.5
        private const float DUO_EXP_BONUS = 0.25f;         // 双人经验加成 +25%

        /// <summary>
        /// 计算掉落奖励
        /// </summary>
        public int CalculateDropReward(int baseReward, int playerCount)
        {
            if (playerCount == 2)
            {
                return (int)(baseReward * DUO_REWARD_MULTIPLIER);
            }
            return baseReward;
        }

        /// <summary>
        /// 计算经验奖励
        /// </summary>
        public int CalculateExpReward(int baseExp, int playerCount)
        {
            if (playerCount == 2)
            {
                return (int)(baseExp * (1 + DUO_EXP_BONUS));
            }
            return baseExp;
        }

        /// <summary>
        /// 分配团队奖励
        /// </summary>
        public Dictionary<int, Reward> DistributeTeamRewards(List<int> playerIds, int totalExp, List<string> items)
        {
            var rewards = new Dictionary<int, Reward>();
            int playerCount = playerIds.Count;

            // 经验按人数分配，但有团队加成
            int expPerPlayer = CalculateExpReward(totalExp, playerCount) / playerCount;

            foreach (var playerId in playerIds)
            {
                rewards[playerId] = new Reward
                {
                    Experience = expPerPlayer,
                    Items = new List<string>(items)
                };
            }

            return rewards;
        }
    }

    /// <summary>
    /// 奖励数据
    /// </summary>
    public class Reward
    {
        public int Experience { get; set; }
        public List<string> Items { get; set; } = new List<string>();
    }
}
