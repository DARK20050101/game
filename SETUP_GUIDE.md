# Setup Guide - Pixel Co-op RPG

This guide will walk you through setting up the technical framework for the game.

## Prerequisites

### Required Software
- **Unity 2021.3 LTS or higher** - Download from [Unity Hub](https://unity.com/download)
- **Visual Studio 2019/2022** or **Visual Studio Code** - For C# development
- **Git** - For version control
- **Photon Account** - Sign up at [Photon Engine](https://www.photonengine.com/)

### System Requirements
- **OS**: Windows 10/11, macOS 10.13+, or Linux
- **RAM**: 8GB minimum, 16GB recommended
- **Storage**: 10GB free space
- **Network**: Stable internet connection for Photon services

---

## Step 1: Unity Project Setup

### 1.1 Create New Unity Project

1. Open Unity Hub
2. Click "New Project"
3. Select **2D (URP)** template (Universal Render Pipeline for optimized 2D)
4. Name: `PixelCoopRPG`
5. Location: Your workspace directory
6. Click "Create Project"

### 1.2 Configure Project Settings

Once Unity opens:

#### Project Settings > Player
1. **Company Name**: Your company/team name
2. **Product Name**: Pixel Co-op RPG
3. **Version**: 0.1.0
4. **Default Icon**: (Add later)

#### Project Settings > Quality
1. Set default quality level to **High**
2. Configure:
   - V Sync Count: Every V Blank
   - Anti Aliasing: Disabled (for pixel art)
   - Anisotropic Textures: Disabled

#### Project Settings > Graphics
1. Ensure **Universal Render Pipeline Asset** is assigned
2. Configure URP Asset:
   - HDR: Off
   - MSAA: Off (not needed for pixel art)
   - Render Scale: 1.0

### 1.3 Set Up Pixel Perfect Camera

1. In top menu: **Window > Package Manager**
2. Search for **"2D Pixel Perfect"**
3. Click **Install**
4. In Hierarchy, select **Main Camera**
5. **Add Component > Pixel Perfect Camera**
6. Configure:
   - Assets Pixels Per Unit: 16 (adjust based on your sprites)
   - Reference Resolution: 320x180 (base resolution)
   - Upscale Render Texture: Checked
   - Pixel Snapping: Checked
   - Crop Frame: Depends on preference

---

## Step 2: Import Project Files

### 2.1 Copy Scripts to Unity Project

From this repository, copy the following directories to your Unity project:

```bash
# From repository root
cp -r Assets/Scripts /path/to/UnityProject/Assets/
```

Or manually:
1. Copy `Assets/Scripts` folder
2. Paste into your Unity project's `Assets` folder
3. Unity will automatically detect and import the scripts

### 2.2 Verify Script Compilation

1. Wait for Unity to compile scripts
2. Check Console window (Window > General > Console) for any errors
3. Note: Scripts will show errors until Photon PUN 2 is installed (next step)

---

## Step 3: Photon PUN 2 Setup

### 3.1 Get Photon AppId

1. Go to [Photon Engine Dashboard](https://dashboard.photonengine.com/)
2. Sign up or log in
3. Click **"Create a New App"**
4. Configure:
   - Photon Type: **Photon PUN**
   - Name: PixelCoopRPG
   - Description: Pixel art co-op RPG multiplayer
5. Click **Create**
6. Copy your **App ID** (you'll need this soon)

### 3.2 Import Photon PUN 2

#### Method 1: Unity Asset Store (Recommended)
1. Open **Window > Asset Store**
2. Search for **"PUN 2 - FREE"**
3. Click **Download** then **Import**
4. Import all files
5. Click **Import**

#### Method 2: Download from Photon Website
1. Download PUN 2 from [Photon Unity Networking](https://www.photonengine.com/pun)
2. Import the `.unitypackage` file into Unity
3. **Assets > Import Package > Custom Package**

### 3.3 Configure Photon

1. After import, PUN Setup window should appear automatically
   - If not: **Window > Photon Unity Networking > PUN Wizard**
2. Paste your **App ID** in the field
3. Click **Setup Project**
4. Configuration file created at: `Assets/Photon/PhotonUnityNetworking/Resources/PhotonServerSettings.asset`

### 3.4 Configure Photon Settings

1. Locate `PhotonServerSettings` asset (above path)
2. Configure:
   - **App Id PUN**: (Your App ID - should be set)
   - **Fixed Region**: (Leave blank for auto-selection, or set to nearest region)
   - **Protocol**: UDP (faster for games)
   - **Network Logging**: Informational (for development)

---

## Step 4: Create Game Manager Setup

### 4.1 Create Network Setup Scene

1. Create new scene: **File > New Scene**
2. Save as: `Assets/Scenes/NetworkSetup.scene`
3. In Hierarchy, right-click and create:
   - **Create Empty** → Name it `NetworkManager`
   - **Create Empty** → Name it `DataManager`
   - **Create Empty** → Name it `PerformanceMonitors`

### 4.2 Add Network Components

1. Select **NetworkManager** GameObject
2. **Add Component > Network Manager** (our script)
3. **Add Component > Network Performance Monitor**
4. Configure NetworkManager:
   - Game Version: 1.0
   - Max Players Per Room: 2
   - Send Rate: 20
   - Serialization Rate: 10

### 4.3 Add Data Components

1. Select **DataManager** GameObject
2. **Add Component > Data Manager** (our script)
3. **Add Component > Cloud Data Manager**
4. Configure DataManager:
   - Auto Save Interval: 60
   - Max Backups: 3

### 4.4 Add Performance Components

1. Select **PerformanceMonitors** GameObject
2. **Add Component > Frame Rate Monitor**
3. Configure:
   - Enable Monitoring: ✓
   - Show FPS Counter: ✓
   - Target FPS: 60

### 4.5 Save Scene

1. **File > Save**
2. Set as startup scene: **File > Build Settings**
   - Add Open Scenes
   - Drag `NetworkSetup` to top of list

---

## Step 5: Test Basic Networking

### 5.1 Create Simple Test Scene

1. Create new scene: `Assets/Scenes/TestScene.scene`
2. Add a simple cube or sprite to represent a player
3. Save scene

### 5.2 Create Test Player Prefab

1. In Hierarchy, create **Cube** or **2D Sprite**
2. Name it `TestPlayer`
3. Add Components:
   - **Photon View** (from Photon)
   - **Photon Transform View** (from Photon)
   - **Player Network Sync** (our script)
4. Configure Photon View:
   - Observed Components: Add `Photon Transform View`
   - Synchronization: Reliable Delta Compressed
5. Drag `TestPlayer` to `Assets/Prefabs` folder
6. Delete from scene

### 5.3 Create Network Test Script

Create `Assets/Scripts/Testing/NetworkTest.cs`:

```csharp
using UnityEngine;
using Photon.Pun;

public class NetworkTest : MonoBehaviour
{
    public GameObject playerPrefab;

    private void Start()
    {
        // Ensure managers are initialized
        if (NetworkManager.Instance == null)
        {
            Debug.LogError("NetworkManager not found! Load NetworkSetup scene first.");
            return;
        }

        // Connect and join room
        NetworkManager.Instance.OnRoomJoinedCallback += OnRoomJoined;
        NetworkManager.Instance.Connect();
    }

    private void OnRoomJoined()
    {
        Debug.Log("Room joined! Spawning player...");
        
        // Spawn player at random position
        Vector3 spawnPos = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
    }

    private void Update()
    {
        // Quick test: Press C to connect, R to create room, J to join random room
        if (Input.GetKeyDown(KeyCode.C))
        {
            NetworkManager.Instance.Connect();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            NetworkManager.Instance.CreateRoom();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            NetworkManager.Instance.JoinRandomRoom();
        }
    }
}
```

### 5.4 Run Basic Test

1. In `TestScene`, create empty GameObject named `NetworkTest`
2. Add `NetworkTest` component
3. Assign `TestPlayer` prefab to the field
4. **File > Build Settings**
   - Add `NetworkSetup` scene (index 0)
   - Add `TestScene` scene (index 1)
5. Press **Play** in Unity
6. Check Console for connection status
7. Press **R** to create room
8. You should see player prefab spawn

### 5.5 Test Two Clients

To test multiplayer:

#### Option A: Build and Run
1. **File > Build and Run**
2. Run the build
3. Press Play in Unity Editor
4. Both should connect and see each other

#### Option B: ParrelSync (Unity Editor Cloning)
1. Install ParrelSync from Package Manager (optional)
2. Create clone project
3. Run both editor instances

---

## Step 6: Verify Data Storage

### 6.1 Test Local Storage

Create test script `Assets/Scripts/Testing/DataTest.cs`:

```csharp
using UnityEngine;

public class DataTest : MonoBehaviour
{
    private void Start()
    {
        TestSaveLoad();
    }

    private void TestSaveLoad()
    {
        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager not found!");
            return;
        }

        // Create test data
        PlayerData testData = DataManager.Instance.GetPlayerData();
        testData.characterName = "TestHero";
        testData.level = 5;
        testData.experience = 1500;
        
        // Save
        DataManager.Instance.UpdatePlayerData(testData);
        DataManager.Instance.ForceSave();
        
        Debug.Log($"Saved: {testData.characterName}, Level {testData.level}");
        
        // Load
        DataManager.Instance.LoadPlayerData();
        PlayerData loadedData = DataManager.Instance.GetPlayerData();
        
        Debug.Log($"Loaded: {loadedData.characterName}, Level {loadedData.level}");
        
        // Verify
        if (loadedData.characterName == "TestHero" && loadedData.level == 5)
        {
            Debug.Log("✓ Save/Load test PASSED");
        }
        else
        {
            Debug.LogError("✗ Save/Load test FAILED");
        }
    }
}
```

### 6.2 Check Save Location

After running test:
1. Find save location: `Application.persistentDataPath`
   - Windows: `%USERPROFILE%\AppData\LocalLow\<CompanyName>\<ProductName>`
   - Mac: `~/Library/Application Support/<CompanyName>/<ProductName>`
   - Linux: `~/.config/unity3d/<CompanyName>/<ProductName>`
2. Navigate to `SaveData` folder
3. Verify `player.json` exists and contains data

---

## Step 7: Performance Validation

### 7.1 Enable Performance Monitoring

1. Ensure all performance monitors are enabled:
   - `NetworkPerformanceMonitor`: Show Performance UI = ✓
   - `FrameRateMonitor`: Show FPS Counter = ✓

### 7.2 Run Performance Test

1. Connect two clients (build + editor)
2. Move players around
3. Observe performance UI:
   - **Network**: Check ping, connection quality
   - **FPS**: Monitor frame rate stability
4. Expected results:
   - Ping: < 100ms (local network)
   - FPS: Stable 60
   - No desyncs

### 7.3 Stress Test (Optional)

Create artificial load to test performance:
- Spawn multiple objects
- Rapid network updates
- Monitor frame drops and ping spikes

---

## Step 8: Configuration Checklist

Before proceeding to game development, verify:

### Technical Setup
- [x] Unity 2021.3 LTS installed
- [x] Project created with 2D URP template
- [x] Pixel Perfect Camera configured
- [x] All scripts imported and compiled
- [x] Photon PUN 2 imported and configured
- [x] Photon AppId set

### Core Systems
- [x] NetworkManager functional
- [x] DataManager saving/loading
- [x] CloudDataManager connected
- [x] Performance monitors active

### Testing
- [x] Two clients can connect
- [x] Player synchronization works
- [x] Data persistence verified
- [x] Performance metrics displayed

---

## Troubleshooting

### Photon Connection Issues
**Problem**: Cannot connect to Photon
- Verify App ID is correct
- Check internet connection
- Try different region in PhotonServerSettings
- Check firewall settings

### Script Compilation Errors
**Problem**: Scripts have errors after import
- Ensure Photon PUN 2 is fully imported
- Check Unity version compatibility
- Verify all scripts are in correct folders

### Performance Issues
**Problem**: Low FPS or high ping
- Reduce send/serialization rate
- Optimize scene (fewer objects)
- Check for network congestion
- Profile with Unity Profiler

### Save/Load Issues
**Problem**: Data not persisting
- Check save directory permissions
- Verify DataManager is in scene
- Check Console for error messages
- Try deleting save data and recreating

---

## Next Steps

Now that the technical framework is set up:

1. **Character Development**: Create player sprites and animations
2. **Map Generation**: Implement procedural map system
3. **Combat System**: Develop attack and skill mechanics
4. **UI Development**: Create HUD, inventory, menus
5. **Co-op Features**: Implement proximity buffs, boss mechanics

Refer to `TECHNICAL_ARCHITECTURE.md` for detailed system documentation.

---

## Support Resources

- **Unity Documentation**: https://docs.unity3d.com/
- **Photon Documentation**: https://doc.photonengine.com/pun/current/
- **Project Repository**: Check README.md for updates
- **Issues**: Report bugs via GitHub Issues

---

*Last Updated: 2025-12-19*
