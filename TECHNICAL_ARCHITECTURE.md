# Technical Architecture Documentation

## Pixel Co-op RPG - Technical Framework

### 📋 Overview

This document outlines the technical architecture of the Pixel Co-op RPG game, including engine selection, networking infrastructure, and data storage solutions.

---

## 🎮 Engine Selection

### Chosen Engine: Unity 2022.3.10f1

**Rationale:**
- **Pixel-Perfect Support**: Unity provides robust 2D pixel-art rendering through the `com.unity.2d.pixel-perfect` package
- **Cross-Platform**: Build for PC, mobile, and consoles from a single codebase
- **Photon Integration**: Native support for Photon PUN 2 networking
- **Community & Assets**: Large ecosystem of pixel-art assets and tools
- **Performance**: Excellent 2D performance with optimized rendering pipeline

### Key Unity Packages

| Package | Version | Purpose |
|---------|---------|---------|
| `com.unity.2d.pixel-perfect` | 5.0.3 | Pixel-perfect camera rendering |
| `com.unity.2d.animation` | 9.0.4 | 2D sprite animation |
| `com.unity.2d.tilemap` | 1.0.0 | Tilemap system for levels |
| `com.unity.nuget.newtonsoft-json` | 3.2.1 | JSON serialization |

### Performance Targets

- **Frame Rate**: 60 FPS minimum on target platforms
- **Resolution**: Supports pixel-perfect scaling from 240p to 4K
- **Build Size**: Target < 500MB for initial release

---

## 🌐 Networking Architecture

### Photon PUN 2 Integration

**Why Photon PUN 2:**
- **Low Latency**: Optimized for real-time multiplayer (target ≤200ms)
- **Easy Integration**: Simple API for Unity developers
- **Scalability**: Cloud-based infrastructure handles server management
- **Free Tier**: 20 CCU free tier for development and testing
- **Regional Servers**: Global server distribution for optimal latency

### Network Components

#### 1. PhotonNetworkManager (`PhotonNetworkManager.cs`)

**Purpose**: Core network manager handling connection and room management

**Key Features:**
- Automatic reconnection logic
- Room creation/joining with matchmaking
- Player limit enforcement (2 players per room)
- Connection state monitoring
- Integration with performance tracker

**Usage:**
```csharp
// Connect to Photon
PhotonNetworkManager.Instance.ConnectToPhoton();

// Join or create room
PhotonNetworkManager.Instance.JoinOrCreateRoom("RoomName");

// Check connection status
bool isConnected = PhotonNetworkManager.Instance.IsConnected;
```

#### 2. PlayerNetworkSync (`PlayerNetworkSync.cs`)

**Purpose**: Synchronizes player position, rotation, and state across network

**Key Features:**
- Position/rotation interpolation for smooth movement
- Optimized update rate (20 updates/second by default)
- Animation state synchronization
- RPC support for important events
- Network lag tracking

**Optimization:**
- Position threshold filtering (only send if moved > 0.01 units)
- Velocity prediction for smoother remote player movement
- Configurable send rate to balance accuracy vs bandwidth

**Usage:**
```csharp
// Set animation state (automatically synced)
playerNetworkSync.SetAnimationState("run");

// Send synchronized event
playerNetworkSync.SendSyncEvent("attack", "sword_slash");
```

#### 3. NetworkPerformanceTracker (`NetworkPerformanceTracker.cs`)

**Purpose**: Monitors and reports network performance metrics

**Key Metrics:**
- **Latency/Ping**: Current and average latency (target ≤200ms)
- **Frame Drops**: Count of frames below 80% of target FPS
- **Sync Accuracy**: Distance-based synchronization accuracy
- **Connection Uptime**: Total connected time
- **Disconnect Count**: Number of disconnections

**Performance Thresholds:**
- **Excellent**: Latency ≤200ms, FPS ≥54 (90% of 60 FPS)
- **Good**: Latency ≤500ms, FPS ≥42 (70% of 60 FPS)
- **Fair**: Latency ≤750ms
- **Poor**: Latency >750ms

**Usage:**
```csharp
// Get performance report
var report = NetworkPerformanceTracker.Instance.GenerateReport();
Debug.Log($"Average Latency: {report.averageLatency}ms");
Debug.Log($"Performance Status: {report.performanceStatus}");
```

### Network Data Structures

**NetworkSyncData**: Real-time player synchronization
- Position, velocity, animation state
- Timestamp for lag compensation
- Health and status effects

**SessionData**: Matchmaking with level restrictions
- Level-based matchmaking (±3 levels)
- Room capacity management
- Session metadata

---

## 💾 Data Storage Architecture

### Hybrid Storage Approach

The game uses a **hybrid storage architecture** with clear separation of concerns:

- **Local Storage**: Player progress, character data, base layouts
- **Cloud Storage**: Friends list, achievements, leaderboards

### Local Storage System

#### LocalStorageManager (`LocalStorageManager.cs`)

**Storage Location**: `Application.persistentDataPath`
- **Windows**: `%APPDATA%\LocalLow\CompanyName\GameName`
- **macOS**: `~/Library/Application Support/CompanyName/GameName`
- **Linux**: `~/.config/unity3d/CompanyName/GameName`
- **Mobile**: Platform-specific persistent data directory

**Data Files:**
1. `player_data.json` - Character stats, inventory, position
2. `progress_data.json` - Quest completion, unlocked areas, playtime
3. `base_layout.json` - Base building placements and upgrades

**Features:**
- Auto-save functionality (configurable interval)
- JSON serialization with Newtonsoft.Json
- Atomic file operations for data integrity
- Future-ready for encryption

**Usage:**
```csharp
// Save player data
PlayerData player = new PlayerData();
LocalStorageManager.Instance.SavePlayerData(player);

// Load player data
PlayerData loaded = LocalStorageManager.Instance.LoadPlayerData();

// Check if data exists
bool hasData = LocalStorageManager.Instance.DataExists("player");
```

### Cloud Storage System

#### CloudStorageManager (`CloudStorageManager.cs`)

**Purpose**: Manages online-only data (friends, achievements, stats)

**Architecture Note**: 
Current implementation is a **placeholder framework** ready for integration with:
- **Firebase Realtime Database** / Firestore
- **PlayFab** 
- **Custom REST API Backend**

**Features:**
- Async/await pattern for non-blocking operations
- Retry logic with exponential backoff
- Local caching for offline support
- Connection state monitoring

**Data Types:**
1. Friends List - Social connections, online status
2. Achievements - Unlocked achievements, progress
3. Leaderboards - (future enhancement)

**Usage:**
```csharp
// Save friends list to cloud
List<FriendData> friends = new List<FriendData>();
await CloudStorageManager.Instance.SaveFriendsList(playerId, friends);

// Load achievements from cloud
List<AchievementData> achievements = 
    await CloudStorageManager.Instance.LoadAchievements(playerId);

// Update online status
await CloudStorageManager.Instance.UpdatePlayerOnlineStatus(playerId, true);
```

### Data Synchronization System

#### DataSyncManager (`DataSyncManager.cs`)

**Purpose**: Coordinates synchronization between local and cloud storage

**Features:**
- Auto-sync at configurable intervals (default: 5 minutes)
- Conflict resolution strategies:
  - `LocalFirst`: Always prefer local data
  - `CloudFirst`: Always prefer cloud data  
  - `MostRecent`: Use most recently modified data
- Sync operation queue with retry logic
- Comprehensive sync status reporting

**Sync Process:**
1. Load data from local storage
2. Load data from cloud storage (if connected)
3. Compare timestamps
4. Resolve conflicts using configured strategy
5. Write merged data to both local and cloud
6. Update sync status and metrics

**Usage:**
```csharp
// Force immediate sync
DataSyncManager.Instance.ForceSync();

// Get sync status
var status = DataSyncManager.Instance.GetSyncStatus();
Debug.Log($"Pending operations: {status.pendingOperations}");

// Queue failed operation for retry
var operation = new SyncOperation(SyncOperationType.PlayerData);
DataSyncManager.Instance.QueueSyncOperation(operation);
```

---

## 🔍 Data Consistency Testing

### DataConsistencyTester (`DataConsistencyTester.cs`)

**Purpose**: Automated testing framework for data integrity

**Test Categories:**

1. **Local Storage Tests**
   - Save/Load operations
   - Data integrity across multiple saves
   - Concurrent operation handling

2. **Cloud Storage Tests**
   - Connection reliability
   - Save/Load functionality
   - Error handling and retries

3. **Synchronization Tests**
   - Consistency between local and cloud
   - Conflict resolution accuracy
   - Sync operation reliability

4. **Data Structure Tests**
   - Serialization/deserialization
   - Structure validity
   - Cross-platform compatibility

**Usage:**
```csharp
// Run all tests
DataConsistencyTester tester = GetComponent<DataConsistencyTester>();
tester.RunAllTests();

// Get test results
List<TestResult> results = tester.GetTestResults();
```

**Test Output Example:**
```
[DataConsistencyTester] ===== TEST REPORT =====
Total Tests: 9
Passed: 9
Failed: 0
Pass Rate: 100.0%
================================
```

---

## 📊 Cross-Platform Data Structures

All data structures are designed for cross-platform compatibility:

### PlayerData
- Character stats (level, health, mana)
- Position and current scene
- Inventory and gold
- Timestamps for synchronization

### BaseLayoutData
- Building placements and types
- Building levels and construction status
- Owner identification
- Last modified timestamp

### GameProgressData
- Quest completion tracking
- Unlocked areas
- Total playtime
- Achievement progress

### FriendData
- Friend identification and status
- Online/offline state
- Last seen timestamp

### AchievementData
- Achievement unlock status
- Progress tracking (0.0 to 1.0)
- Unlock timestamp

---

## 🚀 Getting Started

### Initial Setup

1. **Open Unity Project**
   ```bash
   cd /path/to/game
   # Open with Unity 2022.3.10f1 or later
   ```

2. **Install Photon PUN 2**
   - Download from Unity Asset Store or Photon website
   - Import into project
   - Configure AppId in PhotonServerSettings

3. **Setup Network Managers**
   - Create empty GameObject "NetworkManager"
   - Add `PhotonNetworkManager` component
   - Add `NetworkPerformanceTracker` component

4. **Setup Data Managers**
   - Create empty GameObject "DataManager"
   - Add `LocalStorageManager` component
   - Add `CloudStorageManager` component
   - Add `DataSyncManager` component

### Testing Network Connection

```csharp
// In a MonoBehaviour
void Start()
{
    // Connect to Photon
    PhotonNetworkManager.Instance.ConnectToPhoton();
}

void Update()
{
    if (PhotonNetworkManager.Instance.IsConnected && Input.GetKeyDown(KeyCode.J))
    {
        // Join random room
        PhotonNetworkManager.Instance.JoinOrCreateRoom();
    }
}
```

### Testing Data Storage

```csharp
void TestDataStorage()
{
    // Create and save player data
    PlayerData player = new PlayerData
    {
        playerName = "TestPlayer",
        level = 10
    };
    
    LocalStorageManager.Instance.SavePlayerData(player);
    
    // Load player data
    PlayerData loaded = LocalStorageManager.Instance.LoadPlayerData();
    Debug.Log($"Loaded: {loaded.playerName}, Level {loaded.level}");
}
```

---

## 📈 Performance Monitoring

### Key Performance Indicators (KPIs)

1. **Network Performance**
   - Target latency: ≤200ms
   - Maximum acceptable latency: 500ms
   - Target FPS: 60
   - Frame drop threshold: <5% of frames

2. **Synchronization Accuracy**
   - Position sync threshold: 0.01 units
   - Update rate: 20 Hz (configurable)
   - Sync accuracy: <0.1 units average drift

3. **Data Consistency**
   - Save operation success rate: >99%
   - Cloud sync success rate: >95%
   - Data integrity test pass rate: 100%

### Monitoring Tools

- **NetworkPerformanceTracker**: Real-time network metrics
- **DataConsistencyTester**: Automated data integrity tests
- **Unity Profiler**: Built-in performance profiling

---

## 🔧 Configuration

### Network Settings

Edit in `PhotonNetworkManager` component:
- **Game Version**: Version string for matchmaking
- **Max Players Per Room**: 2 (for co-op gameplay)
- **Auto Connect**: Enable/disable automatic connection

### Storage Settings

Edit in `LocalStorageManager` component:
- **Auto Save**: Enable/disable auto-save
- **Auto Save Interval**: Seconds between auto-saves (default: 60)
- **Enable Encryption**: Future feature flag

Edit in `CloudStorageManager` component:
- **Enable Cloud Sync**: Enable/disable cloud features
- **Sync Interval**: Seconds between cloud syncs (default: 120)
- **Max Retries**: Number of retry attempts (default: 3)

### Sync Settings

Edit in `DataSyncManager` component:
- **Auto Sync**: Enable/disable automatic synchronization
- **Sync Interval**: Seconds between sync operations (default: 300)
- **Conflict Strategy**: LocalFirst / CloudFirst / MostRecent

---

## 🔐 Security Considerations

### Current Implementation
- Local data stored in JSON (unencrypted)
- Cloud operations use HTTPS (when integrated)
- Player IDs use GUID for uniqueness

### Future Enhancements
- Local data encryption
- Cloud authentication (OAuth, Firebase Auth)
- Anti-cheat measures for competitive features
- Rate limiting for API calls

---

## 📝 Next Steps

1. **Photon Setup**: Configure AppId and test connection
2. **Player Controller**: Implement character movement with network sync
3. **Map Generation**: Random terrain generation with network synchronization
4. **Cloud Backend**: Integrate Firebase/PlayFab for production
5. **Matchmaking**: Implement level-based matchmaking (±3 levels)
6. **UI Development**: Create network status display and data management UI

---

## 🤝 Contributing

When adding new features:
1. Follow existing code structure and naming conventions
2. Add XML documentation comments
3. Update relevant data structures in `DataStructures.cs`
4. Add tests to `DataConsistencyTester` if applicable
5. Update this documentation

---

## 📚 Additional Resources

- [Unity 2D Documentation](https://docs.unity3d.com/Manual/2Dgameplay.html)
- [Photon PUN 2 Documentation](https://doc.photonengine.com/pun/current/getting-started/pun-intro)
- [Pixel Perfect Package](https://docs.unity3d.com/Packages/com.unity.2d.pixel-perfect@5.0/manual/index.html)
- [Unity Networking Best Practices](https://docs.unity3d.com/Manual/NetworkProgramming.html)

---

*Last Updated: 2025-12-19*
*Framework Version: 1.0*
