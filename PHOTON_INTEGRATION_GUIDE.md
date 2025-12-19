# Photon PUN 2 Integration Guide

## Quick Start Guide for Photon PUN 2 Setup

### 📋 Prerequisites

- Unity 2022.3.10f1 or later
- Photon account (free tier available)
- Internet connection for cloud connectivity

---

## 🚀 Installation Steps

### Step 1: Create Photon Account

1. Go to [Photon Engine Dashboard](https://dashboard.photonengine.com/)
2. Sign up for a free account
3. Create a new application:
   - **Type**: Photon PUN
   - **Name**: PixelCoopRPG (or your game name)
4. Copy the **App ID** (you'll need this later)

### Step 2: Import Photon PUN 2

#### Option A: Unity Asset Store
1. Open Unity Asset Store
2. Search for "Photon PUN 2 - FREE"
3. Download and import the package
4. Import all files when prompted

#### Option B: Direct Download
1. Download PUN 2 from [Photon Website](https://www.photonengine.com/pun)
2. Import the `.unitypackage` file via Assets → Import Package → Custom Package

### Step 3: Configure Photon Settings

1. After import, the Photon Setup Wizard should appear automatically
2. If not, go to Window → Photon Unity Networking → PUN Wizard
3. Enter your **App ID** from Step 1
4. Click "Setup Project"

**Manual Configuration** (if needed):
- Find or create: `Assets/Photon/PhotonUnityNetworking/Resources/PhotonServerSettings.asset`
- Set App ID in Inspector
- Set App Version to "1.0" (or your version)
- Set Protocol to "Udp" for best performance
- Enable "Auto-Join Lobby"

---

## 🔧 Project Configuration

### Required Photon Settings

Edit `PhotonServerSettings` asset:

```
App Settings:
├── App Id Realtime: [Your App ID]
├── App Version: "1.0"
├── Use Name Server: ✓
└── Protocol: UDP

Dev Settings:
├── Network Logging: Info (or Warning for production)
└── Enable Lobby Stats: ✓

PUN Settings:
├── Auto-Join Lobby: ✓
├── Enable Support Logger: ✓ (disable in production)
└── Run In Background: ✓
```

### Network Settings in Unity

1. **Edit → Project Settings → Player**
2. Under "Other Settings":
   - Enable "Run In Background" ✓
   - Set "Internet Access" to "Require" (for mobile)
3. Under "Quality Settings":
   - Set "V Sync Count" to "Don't Sync" (for multiplayer)

---

## 🎮 Implementing Basic Multiplayer

### Scene Setup

#### 1. Create Network Manager Scene

Create a new scene `NetworkSetup.unity`:

1. Create empty GameObject: "NetworkManager"
2. Add components:
   - `PhotonNetworkManager` (our custom script)
   - `NetworkPerformanceTracker` (our custom script)
3. Mark as DontDestroyOnLoad in PhotonNetworkManager

#### 2. Create Game Scene

Create `GameScene.unity`:

1. Set up your game environment
2. Create a prefab for the player character

#### 3. Setup Player Prefab

Your player prefab must have:
- `PhotonView` component (from Photon)
- `PlayerNetworkSync` component (our custom script)
- `PhotonTransformView` component (optional, for built-in sync)

**PhotonView Configuration:**
```
Observed Components:
├── PlayerNetworkSync (required)
└── PhotonTransformView (optional)

Settings:
├── Ownership Transfer: Takeover
└── Synchronization: Unreliable On Change
```

**Important**: Place player prefab in `Resources/` folder!
- Path: `Assets/Resources/Player.prefab`
- Photon requires network-spawned prefabs to be in Resources folder

---

## 💻 Code Implementation

### Basic Connection Flow

```csharp
using UnityEngine;
using Photon.Pun;

public class GameStarter : MonoBehaviour
{
    void Start()
    {
        // Connect to Photon
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetworkManager.Instance.ConnectToPhoton();
        }
    }
    
    void Update()
    {
        // Press J to join room
        if (Input.GetKeyDown(KeyCode.J) && PhotonNetwork.IsConnected)
        {
            PhotonNetworkManager.Instance.JoinOrCreateRoom();
        }
        
        // Display connection status
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log($"Connected! Ping: {PhotonNetwork.GetPing()}ms");
        }
    }
}
```

### Spawn Player in Room

```csharp
using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private Vector3 spawnPoint = Vector3.zero;
    
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room - spawning player");
        
        // Spawn player at random position
        Vector3 randomPosition = spawnPoint + Random.insideUnitSphere * 2f;
        
        // Instantiate networked player
        GameObject player = PhotonNetwork.Instantiate(
            "Player",  // Prefab name in Resources folder
            randomPosition, 
            Quaternion.identity
        );
        
        Debug.Log($"Player spawned: {player.name}");
    }
}
```

### Sync Custom Data with RPC

```csharp
using Photon.Pun;
using UnityEngine;

public class PlayerActions : MonoBehaviourPun
{
    // Call this method on local player
    public void Attack()
    {
        if (!photonView.IsMine) return;
        
        // Trigger attack animation locally
        PlayAttackAnimation();
        
        // Sync with other players
        photonView.RPC(nameof(RPC_Attack), RpcTarget.Others);
    }
    
    // This method will be called on remote players
    [PunRPC]
    void RPC_Attack()
    {
        PlayAttackAnimation();
    }
    
    void PlayAttackAnimation()
    {
        Debug.Log("Playing attack animation");
        // Your animation code here
    }
}
```

---

## 🔍 Testing Multiplayer

### Local Testing (Single Machine)

#### Method 1: Build and Editor
1. Build your game (File → Build and Run)
2. Keep Unity Editor open
3. Start the built game
4. Both should connect and join the same room

#### Method 2: Multiple Editors (Recommended)
1. Install ParrelSync from GitHub or Asset Store
2. Create a clone project
3. Run both Unity instances simultaneously
4. Both can connect to Photon independently

**ParrelSync Setup:**
```bash
# Install via Package Manager
# Add from git URL:
https://github.com/VeriorPies/ParrelSync.git?path=/ParrelSync
```

### Network Testing Checklist

- [ ] Connection to Photon Master Server
- [ ] Room creation/joining
- [ ] Player spawning
- [ ] Position synchronization
- [ ] Animation synchronization
- [ ] RPC calls working
- [ ] Latency acceptable (<200ms)
- [ ] No visible lag/stuttering
- [ ] Disconnect handling

---

## 📊 Monitoring & Debugging

### Enable Photon Logging

```csharp
void Start()
{
    // Enable detailed logging
    PhotonNetwork.LogLevel = PunLogLevel.Full;
}
```

### Common Log Messages

```
[PUN] Connecting to Master Server...
[PUN] Connected to Master Server
[PUN] Joined Lobby
[PUN] Creating room...
[PUN] Joined room: [Room Name]
[PUN] Player entered: [Player Name]
```

### Performance Monitoring

Use our `NetworkPerformanceTracker`:

```csharp
void Update()
{
    var report = NetworkPerformanceTracker.Instance.GenerateReport();
    
    // Display in UI
    pingText.text = $"Ping: {report.currentPing}ms";
    fpsText.text = $"FPS: {report.averageFPS:F0}";
    statusText.text = $"Status: {report.performanceStatus}";
}
```

---

## ⚡ Optimization Tips

### 1. Reduce Network Traffic

```csharp
// Only sync when necessary
[SerializeField] private float positionThreshold = 0.01f;

void Update()
{
    if (!photonView.IsMine) return;
    
    float distanceMoved = Vector3.Distance(transform.position, lastPosition);
    if (distanceMoved < positionThreshold)
    {
        // Don't send update if barely moved
        return;
    }
    
    lastPosition = transform.position;
}
```

### 2. Use Interest Groups

```csharp
// Assign players to interest groups based on area
void OnEnterArea(int areaId)
{
    // Only receive updates from players in same area
    PhotonNetwork.SetInterestGroups(new byte[] { (byte)areaId }, null);
}
```

### 3. Optimize Serialization

```csharp
public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    if (stream.IsWriting)
    {
        // Send only what's needed
        stream.SendNext(transform.position);
        // Don't send full Vector3 if Y is always 0
        stream.SendNext(new Vector2(transform.position.x, transform.position.z));
    }
    else
    {
        // Receive data
        Vector2 pos = (Vector2)stream.ReceiveNext();
        networkPosition = new Vector3(pos.x, 0, pos.y);
    }
}
```

### 4. Adjust Send Rate

For 2-player game, higher send rate is acceptable:

```csharp
void Start()
{
    // Increase send rate for better accuracy (default: 10)
    PhotonNetwork.SendRate = 20;
    PhotonNetwork.SerializationRate = 20;
}
```

---

## 🐛 Troubleshooting

### Common Issues

#### Issue: "App ID not configured"
**Solution**: Set App ID in `PhotonServerSettings` asset

#### Issue: "Cannot join room"
**Solution**: 
- Check if room is full (max 2 players)
- Verify game version matches
- Try creating new room instead

#### Issue: "High latency"
**Solution**:
- Check internet connection
- Try different Photon region
- Reduce send rate if bandwidth limited

#### Issue: "Player doesn't spawn"
**Solution**:
- Ensure prefab is in `Resources/` folder
- Check PhotonView component exists
- Verify `PhotonNetwork.Instantiate` is called

#### Issue: "Synchronization lag"
**Solution**:
- Increase interpolation speed in `PlayerNetworkSync`
- Use prediction/extrapolation
- Check `SendRate` and `SerializationRate`

### Debug Commands

```csharp
// Force disconnect
PhotonNetwork.Disconnect();

// Get room info
Debug.Log($"Room: {PhotonNetwork.CurrentRoom.Name}");
Debug.Log($"Players: {PhotonNetwork.CurrentRoom.PlayerCount}");

// Get player info
Debug.Log($"Local Player: {PhotonNetwork.NickName}");
Debug.Log($"Is Master Client: {PhotonNetwork.IsMasterClient}");

// Server info
Debug.Log($"Region: {PhotonNetwork.CloudRegion}");
Debug.Log($"Server: {PhotonNetwork.ServerAddress}");
```

---

## 🌍 Region Selection

Photon has servers worldwide. For best performance:

```csharp
// Connect to specific region
PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia"; // or "us", "eu", etc.

// Or use best region (automatic)
PhotonNetwork.ConnectToRegion(""); // Empty = best region
```

**Available Regions:**
- `us` - United States
- `eu` - Europe
- `asia` - Asia
- `jp` - Japan
- `au` - Australia
- `kr` - South Korea
- `in` - India
- `ru` - Russia
- `sa` - South America

---

## 📈 Scalability Considerations

### Free Tier Limits
- 20 Concurrent Users (CCU)
- Unlimited messages
- All regions available

### Paid Tiers
- More CCU as needed
- Priority support
- Custom server options

### For 200+ CCU Target
- Will need paid tier
- Consider implementing:
  - Server-side logic (Photon Server SDK)
  - Custom matchmaking
  - Load balancing

---

## 🎯 Best Practices

1. **Always check `photonView.IsMine`** before sending input
2. **Use RPC for important events** (attacks, pickups, etc.)
3. **Interpolate remote player positions** for smooth movement
4. **Handle disconnections gracefully**
5. **Test with real network conditions** (not just LAN)
6. **Monitor performance metrics** continuously
7. **Implement reconnection logic** for mobile players
8. **Use object pooling** for frequent instantiation
9. **Compress data** when sending large payloads
10. **Version your game** to prevent incompatible clients

---

## 📚 Additional Resources

- [Photon PUN 2 Documentation](https://doc.photonengine.com/pun/current/getting-started/pun-intro)
- [Photon Best Practices](https://doc.photonengine.com/pun/current/gameplay/optimization)
- [Photon Community Forum](https://forum.photonengine.com/)
- [Photon Discord](https://discord.gg/photonengine)

---

## ✅ Integration Checklist

- [ ] Photon account created
- [ ] App ID obtained
- [ ] PUN 2 imported into Unity
- [ ] PhotonServerSettings configured
- [ ] NetworkManager scene created
- [ ] Player prefab in Resources folder
- [ ] PhotonView components added
- [ ] Custom sync scripts attached
- [ ] Connection tested in editor
- [ ] Multi-player tested with build
- [ ] Performance monitoring active
- [ ] Disconnect handling implemented
- [ ] Error handling added
- [ ] Documentation updated

---

*Last Updated: 2025-12-19*
*Photon PUN 2 Version: 2.x*
