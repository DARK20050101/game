# Technical Architecture Documentation

## 1. Game Engine Selection: Unity

### Why Unity?
- **Pixel-Perfect Rendering**: Built-in Pixel Perfect Camera component
- **Cross-Platform Support**: Easy deployment to PC, Mobile, Console
- **Strong 2D Pipeline**: Comprehensive 2D toolset and sprite management
- **Large Community**: Extensive resources and asset store
- **Photon Integration**: Native support for Photon PUN 2

### Unity Configuration
- **Unity Version**: 2021.3 LTS or higher (recommended)
- **Rendering Pipeline**: Universal Render Pipeline (URP) for optimized 2D rendering
- **Pixel Perfect Camera**: Ensures crisp pixel art at any resolution
- **Target Platforms**: Windows, macOS, Linux (initial), Mobile (future)

### Performance Targets
- **Frame Rate**: Stable 60 FPS
- **Resolution**: Pixel-perfect scaling (base resolution: 320x180, scaled 4x-6x)
- **Memory**: < 500MB for core gameplay
- **Load Times**: < 3 seconds for scene transitions

---

## 2. Network Architecture: Photon PUN 2

### Integration Overview
Photon PUN 2 (Photon Unity Networking) provides real-time multiplayer capabilities with:
- **Low Latency**: < 200ms target for synchronization
- **Room-Based System**: Simple matchmaking and friend invites
- **State Synchronization**: Automatic syncing of player positions, actions, and game state
- **RPC Support**: Remote procedure calls for instant events

### Network Components

#### 2.1 NetworkManager
- Handles connection to Photon Cloud
- Manages room creation/joining
- Implements matchmaking logic (level difference ±3)
- Monitors connection quality

#### 2.2 Player Synchronization
**Synchronized Data:**
- Position & Rotation (10 updates/sec)
- Animation State (on change)
- Health/Stats (on change)
- Actions (attacks, skills) via RPC

**Optimization:**
- Interpolation for smooth movement
- Client-side prediction
- State compression

#### 2.3 World Synchronization
**Master Client Authority:**
- Map seed distribution (deterministic generation)
- Monster spawn/despawn
- Loot drop coordination
- World events

**Critical Sync Points:**
- Resource nodes (exact positioning)
- Monster locations (path-finding sync)
- Boss mechanics (phase transitions)

### Network Data Structures

```csharp
// Player State (Serialized)
struct PlayerNetworkState {
    int playerId;
    Vector2 position;
    float rotation;
    int health;
    int maxHealth;
    int level;
    string animationState;
    bool isAttacking;
}

// World State (Master Client)
struct WorldNetworkState {
    int mapSeed;
    Vector2[] monsterPositions;
    int[] monsterHealths;
    bool[] resourceNodeStates;
}

// Action Event (RPC)
struct ActionEvent {
    int playerId;
    ActionType type; // Attack, Skill, Interact
    Vector2 position;
    Vector2 direction;
    int targetId;
}
```

### Connection Flow
1. **Initialize**: Connect to Photon Cloud
2. **Authenticate**: User ID validation
3. **Lobby**: Browse/create rooms, friend invites
4. **Room Join**: Sync world state from master
5. **Gameplay**: Continuous state synchronization
6. **Disconnect**: Graceful cleanup, save progress

### Performance Monitoring
- **Ping Monitoring**: Display real-time latency
- **Packet Loss Detection**: Alert on >5% loss
- **Desync Detection**: Periodic state validation
- **Frame Drop Tracking**: Monitor client performance

---

## 3. Data Storage Architecture

### 3.1 Local Storage (Client-Side)

**Purpose**: Fast access to player-specific data that doesn't require server validation

**Storage Method**: PlayerPrefs (simple data) + JSON files (complex data)

**Data Categories:**

#### Player Progress
```json
{
  "playerId": "uuid",
  "characterName": "string",
  "level": 1,
  "experience": 0,
  "position": {"x": 0, "y": 0},
  "inventory": [],
  "equippedItems": {},
  "skillTree": {},
  "tutorialProgress": {},
  "settings": {
    "volume": 0.8,
    "resolution": "1920x1080",
    "fullscreen": true
  }
}
```

#### Base Layout
```json
{
  "baseId": "uuid",
  "buildings": [
    {
      "type": "workshop",
      "position": {"x": 10, "y": 5},
      "level": 2
    }
  ],
  "technologies": [],
  "resources": {
    "wood": 100,
    "stone": 50
  }
}
```

**Implementation:**
- Auto-save every 60 seconds
- Save on critical events (level up, major progress)
- Backup system (keep last 3 saves)
- Corruption detection and recovery

### 3.2 Cloud Storage (Server-Side)

**Purpose**: Shared data, achievements, and social features

**Storage Method**: Photon Cloud (custom properties) + Backend API (future)

**Data Categories:**

#### Friends & Social
```json
{
  "playerId": "uuid",
  "friends": ["friendId1", "friendId2"],
  "pendingInvites": [],
  "recentPlayers": [],
  "blockedPlayers": []
}
```

#### Achievements & Statistics
```json
{
  "playerId": "uuid",
  "achievements": [
    {
      "id": "first_boss",
      "unlockedAt": "2025-12-19T00:00:00Z",
      "progress": 1.0
    }
  ],
  "statistics": {
    "totalPlayTime": 3600,
    "monstersKilled": 150,
    "coopSessionsCompleted": 5,
    "highestLevel": 10
  }
}
```

#### Co-op Session Data
```json
{
  "sessionId": "uuid",
  "participants": ["player1", "player2"],
  "startTime": "2025-12-19T00:00:00Z",
  "endTime": "2025-12-19T01:00:00Z",
  "achievements": [],
  "lootDistribution": {}
}
```

### 3.3 Data Synchronization Strategy

**Local → Cloud Sync:**
- Achievements unlocked → Immediate upload
- Statistics update → Batched every 5 minutes
- Friend list changes → Immediate sync
- Session completion → Upload at end

**Cloud → Local Sync:**
- On game start: Download latest cloud data
- On reconnect: Validate and merge changes
- Conflict resolution: Server data takes priority for achievements/friends

**Consistency Validation:**
- Checksum validation for critical data
- Periodic integrity checks
- Rollback capability for corrupted data

---

## 4. Performance Monitoring & KPIs

### 4.1 Network Performance

**Key Metrics:**
- **Latency**: Target < 200ms average
  - Excellent: < 100ms
  - Good: 100-150ms
  - Acceptable: 150-200ms
  - Poor: > 200ms

- **Packet Loss**: Target < 1%
  - Monitor: Log all packets > 5% loss
  - Alert: Notify player when > 10% loss

- **Synchronization Accuracy**:
  - Position deviation: < 0.5 units
  - Action timing deviation: < 100ms
  - State consistency: 99.9% agreement

**Monitoring Implementation:**
```csharp
class NetworkPerformanceMonitor {
    float averagePing;
    float packetLossRate;
    int desyncEvents;
    
    void Update() {
        // Track ping every frame
        // Calculate packet loss over 10s window
        // Detect desyncs via state comparison
    }
}
```

### 4.2 Client Performance

**Frame Rate:**
- Target: Stable 60 FPS
- Minimum: 30 FPS
- Alert: Log when < 45 FPS for > 5 seconds

**Frame Drop Detection:**
```csharp
class FrameRateMonitor {
    Queue<float> frameTimes;
    int frameDropCount;
    
    void LateUpdate() {
        float deltaTime = Time.unscaledDeltaTime;
        if (deltaTime > 0.033f) { // > 30 FPS drop
            frameDropCount++;
            LogFrameDrop();
        }
    }
}
```

### 4.3 Data Consistency

**Validation Tests:**
1. **Save/Load Integrity**
   - Write test data → Save → Load → Compare
   - Test with corrupted files
   - Test with version migrations

2. **Network State Sync**
   - Compare master state vs client state
   - Validate monster positions (< 1 unit deviation)
   - Verify loot distribution matches

3. **Concurrent Modification**
   - Test simultaneous saves
   - Test network interruption during save
   - Test multiple clients modifying shared state

**Automated Testing:**
```csharp
class DataConsistencyTests {
    [Test] void SaveLoadIntegrity();
    [Test] void NetworkStateSyncAccuracy();
    [Test] void CloudSyncConflictResolution();
    [Test] void CorruptionRecovery();
}
```

### 4.4 Performance Benchmarks

**Target Specifications:**
- **Concurrent Players**: Handle 200+ simultaneous connections (server-side)
- **Room Size**: 2-player rooms (primary), scalable to 4-player (future)
- **Scene Load Time**: < 3 seconds
- **Network Traffic**: < 10 KB/s per player (average)
- **Memory Usage**: < 500 MB (client)
- **CPU Usage**: < 30% (single core) on minimum spec hardware

**Minimum Hardware Specs:**
- CPU: Intel Core i3 / AMD Ryzen 3
- RAM: 4 GB
- GPU: Integrated graphics (Intel HD 4000+)
- Network: 1 Mbps down/up

---

## 5. Development Roadmap

### Phase 1: Foundation (Current)
- ✅ Unity project structure
- ✅ Photon PUN 2 integration documentation
- ✅ Data storage architecture design
- ✅ Performance monitoring framework

### Phase 2: Network Implementation (Next)
- [ ] Implement NetworkManager
- [ ] Create player synchronization system
- [ ] Test 2-player connection and sync
- [ ] Implement performance monitoring

### Phase 3: Data Systems (Future)
- [ ] Implement local save system
- [ ] Create cloud storage integration
- [ ] Build data sync manager
- [ ] Add consistency validation

### Phase 4: Optimization & Testing (Future)
- [ ] Performance profiling and optimization
- [ ] Network stress testing (200+ CCU simulation)
- [ ] Cross-platform testing
- [ ] Documentation and deployment guides

---

## 6. Technology Stack Summary

| Component | Technology | Purpose |
|-----------|-----------|---------|
| Game Engine | Unity 2021.3 LTS+ | Core game development |
| Rendering | Universal Render Pipeline | Optimized 2D rendering |
| Pixel Perfect | Pixel Perfect Camera | Crisp pixel art scaling |
| Networking | Photon PUN 2 | Real-time multiplayer |
| Local Storage | PlayerPrefs + JSON | Client-side data persistence |
| Cloud Storage | Photon Custom Properties | Server-side shared data |
| Version Control | Git + GitHub | Source control |
| Build Platform | Unity Cloud Build (future) | Automated builds |

---

## 7. Next Steps

1. **Set up Unity Project**
   - Install Unity 2021.3 LTS
   - Import URP package
   - Configure project settings for 2D pixel art

2. **Integrate Photon PUN 2**
   - Create Photon account
   - Import Photon PUN 2 from Asset Store
   - Configure AppId and region settings

3. **Implement Core Systems**
   - Create NetworkManager script
   - Implement basic player synchronization
   - Build local save/load system

4. **Testing & Validation**
   - Test 2-player connection locally
   - Validate sync accuracy
   - Test data persistence

---

*Document Version: 1.0*  
*Last Updated: 2025-12-19*
