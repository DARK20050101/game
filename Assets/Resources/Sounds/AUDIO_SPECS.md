# Audio Specifications / 音效规范

## 概述 / Overview

本文档定义了游戏中所有音效的规格和要求。所有音效应遵循复古像素游戏的音效风格。
This document defines specifications and requirements for all audio in the game. All audio should follow retro pixel game audio style.

---

## 一、对讲机音效 / Walkie-Talkie Sound Effects

### 1.1 walkie_press.wav (按下音效)

**技术规格 / Technical Specs:**
- **格式**: WAV (未压缩) 或 OGG (高质量)
- **采样率**: 44100 Hz
- **位深度**: 16-bit
- **声道**: Mono (单声道)
- **时长**: ~0.1 秒

**音效描述 / Sound Description:**
- 类型: 短促的机械"咔嗒"声
- 特点: 清脆、瞬间、复古
- 参考: 老式对讲机PTT按钮按下的声音
- 音调: 中高频 (800-1200 Hz)

**制作建议 / Production Tips:**
```
可以使用以下方式制作:
1. 录制真实对讲机按钮声音
2. 合成波形: 短促的方波 + 轻微噪音
3. 添加轻微的"咔嗒"机械声
4. 使用限幅器避免爆音
```

**波形特征 / Waveform Characteristics:**
```
音量包络 (Volume Envelope):
^
|    _
|   | \
|   |  \_____
+------------>
0   0.05  0.1s
```

---

### 1.2 walkie_release.wav (松开音效)

**技术规格 / Technical Specs:**
- **格式**: WAV 或 OGG
- **采样率**: 44100 Hz
- **位深度**: 16-bit
- **声道**: Mono
- **时长**: ~0.1 秒

**音效描述 / Sound Description:**
- 类型: 更短促的"咔"声
- 特点: 比按下音稍低沉
- 音调: 比press音低50-100 Hz

**制作建议 / Production Tips:**
```
基于walkie_press.wav:
1. 降低音调 10-15%
2. 缩短持续时间 20%
3. 减少高频成分
4. 可选: 添加轻微回弹声
```

---

### 1.3 walkie_static_loop.wav (静态噪音循环)

**技术规格 / Technical Specs:**
- **格式**: WAV 或 OGG
- **采样率**: 44100 Hz
- **位深度**: 16-bit
- **声道**: Mono
- **时长**: 2-4 秒 (无缝循环)
- **循环**: 必须无缝衔接

**音效描述 / Sound Description:**
- 类型: 轻微的白噪音/电流声
- 特点: 平稳、连续、不刺耳
- 音量: 较低，作为背景音
- 频率范围: 200-6000 Hz (模拟老式无线电)

**制作建议 / Production Tips:**
```
制作方法:
1. 生成白噪音基底
2. 应用带通滤波器 (200-6000 Hz)
3. 添加轻微的音量调制 (模拟信号波动)
4. 确保首尾无缝连接 (crossfade)
5. 音量标准化到 -20 dB

调制效果:
- 频率: 2-5 Hz (慢速波动)
- 深度: 10-20% (轻微变化)
```

**循环质量检查 / Loop Quality Check:**
```
✓ 首尾衔接无"咔嗒"声
✓ 音量稳定无突变
✓ 频率分布均匀
✓ 不刺耳不单调
```

---

## 二、建筑升级音效 / Building Upgrade Sound Effects

### 2.1 upgrade_sound.wav (升级音效)

**技术规格 / Technical Specs:**
- **格式**: WAV 或 OGG
- **采样率**: 44100 Hz
- **位深度**: 16-bit
- **声道**: Stereo (立体声)
- **时长**: 1-2 秒

**音效描述 / Sound Description:**
- 类型: 成就感、向上的音效
- 组成: 叮叮当当的建造声 + 魔法光芒声
- 特点: 明亮、正面、激励
- 音调: 从中频向高频上升 (C4 -> C5)

**音效层次 / Sound Layers:**

1. **建造层 (Construction Layer)**
   ```
   - 锤子敲击声 (2-3次)
   - 木材/石材碰撞声
   - 节奏: 0.0s, 0.3s, 0.6s
   ```

2. **魔法层 (Magic Layer)**
   ```
   - 闪光音效 (sparkle)
   - 能量上升音 (rising tone)
   - 从0.4s开始渐入
   ```

3. **完成层 (Completion Layer)**
   ```
   - 悦耳的"叮"声
   - 在1.0s位置
   - 频率: 1047 Hz (C6)
   ```

**制作建议 / Production Tips:**
```
参考音效类型:
- 游戏: 星露谷物语的建筑完成音
- 参考音: 风铃、八音盒、魔法棒挥动
- 合成方法: 正弦波 + FM合成 + 采样

频率序列示例:
523Hz -> 659Hz -> 784Hz -> 1047Hz
(C5)    (E5)     (G5)     (C6)
```

---

## 三、技能释放音效 / Skill Cast Sound Effects

### 3.1 mage_spell_cast.wav (法师施法)

**技术规格 / Technical Specs:**
- **格式**: WAV 或 OGG
- **采样率**: 44100 Hz
- **位深度**: 16-bit
- **声道**: Stereo
- **时长**: 0.5-1 秒

**音效描述 / Sound Description:**
- 类型: 魔法能量聚集和释放
- 特点: 神秘、有力、能量感
- 音调变化: 低 -> 高 -> 爆发

**音效阶段 / Sound Phases:**

1. **蓄力阶段 (0.0-0.3s)**
   ```
   - 低频嗡嗡声逐渐增强
   - 频率: 100-300 Hz
   - 音量: 淡入
   ```

2. **释放阶段 (0.3-0.5s)**
   ```
   - 高频能量爆发
   - 频率: 快速扫描 2000-5000 Hz
   - 添加"呼啸"感
   ```

3. **余韵阶段 (0.5-1.0s)**
   ```
   - 魔法粒子消散声
   - 高频噪音淡出
   - 可选: 轻微回响
   ```

---

## 四、音效集成规范 / Audio Integration Specifications

### 4.1 Unity音频导入设置 / Unity Audio Import Settings

**一般音效 (SFX) / General Sound Effects:**
```
Load Type: Decompress On Load
Compression Format: Vorbis (质量 70%)
Sample Rate Setting: Preserve Sample Rate
Preload Audio Data: ✓
```

**循环音效 / Looping Sounds:**
```
Load Type: Streaming
Compression Format: Vorbis (质量 80%)
Sample Rate Setting: Preserve Sample Rate
Preload Audio Data: ✗
```

**短促音效 / Short SFX:**
```
Load Type: Decompress On Load
Compression Format: PCM (无压缩)
Sample Rate Setting: Preserve Sample Rate
Preload Audio Data: ✓
```

### 4.2 AudioSource配置建议 / AudioSource Configuration

**对讲机按钮音效 / Walkie-Talkie Button SFX:**
```csharp
audioSource.volume = 0.7f;
audioSource.pitch = Random.Range(0.95f, 1.05f); // 轻微音调变化
audioSource.spatialBlend = 0f; // 2D音效
audioSource.priority = 128; // 中等优先级
```

**对讲机静态噪音 / Walkie-Talkie Static:**
```csharp
audioSource.volume = 0.3f;
audioSource.loop = true;
audioSource.spatialBlend = 0f;
audioSource.priority = 200; // 低优先级
```

**建筑升级音效 / Building Upgrade SFX:**
```csharp
audioSource.volume = 0.8f;
audioSource.spatialBlend = 0.5f; // 半3D效果
audioSource.minDistance = 5f;
audioSource.maxDistance = 20f;
audioSource.priority = 64; // 高优先级
```

### 4.3 音量平衡 / Volume Balance

**标准音量等级 / Standard Volume Levels:**
- UI音效: 60-80%
- 环境音效: 40-60%
- 背景音乐: 30-50%
- 语音通话: 70-90%

---

## 五、音效文件清单 / Audio File Checklist

### 必需音效 / Required Audio Files

#### 对讲机系统 (3个文件)
- [ ] walkie_press.wav (~0.1s, Mono, 44.1kHz)
- [ ] walkie_release.wav (~0.1s, Mono, 44.1kHz)
- [ ] walkie_static_loop.wav (2-4s循环, Mono, 44.1kHz)

#### 建筑系统 (1个文件)
- [ ] upgrade_sound.wav (1-2s, Stereo, 44.1kHz)

#### 技能音效 (每职业至少1个)
- [ ] mage_spell_cast.wav (0.5-1s, Stereo, 44.1kHz)
- [ ] warrior_skill_sound.wav (可选)
- [ ] archer_skill_sound.wav (可选)

---

## 六、复古音效风格指南 / Retro Audio Style Guide

### 特征元素 / Characteristic Elements

1. **8-bit/16-bit音色 / 8-bit/16-bit Timbres**
   - 使用方波、三角波、锯齿波
   - 有限的音色变化
   - 清晰的音调

2. **简单的音频处理 / Simple Audio Processing**
   - 避免过多混响
   - 轻度使用延迟效果
   - 保持声音干净利落

3. **瞬态清晰 / Clear Transients**
   - 快速的起音和衰减
   - 避免过度柔化
   - 强调节奏感

### 推荐工具 / Recommended Tools

**音效合成 / Sound Synthesis:**
- Bfxr (免费在线8-bit音效生成器)
- ChipTone (芯片音乐音效制作)
- FamiTracker (NES风格音效)

**音频编辑 / Audio Editing:**
- Audacity (免费开源)
- Adobe Audition (专业)
- Reaper (轻量级DAW)

---

## 七、测试标准 / Testing Standards

### 音效质量检查 / Audio Quality Check

- [ ] 无爆音、削波、失真
- [ ] 音量适中，不过响或过小
- [ ] 循环音效无缝衔接
- [ ] 与其他音效音量平衡
- [ ] 在不同设备上测试 (耳机、扬声器)
- [ ] 与游戏视觉效果同步

### 性能检查 / Performance Check

- [ ] 文件大小合理 (单个文件 < 500KB)
- [ ] 加载时间可接受
- [ ] 同时播放多个音效无卡顿
- [ ] 内存占用合理

---

**音效设计原则 / Audio Design Principles:**
1. 清晰识别 - 每个音效有明确的含义
2. 风格统一 - 所有音效协调一致
3. 不刺耳 - 长时间游玩不疲劳
4. 增强反馈 - 配合视觉效果提供即时反馈

**文档版本 / Document Version:** 1.0  
**最后更新 / Last Updated:** 2025-12-19
