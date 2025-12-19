using UnityEngine;
using Photon.Pun;
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

/// <summary>
/// Manages cloud-based data storage using Photon Custom Properties
/// Handles friends, achievements, and social features
/// </summary>
public class CloudDataManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    public static CloudDataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region Settings
    [Header("Sync Settings")]
    [Tooltip("Sync interval for statistics (seconds)")]
    public float statisticsSyncInterval = 300f; // 5 minutes
    
    [Tooltip("Enable cloud sync")]
    public bool enableCloudSync = true;
    #endregion

    #region State
    private CloudPlayerData cloudPlayerData;
    private float lastStatisticsSync = 0f;
    private bool isInitialized = false;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            InitializeCloudData();
        }
    }

    private void Update()
    {
        if (!enableCloudSync || !PhotonNetwork.IsConnected) return;

        // Periodic statistics sync
        if (Time.time - lastStatisticsSync >= statisticsSyncInterval)
        {
            SyncStatistics();
            lastStatisticsSync = Time.time;
        }
    }
    #endregion

    #region Initialization
    private void InitializeCloudData()
    {
        if (isInitialized) return;

        // Load cloud data from player properties
        LoadCloudData();
        isInitialized = true;
    }

    private void LoadCloudData()
    {
        if (!PhotonNetwork.IsConnected || PhotonNetwork.LocalPlayer == null)
        {
            Debug.LogWarning("Cannot load cloud data: not connected");
            return;
        }

        Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;
        
        cloudPlayerData = new CloudPlayerData();

        // Load friends list
        if (props.ContainsKey("friends"))
        {
            string friendsJson = (string)props["friends"];
            if (!string.IsNullOrEmpty(friendsJson))
            {
                cloudPlayerData.friends = JsonUtility.FromJson<StringList>(friendsJson).items;
            }
        }

        // Load achievements
        if (props.ContainsKey("achievements"))
        {
            string achievementsJson = (string)props["achievements"];
            if (!string.IsNullOrEmpty(achievementsJson))
            {
                cloudPlayerData.achievements = JsonUtility.FromJson<AchievementList>(achievementsJson).items;
            }
        }

        // Load statistics
        if (props.ContainsKey("statistics"))
        {
            string statsJson = (string)props["statistics"];
            if (!string.IsNullOrEmpty(statsJson))
            {
                cloudPlayerData.statistics = JsonUtility.FromJson<PlayerStatistics>(statsJson);
            }
        }

        Debug.Log("Cloud data loaded");
    }
    #endregion

    #region Friends Management
    /// <summary>
    /// Add a friend
    /// </summary>
    public void AddFriend(string friendId)
    {
        if (cloudPlayerData.friends.Contains(friendId))
        {
            Debug.LogWarning($"Friend {friendId} already in list");
            return;
        }

        cloudPlayerData.friends.Add(friendId);
        SyncFriendsList();
        Debug.Log($"Added friend: {friendId}");
    }

    /// <summary>
    /// Remove a friend
    /// </summary>
    public void RemoveFriend(string friendId)
    {
        if (cloudPlayerData.friends.Remove(friendId))
        {
            SyncFriendsList();
            Debug.Log($"Removed friend: {friendId}");
        }
    }

    /// <summary>
    /// Get friends list
    /// </summary>
    public List<string> GetFriends()
    {
        return new List<string>(cloudPlayerData.friends);
    }

    /// <summary>
    /// Sync friends list to cloud
    /// </summary>
    private void SyncFriendsList()
    {
        if (!PhotonNetwork.IsConnected) return;

        StringList friendsList = new StringList { items = cloudPlayerData.friends };
        string json = JsonUtility.ToJson(friendsList);

        Hashtable props = new Hashtable
        {
            { "friends", json }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    #endregion

    #region Achievements Management
    /// <summary>
    /// Unlock an achievement
    /// </summary>
    public void UnlockAchievement(string achievementId, string achievementName)
    {
        // Check if already unlocked
        if (cloudPlayerData.achievements.Exists(a => a.id == achievementId))
        {
            Debug.LogWarning($"Achievement {achievementId} already unlocked");
            return;
        }

        AchievementData achievement = new AchievementData
        {
            id = achievementId,
            name = achievementName,
            unlockedAt = DateTime.UtcNow.ToString("o"),
            progress = 1.0f
        };

        cloudPlayerData.achievements.Add(achievement);
        SyncAchievements();
        
        Debug.Log($"Achievement unlocked: {achievementName}");
        
        // TODO: Trigger achievement notification UI
    }

    /// <summary>
    /// Update achievement progress
    /// </summary>
    public void UpdateAchievementProgress(string achievementId, float progress)
    {
        AchievementData achievement = cloudPlayerData.achievements.Find(a => a.id == achievementId);
        
        if (achievement != null)
        {
            achievement.progress = Mathf.Clamp01(progress);
            
            if (achievement.progress >= 1.0f && string.IsNullOrEmpty(achievement.unlockedAt))
            {
                achievement.unlockedAt = DateTime.UtcNow.ToString("o");
                Debug.Log($"Achievement completed: {achievement.name}");
            }
            
            SyncAchievements();
        }
    }

    /// <summary>
    /// Get all achievements
    /// </summary>
    public List<AchievementData> GetAchievements()
    {
        return new List<AchievementData>(cloudPlayerData.achievements);
    }

    /// <summary>
    /// Sync achievements to cloud
    /// </summary>
    private void SyncAchievements()
    {
        if (!PhotonNetwork.IsConnected) return;

        AchievementList achievementList = new AchievementList { items = cloudPlayerData.achievements };
        string json = JsonUtility.ToJson(achievementList);

        Hashtable props = new Hashtable
        {
            { "achievements", json }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    #endregion

    #region Statistics Management
    /// <summary>
    /// Update player statistics
    /// </summary>
    public void UpdateStatistics(Action<PlayerStatistics> updateAction)
    {
        updateAction?.Invoke(cloudPlayerData.statistics);
    }

    /// <summary>
    /// Get player statistics
    /// </summary>
    public PlayerStatistics GetStatistics()
    {
        return cloudPlayerData.statistics;
    }

    /// <summary>
    /// Sync statistics to cloud
    /// </summary>
    private void SyncStatistics()
    {
        if (!PhotonNetwork.IsConnected) return;

        string json = JsonUtility.ToJson(cloudPlayerData.statistics);

        Hashtable props = new Hashtable
        {
            { "statistics", json }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        Debug.Log("Statistics synced to cloud");
    }
    #endregion

    #region Session Management
    /// <summary>
    /// Start a co-op session
    /// </summary>
    public void StartCoopSession()
    {
        if (!PhotonNetwork.InRoom) return;

        CoopSessionData session = new CoopSessionData
        {
            sessionId = System.Guid.NewGuid().ToString(),
            startTime = DateTime.UtcNow.ToString("o")
        };

        // Get participant IDs
        foreach (var player in PhotonNetwork.PlayerList)
        {
            session.participants.Add(player.UserId);
        }

        cloudPlayerData.currentSession = session;
        Debug.Log($"Co-op session started: {session.sessionId}");
    }

    /// <summary>
    /// End co-op session and save results
    /// </summary>
    public void EndCoopSession()
    {
        if (cloudPlayerData.currentSession == null) return;

        cloudPlayerData.currentSession.endTime = DateTime.UtcNow.ToString("o");
        
        // Update statistics
        cloudPlayerData.statistics.coopSessionsCompleted++;
        
        Debug.Log($"Co-op session ended: {cloudPlayerData.currentSession.sessionId}");
        
        // Archive session (could be sent to backend in full implementation)
        cloudPlayerData.currentSession = null;
        
        SyncStatistics();
    }
    #endregion

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        InitializeCloudData();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isInitialized = false;
    }

    public override void OnJoinedRoom()
    {
        StartCoopSession();
    }

    public override void OnLeftRoom()
    {
        EndCoopSession();
    }
    #endregion
}

#region Cloud Data Structures

[Serializable]
public class CloudPlayerData
{
    public List<string> friends;
    public List<string> pendingInvites;
    public List<string> recentPlayers;
    public List<string> blockedPlayers;
    public List<AchievementData> achievements;
    public PlayerStatistics statistics;
    public CoopSessionData currentSession;

    public CloudPlayerData()
    {
        friends = new List<string>();
        pendingInvites = new List<string>();
        recentPlayers = new List<string>();
        blockedPlayers = new List<string>();
        achievements = new List<AchievementData>();
        statistics = new PlayerStatistics();
        currentSession = null;
    }
}

[Serializable]
public class AchievementData
{
    public string id;
    public string name;
    public string unlockedAt;
    public float progress;
}

[Serializable]
public class PlayerStatistics
{
    public float totalPlayTime;
    public int monstersKilled;
    public int bossesDefeated;
    public int coopSessionsCompleted;
    public int highestLevel;
    public int itemsCollected;
    public int deathCount;

    public PlayerStatistics()
    {
        totalPlayTime = 0f;
        monstersKilled = 0;
        bossesDefeated = 0;
        coopSessionsCompleted = 0;
        highestLevel = 1;
        itemsCollected = 0;
        deathCount = 0;
    }
}

[Serializable]
public class CoopSessionData
{
    public string sessionId;
    public List<string> participants;
    public string startTime;
    public string endTime;
    public List<string> achievementsUnlocked;

    public CoopSessionData()
    {
        participants = new List<string>();
        achievementsUnlocked = new List<string>();
    }
}

// Helper classes for JSON serialization
[Serializable]
public class StringList
{
    public List<string> items;
}

[Serializable]
public class AchievementList
{
    public List<AchievementData> items;
}

#endregion
