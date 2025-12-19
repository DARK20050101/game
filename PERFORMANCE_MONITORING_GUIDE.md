# Performance Monitoring Guide

## Overview

This guide covers performance monitoring, metrics collection, and optimization strategies for the Pixel Co-op RPG multiplayer game.

---

## 🎯 Performance Targets

### Network Performance

| Metric | Target | Acceptable | Poor |
|--------|--------|-----------|------|
| **Latency** | ≤200ms | ≤500ms | >500ms |
| **Jitter** | <50ms | <100ms | >100ms |
| **Packet Loss** | <1% | <5% | >5% |
| **Frame Rate** | 60 FPS | 45 FPS | <30 FPS |
| **Sync Accuracy** | <0.1 units | <0.5 units | >0.5 units |

### Data Storage Performance

| Operation | Target | Acceptable |
|-----------|--------|-----------|
| **Local Save** | <50ms | <200ms |
| **Local Load** | <100ms | <500ms |
| **Cloud Save** | <500ms | <2000ms |
| **Cloud Load** | <1000ms | <3000ms |
| **Data Sync** | <2000ms | <5000ms |

### System Performance

| Metric | Target | Minimum |
|--------|--------|---------|
| **FPS** | 60 | 30 |
| **Memory Usage** | <500MB | <1GB |
| **CPU Usage** | <50% | <80% |
| **Load Time** | <3s | <10s |

---

## 📊 Monitoring Tools

### 1. NetworkPerformanceTracker

**Purpose**: Real-time network performance monitoring

**Metrics Collected:**
- Current ping/latency
- Average latency over time
- Frame rate (FPS)
- Frame drop count
- Synchronization accuracy
- Connection uptime
- Disconnect count

**Setup:**
```csharp
// Add to NetworkManager GameObject
GameObject networkManager = new GameObject("NetworkManager");
networkManager.AddComponent<NetworkPerformanceTracker>();
```

**Usage:**
```csharp
// Get current metrics
var tracker = NetworkPerformanceTracker.Instance;
Debug.Log($"Ping: {tracker.CurrentPing}ms");
Debug.Log($"Average Latency: {tracker.AverageLatency * 1000}ms");
Debug.Log($"Average FPS: {tracker.AverageFPS}");

// Get comprehensive report
var report = tracker.GenerateReport();
Debug.Log($"Performance Status: {report.performanceStatus}");
```

**Report Structure:**
```csharp
public struct PerformanceReport
{
    public int currentPing;           // Current ping in ms
    public float averageLatency;      // Average latency in ms
    public float averageFPS;          // Average frame rate
    public int frameDrops;            // Total frame drops
    public float syncAccuracy;        // Sync accuracy metric
    public float connectionUptime;    // Uptime in seconds
    public int totalDisconnects;      // Disconnect count
    public string performanceStatus;  // Overall status
}
```

---

### 2. Unity Profiler

**Access**: Window → Analysis → Profiler

**Key Areas to Monitor:**

#### CPU Usage
```
Modules to watch:
├── Rendering (should be <30% for 2D pixel game)
├── Scripts (game logic)
├── Physics (2D physics calculations)
└── Networking (Photon overhead)
```

#### Memory
```
Track these allocations:
├── Total Allocated (target: <500MB)
├── GC Allocated (minimize per frame)
├── Textures (pixel sprites)
└── Audio clips
```

#### Rendering
```
Monitor:
├── Batches (minimize draw calls)
├── SetPass calls (combine materials)
├── Triangles/Vertices (keep low for 2D)
└── Used Textures (atlas usage)
```

---

### 3. Custom Performance Logging

**Implementation:**

```csharp
public class PerformanceLogger : MonoBehaviour
{
    [SerializeField] private float logInterval = 5f;
    private float nextLogTime;
    
    void Update()
    {
        if (Time.time >= nextLogTime)
        {
            LogPerformanceMetrics();
            nextLogTime = Time.time + logInterval;
        }
    }
    
    void LogPerformanceMetrics()
    {
        // Network metrics
        var networkReport = NetworkPerformanceTracker.Instance.GenerateReport();
        
        // System metrics
        float fps = 1f / Time.unscaledDeltaTime;
        long memoryUsage = System.GC.GetTotalMemory(false) / 1048576; // MB
        
        // Data sync metrics
        var syncStatus = DataSyncManager.Instance.GetSyncStatus();
        
        Debug.Log($"[Performance] FPS: {fps:F1} | " +
                  $"Ping: {networkReport.currentPing}ms | " +
                  $"Memory: {memoryUsage}MB | " +
                  $"Status: {networkReport.performanceStatus}");
    }
}
```

---

## 🔍 Key Performance Indicators (KPIs)

### Network KPIs

#### 1. Latency (Ping)
**What it measures**: Round-trip time for network messages

**How to monitor**:
```csharp
int ping = PhotonNetwork.GetPing();
NetworkPerformanceTracker.Instance.RecordLatency(ping / 1000f);
```

**Optimization**:
- Use nearest Photon region
- Reduce network update frequency
- Implement client-side prediction

#### 2. Frame Drops
**What it measures**: Frames below target FPS

**How to monitor**:
```csharp
float currentFPS = 1f / Time.unscaledDeltaTime;
if (currentFPS < targetFPS * 0.8f)
{
    frameDropCount++;
}
```

**Optimization**:
- Profile CPU/GPU bottlenecks
- Reduce draw calls
- Optimize physics calculations

#### 3. Synchronization Accuracy
**What it measures**: Positional difference between players

**How to monitor**:
```csharp
float distance = Vector3.Distance(actualPosition, networkPosition);
NetworkPerformanceTracker.Instance.RecordSyncData(distance, timeDelta);
```

**Optimization**:
- Increase send rate for critical objects
- Implement interpolation/extrapolation
- Use Photon's lag compensation

---

### Data Storage KPIs

#### 1. Save Operation Success Rate
**What it measures**: Percentage of successful saves

**How to monitor**:
```csharp
int totalSaves = successfulSaves + failedSaves;
float successRate = (float)successfulSaves / totalSaves * 100f;
```

**Target**: >99% success rate

#### 2. Sync Completion Time
**What it measures**: Time to complete full data sync

**How to monitor**:
```csharp
float startTime = Time.time;
await DataSyncManager.Instance.SyncAllData();
float duration = Time.time - startTime;
```

**Target**: <2 seconds for full sync

#### 3. Data Consistency
**What it measures**: Data integrity validation

**How to monitor**:
```csharp
DataConsistencyTester tester = GetComponent<DataConsistencyTester>();
tester.RunAllTests();
float passRate = (float)tester.passedTests / tester.totalTests;
```

**Target**: 100% pass rate

---

## 📈 Performance Dashboard

### In-Game Debug Display

```csharp
public class PerformanceHUD : MonoBehaviour
{
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private KeyCode toggleKey = KeyCode.F3;
    
    private GUIStyle style;
    
    void Start()
    {
        style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.white;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            showDebugInfo = !showDebugInfo;
        }
    }
    
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        int y = 10;
        int lineHeight = 20;
        
        // Network info
        GUI.Label(new Rect(10, y, 300, lineHeight), 
            $"FPS: {(1f / Time.unscaledDeltaTime):F1}", style);
        y += lineHeight;
        
        if (PhotonNetwork.IsConnected)
        {
            GUI.Label(new Rect(10, y, 300, lineHeight), 
                $"Ping: {PhotonNetwork.GetPing()}ms", style);
            y += lineHeight;
            
            GUI.Label(new Rect(10, y, 300, lineHeight), 
                $"Players: {PhotonNetwork.CurrentRoom?.PlayerCount ?? 0}", style);
            y += lineHeight;
        }
        
        // Memory info
        long memory = System.GC.GetTotalMemory(false) / 1048576;
        GUI.Label(new Rect(10, y, 300, lineHeight), 
            $"Memory: {memory}MB", style);
        y += lineHeight;
        
        // Performance status
        var report = NetworkPerformanceTracker.Instance?.GenerateReport();
        if (report.HasValue)
        {
            Color statusColor = report.Value.performanceStatus switch
            {
                "Excellent" => Color.green,
                "Good" => Color.yellow,
                "Fair" => Color.orange,
                _ => Color.red
            };
            
            style.normal.textColor = statusColor;
            GUI.Label(new Rect(10, y, 300, lineHeight), 
                $"Status: {report.Value.performanceStatus}", style);
            style.normal.textColor = Color.white;
        }
    }
}
```

**Controls**: Press F3 to toggle debug display

---

## ⚡ Optimization Strategies

### Network Optimization

#### 1. Reduce Update Frequency
```csharp
// Only send updates when necessary
private Vector3 lastSentPosition;
private float sendThreshold = 0.01f;

void Update()
{
    if (!photonView.IsMine) return;
    
    float distance = Vector3.Distance(transform.position, lastSentPosition);
    if (distance < sendThreshold)
    {
        return; // Don't send update
    }
    
    lastSentPosition = transform.position;
}
```

#### 2. Use Interest Groups
```csharp
// Only receive updates from nearby players
void OnEnterRegion(int regionId)
{
    PhotonNetwork.SetInterestGroups(
        new byte[] { (byte)regionId },  // Subscribe
        null                             // Unsubscribe from others
    );
}
```

#### 3. Compress Data
```csharp
public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    if (stream.IsWriting)
    {
        // Send compressed position (short instead of float)
        short x = (short)(transform.position.x * 100);
        short y = (short)(transform.position.y * 100);
        stream.SendNext(x);
        stream.SendNext(y);
    }
    else
    {
        short x = (short)stream.ReceiveNext();
        short y = (short)stream.ReceiveNext();
        networkPosition = new Vector3(x / 100f, y / 100f, 0);
    }
}
```

---

### Rendering Optimization

#### 1. Sprite Atlasing
```csharp
// Use Sprite Atlas for batch rendering
// Assets → Create → 2D → Sprite Atlas
// Add sprites to atlas to reduce draw calls
```

#### 2. Object Pooling
```csharp
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 20;
    
    private Queue<GameObject> pool = new Queue<GameObject>();
    
    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
    
    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(prefab);
    }
    
    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

#### 3. Culling
```csharp
// Disable off-screen objects
void Update()
{
    bool isVisible = IsVisibleFromCamera();
    spriteRenderer.enabled = isVisible;
    
    if (!isVisible)
    {
        // Skip expensive calculations
        return;
    }
}
```

---

### Memory Optimization

#### 1. Minimize Allocations
```csharp
// Bad: Creates garbage every frame
void Update()
{
    string status = "Player Health: " + health;
}

// Good: Reuse StringBuilder
private StringBuilder sb = new StringBuilder();
void Update()
{
    sb.Clear();
    sb.Append("Player Health: ");
    sb.Append(health);
}
```

#### 2. Unload Unused Assets
```csharp
// After scene transition
Resources.UnloadUnusedAssets();
System.GC.Collect();
```

#### 3. Use Object Pooling (see above)

---

## 🐛 Troubleshooting Performance Issues

### High Latency

**Symptoms**: Ping >500ms, laggy movement

**Diagnosis**:
```csharp
Debug.Log($"Region: {PhotonNetwork.CloudRegion}");
Debug.Log($"Server: {PhotonNetwork.ServerAddress}");
Debug.Log($"RTT: {PhotonNetwork.GetPing()}ms");
```

**Solutions**:
1. Connect to nearest region
2. Check internet connection
3. Reduce send rate
4. Implement lag compensation

---

### Low Frame Rate

**Symptoms**: FPS <30, stuttering

**Diagnosis**:
1. Open Unity Profiler
2. Check CPU/GPU usage
3. Identify bottlenecks

**Solutions**:
- Reduce draw calls (use sprite atlases)
- Optimize physics (use layers, reduce colliders)
- Profile and optimize expensive scripts
- Reduce particle effects

---

### High Memory Usage

**Symptoms**: Memory >1GB, crashes

**Diagnosis**:
```csharp
long memory = System.GC.GetTotalMemory(false);
Debug.Log($"Memory: {memory / 1048576}MB");
```

**Solutions**:
- Use object pooling
- Unload unused assets
- Optimize texture sizes
- Fix memory leaks (unsubscribe events)

---

### Data Sync Issues

**Symptoms**: Data not saving/loading correctly

**Diagnosis**:
```csharp
// Run data consistency tests
DataConsistencyTester tester = GetComponent<DataConsistencyTester>();
tester.RunAllTests();

// Check sync status
var status = DataSyncManager.Instance.GetSyncStatus();
Debug.Log($"Pending operations: {status.pendingOperations}");
Debug.Log($"Failed syncs: {status.failedSyncs}");
```

**Solutions**:
1. Check file permissions
2. Verify cloud connection
3. Implement retry logic
4. Add data validation

---

## 📊 Performance Testing

### Load Testing

```csharp
// Simulate high load
IEnumerator LoadTest()
{
    for (int i = 0; i < 100; i++)
    {
        // Spawn entities
        GameObject obj = Instantiate(enemyPrefab);
        
        yield return new WaitForSeconds(0.1f);
        
        // Monitor performance
        float fps = 1f / Time.unscaledDeltaTime;
        if (fps < 30)
        {
            Debug.LogWarning($"FPS dropped below 30 at {i} entities");
            break;
        }
    }
}
```

### Network Testing

```csharp
// Test network under poor conditions
void SimulateNetworkConditions()
{
    // In Photon, you can test with Network Simulator
    // Edit → Project Settings → Photon → Network Simulator
    // Set Incoming/Outgoing lag and packet loss
}
```

### Stress Testing

```csharp
// Rapid save/load operations
IEnumerator StressTest()
{
    for (int i = 0; i < 1000; i++)
    {
        PlayerData data = new PlayerData { level = i };
        LocalStorageManager.Instance.SavePlayerData(data);
        
        if (i % 100 == 0)
        {
            yield return null; // Yield occasionally
        }
    }
    
    Debug.Log("Stress test completed");
}
```

---

## 📝 Performance Checklist

### Pre-Release

- [ ] Average FPS ≥60 on target hardware
- [ ] Latency <200ms to nearest Photon region
- [ ] No memory leaks detected
- [ ] Save/load operations <200ms
- [ ] Data consistency tests pass 100%
- [ ] Network sync accurate within 0.1 units
- [ ] Frame drops <5% of total frames
- [ ] Build size <500MB
- [ ] Load times <3 seconds

### During Development

- [ ] Profile regularly with Unity Profiler
- [ ] Monitor network performance tracker
- [ ] Run data consistency tests
- [ ] Check for memory allocations in hot paths
- [ ] Test on minimum spec hardware
- [ ] Test with simulated poor network
- [ ] Verify cross-platform compatibility

---

## 🔗 Related Documentation

- [Technical Architecture](TECHNICAL_ARCHITECTURE.md)
- [Photon Integration Guide](PHOTON_INTEGRATION_GUIDE.md)
- [Data Storage Schema](DATA_STORAGE_SCHEMA.md)

---

*Last Updated: 2025-12-19*
*Performance Framework Version: 1.0*
