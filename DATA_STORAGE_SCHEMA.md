# Data Storage Schema Documentation

## Overview

This document describes the data storage architecture, schema definitions, and usage guidelines for the Pixel Co-op RPG game.

---

## 📊 Storage Architecture

### Hybrid Storage Model

```
┌─────────────────────────────────────────────────────┐
│                  Application Layer                  │
├─────────────────────────────────────────────────────┤
│              Data Sync Manager                      │
│         (Conflict Resolution & Sync Logic)          │
├──────────────────────┬──────────────────────────────┤
│   Local Storage      │      Cloud Storage           │
│   (JSON Files)       │   (Firebase/PlayFab/API)     │
├──────────────────────┴──────────────────────────────┤
│              Persistent Data Storage                │
└─────────────────────────────────────────────────────┘
```

### Storage Responsibilities

| Data Type | Local Storage | Cloud Storage | Sync Required |
|-----------|---------------|---------------|---------------|
| Player Stats | ✓ | ✗ | ✗ |
| Game Progress | ✓ | ✗ | ✗ |
| Base Layout | ✓ | ✗ | ✗ |
| Friends List | ✗ | ✓ | ✓ |
| Achievements | ✗ | ✓ | ✓ |
| Leaderboards | ✗ | ✓ | ✓ |
| Session Data | Memory Only | ✗ | ✗ |

---

## 📁 Local Storage Schema

### File Structure

```
{PersistentDataPath}/
├── player_data.json       # Character stats and inventory
├── progress_data.json     # Quest and progression data
└── base_layout.json       # Base building data
```

### Platform-Specific Paths

| Platform | Path |
|----------|------|
| Windows | `%APPDATA%\LocalLow\CompanyName\PixelCoopRPG\` |
| macOS | `~/Library/Application Support/CompanyName/PixelCoopRPG/` |
| Linux | `~/.config/unity3d/CompanyName/PixelCoopRPG/` |
| iOS | `Application/[GUID]/Documents/` |
| Android | `/storage/emulated/0/Android/data/com.CompanyName.PixelCoopRPG/files/` |

---

## 📋 Data Schemas

### 1. PlayerData

**File**: `player_data.json`
**Purpose**: Core player character information

```json
{
  "playerId": "550e8400-e29b-41d4-a716-446655440000",
  "playerName": "DragonSlayer",
  "level": 15,
  "experience": 2450,
  "positionX": 123.45,
  "positionY": 67.89,
  "positionZ": 0.0,
  "currentScene": "ForestLevel",
  "lastSaveTime": 1703001234,
  "health": 85,
  "maxHealth": 100,
  "mana": 45,
  "maxMana": 60,
  "inventoryItems": [
    "item_sword_001",
    "item_potion_health",
    "item_key_dungeon"
  ],
  "gold": 1250
}
```

**Field Descriptions:**

| Field | Type | Description | Required |
|-------|------|-------------|----------|
| `playerId` | string (GUID) | Unique player identifier | ✓ |
| `playerName` | string | Player display name | ✓ |
| `level` | int | Current character level | ✓ |
| `experience` | int | Current XP points | ✓ |
| `positionX/Y/Z` | float | Last saved position | ✓ |
| `currentScene` | string | Last played scene name | ✓ |
| `lastSaveTime` | long | Unix timestamp of last save | ✓ |
| `health` | int | Current health points | ✓ |
| `maxHealth` | int | Maximum health capacity | ✓ |
| `mana` | int | Current mana points | ✓ |
| `maxMana` | int | Maximum mana capacity | ✓ |
| `inventoryItems` | string[] | Array of item IDs | ✓ |
| `gold` | int | Currency amount | ✓ |

**Usage Example:**
```csharp
// Create new player
PlayerData player = new PlayerData
{
    playerName = "NewHero",
    level = 1
};

// Save to disk
LocalStorageManager.Instance.SavePlayerData(player);

// Load from disk
PlayerData loaded = LocalStorageManager.Instance.LoadPlayerData();
```

---

### 2. GameProgressData

**File**: `progress_data.json`
**Purpose**: Track quest completion and game progression

```json
{
  "playerId": "550e8400-e29b-41d4-a716-446655440000",
  "currentLevel": 3,
  "completedQuests": [
    "quest_tutorial_001",
    "quest_forest_001",
    "quest_village_rescue"
  ],
  "unlockedAreas": [
    "area_forest",
    "area_village",
    "area_cave_entrance"
  ],
  "playtimeSeconds": 12345.67,
  "lastPlayTime": 1703001234
}
```

**Field Descriptions:**

| Field | Type | Description | Required |
|-------|------|-------------|----------|
| `playerId` | string (GUID) | Associated player ID | ✓ |
| `currentLevel` | int | Current game level/chapter | ✓ |
| `completedQuests` | string[] | Array of completed quest IDs | ✓ |
| `unlockedAreas` | string[] | Array of unlocked area IDs | ✓ |
| `playtimeSeconds` | float | Total playtime in seconds | ✓ |
| `lastPlayTime` | long | Unix timestamp of last play | ✓ |

---

### 3. BaseLayoutData

**File**: `base_layout.json`
**Purpose**: Player base building and layout configuration

```json
{
  "baseId": "base_550e8400-e29b-41d4-a716-446655440000",
  "ownerId": "550e8400-e29b-41d4-a716-446655440000",
  "buildings": [
    {
      "buildingId": "building_001",
      "buildingType": "house",
      "positionX": 10.5,
      "positionY": 5.2,
      "level": 2,
      "isConstructed": true
    },
    {
      "buildingId": "building_002",
      "buildingType": "storage",
      "positionX": 15.0,
      "positionY": 5.0,
      "level": 1,
      "isConstructed": true
    }
  ],
  "lastModified": 1703001234
}
```

**Field Descriptions:**

| Field | Type | Description | Required |
|-------|------|-------------|----------|
| `baseId` | string (GUID) | Unique base identifier | ✓ |
| `ownerId` | string (GUID) | Player who owns this base | ✓ |
| `buildings` | BuildingData[] | Array of building objects | ✓ |
| `lastModified` | long | Unix timestamp of last edit | ✓ |

**BuildingData Schema:**

| Field | Type | Description | Required |
|-------|------|-------------|----------|
| `buildingId` | string (GUID) | Unique building identifier | ✓ |
| `buildingType` | string | Building type/category | ✓ |
| `positionX/Y` | float | World position coordinates | ✓ |
| `level` | int | Building upgrade level | ✓ |
| `isConstructed` | bool | Construction complete flag | ✓ |

---

## ☁️ Cloud Storage Schema

### 4. FriendData

**Storage**: Cloud Database
**Purpose**: Social features and friend connections

```json
{
  "friendId": "660e8400-e29b-41d4-a716-446655440001",
  "friendName": "CoopBuddy",
  "friendLevel": 14,
  "isOnline": true,
  "lastSeenTime": 1703001234
}
```

**Field Descriptions:**

| Field | Type | Description | Required |
|-------|------|-------------|----------|
| `friendId` | string (GUID) | Friend's player ID | ✓ |
| `friendName` | string | Friend's display name | ✓ |
| `friendLevel` | int | Friend's character level | ✓ |
| `isOnline` | bool | Current online status | ✓ |
| `lastSeenTime` | long | Unix timestamp of last activity | ✓ |

**Cloud Storage Structure:**
```
/players/{playerId}/friends/
  ├── {friendId1}/
  │   └── friend_data
  ├── {friendId2}/
  │   └── friend_data
  └── ...
```

---

### 5. AchievementData

**Storage**: Cloud Database
**Purpose**: Achievement tracking and rewards

```json
{
  "achievementId": "ach_dragon_slayer",
  "achievementName": "Dragon Slayer",
  "description": "Defeat the Ancient Dragon",
  "isUnlocked": true,
  "progress": 1.0,
  "unlockedTime": 1703001234
}
```

**Field Descriptions:**

| Field | Type | Description | Required |
|-------|------|-------------|----------|
| `achievementId` | string | Unique achievement identifier | ✓ |
| `achievementName` | string | Display name | ✓ |
| `description` | string | Achievement description | ✓ |
| `isUnlocked` | bool | Unlock status | ✓ |
| `progress` | float | Progress (0.0 to 1.0) | ✓ |
| `unlockedTime` | long | Unix timestamp when unlocked | ✓ |

**Cloud Storage Structure:**
```
/players/{playerId}/achievements/
  ├── {achievementId1}/
  │   └── achievement_data
  ├── {achievementId2}/
  │   └── achievement_data
  └── ...
```

---

## 🔄 Network Sync Schema

### 6. NetworkSyncData

**Storage**: Memory only (real-time sync via Photon)
**Purpose**: Real-time player position and state synchronization

```json
{
  "playerId": "550e8400-e29b-41d4-a716-446655440000",
  "positionX": 123.45,
  "positionY": 67.89,
  "positionZ": 0.0,
  "velocityX": 2.5,
  "velocityY": 0.0,
  "animationState": "run",
  "health": 85,
  "timestamp": 1703001234567
}
```

**Usage**: Synced via Photon PUN 2 `OnPhotonSerializeView()`

---

### 7. SessionData

**Storage**: Memory only (room metadata)
**Purpose**: Matchmaking and room management

```json
{
  "sessionId": "room_550e8400-e29b",
  "hostPlayerId": "550e8400-e29b-41d4-a716-446655440000",
  "hostLevel": 15,
  "maxPlayers": 2,
  "currentPlayers": 1,
  "isOpen": true,
  "createdTime": 1703001234
}
```

**Field Descriptions:**

| Field | Type | Description | Required |
|-------|------|-------------|----------|
| `sessionId` | string | Unique room identifier | ✓ |
| `hostPlayerId` | string | Room host player ID | ✓ |
| `hostLevel` | int | Host player level | ✓ |
| `maxPlayers` | int | Maximum players (2) | ✓ |
| `currentPlayers` | int | Current player count | ✓ |
| `isOpen` | bool | Room accepting players | ✓ |
| `createdTime` | long | Unix timestamp | ✓ |

**Matchmaking Logic:**
```csharp
public bool IsLevelCompatible(int playerLevel)
{
    // Allow ±3 level difference
    return Mathf.Abs(playerLevel - hostLevel) <= 3;
}
```

---

## 🔐 Data Security

### Current Implementation

1. **Local Storage**: 
   - Plain JSON format
   - OS-level file permissions
   - No encryption (planned for future)

2. **Cloud Storage**:
   - HTTPS transport encryption
   - Authentication via cloud provider
   - Access control via user sessions

### Future Enhancements

- AES-256 encryption for local files
- Checksum verification for data integrity
- Obfuscation of sensitive game data
- Anti-cheat validation

---

## 📝 Data Validation

### Validation Rules

```csharp
// Player level validation
if (playerData.level < 1 || playerData.level > 100)
{
    Debug.LogError("Invalid player level");
    return false;
}

// Health validation
if (playerData.health > playerData.maxHealth)
{
    playerData.health = playerData.maxHealth;
}

// Position validation (bounds checking)
if (!IsValidPosition(playerData.position))
{
    playerData.position = GetDefaultSpawnPoint();
}

// Timestamp validation
if (playerData.lastSaveTime > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
{
    Debug.LogWarning("Future timestamp detected - clock skew?");
}
```

---

## 🔄 Data Migration

### Version Control

Each save file should include version:
```json
{
  "version": "1.0",
  "data": { ... }
}
```

### Migration Strategy

```csharp
public PlayerData LoadPlayerData()
{
    string json = LoadFromDisk();
    var saveData = ParseVersion(json);
    
    switch (saveData.version)
    {
        case "1.0":
            return saveData.data;
        case "0.9":
            return MigrateFrom09To10(saveData.data);
        default:
            return new PlayerData(); // Reset
    }
}
```

---

## 🧪 Testing Data Storage

### Test Cases

1. **Save/Load Test**
   ```csharp
   PlayerData original = CreateTestPlayer();
   SavePlayerData(original);
   PlayerData loaded = LoadPlayerData();
   Assert.AreEqual(original.playerId, loaded.playerId);
   ```

2. **Corruption Test**
   ```csharp
   // Corrupt file and ensure graceful handling
   CorruptSaveFile();
   PlayerData loaded = LoadPlayerData();
   Assert.IsNotNull(loaded); // Should create new
   ```

3. **Concurrency Test**
   ```csharp
   // Rapid save operations
   for (int i = 0; i < 100; i++)
   {
       player.level = i;
       SavePlayerData(player);
   }
   Assert.AreEqual(99, LoadPlayerData().level);
   ```

---

## 📊 Performance Considerations

### File Size Estimates

| Data Type | Typical Size | Max Size |
|-----------|--------------|----------|
| PlayerData | ~1-2 KB | ~10 KB |
| ProgressData | ~2-5 KB | ~20 KB |
| BaseLayoutData | ~5-20 KB | ~100 KB |
| FriendData (per friend) | ~200 bytes | ~500 bytes |
| AchievementData (per) | ~150 bytes | ~300 bytes |

### Optimization Tips

1. **Batch Saves**: Group related data saves
2. **Delta Updates**: Only save changed data
3. **Compression**: Use gzip for large saves
4. **Lazy Loading**: Load data on-demand
5. **Caching**: Keep frequently accessed data in memory

---

## 🛠️ Development Tools

### Debug Commands

```csharp
// Clear all save data
LocalStorageManager.Instance.ClearAllData();

// Export save data
string json = ExportSaveData();
Debug.Log(json);

// Import save data (for testing)
ImportSaveData(testJson);

// Validate save integrity
bool valid = ValidateSaveData();
```

---

## 📚 Best Practices

1. **Always validate loaded data**
2. **Use try-catch for file operations**
3. **Implement auto-save frequently** (every 60s)
4. **Keep backups** (previous save file)
5. **Version your save format**
6. **Test cross-platform compatibility**
7. **Handle file access errors gracefully**
8. **Don't store passwords or tokens**
9. **Use atomic file writes** (write to temp, then rename)
10. **Log save/load operations** for debugging

---

## 🔗 Related Documentation

- [Technical Architecture](TECHNICAL_ARCHITECTURE.md)
- [Photon Integration Guide](PHOTON_INTEGRATION_GUIDE.md)
- [Performance Monitoring Guide](PERFORMANCE_MONITORING_GUIDE.md)

---

*Last Updated: 2025-12-19*
*Schema Version: 1.0*
