using System;
using UnityEngine;

namespace PixelCoopRPG.Data
{
    /// <summary>
    /// Cross-platform data structures for network synchronization and storage.
    /// Ensures consistent data format across all platforms.
    /// </summary>
    
    /// <summary>
    /// Player persistent data structure
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public string playerId;
        public string playerName;
        public int level;
        public int experience;
        public float positionX;
        public float positionY;
        public float positionZ;
        public string currentScene;
        public long lastSaveTime;
        
        // Stats
        public int health;
        public int maxHealth;
        public int mana;
        public int maxMana;
        
        // Inventory (simplified - can be expanded)
        public string[] inventoryItems;
        public int gold;
        
        public PlayerData()
        {
            playerId = Guid.NewGuid().ToString();
            playerName = "Player";
            level = 1;
            experience = 0;
            health = 100;
            maxHealth = 100;
            mana = 50;
            maxMana = 50;
            gold = 0;
            inventoryItems = new string[0];
            lastSaveTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
    
    /// <summary>
    /// Base building/layout data structure
    /// </summary>
    [Serializable]
    public class BaseLayoutData
    {
        public string baseId;
        public string ownerId;
        public BuildingData[] buildings;
        public long lastModified;
        
        public BaseLayoutData()
        {
            baseId = Guid.NewGuid().ToString();
            buildings = new BuildingData[0];
            lastModified = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
    
    /// <summary>
    /// Individual building data
    /// </summary>
    [Serializable]
    public class BuildingData
    {
        public string buildingId;
        public string buildingType;
        public float positionX;
        public float positionY;
        public int level;
        public bool isConstructed;
        
        public BuildingData(string type, Vector2 position)
        {
            buildingId = Guid.NewGuid().ToString();
            buildingType = type;
            positionX = position.x;
            positionY = position.y;
            level = 1;
            isConstructed = true;
        }
    }
    
    /// <summary>
    /// Game progress data
    /// </summary>
    [Serializable]
    public class GameProgressData
    {
        public string playerId;
        public int currentLevel;
        public string[] completedQuests;
        public string[] unlockedAreas;
        public float playtimeSeconds;
        public long lastPlayTime;
        
        public GameProgressData()
        {
            completedQuests = new string[0];
            unlockedAreas = new string[0];
            playtimeSeconds = 0f;
            lastPlayTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
    
    /// <summary>
    /// Friend system data (cloud-stored)
    /// </summary>
    [Serializable]
    public class FriendData
    {
        public string friendId;
        public string friendName;
        public int friendLevel;
        public bool isOnline;
        public long lastSeenTime;
        
        public FriendData(string id, string name, int level)
        {
            friendId = id;
            friendName = name;
            friendLevel = level;
            isOnline = false;
            lastSeenTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
    
    /// <summary>
    /// Achievement data (cloud-stored)
    /// </summary>
    [Serializable]
    public class AchievementData
    {
        public string achievementId;
        public string achievementName;
        public string description;
        public bool isUnlocked;
        public float progress;
        public long unlockedTime;
        
        public AchievementData(string id, string name, string desc)
        {
            achievementId = id;
            achievementName = name;
            description = desc;
            isUnlocked = false;
            progress = 0f;
            unlockedTime = 0;
        }
    }
    
    /// <summary>
    /// Network sync data structure for real-time synchronization
    /// </summary>
    [Serializable]
    public class NetworkSyncData
    {
        public string playerId;
        public float positionX;
        public float positionY;
        public float positionZ;
        public float velocityX;
        public float velocityY;
        public string animationState;
        public int health;
        public long timestamp;
        
        public NetworkSyncData()
        {
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        
        public void UpdateFromTransform(Transform transform, Vector2 velocity)
        {
            positionX = transform.position.x;
            positionY = transform.position.y;
            positionZ = transform.position.z;
            velocityX = velocity.x;
            velocityY = velocity.y;
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        
        public Vector3 GetPosition()
        {
            return new Vector3(positionX, positionY, positionZ);
        }
        
        public Vector2 GetVelocity()
        {
            return new Vector2(velocityX, velocityY);
        }
    }
    
    /// <summary>
    /// Session data for matchmaking with level restriction (±3 levels)
    /// </summary>
    [Serializable]
    public class SessionData
    {
        public string sessionId;
        public string hostPlayerId;
        public int hostLevel;
        public int maxPlayers;
        public int currentPlayers;
        public bool isOpen;
        public long createdTime;
        
        public SessionData()
        {
            sessionId = Guid.NewGuid().ToString();
            maxPlayers = 2;
            currentPlayers = 1;
            isOpen = true;
            createdTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        
        /// <summary>
        /// Check if player level is within acceptable range (±3)
        /// </summary>
        public bool IsLevelCompatible(int playerLevel)
        {
            return Mathf.Abs(playerLevel - hostLevel) <= 3;
        }
    }
}
