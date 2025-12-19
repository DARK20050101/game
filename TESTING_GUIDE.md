# Testing & Validation Guide

This document outlines the testing procedures and validation criteria for the technical framework.

## Overview

The testing framework covers four main areas:
1. **Network Functionality** - Connection, synchronization, room management
2. **Data Persistence** - Save/load operations, backup system
3. **Performance Metrics** - Frame rate, latency, desync detection
4. **Cross-Platform** - Multi-client testing

---

## 1. Network Testing

### 1.1 Connection Tests

#### Test: Basic Connection
**Objective**: Verify client can connect to Photon Master Server

**Steps**:
1. Launch game
2. Call `NetworkManager.Instance.Connect()`
3. Wait for connection

**Expected Results**:
- Connection established within 5 seconds
- Console shows: "Connected to Photon Master Server"
- Network status shows: "Connected"

**Pass Criteria**:
- ✓ Connection successful
- ✓ No timeout errors
- ✓ Automatic lobby join

---

#### Test: Room Creation
**Objective**: Verify room creation and configuration

**Steps**:
1. Connect to Photon
2. Call `NetworkManager.Instance.CreateRoom()`
3. Verify room properties

**Expected Results**:
- Room created with unique name
- Max players = 2
- Custom properties set (level range)
- Console shows: "Room created successfully"

**Pass Criteria**:
- ✓ Room appears in lobby
- ✓ Properties correctly set
- ✓ Creator is master client

---

#### Test: Matchmaking
**Objective**: Verify level-based matchmaking works

**Steps**:
1. Client A (Level 5) creates room
2. Client B (Level 6) calls `JoinRandomRoom()`
3. Verify successful match

**Expected Results**:
- Client B joins Client A's room
- Both clients see each other
- Room player count = 2

**Pass Criteria**:
- ✓ Match found within level tolerance (±3)
- ✓ Both clients synchronized
- ✓ No matchmaking errors

**Negative Test**:
- Client C (Level 15) should NOT match with level 5 room
- Should create new room instead

---

### 1.2 Synchronization Tests

#### Test: Position Synchronization
**Objective**: Verify player position sync accuracy

**Setup**:
- Two clients connected in same room
- Each spawns player prefab
- Move Player A to position (5, 3)

**Expected Results**:
- Player B sees Player A at (5, 3)
- Deviation < 0.5 units
- Update latency < 200ms

**Pass Criteria**:
- ✓ Position matches within tolerance
- ✓ Smooth interpolation
- ✓ No visible jitter

**Measurement**:
```csharp
// On Client B
Vector3 localPos = playerA_LocalTransform.position;
Vector3 networkPos = playerA_NetworkSync.networkPosition;
float deviation = Vector3.Distance(localPos, networkPos);
Assert.IsTrue(deviation < 0.5f, "Position desync detected");
```

---

#### Test: Action Synchronization (RPC)
**Objective**: Verify RPC calls propagate correctly

**Steps**:
1. Client A triggers attack action
2. Verify Client B receives RPC
3. Measure latency

**Expected Results**:
- RPC received by Client B
- Latency < 100ms
- Correct parameters received

**Pass Criteria**:
- ✓ RPC executed on all clients
- ✓ Animation plays on both clients
- ✓ No packet loss

---

#### Test: World State Synchronization
**Objective**: Verify master client broadcasts world state

**Steps**:
1. Master client spawns 5 monsters
2. Non-master client validates state
3. Master client kills monster #3
4. Verify state update on all clients

**Expected Results**:
- All clients have identical monster positions
- Monster death propagated to all clients
- State validation passes

**Pass Criteria**:
- ✓ Monster count matches (4 after kill)
- ✓ Positions within 1 unit tolerance
- ✓ No ghost monsters (desynced entities)

---

### 1.3 Edge Case Tests

#### Test: Master Client Migration
**Objective**: Verify smooth master client handoff

**Steps**:
1. Client A (master) and Client B connected
2. Client A disconnects
3. Verify Client B becomes new master

**Expected Results**:
- Client B promoted to master
- World state preserved
- No game interruption

**Pass Criteria**:
- ✓ New master assigned within 1 second
- ✓ Game continues seamlessly
- ✓ No data loss

---

#### Test: Reconnection
**Objective**: Handle network interruption gracefully

**Steps**:
1. Client connected in room
2. Simulate network loss (disconnect WiFi briefly)
3. Restore connection

**Expected Results**:
- Client detects disconnection
- Automatic reconnection attempt
- State resynchronization

**Pass Criteria**:
- ✓ Reconnection successful within 10 seconds
- ✓ Player data intact
- ✓ Game state recovered

---

## 2. Data Persistence Testing

### 2.1 Local Storage Tests

#### Test: Save Player Data
**Objective**: Verify data saves correctly

**Steps**:
1. Modify player data (level, name, inventory)
2. Call `DataManager.Instance.ForceSave()`
3. Verify file creation

**Expected Results**:
- File created at correct path
- JSON format valid
- All data fields present

**Pass Criteria**:
- ✓ File exists
- ✓ Data readable
- ✓ No corruption

**Validation**:
```bash
# Check save file
cat ~/Library/Application\ Support/CompanyName/ProductName/SaveData/player.json
# Should show valid JSON with player data
```

---

#### Test: Load Player Data
**Objective**: Verify data loads correctly

**Steps**:
1. Restart game
2. Call `DataManager.Instance.LoadPlayerData()`
3. Compare loaded data with saved data

**Expected Results**:
- All fields match original
- No data loss
- Load time < 1 second

**Pass Criteria**:
- ✓ Data integrity maintained
- ✓ All nested objects restored
- ✓ No default values (unless appropriate)

---

#### Test: Backup System
**Objective**: Verify backup creation and recovery

**Steps**:
1. Save data (creates player.json)
2. Save again (creates player.json.backup1)
3. Save third time (creates player.json.backup2)
4. Corrupt player.json manually
5. Restart game

**Expected Results**:
- Backup files created (up to 3)
- Corrupted file detected
- Automatic recovery from backup1
- Console shows: "Loaded from backup"

**Pass Criteria**:
- ✓ Backups created correctly
- ✓ Recovery successful
- ✓ Oldest backup rotated out

---

#### Test: Auto-Save
**Objective**: Verify periodic auto-save works

**Steps**:
1. Start game with auto-save interval = 60s
2. Modify player data
3. Wait 60 seconds
4. Check for save trigger

**Expected Results**:
- Save triggered automatically
- Console shows: "All data saved"
- No gameplay interruption

**Pass Criteria**:
- ✓ Auto-save fires on schedule
- ✓ No performance impact
- ✓ "Dirty" flag cleared after save

---

### 2.2 Cloud Storage Tests

#### Test: Achievement Sync
**Objective**: Verify achievements sync to cloud

**Steps**:
1. Connect to Photon
2. Unlock achievement: "First Boss"
3. Verify sync to player properties
4. Disconnect and reconnect
5. Verify achievement persists

**Expected Results**:
- Achievement added to cloud data
- Photon custom properties updated
- Data persists across sessions

**Pass Criteria**:
- ✓ Achievement in CustomProperties
- ✓ Survives disconnect/reconnect
- ✓ Timestamp recorded correctly

---

#### Test: Friends List Sync
**Objective**: Verify friends list synchronizes

**Steps**:
1. Add friend "friend_123"
2. Verify sync to cloud
3. Open second client with same account
4. Verify friend appears

**Expected Results**:
- Friend added immediately
- Synced to Photon properties
- Visible across all sessions

**Pass Criteria**:
- ✓ Friend list updated
- ✓ Cloud sync confirmed
- ✓ No duplicate entries

---

#### Test: Statistics Batching
**Objective**: Verify statistics sync every 5 minutes

**Steps**:
1. Play game, generate statistics
2. Wait 5 minutes
3. Verify sync triggered

**Expected Results**:
- Statistics batched during gameplay
- Sync triggered at interval
- Console shows: "Statistics synced to cloud"

**Pass Criteria**:
- ✓ Batch interval respected
- ✓ All stats uploaded
- ✓ No sync spam

---

### 2.3 Data Consistency Tests

#### Test: Concurrent Modification
**Objective**: Ensure data integrity under concurrent access

**Steps**:
1. Client A modifies local data
2. Client B modifies cloud data simultaneously
3. Both save
4. Verify conflict resolution

**Expected Results**:
- Conflict detected
- Resolution strategy applied (server wins)
- No data corruption

**Pass Criteria**:
- ✓ Conflict handled gracefully
- ✓ Deterministic outcome
- ✓ User notified if needed

---

#### Test: Version Migration
**Objective**: Handle save format changes

**Steps**:
1. Create save with version 1.0 format
2. Update game to version 2.0 (new fields)
3. Load old save

**Expected Results**:
- Old save detected
- Migration performed
- New fields initialized with defaults
- Save updated to new format

**Pass Criteria**:
- ✓ No load errors
- ✓ Backward compatibility
- ✓ User progress preserved

---

## 3. Performance Testing

### 3.1 Network Performance

#### Test: Latency Under Normal Conditions
**Objective**: Measure baseline network performance

**Setup**: Two clients, stable network

**Measurements**:
- Average ping over 60 seconds
- Packet loss rate
- Desync events

**Expected Results**:
- Ping: < 100ms average
- Packet loss: < 1%
- Desyncs: 0

**Pass Criteria**:
- ✓ Meets latency target (< 200ms)
- ✓ Stable connection
- ✓ No desyncs

---

#### Test: Latency Under Load
**Objective**: Measure performance under network stress

**Setup**: Two clients, artificial latency added (100ms)

**Expected Results**:
- Ping: ~100ms + network overhead (< 150ms)
- Interpolation compensates for lag
- Gameplay still smooth

**Pass Criteria**:
- ✓ Performance degradation < 50%
- ✓ No timeout disconnections
- ✓ User experience acceptable

---

#### Test: Bandwidth Usage
**Objective**: Measure network traffic

**Setup**: Monitor network traffic over 5 minutes

**Expected Results**:
- Upstream: < 10 KB/s per player
- Downstream: < 15 KB/s per player
- No traffic spikes

**Pass Criteria**:
- ✓ Within bandwidth budget
- ✓ Consistent traffic pattern
- ✓ No memory leaks

---

### 3.2 Client Performance

#### Test: Frame Rate Stability
**Objective**: Maintain 60 FPS during gameplay

**Setup**: Single client, typical gameplay

**Measurements**:
- Average FPS over 5 minutes
- Frame drops count
- 1% low FPS

**Expected Results**:
- Average: 60 FPS
- Frame drops: < 10
- 1% low: > 45 FPS

**Pass Criteria**:
- ✓ Stable 60 FPS
- ✓ No sustained frame drops
- ✓ Smooth gameplay

---

#### Test: Memory Usage
**Objective**: Ensure no memory leaks

**Setup**: Play for 30 minutes, monitor memory

**Expected Results**:
- Starting memory: ~300 MB
- After 30 min: < 500 MB
- No continuous growth

**Pass Criteria**:
- ✓ Memory stable
- ✓ No leaks detected
- ✓ GC pauses < 5ms

---

### 3.3 Stress Testing

#### Test: Rapid State Changes
**Objective**: Handle frequent updates without issues

**Setup**: Spawn/despawn 50 objects rapidly

**Expected Results**:
- All clients stay synchronized
- No crashes
- Frame rate > 45 FPS

**Pass Criteria**:
- ✓ System handles load
- ✓ No desyncs
- ✓ Performance acceptable

---

## 4. Cross-Platform Testing

### 4.1 Multi-Client Testing

#### Test Configuration
- **Client A**: Unity Editor (Windows)
- **Client B**: Standalone Build (Windows)
- **Client C**: Standalone Build (Mac)

#### Test: Cross-Platform Compatibility
**Objective**: Verify different clients can connect

**Steps**:
1. Launch all three clients
2. All join same room
3. Test synchronization

**Expected Results**:
- All clients connect successfully
- Synchronization works across platforms
- No platform-specific issues

**Pass Criteria**:
- ✓ Cross-platform connectivity
- ✓ Identical behavior
- ✓ No serialization issues

---

## 5. Automated Test Suite

### Unit Tests

Create `Assets/Scripts/Testing/Editor/NetworkTests.cs`:

```csharp
using NUnit.Framework;
using UnityEngine;

public class NetworkTests
{
    [Test]
    public void NetworkManager_Singleton_NotNull()
    {
        // Verify singleton pattern
        GameObject go = new GameObject();
        go.AddComponent<NetworkManager>();
        Assert.IsNotNull(NetworkManager.Instance);
    }

    [Test]
    public void DataManager_SaveLoad_PreservesData()
    {
        // Test save/load cycle
        GameObject go = new GameObject();
        go.AddComponent<DataManager>();
        
        PlayerData originalData = new PlayerData
        {
            characterName = "TestHero",
            level = 10
        };
        
        DataManager.Instance.UpdatePlayerData(originalData);
        DataManager.Instance.ForceSave();
        DataManager.Instance.LoadPlayerData();
        
        PlayerData loadedData = DataManager.Instance.GetPlayerData();
        Assert.AreEqual("TestHero", loadedData.characterName);
        Assert.AreEqual(10, loadedData.level);
    }
}
```

### Integration Tests

Run full workflow tests:
1. Connect → Create Room → Spawn Player → Synchronize
2. Save Data → Disconnect → Reconnect → Load Data
3. Unlock Achievement → Sync Cloud → Verify Persistence

---

## 6. Performance Benchmarks

### Target KPIs

| Metric | Target | Acceptable | Poor |
|--------|--------|------------|------|
| Ping | < 100ms | < 150ms | < 200ms |
| FPS | 60 | 45-60 | < 45 |
| Packet Loss | < 1% | < 3% | < 5% |
| Position Desync | < 0.5 units | < 1 unit | > 1 unit |
| Save Time | < 100ms | < 500ms | > 1s |
| Load Time | < 1s | < 3s | > 5s |

### Acceptance Criteria

To pass validation, system must meet:
- ✓ All "Target" KPIs in ideal conditions
- ✓ All "Acceptable" KPIs in normal conditions
- ✓ No "Poor" ratings in any metric
- ✓ Zero critical bugs
- ✓ < 5 minor bugs

---

## 7. Test Execution Checklist

### Pre-Release Testing

- [ ] All network connection tests passed
- [ ] All synchronization tests passed
- [ ] All data persistence tests passed
- [ ] All performance benchmarks met
- [ ] Cross-platform testing completed
- [ ] Edge cases handled
- [ ] Automated tests passed
- [ ] Manual QA completed
- [ ] Performance profiling done
- [ ] Documentation updated

---

## 8. Bug Reporting Template

When issues are found:

```markdown
**Bug Title**: [Short description]

**Severity**: Critical / High / Medium / Low

**Steps to Reproduce**:
1. [Step 1]
2. [Step 2]
3. [Result]

**Expected Behavior**: [What should happen]

**Actual Behavior**: [What actually happens]

**Environment**:
- Unity Version: 
- Platform: 
- Network: 
- Photon Region: 

**Logs**: [Attach relevant console logs]

**Screenshots**: [If applicable]
```

---

*Last Updated: 2025-12-19*
