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

### 🎨 三、美术风格 (Phase 3: Art & Style)
- **像素风格**: 复古像素风
- **动画标准**: 8向基础动作 (Idle/Run/Atk/Hit/Die/Interact)
- **视觉反馈**: 
  - 施法粒子流动效果
  - 联机协作高亮标识
- **UI/UX**: 
  - 像素对讲机语音按钮
  - 队友状态实时监控

### 🧪 四、测试目标 (Phase 4: QA)
- **稳定性**: 模拟 200+ 并发，目标延迟 ≤200ms
- **平衡性**: 双人掉率 = 单人 x 1.5
- **体验**: 极简联机邀请流程

---

## 📂 目录结构 (建议)

```
/Assets
  /Scripts
    /Network (Photon Manager)
    /Player (Controller, Stats)
    /Map (Generation, Interactive)
  /Sprites
    /Characters
    /Environment
  /Prefabs
  /Resources
```

## 🤝 贡献与反馈
欢迎提交 Issue 或 PR 参与开发讨论。
