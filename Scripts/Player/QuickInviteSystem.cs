using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Game.Scripts.Player
{
    /// <summary>
    /// 一键邀请系统
    /// 简化联机邀请流程，提供快速匹配和邀请功能
    /// </summary>
    public class QuickInviteSystem
    {
        private List<Friend> friendList = new List<Friend>();
        private List<Player> recentPlayers = new List<Player>();
        private Dictionary<int, Invitation> pendingInvitations = new Dictionary<int, Invitation>();

        public event Action<Invitation> OnInvitationReceived;
        public event Action<int> OnInvitationAccepted;
        public event Action<int> OnInvitationDeclined;

        /// <summary>
        /// 一键邀请好友
        /// </summary>
        public async Task<bool> QuickInvite(int friendId)
        {
            var friend = friendList.FirstOrDefault(f => f.Id == friendId);
            if (friend == null)
            {
                Console.WriteLine("好友不存在");
                return false;
            }

            if (!friend.IsOnline)
            {
                Console.WriteLine($"{friend.Name} 不在线");
                return false;
            }

            // 创建邀请
            var invitation = new Invitation
            {
                Id = GenerateInvitationId(),
                SenderId = GetCurrentPlayerId(),
                ReceiverId = friendId,
                Timestamp = DateTime.Now,
                Status = InvitationStatus.Pending
            };

            // 发送邀请（模拟网络延迟）
            await Task.Delay(50);

            pendingInvitations[invitation.Id] = invitation;

            Console.WriteLine($"已向 {friend.Name} 发送邀请");
            return true;
        }

        /// <summary>
        /// 批量邀请（一键邀请多个好友）
        /// </summary>
        public async Task<int> QuickInviteMultiple(List<int> friendIds)
        {
            int successCount = 0;

            foreach (var friendId in friendIds)
            {
                bool success = await QuickInvite(friendId);
                if (success) successCount++;
            }

            Console.WriteLine($"成功发送 {successCount}/{friendIds.Count} 个邀请");
            return successCount;
        }

        /// <summary>
        /// 一键接受邀请
        /// </summary>
        public async Task<bool> QuickAccept(int invitationId)
        {
            if (!pendingInvitations.ContainsKey(invitationId))
            {
                Console.WriteLine("邀请不存在");
                return false;
            }

            var invitation = pendingInvitations[invitationId];
            invitation.Status = InvitationStatus.Accepted;

            // 自动加入房间
            await JoinGameSession(invitation.SenderId);

            pendingInvitations.Remove(invitationId);
            OnInvitationAccepted?.Invoke(invitationId);

            Console.WriteLine("已接受邀请并加入游戏");
            return true;
        }

        /// <summary>
        /// 快速拒绝邀请
        /// </summary>
        public void QuickDecline(int invitationId)
        {
            if (!pendingInvitations.ContainsKey(invitationId))
            {
                Console.WriteLine("邀请不存在");
                return;
            }

            var invitation = pendingInvitations[invitationId];
            invitation.Status = InvitationStatus.Declined;

            pendingInvitations.Remove(invitationId);
            OnInvitationDeclined?.Invoke(invitationId);

            Console.WriteLine("已拒绝邀请");
        }

        /// <summary>
        /// 智能推荐玩家（基于最近游戏记录）
        /// </summary>
        public List<Player> GetRecommendedPlayers(int count = 5)
        {
            // 根据最近游戏时间、游戏次数、等级差等因素推荐
            var recommended = recentPlayers
                .Where(p => p.IsOnline)
                .OrderByDescending(p => p.LastPlayedTogether)
                .ThenByDescending(p => p.CoopCount)
                .Take(count)
                .ToList();

            return recommended;
        }

        /// <summary>
        /// 获取在线好友列表（快速访问）
        /// </summary>
        public List<Friend> GetOnlineFriends()
        {
            return friendList.Where(f => f.IsOnline).OrderBy(f => f.Name).ToList();
        }

        /// <summary>
        /// 添加最近玩家
        /// </summary>
        public void AddRecentPlayer(Player player)
        {
            var existing = recentPlayers.FirstOrDefault(p => p.Id == player.Id);
            if (existing != null)
            {
                existing.LastPlayedTogether = DateTime.Now;
                existing.CoopCount++;
            }
            else
            {
                player.LastPlayedTogether = DateTime.Now;
                player.CoopCount = 1;
                recentPlayers.Add(player);

                // 保持列表大小
                if (recentPlayers.Count > 20)
                {
                    recentPlayers = recentPlayers
                        .OrderByDescending(p => p.LastPlayedTogether)
                        .Take(20)
                        .ToList();
                }
            }
        }

        /// <summary>
        /// 加入游戏会话
        /// </summary>
        private async Task JoinGameSession(int hostPlayerId)
        {
            Console.WriteLine($"正在加入玩家 {hostPlayerId} 的游戏...");
            await Task.Delay(200); // 模拟加入延迟
            Console.WriteLine("成功加入游戏");
        }

        /// <summary>
        /// 生成邀请ID
        /// </summary>
        private int GenerateInvitationId()
        {
            return new Random().Next(10000, 99999);
        }

        /// <summary>
        /// 获取当前玩家ID
        /// </summary>
        private int GetCurrentPlayerId()
        {
            return 1; // 模拟
        }

        /// <summary>
        /// 初始化测试数据
        /// </summary>
        public void InitializeTestData()
        {
            // 添加测试好友
            friendList.Add(new Friend { Id = 101, Name = "玩家A", IsOnline = true, Level = 15 });
            friendList.Add(new Friend { Id = 102, Name = "玩家B", IsOnline = true, Level = 18 });
            friendList.Add(new Friend { Id = 103, Name = "玩家C", IsOnline = false, Level = 12 });
            friendList.Add(new Friend { Id = 104, Name = "玩家D", IsOnline = true, Level = 16 });

            // 添加最近玩家
            recentPlayers.Add(new Player 
            { 
                Id = 201, 
                Name = "最近玩家1", 
                IsOnline = true, 
                Level = 14,
                LastPlayedTogether = DateTime.Now.AddHours(-2),
                CoopCount = 5
            });
            recentPlayers.Add(new Player 
            { 
                Id = 202, 
                Name = "最近玩家2", 
                IsOnline = true, 
                Level = 17,
                LastPlayedTogether = DateTime.Now.AddDays(-1),
                CoopCount = 3
            });
        }
    }

    /// <summary>
    /// 好友信息
    /// </summary>
    public class Friend
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOnline { get; set; }
        public int Level { get; set; }
    }

    /// <summary>
    /// 玩家信息
    /// </summary>
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOnline { get; set; }
        public int Level { get; set; }
        public DateTime LastPlayedTogether { get; set; }
        public int CoopCount { get; set; }
    }

    /// <summary>
    /// 邀请信息
    /// </summary>
    public class Invitation
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public DateTime Timestamp { get; set; }
        public InvitationStatus Status { get; set; }
    }

    /// <summary>
    /// 邀请状态
    /// </summary>
    public enum InvitationStatus
    {
        Pending,
        Accepted,
        Declined,
        Expired
    }
}
