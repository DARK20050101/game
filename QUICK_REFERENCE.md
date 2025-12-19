# Quick Reference Guide

Quick reference for common operations in the technical framework.

## Network Operations

### Connect to Photon
```csharp
NetworkManager.Instance.Connect();
```

### Create a Room
```csharp
NetworkManager.Instance.CreateRoom("MyRoom");
// or auto-generate name:
NetworkManager.Instance.CreateRoom();
```

### Join Random Room (with matchmaking)
```csharp
NetworkManager.Instance.JoinRandomRoom();
// Automatically matches by player level ±3
```

### Join Specific Room
```csharp
NetworkManager.Instance.JoinRoom("RoomName");
```

### Leave Room
```csharp
NetworkManager.Instance.LeaveRoom();
```

### Check Connection Status
```csharp
string status = NetworkManager.Instance.GetConnectionStatus();
bool canStart = NetworkManager.Instance.CanStartGame();
```

---

## Player Synchronization

### Sync Animation
```csharp
PlayerNetworkSync playerSync = GetComponent<PlayerNetworkSync>();
playerSync.SyncAnimation("Attack");
```

### Trigger Action (RPC)
```csharp
playerSync.TriggerAction("Attack", transform.position, transform.forward);
```

---

## World Synchronization

### Spawn Monster (Master Client Only)
```csharp
if (PhotonNetwork.IsMasterClient)
{
    int monsterId = 1001;
    Vector3 position = new Vector3(10, 5, 0);
    int health = 100;
    
    WorldNetworkSync.Instance.RegisterMonsterSpawn(monsterId, position, health);
}
```

### Update Monster State
```csharp
WorldNetworkSync.Instance.UpdateMonsterState(monsterId, newPosition, newHealth);
```

### Monster Defeated
```csharp
WorldNetworkSync.Instance.MonsterDefeated(monsterId, lootPosition);
```

### Set Resource Node State
```csharp
WorldNetworkSync.Instance.SetResourceNodeState(nodeId, isActive: false);
```

---

## Data Management

### Save Player Data
```csharp
PlayerData data = DataManager.Instance.GetPlayerData();
data.level = 10;
data.experience = 5000;

DataManager.Instance.UpdatePlayerData(data);
DataManager.Instance.ForceSave(); // Immediate save
// or wait for auto-save (60 seconds)
```

### Load Player Data
```csharp
DataManager.Instance.LoadPlayerData();
PlayerData data = DataManager.Instance.GetPlayerData();
Debug.Log($"Player: {data.characterName}, Level: {data.level}");
```

### Save/Load All Data
```csharp
DataManager.Instance.SaveAllData();
DataManager.Instance.LoadAllData();
```

### Update Settings
```csharp
GameSettings settings = DataManager.Instance.GetSettings();
settings.masterVolume = 0.8f;
settings.fullscreen = true;

DataManager.Instance.UpdateSettings(settings);
// Automatically saves and applies
```

---

## Cloud Data

### Add Friend
```csharp
CloudDataManager.Instance.AddFriend("friend_user_id");
```

### Get Friends List
```csharp
List<string> friends = CloudDataManager.Instance.GetFriends();
```

### Unlock Achievement
```csharp
CloudDataManager.Instance.UnlockAchievement("first_boss", "Defeated First Boss");
```

### Update Achievement Progress
```csharp
CloudDataManager.Instance.UpdateAchievementProgress("boss_hunter", 0.75f);
```

### Update Statistics
```csharp
CloudDataManager.Instance.UpdateStatistics(stats =>
{
    stats.monstersKilled++;
    stats.totalPlayTime += Time.deltaTime;
});
```

---

## Performance Monitoring

### Get Network Performance
```csharp
int ping = NetworkPerformanceMonitor.Instance.GetCurrentPing();
float avgPing = NetworkPerformanceMonitor.Instance.GetAveragePing();
var quality = NetworkPerformanceMonitor.Instance.GetConnectionQuality();

Debug.Log($"Ping: {ping}ms, Quality: {quality}");
```

### Report Desync
```csharp
NetworkPerformanceMonitor.Instance.ReportDesync("Position mismatch", deviation: 1.5f);
```

### Get Frame Rate
```csharp
float fps = FrameRateMonitor.Instance.GetCurrentFPS();
float avgFps = FrameRateMonitor.Instance.GetAverageFPS();
int drops = FrameRateMonitor.Instance.GetFrameDropCount();
```

### Get Performance Summary
```csharp
string summary = FrameRateMonitor.Instance.GetPerformanceSummary();
Debug.Log(summary);
```

---

## Common Patterns

### Initialize Managers in Scene
```csharp
void Awake()
{
    // Managers auto-initialize as singletons
    // Just ensure GameObjects exist in scene with components:
    // - NetworkManager
    // - DataManager
    // - CloudDataManager
    // - NetworkPerformanceMonitor
    // - FrameRateMonitor
}
```

### Network Ready Check
```csharp
void Update()
{
    if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
    {
        // Network is ready for gameplay
        if (NetworkManager.Instance.CanStartGame())
        {
            // Can start match (2+ players)
        }
    }
}
```

### Photon Callbacks
```csharp
public class MyNetworkHandler : MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room!");
        // Spawn player, initialize game
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} joined!");
    }
}
```

---

## Data Models

### Create Player Data
```csharp
PlayerData player = new PlayerData();
player.characterName = "Hero";
player.level = 1;
player.position = Vector2.zero;
```

### Create Item
```csharp
ItemData item = new ItemData
{
    itemName = "Iron Sword",
    type = ItemType.Weapon,
    quantity = 1,
    rarity = 2, // Rare
    strengthBonus = 10
};

player.inventory.Add(item);
```

### Create Building
```csharp
BuildingData building = new BuildingData
{
    buildingType = "Workshop",
    position = new Vector2Int(5, 5),
    level = 1,
    isConstructed = true
};

BaseLayoutData baseLayout = DataManager.Instance.GetBaseLayout();
baseLayout.buildings.Add(building);
```

---

## Error Handling

### Network Errors
```csharp
NetworkManager.Instance.OnRoomJoinFailedCallback += () =>
{
    Debug.LogError("Failed to join room");
    // Show UI error message
};
```

### Save/Load Errors
All save/load operations have built-in error handling with logging.
Check Console for error messages if operations fail.

### Backup Recovery
If save file is corrupted, DataManager automatically attempts to load from backup:
```
Failed to load player data: [error]
Loaded from backup: player.json.backup1
```

---

## Testing Shortcuts

### Quick Connect and Create Room
```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.C))
        NetworkManager.Instance.Connect();
    
    if (Input.GetKeyDown(KeyCode.R))
        NetworkManager.Instance.CreateRoom();
    
    if (Input.GetKeyDown(KeyCode.J))
        NetworkManager.Instance.JoinRandomRoom();
}
```

### Performance Monitoring UI
Set these in Inspector:
- `NetworkPerformanceMonitor.showPerformanceUI = true`
- `FrameRateMonitor.showFPSCounter = true`

---

## Important Notes

### Master Client Authority
- Only Master Client can spawn/despawn monsters
- Always check: `if (PhotonNetwork.IsMasterClient)`
- Master Client migration is automatic

### Photon View Required
For network synchronized objects, ensure:
1. GameObject has `PhotonView` component
2. PhotonView observes the sync script
3. Object is instantiated via `PhotonNetwork.Instantiate()`

### Save Data Location
```csharp
string savePath = Application.persistentDataPath;
// Windows: %USERPROFILE%\AppData\LocalLow\CompanyName\ProductName
// Mac: ~/Library/Application Support/CompanyName/ProductName
```

---

## Performance Tips

1. **Reduce Sync Frequency**: Only sync what changes
2. **Use RPCs Sparingly**: Batch updates when possible
3. **Compress Data**: Use compact data structures
4. **Master Client Authority**: Centralize world state
5. **Client Prediction**: Interpolate for smooth movement

---

## Debugging

### Enable Logging
```csharp
NetworkPerformanceMonitor.Instance.enableLogging = true;
DataManager.Instance.enableEncryption = false; // For debugging
```

### Check Network State
```csharp
Debug.Log($"Connected: {PhotonNetwork.IsConnected}");
Debug.Log($"In Room: {PhotonNetwork.InRoom}");
Debug.Log($"Is Master: {PhotonNetwork.IsMasterClient}");
Debug.Log($"Players: {PhotonNetwork.CurrentRoom?.PlayerCount}");
```

### Inspect Save Files
Navigate to save directory and open JSON files with text editor.

---

*Quick Reference v1.0 - Last Updated: 2025-12-19*
