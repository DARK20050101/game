# game
# Pixel Co-op RPG (项目代号)

> 一个基于 Unity/GameMaker 的像素风双人联机 RPG 游戏。

## 📅 项目规划概览

本项目旨在开发一款注重流畅动作体验与双人协作的像素 RPG。核心在于低延迟的联机同步与丰富的双人交互机制。

### 🛠️ 一、技术框架 (Phase 1: Tech Stack)
- **游戏引擎**: GameMaker Studio 2 或 Unity (配合像素插件)
- **网络引擎**: Photon PUN 2
  - 目标：低延迟双人同步 (位置/攻击/掉落)
- **数据存储**: 
  - 本地：基础进度 (等级/布局)
  - 云端：联机数据 (好友/成就)

### 🚀 二、核心功能 (Phase 2: Core Features)

#### 1. 单人基础
- [ ] 角色控制 (移动/攻击/技能)
- [ ] 随机地图生成 (资源点/怪物分布)
- [ ] 基地建造系统 (模块化建筑)

#### 2. 双人联机
- [ ] 好友系统 & 匹配逻辑 (等级差±3)
- [ ] **核心同步**: 地形/怪物位置完全一致
- [ ] **协作机制**: 
  - 距离≤5格触发 Buff
  - Boss战弱点配合 (仇恨牵制 + 尾部攻击)

#### 3. 成长体系
- [ ] 技能树 (含队友增益分支)
- [ ] 基地科技 (如“共享仓库”)

### 🎨 三、美术风格 (Phase 3: Art & Style) ✅ 已实现框架
- **像素风格**: 复古像素风
- **动画标准**: 8个基础动作帧 (Idle/Walk/Run/Attack/Skill/Hit/Death/Interact)
  - ✅ 完整动画控制器系统
  - ✅ 每职业8动作配置
  - ✅ 法师施法粒子特效支持
- **视觉反馈**: 
  - ✅ 施法粒子流动效果系统
  - ✅ 联机协作高亮标识系统
  - ✅ 建筑升级像素变化动画
- **UI/UX**: 
  - ✅ 像素Hearts血条系统（队友实时状态）
  - ✅ 像素对讲机语音按钮（含复古音效）
  - ✅ 协作点提示系统

### 🧪 四、测试目标 (Phase 4: QA)
- **稳定性**: 模拟 200+ 并发，目标延迟 ≤200ms
- **平衡性**: 双人掉率 = 单人 x 1.5
- **体验**: 极简联机邀请流程

---

## 📂 目录结构

```
/Assets
  /Scripts
    /Character
      - CharacterAnimationController.cs (角色动画控制器)
      - AnimationConfig.cs (动画配置ScriptableObject)
    /UI
      - TeammateHealthBarUI.cs (队友像素血条)
      - PixelWalkieTalkieButton.cs (对讲机语音按钮)
    /Scene
      - CollaborationPromptSystem.cs (协作提示系统)
      - BuildingUpgradeSystem.cs (建筑升级系统)
    /Network
      - NetworkAnimationSync.cs (网络动画同步)
      - NetworkCollaborationSync.cs (网络协作同步)
  /Sprites
    /Characters (角色动画帧精灵图)
      /Warrior, /Mage, /Archer
    /Environment (场景和建筑精灵图)
    /UI (UI元素精灵图)
  /Prefabs
    /Character, /UI, /Scene
  /Resources
    /Animations (动画配置)
    /Sounds (音效资源)
  - example_configuration.json (示例配置)
  - README.md

/PIXEL_ART_SPECS.md (像素美术详细规范)
```

## 📖 文档

- **[PIXEL_ART_SPECS.md](PIXEL_ART_SPECS.md)** - 像素美术与交互细节完整规范
- **[Assets/README.md](Assets/README.md)** - 资源目录使用指南
- **[Assets/Sprites/Characters/CHARACTER_SPRITE_SPECS.md](Assets/Sprites/Characters/CHARACTER_SPRITE_SPECS.md)** - 角色精灵图规范
- **[Assets/Sprites/UI/UI_SPRITE_SPECS.md](Assets/Sprites/UI/UI_SPRITE_SPECS.md)** - UI精灵图规范
- **[Assets/Resources/Sounds/AUDIO_SPECS.md](Assets/Resources/Sounds/AUDIO_SPECS.md)** - 音效规范

## 🎯 像素美术与交互系统 (已实现)

### ✅ 角色动画系统
- **8个动作状态**: Idle, Walk, Run, Attack, Skill, Hit, Death, Interact
- **职业支持**: Warrior (战士), Mage (法师), Archer (弓箭手)
- **法师特效**: 施法时的魔法粒子流动效果
- **网络同步**: 支持Photon PUN 2多人动画同步

### ✅ 场景交互系统
- **协作提示**: 稀有点自动检测附近玩家，显示协作提示UI
- **建筑升级**: 3级升级系统，带像素变化动画和粒子特效
- **视觉反馈**: 金色高亮、脉冲动画、协作距离检测

### ✅ UI/UX系统
- **队友血条**: 像素Hearts健康显示，支持满心/半心/空心状态
- **对讲机按钮**: 按压式语音通话，含复古音效（按下/松开/静态噪音）
- **状态指示**: 实时显示通话状态，信号指示器闪烁效果

### 📦 待补充资源
- [ ] 角色动画帧精灵图（各职业8动作）
- [ ] UI精灵图（心形、对讲机图标等）
- [ ] 音效文件（对讲机音效、建筑升级音效）
- [ ] 建筑升级精灵图（各等级）

## 🤝 贡献与反馈
欢迎提交 Issue 或 PR 参与开发讨论。
