# Integration Guide / 集成指南

本指南帮助开发者将像素美术与交互系统集成到Unity项目中。
This guide helps developers integrate the pixel art and interaction systems into a Unity project.

---

## 快速开始 / Quick Start

### 1. 准备Unity项目 / Prepare Unity Project

#### 1.1 创建新Unity项目
```
Unity版本推荐: 2020.3 LTS 或更高
项目类型: 2D
```

#### 1.2 安装必需的包 / Install Required Packages

通过Unity Package Manager安装：
```
1. Window > Package Manager
2. 搜索并安装以下包:
   - 2D Pixel Perfect
   - Cinemachine (可选，用于相机控制)
```

#### 1.3 安装Photon PUN 2 (联机功能)
```
1. Asset Store > 搜索 "Photon PUN 2"
2. 下载并导入
3. 配置Photon AppId (在PUN Wizard中)
```

### 2. 导入项目文件 / Import Project Files

#### 2.1 复制Assets目录
```bash
# 将本项目的Assets目录内容复制到Unity项目的Assets目录
cp -r /path/to/this/project/Assets/* /path/to/unity/project/Assets/
```

#### 2.2 等待Unity编译
Unity将自动检测并编译所有C#脚本。检查Console确保无错误。

### 3. 配置Pixel Perfect Camera / 设置像素完美相机

#### 3.1 创建相机
```
1. GameObject > 2D Object > Camera
2. 添加组件: Pixel Perfect Camera
3. 配置参数:
   - Assets Pixels Per Unit: 32
   - Reference Resolution: 640x360
   - Upscale Render Texture: ✓
   - Pixel Snapping: ✓
```

---

## 系统集成步骤 / System Integration Steps

### A. 角色动画系统 / Character Animation System

#### 步骤 1: 创建动画配置 ScriptableObject

1. **创建配置资源**
   ```
   右键菜单 > Create > Game > Animation Config
   命名: WarriorAnimationConfig (或其他职业名)
   ```

2. **配置动画数据**
   ```csharp
   打开配置文件，填写:
   - Class Name: "Warrior"
   - 为每个动作状态(Idle/Walk/Run等)分配精灵帧
   - 设置帧率和循环选项
   - 配置技能粒子效果预制件
   ```

#### 步骤 2: 创建角色预制件

1. **创建GameObject**
   ```
   GameObject > 2D Object > Sprite
   命名: PlayerCharacter
   ```

2. **添加组件**
   ```
   - SpriteRenderer (自动添加)
   - Animator
   - CharacterAnimationController (脚本)
   - NetworkAnimationSync (如需联机)
   - PhotonView (如需联机)
   ```

3. **配置CharacterAnimationController**
   ```
   - Animator: 拖入Animator组件
   - Sprite Renderer: 拖入SpriteRenderer组件
   - Skill Particle Effect: 拖入粒子系统预制件
   - Mage Spell Effect: (法师专用)拖入法师粒子预制件
   ```

4. **创建Animator Controller**
   ```
   右键 > Create > Animator Controller
   命名: CharacterAnimator
   
   添加参数:
   - Name: AnimationState
   - Type: Int
   - Default: 0
   
   创建8个状态对应8种动作
   设置transitions基于AnimationState参数
   ```

#### 步骤 3: 使用动画系统

```csharp
// 在你的角色控制脚本中
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterAnimationController animController;
    
    void Start()
    {
        animController = GetComponent<CharacterAnimationController>();
        animController.SetCharacterClass("Warrior");
    }
    
    void Update()
    {
        // 根据输入播放动画
        if (Input.GetKey(KeyCode.W))
        {
            animController.PlayAnimation(
                CharacterAnimationController.AnimationState.Walk);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            animController.PlayAnimation(
                CharacterAnimationController.AnimationState.Attack);
        }
        else
        {
            animController.PlayAnimation(
                CharacterAnimationController.AnimationState.Idle);
        }
    }
}
```

---

### B. UI系统集成 / UI System Integration

#### B1. 队友血条系统 / Teammate Health Bar

1. **创建UI Canvas**
   ```
   GameObject > UI > Canvas
   Canvas Scaler:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 640x360
   - Match: 0.5 (Width/Height)
   ```

2. **创建血条预制件**
   ```
   在Canvas下创建:
   GameObject > UI > Panel (命名: TeammateHealthBar)
   
   添加子对象:
   - Text (队友名称)
   - Image (队友头像)
   - HorizontalLayoutGroup (心形容器)
     - 添加10个Image子对象 (心形图标)
   ```

3. **配置TeammateHealthBarUI组件**
   ```
   添加TeammateHealthBarUI脚本到Panel
   
   配置:
   - Max Hearts: 10
   - Heart Spacing: 8
   - Teammate Name Text: 拖入文本对象
   - Teammate Avatar: 拖入头像Image
   - Hearts列表: 添加10个心形Image
     - 为每个心形配置fullHeart/halfHeart/emptyHeart精灵
   ```

4. **使用血条**
   ```csharp
   TeammateHealthBarUI healthBar = 
       GetComponent<TeammateHealthBarUI>();
   
   // 更新血量
   healthBar.UpdateHealth(currentHealth: 75, maxHealth: 100);
   
   // 设置队友信息
   healthBar.SetTeammateInfo("Player2", avatarSprite);
   
   // 触发受伤动画
   healthBar.PlayDamageAnimation();
   ```

#### B2. 对讲机按钮 / Walkie-Talkie Button

1. **创建按钮UI**
   ```
   在Canvas下:
   GameObject > UI > Button (命名: WalkieTalkieButton)
   
   Position: (-300, -200) (左下角)
   Size: (64, 64)
   ```

2. **配置按钮**
   ```
   删除默认Button脚本
   添加PixelWalkieTalkieButton脚本
   添加AudioSource组件
   
   配置:
   - Button Image: 主Image组件
   - Button Normal Sprite: 正常状态精灵
   - Button Pressed Sprite: 按下状态精灵
   - Audio Source: AudioSource组件
   - Press Sound: 按下音效Clip
   - Release Sound: 松开音效Clip
   - Static Noise Loop: 静态噪音循环Clip
   
   添加子对象:
   - Image (信号指示器)
   - Text (状态文本)
   ```

3. **使用对讲机**
   ```csharp
   PixelWalkieTalkieButton walkieTalkie = 
       GetComponent<PixelWalkieTalkieButton>();
   
   // 检查是否正在通话
   if (walkieTalkie.IsTalking())
   {
       Debug.Log("玩家正在说话");
   }
   ```

---

### C. 场景交互系统 / Scene Interaction Systems

#### C1. 协作提示系统 / Collaboration Prompt System

1. **设置场景管理器**
   ```
   创建空GameObject: GameManager
   添加CollaborationPromptSystem脚本
   添加PhotonView (如需联机)
   添加NetworkCollaborationSync (如需联机)
   ```

2. **创建协作点**
   ```
   创建GameObject: CollaborationPoint
   Position: 放置在场景中需要协作的位置
   
   在GameManager的CollaborationPromptSystem中:
   - 添加协作点到列表
   - 配置:
     * Point Name: "宝箱"
     * Point Transform: 拖入CollaborationPoint
     * Required Players: 2
     * Activation Radius: 5.0
     * Prompt Text: "需要2名玩家开启"
   ```

3. **创建提示UI预制件**
   ```
   GameObject > UI > Panel
   添加CollaborationPromptUI脚本
   
   子对象:
   - Image (图标)
   - Text (提示文本)
   - Text (玩家计数)
   - Image (高亮效果)
   
   保存为Prefab
   在CollaborationPromptSystem中设置Prompt Prefab
   ```

#### C2. 建筑升级系统 / Building Upgrade System

1. **准备建筑对象**
   ```
   创建GameObject: Building
   添加SpriteRenderer组件
   创建子对象: EffectSpawnPoint (用于粒子效果位置)
   ```

2. **配置BuildingUpgradeSystem**
   ```
   创建空GameObject: BuildingManager
   添加BuildingUpgradeSystem脚本
   添加AudioSource组件
   
   配置Buildings列表:
   每个建筑配置:
   - Building Name: "MainHouse"
   - Building Object: 拖入建筑GameObject
   - Sprite Renderer: 拖入SpriteRenderer
   - Current Level: 0
   - Effect Spawn Point: 拖入效果生成点
   
   配置Levels列表:
   - Level 0: 基础精灵
   - Level 1: 升级精灵
   - Level 2: 高级精灵
   每个等级设置:
   - Building Sprite
   - Level Name
   - Tint Color
   - Upgrade Effect (可选)
   - Upgrade Sound (可选)
   ```

3. **触发建筑升级**
   ```csharp
   BuildingUpgradeSystem buildingSystem = 
       GetComponent<BuildingUpgradeSystem>();
   
   // 升级建筑
   buildingSystem.UpgradeBuilding("MainHouse");
   
   // 或直接设置等级
   buildingSystem.SetBuildingLevel("MainHouse", 2);
   
   // 获取当前等级
   int level = buildingSystem.GetBuildingLevel("MainHouse");
   ```

---

## 联机功能集成 / Multiplayer Integration

### 配置Photon PUN 2

1. **设置PhotonView**
   ```
   在角色预制件上:
   - 添加PhotonView组件
   - Observed Components: 添加NetworkAnimationSync
   - Synchronization: Unreliable On Change
   ```

2. **使用网络同步动画**
   ```csharp
   NetworkAnimationSync networkSync = 
       GetComponent<NetworkAnimationSync>();
   
   // 播放动画并同步
   networkSync.PlayAnimationNetworked(
       CharacterAnimationController.AnimationState.Attack);
   ```

3. **协作系统网络同步**
   ```csharp
   NetworkCollaborationSync collaborationSync = 
       GetComponent<NetworkCollaborationSync>();
   
   // Master Client激活协作点
   if (PhotonNetwork.IsMasterClient)
   {
       collaborationSync.ActivateCollaborationPoint("rare_chest");
   }
   ```

---

## 资源准备清单 / Asset Preparation Checklist

在完全运行系统前，需要准备以下资源：

### 必需的精灵图 / Required Sprites
- [ ] 角色动画帧 (每职业8动作)
- [ ] UI元素 (心形、对讲机)
- [ ] 建筑升级图 (各等级)
- [ ] 协作点图标

### 必需的音效 / Required Audio
- [ ] 对讲机音效 (3个)
- [ ] 建筑升级音效
- [ ] 技能释放音效

### 可选的粒子效果 / Optional Particle Effects
- [ ] 法师施法粒子
- [ ] 协作点高亮粒子
- [ ] 建筑升级粒子

详细规范请参阅：
- [PIXEL_ART_SPECS.md](../PIXEL_ART_SPECS.md)
- [CHARACTER_SPRITE_SPECS.md](Assets/Sprites/Characters/CHARACTER_SPRITE_SPECS.md)
- [UI_SPRITE_SPECS.md](Assets/Sprites/UI/UI_SPRITE_SPECS.md)
- [AUDIO_SPECS.md](Assets/Resources/Sounds/AUDIO_SPECS.md)

---

## 调试技巧 / Debugging Tips

### 常见问题 / Common Issues

1. **动画不播放**
   ```
   检查:
   - Animator Controller是否正确配置
   - AnimationState参数是否存在
   - 精灵帧是否正确分配
   ```

2. **粒子效果不显示**
   ```
   检查:
   - 粒子系统是否激活
   - Render Mode设置正确
   - 相机渲染层级包含粒子层
   ```

3. **血条不更新**
   ```
   检查:
   - TeammateHealthBarUI组件配置
   - Hearts列表是否包含所有心形Image
   - 精灵图是否正确分配
   ```

4. **对讲机无声音**
   ```
   检查:
   - AudioSource组件存在
   - 音效Clip已分配
   - 音量设置大于0
   - 音频监听器(Audio Listener)存在
   ```

### 测试工具 / Testing Tools

创建测试脚本快速验证功能：

```csharp
using UnityEngine;

public class SystemTester : MonoBehaviour
{
    [Header("测试目标")]
    public CharacterAnimationController animController;
    public TeammateHealthBarUI healthBar;
    public BuildingUpgradeSystem buildingSystem;
    
    void Update()
    {
        // 测试动画 (按1-8键)
        if (Input.GetKeyDown(KeyCode.Alpha1))
            animController.PlayAnimation(
                CharacterAnimationController.AnimationState.Idle);
        // ... 其他按键
        
        // 测试血量 (按H键减血)
        if (Input.GetKeyDown(KeyCode.H))
            healthBar.UpdateHealth(
                Random.Range(0, 100), 100);
        
        // 测试建筑升级 (按U键)
        if (Input.GetKeyDown(KeyCode.U))
            buildingSystem.UpgradeBuilding("MainHouse");
    }
}
```

---

## 性能优化建议 / Performance Optimization

1. **对象池 / Object Pooling**
   ```
   为频繁创建的对象使用对象池:
   - 粒子效果
   - 协作提示UI
   - 音效AudioSource
   ```

2. **动画优化 / Animation Optimization**
   ```
   - 使用Sprite Atlas减少Draw Call
   - 限制同屏动画数量
   - 远离视野的角色降低帧率
   ```

3. **网络优化 / Network Optimization**
   ```
   - 减少RPC调用频率
   - 使用状态压缩
   - 仅同步必要的动画状态变化
   ```

---

## 下一步 / Next Steps

1. 准备美术资源
2. 创建测试场景
3. 实现角色控制逻辑
4. 集成网络功能
5. 进行性能测试

---

**需要帮助?** 查看详细文档或在项目Issue中提问。
**Need help?** Check the detailed documentation or ask in project Issues.
