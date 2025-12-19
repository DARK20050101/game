# Character Sprite Specifications / 角色精灵图规范

## 战士 (Warrior) / Warrior Class

### 动作帧列表 / Action Frame List

#### 1. Idle (待机)
- **文件命名**: `warrior_idle_01.png` to `warrior_idle_06.png`
- **帧数**: 6帧
- **帧率**: 8 FPS
- **循环**: 是
- **描述**: 角色站立呼吸动画，微微晃动

#### 2. Walk (行走)
- **文件命名**: `warrior_walk_01.png` to `warrior_walk_08.png`
- **帧数**: 8帧
- **帧率**: 12 FPS
- **循环**: 是
- **描述**: 标准行走动画，左右脚交替

#### 3. Run (奔跑)
- **文件命名**: `warrior_run_01.png` to `warrior_run_08.png`
- **帧数**: 8帧
- **帧率**: 16 FPS
- **循环**: 是
- **描述**: 快速奔跑动画，身体前倾

#### 4. Attack (攻击)
- **文件命名**: `warrior_attack_01.png` to `warrior_attack_06.png`
- **帧数**: 6帧
- **帧率**: 16 FPS
- **循环**: 否
- **描述**: 剑斩动作，挥剑轨迹清晰
- **关键帧**: 第3帧为击中判定帧

#### 5. Skill (技能)
- **文件命名**: `warrior_skill_01.png` to `warrior_skill_08.png`
- **帧数**: 8帧
- **帧率**: 14 FPS
- **循环**: 否
- **描述**: 旋风斩，360度旋转攻击
- **特效**: 剑刃轨迹粒子

#### 6. Hit (受击)
- **文件命名**: `warrior_hit_01.png` to `warrior_hit_04.png`
- **帧数**: 4帧
- **帧率**: 16 FPS
- **循环**: 否
- **描述**: 后退并护盾格挡姿势

#### 7. Death (死亡)
- **文件命名**: `warrior_death_01.png` to `warrior_death_08.png`
- **帧数**: 8帧
- **帧率**: 10 FPS
- **循环**: 否
- **描述**: 跪倒、倒地、消散动画
- **最终帧**: 保持显示

#### 8. Interact (互动)
- **文件命名**: `warrior_interact_01.png` to `warrior_interact_06.png`
- **帧数**: 6帧
- **帧率**: 12 FPS
- **循环**: 否
- **描述**: 弯腰拾取或操作动作

---

## 法师 (Mage) / Mage Class

### 动作帧列表 / Action Frame List

#### 1. Idle (待机)
- **文件命名**: `mage_idle_01.png` to `mage_idle_06.png`
- **帧数**: 6帧
- **帧率**: 8 FPS
- **循环**: 是
- **描述**: 法杖微微发光，长袍飘动

#### 2. Walk (行走)
- **文件命名**: `mage_walk_01.png` to `mage_walk_08.png`
- **帧数**: 8帧
- **帧率**: 12 FPS
- **循环**: 是
- **描述**: 标准行走，法杖握在手中

#### 3. Run (奔跑)
- **文件命名**: `mage_run_01.png` to `mage_run_08.png`
- **帧数**: 8帧
- **帧率**: 16 FPS
- **循环**: 是
- **描述**: 快速奔跑，长袍飘扬

#### 4. Attack (攻击)
- **文件命名**: `mage_attack_01.png` to `mage_attack_06.png`
- **帧数**: 6帧
- **帧率**: 12 FPS
- **循环**: 否
- **描述**: 法杖挥舞，发射能量弹
- **关键帧**: 第4帧为发射帧

#### 5. Skill (技能) ⭐ 含粒子效果
- **文件命名**: `mage_skill_01.png` to `mage_skill_08.png`
- **帧数**: 8帧
- **帧率**: 14 FPS
- **循环**: 否
- **描述**: 施法姿势，法杖高举
- **粒子效果配置**:
  - 类型: 魔法光点流动
  - 颜色: 蓝色 (100, 150, 255) → 紫色 (180, 100, 255)
  - 数量: 20-30个粒子
  - 运动: 从法杖顶端螺旋向外
  - 大小: 2-4像素
  - 生命周期: 0.5-1秒
  - 发射时机: 第2-6帧

#### 6. Hit (受击)
- **文件命名**: `mage_hit_01.png` to `mage_hit_04.png`
- **帧数**: 4帧
- **帧率**: 16 FPS
- **循环**: 否
- **描述**: 魔法护盾显现，后退防御

#### 7. Death (死亡)
- **文件命名**: `mage_death_01.png` to `mage_death_08.png`
- **帧数**: 8帧
- **帧率**: 10 FPS
- **循环**: 否
- **描述**: 魔法力量消散，身体虚化消失

#### 8. Interact (互动)
- **文件命名**: `mage_interact_01.png` to `mage_interact_06.png`
- **帧数**: 6帧
- **帧率**: 12 FPS
- **循环**: 否
- **描述**: 用法杖轻点，魔法感应动作

---

## 弓箭手 (Archer) / Archer Class

### 动作帧列表 / Action Frame List

#### 1. Idle (待机)
- **文件命名**: `archer_idle_01.png` to `archer_idle_06.png`
- **帧数**: 6帧
- **帧率**: 8 FPS
- **循环**: 是
- **描述**: 持弓站立，警觉姿态

#### 2. Walk (行走)
- **文件命名**: `archer_walk_01.png` to `archer_walk_08.png`
- **帧数**: 8帧
- **帧率**: 12 FPS
- **循环**: 是
- **描述**: 轻快行走，弓搭在肩上

#### 3. Run (奔跑)
- **文件命名**: `archer_run_01.png` to `archer_run_08.png`
- **帧数**: 8帧
- **帧率**: 16 FPS
- **循环**: 是
- **描述**: 快速奔跑，身体敏捷

#### 4. Attack (攻击)
- **文件命名**: `archer_attack_01.png` to `archer_attack_06.png`
- **帧数**: 6帧
- **帧率**: 14 FPS
- **循环**: 否
- **描述**: 拉弓、瞄准、射箭三个阶段
- **关键帧**: 第5帧为箭矢发射帧

#### 5. Skill (技能)
- **文件命名**: `archer_skill_01.png` to `archer_skill_08.png`
- **帧数**: 8帧
- **帧率**: 14 FPS
- **循环**: 否
- **描述**: 箭雨技能，向天空射出多支箭
- **特效**: 3-5支箭矢轨迹

#### 6. Hit (受击)
- **文件命名**: `archer_hit_01.png` to `archer_hit_04.png`
- **帧数**: 4帧
- **帧率**: 16 FPS
- **循环**: 否
- **描述**: 侧身闪避姿势

#### 7. Death (死亡)
- **文件命名**: `archer_death_01.png` to `archer_death_08.png`
- **帧数**: 8帧
- **帧率**: 10 FPS
- **循环**: 否
- **描述**: 膝盖着地，弓掉落，倒下

#### 8. Interact (互动)
- **文件命名**: `archer_interact_01.png` to `archer_interact_06.png`
- **帧数**: 6帧
- **帧率**: 12 FPS
- **循环**: 否
- **描述**: 蹲下检查或采集动作

---

## 技术规范 / Technical Specifications

### 通用设置 / Common Settings
- **分辨率**: 32x32 像素 (推荐) 或 48x48 像素
- **格式**: PNG with transparency
- **色彩模式**: Indexed Color (256 colors max)
- **背景**: 完全透明
- **导出**: 无抗锯齿、无压缩

### Unity导入设置 / Unity Import Settings
```
Texture Type: Sprite (2D and UI)
Sprite Mode: Single
Pixels Per Unit: 32
Filter Mode: Point (no filter)
Compression: None
Max Size: 256 (or as needed)
Format: RGBA 32 bit
```

### 动画控制器参数 / Animation Controller Parameters
- **参数名**: AnimationState
- **类型**: Integer
- **值映射**:
  - 0 = Idle
  - 1 = Walk
  - 2 = Run
  - 3 = Attack
  - 4 = Skill
  - 5 = Hit
  - 6 = Death
  - 7 = Interact

---

## 占位图指南 / Placeholder Guide

在实际美术资源完成前，可以使用以下方式创建占位图：

1. **简单几何形状**：使用基本矩形、圆形表示角色轮廓
2. **颜色区分**：不同职业使用不同颜色（战士-红色、法师-蓝色、弓箭手-绿色）
3. **关键姿态**：至少绘制每个动作的关键帧姿态
4. **统一尺寸**：确保所有帧使用相同的画布尺寸

---

**注意**: 本文档定义了所有需要的精灵图规格。美术人员应严格按照此规范创建资源。
**Note**: This document defines all required sprite specifications. Artists should strictly follow these specifications when creating assets.
