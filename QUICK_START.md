# Quick Start Guide / 快速入门指南

## 🚀 Get Started in 5 Minutes / 5分钟快速开始

---

## Step 1: Open the Project / 打开项目

### Using Unity Hub:
1. Open Unity Hub
2. Click "Add" → "Add project from disk"
3. Select the `game` folder
4. Open with Unity **2022.3.10f1** or later

### 使用 Unity Hub：
1. 打开 Unity Hub
2. 点击"添加" → "从磁盘添加项目"
3. 选择 `game` 文件夹
4. 使用 Unity **2022.3.10f1** 或更高版本打开

---

## Step 2: Install Photon PUN 2 / 安装 Photon PUN 2

### Option A: Unity Asset Store
1. Open Unity Asset Store
2. Search "Photon PUN 2 FREE"
3. Download and import

### Option B: Photon Website
1. Visit: https://www.photonengine.com/pun
2. Download PUN 2
3. Import the `.unitypackage` file

### 选项A：Unity 资源商店
1. 打开 Unity 资源商店
2. 搜索 "Photon PUN 2 FREE"
3. 下载并导入

### 选项B：Photon 官网
1. 访问: https://www.photonengine.com/pun
2. 下载 PUN 2
3. 导入 `.unitypackage` 文件

---

## Step 3: Get Photon App ID / 获取 Photon App ID

1. Register at: https://dashboard.photonengine.com/
2. Create a new application (Type: **Photon PUN**)
3. Copy your **App ID**

1. 注册: https://dashboard.photonengine.com/
2. 创建新应用（类型：**Photon PUN**）
3. 复制你的 **App ID**

---

## Step 4: Configure Photon / 配置 Photon

### In Unity:
1. After importing PUN 2, the setup wizard should appear
2. Paste your **App ID**
3. Click "Setup Project"

### Or manually:
1. Find: `Assets/Photon/PhotonUnityNetworking/Resources/PhotonServerSettings`
2. Paste your **App ID** in the inspector

### 在 Unity 中：
1. 导入 PUN 2 后，设置向导应该会出现
2. 粘贴你的 **App ID**
3. 点击 "Setup Project"

### 或手动配置：
1. 找到: `Assets/Photon/PhotonUnityNetworking/Resources/PhotonServerSettings`
2. 在检视器中粘贴你的 **App ID**

---

## Step 5: Create Test Scene / 创建测试场景

### Quick Test:
1. Create new scene: `TestScene`
2. Create empty GameObject: "NetworkManager"
3. Add components:
   - `PhotonNetworkManager` (from our scripts)
   - `NetworkPerformanceTracker` (from our scripts)
4. Press Play!

### 快速测试：
1. 创建新场景: `TestScene`
2. 创建空物体: "NetworkManager"
3. 添加组件:
   - `PhotonNetworkManager` (来自我们的脚本)
   - `NetworkPerformanceTracker` (来自我们的脚本)
4. 按播放键！

---

## Step 6: Test Connection / 测试连接

### In Play Mode:
- Check Console for: `[PhotonNetworkManager] Connected to Master Server`
- If connected, you'll see the region (e.g., `Region: asia`)

### In the Console:
```
[PhotonNetworkManager] Connecting to Photon Cloud...
[PhotonNetworkManager] Connected to Master Server. Region: asia
[NetworkPerformanceTracker] Connection established. Monitoring started.
```

### 在播放模式中：
- 检查控制台: `[PhotonNetworkManager] Connected to Master Server`
- 如果连接成功，你会看到区域（例如：`Region: asia`）

---

## 📖 What to Read Next / 接下来阅读什么

### For Understanding the System / 理解系统：
1. **Start here**: `TECHNICAL_ARCHITECTURE.md`
   - System overview
   - Component descriptions
   - Architecture decisions

2. **Visual guide**: `ARCHITECTURE_DIAGRAM.md`
   - Visual diagrams
   - Data flow charts
   - Component relationships

### For Implementation / 实现功能：
3. **Networking**: `PHOTON_INTEGRATION_GUIDE.md`
   - Photon setup details
   - Multiplayer implementation
   - Testing and debugging

4. **Data Storage**: `DATA_STORAGE_SCHEMA.md`
   - Data structures
   - Save/Load system
   - Cloud integration

5. **Performance**: `PERFORMANCE_MONITORING_GUIDE.md`
   - Optimization tips
   - Monitoring tools
   - Troubleshooting

### For Overview / 总览：
6. **Summary**: `IMPLEMENTATION_SUMMARY.md`
   - What's implemented
   - Statistics
   - Next steps (Chinese + English)

---

## 🎮 Create Your First Networked Player / 创建第一个联网角色

### 1. Create Player Prefab:
```
1. Create a 2D Sprite GameObject: "Player"
2. Add SpriteRenderer (assign a sprite)
3. Add Rigidbody2D (set Gravity Scale = 0 for top-down)
4. Add PhotonView component
5. Add PlayerNetworkSync component (our script)
6. Save as prefab in: Assets/Resources/Player.prefab
```

### 2. Spawn Player Script:
Create a new script `PlayerSpawner.cs`:

```csharp
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom()
    {
        Vector3 spawnPos = new Vector3(
            Random.Range(-5f, 5f), 
            Random.Range(-5f, 5f), 
            0
        );
        
        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
        Debug.Log("Player spawned!");
    }
}
```

### 3. Add to Scene:
1. Attach `PlayerSpawner` to your NetworkManager
2. Press Play
3. In Console, type to join room: Press 'J' key (if using example code)

---

## 🔍 Troubleshooting / 故障排除

### Problem: "Cannot find PhotonView component"
**Solution**: Import Photon PUN 2 package first

### Problem: "App ID not configured"
**Solution**: Set App ID in PhotonServerSettings

### Problem: "Cannot connect to Photon"
**Solution**: 
- Check internet connection
- Verify App ID is correct
- Check firewall settings

### 问题："找不到 PhotonView 组件"
**解决**: 首先导入 Photon PUN 2 包

### 问题："App ID 未配置"
**解决**: 在 PhotonServerSettings 中设置 App ID

### 问题："无法连接到 Photon"
**解决**: 
- 检查网络连接
- 验证 App ID 正确
- 检查防火墙设置

---

## 📊 Verify Everything Works / 验证一切正常

### Checklist:
- [ ] Unity project opens without errors
- [ ] Photon PUN 2 is imported
- [ ] App ID is configured
- [ ] NetworkManager connects to Photon
- [ ] Console shows "Connected to Master Server"
- [ ] No errors in Console

### 检查清单：
- [ ] Unity 项目打开无错误
- [ ] Photon PUN 2 已导入
- [ ] App ID 已配置
- [ ] NetworkManager 连接到 Photon
- [ ] 控制台显示 "Connected to Master Server"
- [ ] 控制台无错误

---

## 🎯 Next Development Steps / 下一步开发

### Short Term (This Week):
1. Create player sprite and animations
2. Implement movement (8-directional)
3. Test 2-player synchronization
4. Add basic attack animation

### Medium Term (This Month):
5. Create map generation system
6. Implement enemy AI
7. Add inventory system
8. Build base construction system

### Long Term (This Quarter):
9. Implement friend system (cloud)
10. Create achievement system
11. Add matchmaking with level restrictions
12. Polish UI and effects

### 短期（本周）：
1. 创建角色精灵和动画
2. 实现移动（8方向）
3. 测试双人同步
4. 添加基础攻击动画

### 中期（本月）：
5. 创建地图生成系统
6. 实现敌人 AI
7. 添加物品系统
8. 构建基地建造系统

### 长期（本季度）：
9. 实现好友系统（云端）
10. 创建成就系统
11. 添加等级匹配系统
12. 完善 UI 和效果

---

## 💡 Tips / 提示

### For Development:
- **Test frequently**: Test multiplayer early and often
- **Use ParrelSync**: Run multiple Unity editors for testing
- **Monitor performance**: Use NetworkPerformanceTracker
- **Read logs**: Console logs contain important information

### For Learning:
- **Start small**: Build one feature at a time
- **Use examples**: Code examples in documentation
- **Ask questions**: Use GitHub Issues for questions
- **Check docs**: All answers are in the documentation files

### 开发建议：
- **频繁测试**: 尽早并经常测试多人功能
- **使用 ParrelSync**: 运行多个 Unity 编辑器进行测试
- **监控性能**: 使用 NetworkPerformanceTracker
- **阅读日志**: 控制台日志包含重要信息

### 学习建议：
- **从小做起**: 一次构建一个功能
- **使用示例**: 文档中的代码示例
- **提出问题**: 使用 GitHub Issues 提问
- **查看文档**: 所有答案都在文档文件中

---

## 🔗 Useful Links / 有用链接

### Official Resources:
- **Unity Manual**: https://docs.unity3d.com/Manual/
- **Photon Docs**: https://doc.photonengine.com/pun/current/getting-started/pun-intro
- **Photon Dashboard**: https://dashboard.photonengine.com/

### Community:
- **Unity Forums**: https://forum.unity.com/
- **Photon Forum**: https://forum.photonengine.com/
- **Discord**: Join Unity and Photon Discord servers

---

## 🎉 You're Ready! / 准备就绪！

The technical framework is complete. Now it's time to build your game!

技术框架已完成。现在是时候构建你的游戏了！

**Good luck and have fun coding!** / **祝你好运，编码愉快！** 🎮✨

---

*Last Updated: 2025-12-19*
*Framework Version: 1.0*
