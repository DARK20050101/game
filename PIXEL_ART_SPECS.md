# 像素美术与交互细节规范 / Pixel Art and Interaction Details Specification

## 概述 / Overview

本文档详细说明了游戏中像素美术资源的规范和交互系统的实现细节。
This document details the specifications for pixel art assets and implementation details of interaction systems in the game.

---

## 一、角色动画规范 / Character Animation Specifications

### 1.1 动画帧标准 / Animation Frame Standards

每个职业需要包含以下 **8种动作状态**，每种动作包含独立的像素帧序列：
Each class requires the following **8 action states**, each containing independent pixel frame sequences:

| 动作名称 / Action | 英文 | 帧数建议 / Suggested Frames | 是否循环 / Loop | 帧率 / FPS |
|-----------------|------|------------------------|--------------|-----------|
| 待机 | Idle | 4-6 帧 | 是 | 8-12 |
| 行走 | Walk | 6-8 帧 | 是 | 12 |
| 奔跑 | Run | 6-8 帧 | 是 | 16 |
| 攻击 | Attack | 4-6 帧 | 否 | 12-16 |
| 技能 | Skill | 6-8 帧 | 否 | 12-16 |
| 受击 | Hit | 2-4 帧 | 否 | 16 |
| 死亡 | Death | 6-8 帧 | 否 | 10 |
| 互动 | Interact | 4-6 帧 | 否 | 12 |

### 1.2 像素分辨率 / Pixel Resolution

- **角色基础尺寸**: 32x32 像素 或 48x48 像素
- **导出格式**: PNG (透明背景)
- **色彩深度**: 8-bit indexed color (256色调色板)
- **像素完美**: 禁止抗锯齿，保持像素锐利边缘

### 1.3 职业特定动画 / Class-Specific Animations

#### 战士 (Warrior)
- 攻击动画：剑斩动作，带有轨迹残影
- 技能动画：旋风斩，360度旋转攻击

#### 法师 (Mage)
- 攻击动画：法杖挥舞，发射火球
- 技能动画：施法姿势，**包含粒子特效**
  - 粒子效果：魔法光点从法杖向目标飞出
  - 颜色：蓝色/紫色光芒
  - 持续时间：0.5-1秒

#### 弓箭手 (Archer)
- 攻击动画：拉弓、瞄准、射箭
- 技能动画：箭雨技能，多重箭矢

---

## 二、场景细节规范 / Scene Detail Specifications

### 2.1 稀有点协作提示 / Rare Point Collaboration Prompts

稀有资源点或特殊互动点需要多人协作时，显示特殊UI提示：

**视觉元素 / Visual Elements:**
- 金色脉冲光环（距离≤5格触发）
- 像素图标指示所需玩家数量
- 玩家计数器：显示"2/2"等进度
- 高亮描边效果

**实现位置 / Implementation:**
- 文件：`Assets/Scripts/Scene/CollaborationPromptSystem.cs`
- 激活半径：5格（可配置）
- 粒子效果：金色光点环绕

### 2.2 建筑升级像素变化 / Building Upgrade Pixel Changes

建筑在不同等级展示不同的像素外观：

**升级阶段示例 / Upgrade Stage Example:**

| 等级 | 外观描述 | 像素变化 |
|-----|---------|---------|
| 1级 | 木质简易建筑 | 基础32x32像素结构 |
| 2级 | 石材加固 | 添加石材纹理，增加细节 |
| 3级 | 精致装饰 | 添加窗户、旗帜等装饰元素 |

**升级动画效果 / Upgrade Animation Effects:**
- 建筑闪烁效果（白色闪光）
- 建筑缩放脉冲（1.0 -> 1.1 -> 1.0）
- 粒子特效：金色施工粉尘
- 升级音效：叮叮当当的建造声

**实现位置 / Implementation:**
- 文件：`Assets/Scripts/Scene/BuildingUpgradeSystem.cs`
- 动画时长：1秒
- 粒子颜色：RGB(255, 204, 0) - 金色

---

## 三、UI/UX交互规范 / UI/UX Interaction Specifications

### 3.1 队友像素Hearts血条 / Teammate Pixel Hearts Health Bar

**设计规范 / Design Specifications:**
- 心形图标：8x8 像素
- 颜色方案：
  - 满血：红色 RGB(255, 76, 76)
  - 半血：红色（使用半心形sprite）
  - 空血：灰色 RGB(128, 128, 128)
- 最大心形数：10个
- 心形间距：8像素

**状态显示 / Status Display:**
- 每个心形代表10%血量
- 支持半心形显示（5%血量）
- 受伤时心形闪烁动画

**实现位置 / Implementation:**
- 文件：`Assets/Scripts/UI/TeammateHealthBarUI.cs`
- 附加信息：队友名称、头像

### 3.2 像素对讲机语音按钮 / Pixel Walkie-Talkie Voice Button

**视觉设计 / Visual Design:**
- 按钮尺寸：64x64 像素
- 双态设计：
  - 正常状态：浅灰色对讲机图标
  - 按下状态：深灰色，缩小至90%
- 信号指示器：
  - 待机：灰色圆点
  - 通话：绿色闪烁圆点

**音效设计 / Audio Design:**
复古对讲机音效包含三个关键声音：

1. **按下音效 (Press Sound)**
   - 类型：短促的"咔嗒"声
   - 持续时间：~0.1秒
   - 音调：略微随机变化（0.95-1.05倍速）

2. **静态噪音循环 (Static Noise Loop)**
   - 类型：轻微的白噪音/电流声
   - 持续时间：循环播放
   - 音量：30%
   - 播放时机：按住按钮期间

3. **松开音效 (Release Sound)**
   - 类型：短促的"咔"声
   - 持续时间：~0.1秒
   - 音调：略微随机变化

**交互行为 / Interaction Behavior:**
- 按压式交互（按住说话，松开停止）
- 状态文本更新：
  - 待机："按住说话 / HOLD TO TALK"
  - 通话："通话中... / TALKING..."
- 信号粒子效果：绿色光点向外发散

**实现位置 / Implementation:**
- 文件：`Assets/Scripts/UI/PixelWalkieTalkieButton.cs`
- 集成接口：可对接 Photon Voice 或其他语音SDK

---

## 四、联机识别与协作高亮 / Multiplayer Recognition and Collaboration Highlighting

### 4.1 玩家识别 / Player Recognition

**队友标识 / Teammate Identification:**
- 头顶显示名称标签（像素字体）
- 角色描边：
  - 自己：白色描边
  - 队友：绿色描边
  - 其他玩家：无描边

### 4.2 协作高亮 / Collaboration Highlighting

**距离Buff触发视觉 / Distance Buff Visual Trigger:**
- 距离≤5格时，角色周围出现金色光环
- 光环脉冲效果（呼吸灯效果）
- Buff图标显示在血条旁边

**协作行动指示 / Collaborative Action Indicators:**
- 协作互动点：金色高亮 + 脉冲动画
- 参与玩家连线：虚线连接参与者
- 完成度指示：进度条或计数器

---

## 五、像素风格一致性指南 / Pixel Art Consistency Guidelines

### 5.1 通用原则 / General Principles

1. **像素完美对齐 / Pixel Perfect Alignment**
   - 所有像素元素必须对齐到整数坐标
   - 禁止使用Unity的抗锯齿功能

2. **调色板限制 / Palette Limitation**
   - 每个角色/场景使用统一的调色板
   - 建议总色数不超过32色

3. **缩放规则 / Scaling Rules**
   - 使用整数倍缩放（2x, 3x, 4x）
   - 导入设置：Filter Mode = Point (no filter)
   - Compression = None

### 5.2 导入设置模板 / Import Settings Template

```
Texture Type: Sprite (2D and UI)
Pixels Per Unit: 16 或 32
Filter Mode: Point (no filter)
Compression: None
Max Size: 根据需要设置
Format: RGBA 32 bit
```

---

## 六、资源文件组织 / Asset File Organization

### 6.1 目录结构 / Directory Structure

```
/Assets
  /Sprites
    /Characters
      /Warrior
        - warrior_idle_01.png ... warrior_idle_06.png
        - warrior_walk_01.png ... warrior_walk_08.png
        - warrior_run_01.png ... warrior_run_08.png
        - warrior_attack_01.png ... warrior_attack_06.png
        - warrior_skill_01.png ... warrior_skill_08.png
        - warrior_hit_01.png ... warrior_hit_04.png
        - warrior_death_01.png ... warrior_death_08.png
        - warrior_interact_01.png ... warrior_interact_06.png
      /Mage
        - (类似结构)
      /Archer
        - (类似结构)
    /Environment
      /Buildings
        - house_level1.png
        - house_level2.png
        - house_level3.png
      /CollaborationPoints
        - rare_point_icon.png
        - collaboration_highlight.png
    /UI
      /HealthBar
        - heart_full.png
        - heart_half.png
        - heart_empty.png
      /WalkieTalkie
        - walkie_talkie_normal.png
        - walkie_talkie_pressed.png
        - signal_indicator.png
  /Resources
    /Animations
      - (动画控制器配置)
    /Sounds
      /UI
        - walkie_press.wav
        - walkie_release.wav
        - walkie_static_loop.wav
      /Effects
        - upgrade_sound.wav
```

### 6.2 命名规范 / Naming Convention

格式：`{类型}_{对象}_{状态}_{帧号}.png`

示例：
- `char_warrior_idle_01.png`
- `ui_heart_full.png`
- `env_building_level2.png`

---

## 七、实现清单 / Implementation Checklist

### 已完成 / Completed ✓
- [x] 角色动画控制器系统
- [x] 动画配置数据结构
- [x] 队友血条UI系统
- [x] 像素对讲机语音按钮
- [x] 协作提示系统
- [x] 建筑升级系统
- [x] 完整技术文档

### 待美术资源 / Pending Art Assets
- [ ] 战士8动作像素帧
- [ ] 法师8动作像素帧
- [ ] 弓箭手8动作像素帧
- [ ] 建筑升级像素图（各等级）
- [ ] UI像素图标（心形、对讲机等）
- [ ] 协作点图标和高亮效果

### 待音效资源 / Pending Audio Assets
- [ ] 对讲机按下音效
- [ ] 对讲机松开音效
- [ ] 静态噪音循环音效
- [ ] 建筑升级音效

---

## 八、技术集成说明 / Technical Integration Notes

### 8.1 Unity集成 / Unity Integration

1. **将脚本导入Unity项目 / Import Scripts to Unity Project**
   - 复制所有`.cs`文件到Unity项目的`Assets/Scripts`目录

2. **配置动画控制器 / Configure Animation Controller**
   - 创建Animator Controller
   - 添加8个动画状态
   - 设置状态转换条件（基于AnimationState整数参数）

3. **创建ScriptableObject配置 / Create ScriptableObject Configs**
   - 右键菜单: Create > Game > Animation Config
   - 为每个职业配置动画帧和特效

4. **设置Prefabs / Setup Prefabs**
   - 创建角色Prefab，挂载`CharacterAnimationController`
   - 创建UI Prefab，挂载相应的UI脚本

### 8.2 网络同步建议 / Network Sync Recommendations

使用Photon PUN 2时：
- 同步动画状态：使用RPC调用`PlayAnimation()`
- 同步血量：使用PhotonView同步TeammateHealthBarUI
- 语音通话：集成Photon Voice SDK

---

## 九、测试要点 / Testing Checklist

- [ ] 所有职业8种动画正常播放
- [ ] 法师技能粒子效果正确显示
- [ ] 队友血条实时更新准确
- [ ] 对讲机按钮音效正常播放
- [ ] 协作点在多人接近时正确激活
- [ ] 建筑升级动画流畅无卡顿
- [ ] 像素对齐，无模糊或锯齿
- [ ] 联机环境下识别标识清晰

---

## 联系与反馈 / Contact and Feedback

如有疑问或建议，请在项目Issue中提出。
For questions or suggestions, please submit them in the project Issues.

**文档版本 / Document Version:** 1.0  
**最后更新 / Last Updated:** 2025-12-19
