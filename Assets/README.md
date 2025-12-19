# Assets Directory / 资源目录

本目录包含游戏的所有资源文件，包括脚本、精灵图、预制件和其他资源。
This directory contains all game assets including scripts, sprites, prefabs, and other resources.

## 目录结构 / Directory Structure

### Scripts / 脚本
游戏逻辑和系统脚本
Game logic and system scripts

- **Character/** - 角色相关脚本（动画、移动、战斗等）
- **UI/** - 用户界面脚本（血条、对讲机、菜单等）
- **Scene/** - 场景管理脚本（协作系统、建筑升级等）
- **Network/** - 网络同步脚本（多人游戏逻辑）

### Sprites / 精灵图
所有像素艺术资源
All pixel art assets

- **Characters/** - 角色精灵图
  - Warrior/ - 战士职业动画帧
  - Mage/ - 法师职业动画帧
  - Archer/ - 弓箭手职业动画帧
- **Environment/** - 环境和场景精灵图
  - Buildings/ - 建筑物
  - Props/ - 道具和装饰
- **UI/** - UI元素精灵图
  - HealthBar/ - 血条元素
  - WalkieTalkie/ - 对讲机按钮

### Prefabs / 预制件
可复用的游戏对象模板
Reusable game object templates

- **Character/** - 角色预制件
- **UI/** - UI元素预制件
- **Scene/** - 场景对象预制件

### Resources / 资源
运行时动态加载的资源
Resources for runtime dynamic loading

- **Animations/** - 动画控制器和配置
- **Sounds/** - 音效和音乐文件

## 使用指南 / Usage Guide

### 导入像素艺术资源 / Importing Pixel Art Assets

1. 将像素图放入对应的 `Sprites/` 子目录
2. 在Unity中设置导入参数：
   - Texture Type: Sprite (2D and UI)
   - Pixels Per Unit: 16 或 32
   - Filter Mode: Point (no filter)
   - Compression: None

### 创建动画 / Creating Animations

1. 使用 `Scripts/Character/AnimationConfig.cs` 创建ScriptableObject
2. 配置每个职业的8种动画状态
3. 为每个状态分配对应的精灵帧序列

### 配置UI / Setting Up UI

1. 使用 `Scripts/UI/` 中的脚本组件
2. 将精灵图从 `Sprites/UI/` 分配到组件
3. 配置颜色、尺寸等参数

## 资源规范 / Asset Specifications

详细的像素艺术规范和技术要求，请参阅项目根目录的 `PIXEL_ART_SPECS.md` 文档。

For detailed pixel art specifications and technical requirements, please refer to the `PIXEL_ART_SPECS.md` document in the project root directory.

## 待补充资源 / Pending Assets

当前目录结构已创建，但需要以下实际资源文件：
The directory structure is created, but the following actual asset files are needed:

### 角色动画帧 / Character Animation Frames
- [ ] 战士 (Warrior): 8 动作 × 6-8 帧
- [ ] 法师 (Mage): 8 动作 × 6-8 帧
- [ ] 弓箭手 (Archer): 8 动作 × 6-8 帧

### UI元素 / UI Elements
- [ ] 心形血条图标 (3种状态)
- [ ] 对讲机按钮 (2种状态)
- [ ] 信号指示器

### 音效 / Sound Effects
- [ ] 对讲机音效 (3个)
- [ ] 建筑升级音效
- [ ] 技能释放音效

### 粒子效果 / Particle Effects
- [ ] 法师施法粒子
- [ ] 协作点高亮粒子
- [ ] 建筑升级粒子

---

**注意 / Note:** 所有像素艺术必须遵循像素完美原则，避免抗锯齿和模糊效果。
All pixel art must follow pixel-perfect principles, avoiding anti-aliasing and blur effects.
