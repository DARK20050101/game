# UI Sprite Specifications / UI精灵图规范

## 血条系统 / Health Bar System

### 心形图标 / Heart Icons

所有心形图标使用 **8x8 像素** 分辨率

#### 1. heart_full.png (满心)
```
尺寸: 8x8 像素
颜色: 红色 RGB(255, 76, 76)
描述: 完整的像素心形
设计要点:
- 对称设计
- 清晰的心形轮廓
- 实心填充
```

**像素图示例 (X = 红色像素)**:
```
  XX  XX
 XXXXXXXX
XXXXXXXXXX
XXXXXXXXXX
 XXXXXXXX
  XXXXXX
   XXXX
    XX
```

#### 2. heart_half.png (半心)
```
尺寸: 8x8 像素
颜色: 红色 RGB(255, 76, 76) + 深红色 RGB(180, 50, 50)
描述: 半满的心形（左半部分填充）
设计要点:
- 左侧满心，右侧空心
- 或使用渐变效果
```

**像素图示例 (X = 红色, O = 深红)**:
```
  XX  OO
 XXXXOOOO
XXXXXOOOOO
XXXXXOOOOO
 XXXXOOOO
  XXXXOO
   XXOO
    XO
```

#### 3. heart_empty.png (空心)
```
尺寸: 8x8 像素
颜色: 灰色 RGB(128, 128, 128)
描述: 空心的心形轮廓
设计要点:
- 只有边框，内部透明或暗色
- 表示失去的生命值
```

**像素图示例 (X = 灰色边框, . = 透明/暗色)**:
```
  XX  XX
 X....X.X
X..........X
X..........X
 X........X
  X......X
   X....X
    XX
```

---

## 对讲机按钮 / Walkie-Talkie Button

### 主按钮图标 / Main Button Icons

所有对讲机图标使用 **64x64 像素** 分辨率

#### 1. walkie_talkie_normal.png (正常状态)
```
尺寸: 64x64 像素
主色调: 浅灰色 RGB(180, 180, 180)
高光色: 白色 RGB(240, 240, 240)
阴影色: 深灰色 RGB(100, 100, 100)
描述: 复古对讲机外观，带天线
设计元素:
- 矩形机身 (约40x48像素)
- 天线 (6x16像素)
- 侧面按钮细节
- 扬声器网格
```

**设计布局**:
```
       |   天线
       |
    [======]   
    |      |   机身上部
    | 🔊  |   扬声器
    |______|
    |      |   机身下部
    | PTT  |   按键标识
    [======]
```

#### 2. walkie_talkie_pressed.png (按下状态)
```
尺寸: 64x64 像素
主色调: 深灰色 RGB(140, 140, 140) - 比正常状态暗
描述: 与normal相同但整体暗一些，天线略微倾斜
变化:
- 整体色调降低20%
- 微微缩小 (由脚本处理缩放)
- PTT按钮显示按下效果
```

---

## 信号指示器 / Signal Indicator

#### signal_indicator.png
```
尺寸: 16x16 像素
包含两个状态版本:
1. 待机状态: 灰色 RGB(128, 128, 128)
2. 通话状态: 绿色 RGB(100, 255, 100) (由脚本动态着色)

设计: 简单的圆形指示灯
```

**像素图示例 (圆形)**:
```
   XXXX
  XXXXXX
 XXXXXXXX
 XXXXXXXX
 XXXXXXXX
 XXXXXXXX
  XXXXXX
   XXXX
```

---

## 协作点图标 / Collaboration Point Icons

### rare_point_icon.png (稀有点图标)
```
尺寸: 32x32 像素
颜色: 金色 RGB(255, 215, 0)
描述: 星形或钻石形状，表示稀有互动点
动画: 由脚本处理旋转和脉冲效果
```

**设计示例 (星形)**:
```
       XX
       XX
      XXXX
   XX XXXX XX
   XXXXXXXXXX
    XXXXXXXX
  XXXXXXXXXXXX
 XXXXXXXXXXXXXX
  XXXXXXXXXXXX
    XXXXXXXX
   XX XXXX XX
      XXXX
       XX
```

### collaboration_highlight.png (协作高亮圈)
```
尺寸: 64x64 像素
颜色: 金色半透明 RGBA(255, 215, 0, 180)
描述: 圆环或光晕效果
用途: 在协作点周围显示高亮范围
```

**设计示例 (圆环)**:
```
      XXXXXXXX
    XX        XX
  XX            XX
 XX              XX
 X                X
 X                X
 XX              XX
  XX            XX
    XX        XX
      XXXXXXXX
```

---

## 建筑等级图标 / Building Level Icons

### 建筑示例：房屋 / Example Building: House

#### house_level1.png (1级房屋)
```
尺寸: 48x48 像素
风格: 简单木质小屋
颜色方案:
- 墙壁: 棕色 RGB(139, 90, 43)
- 屋顶: 深棕色 RGB(90, 50, 20)
- 门窗: 深色 RGB(50, 30, 20)
```

#### house_level2.png (2级房屋)
```
尺寸: 48x48 像素
风格: 石材加固房屋
新增元素:
- 石材墙壁材质
- 窗户增加
- 烟囱
```

#### house_level3.png (3级房屋)
```
尺寸: 48x48 像素
风格: 精致装饰房屋
新增元素:
- 装饰性窗框
- 旗帜或花盆
- 更多细节
```

---

## 粒子纹理 / Particle Textures

### particle_magic.png (魔法粒子)
```
尺寸: 8x8 像素
颜色: 蓝紫渐变
用途: 法师技能粒子
形状: 圆形光点
```

### particle_spark.png (火花粒子)
```
尺寸: 4x4 像素
颜色: 金黄色
用途: 建筑升级、协作点效果
形状: 菱形或星形
```

### particle_glow.png (光晕粒子)
```
尺寸: 16x16 像素
颜色: 白色半透明
用途: 角色高亮效果
形状: 柔和的圆形光晕（外围渐变透明）
```

---

## 导出清单 / Export Checklist

### 血条系统 (3个文件)
- [ ] heart_full.png (8x8)
- [ ] heart_half.png (8x8)
- [ ] heart_empty.png (8x8)

### 对讲机系统 (3个文件)
- [ ] walkie_talkie_normal.png (64x64)
- [ ] walkie_talkie_pressed.png (64x64)
- [ ] signal_indicator.png (16x16)

### 协作系统 (2个文件)
- [ ] rare_point_icon.png (32x32)
- [ ] collaboration_highlight.png (64x64)

### 建筑系统 (每种建筑3个等级)
- [ ] house_level1.png (48x48)
- [ ] house_level2.png (48x48)
- [ ] house_level3.png (48x48)
- [ ] (其他建筑类似)

### 粒子纹理 (3个文件)
- [ ] particle_magic.png (8x8)
- [ ] particle_spark.png (4x4)
- [ ] particle_glow.png (16x16)

---

## Unity导入配置 / Unity Import Configuration

### 标准UI精灵 / Standard UI Sprites
```
Texture Type: Sprite (2D and UI)
Sprite Mode: Single
Pixels Per Unit: 16
Filter Mode: Point (no filter)
Compression: None
Format: RGBA 32 bit
Wrap Mode: Clamp
```

### 粒子纹理 / Particle Textures
```
Texture Type: Default
Filter Mode: Bilinear (允许粒子柔和)
Wrap Mode: Clamp
Format: RGBA 32 bit
```

---

## 调色板建议 / Color Palette Suggestions

### 主色调 / Primary Colors
- 红色 (血条): RGB(255, 76, 76)
- 金色 (协作): RGB(255, 215, 0)
- 绿色 (通话): RGB(100, 255, 100)
- 灰色 (UI): RGB(180, 180, 180)

### 辅助色 / Secondary Colors
- 深红色: RGB(180, 50, 50)
- 深金色: RGB(200, 170, 0)
- 深绿色: RGB(50, 200, 50)
- 深灰色: RGB(100, 100, 100)

### 高光与阴影 / Highlights & Shadows
- 高光: RGB(240, 240, 240)
- 阴影: RGB(50, 50, 50)

---

**像素完美原则 / Pixel Perfect Principles:**
1. 所有UI元素必须对齐到整数像素坐标
2. 避免使用抗锯齿
3. 使用有限的调色板保持风格统一
4. 测试时使用整数倍缩放 (2x, 3x, 4x)

**测试检查 / Testing Checklist:**
- [ ] 在不同分辨率下清晰可见
- [ ] 像素边缘锐利无模糊
- [ ] 动画过渡自然流畅
- [ ] 颜色对比度足够（可读性）
