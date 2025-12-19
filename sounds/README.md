# 闹钟声音文件目录

## 📁 目录结构

```
sounds/
├── README.md                    # 本文件
├── DEFAULT_SOUNDS_GUIDE.md      # 默认声音获取指南
├── defaults/                    # 默认闹钟声音（推荐）
│   ├── alarm_01_classic_bell.wav
│   ├── alarm_02_electronic_beep.wav
│   ├── alarm_03_gentle_piano.wav
│   ├── alarm_04_bird_chirping.wav
│   └── alarm_05_emergency_alert.wav
└── custom/                      # 用户自定义声音
    └── (用户添加的音频文件)
```

## 🎵 默认声音说明

### 推荐的默认声音集

#### 基础包（5个声音）
1. **alarm_01_classic_bell.wav** - 经典机械闹钟铃声
   - 风格：传统、熟悉
   - 音量：中等
   - 适用：日常使用

2. **alarm_02_electronic_beep.wav** - 电子哔哔声
   - 风格：现代、简洁
   - 音量：中等
   - 适用：办公、学习

3. **alarm_03_gentle_piano.wav** - 柔和钢琴旋律
   - 风格：温和、舒缓
   - 音量：较轻
   - 适用：早晨唤醒

4. **alarm_04_bird_chirping.wav** - 清晨鸟鸣声
   - 风格：自然、清新
   - 音量：较轻
   - 适用：周末、假期

5. **alarm_05_emergency_alert.wav** - 紧急警报声
   - 风格：强烈、紧迫
   - 音量：较大
   - 适用：重要事项

#### 扩展包（额外5个）
6. **alarm_06_digital_clock.wav** - 数字闹钟声音
7. **alarm_07_ocean_waves.wav** - 海浪拍打声
8. **alarm_08_rooster_crow.wav** - 公鸡打鸣
9. **alarm_09_game_sound.wav** - 游戏提示音
10. **alarm_10_bugle_call.wav** - 军号起床号

## 📥 如何获取默认声音

### 方法1：从免费音效网站下载（推荐）

**推荐网站**：
- **Pixabay** - https://pixabay.com/sound-effects/
  - 完全免费，无需署名
  - 搜索："alarm", "bell", "notification"
  
- **Freesound** - https://freesound.org/
  - 大量音效，需要注册
  - 搜索："alarm clock", "morning alarm"

- **Mixkit** - https://mixkit.co/free-sound-effects/alarm/
  - 高质量音效
  - 可商用

**详细指南**：请查看 [DEFAULT_SOUNDS_GUIDE.md](DEFAULT_SOUNDS_GUIDE.md)

### 方法2：使用在线工具生成

- **SFXR** - https://sfxr.me/
  - 8位游戏音效生成器
  - 适合创建简单的哔哔声

- **Beepbox** - https://www.beepbox.co/
  - 在线音乐创作
  - 可以创建旋律型闹钟声

### 方法3：录制真实声音

使用手机或录音设备录制：
- 真实的闹钟声音
- 铃铛声音
- 自然声音（鸟鸣、流水等）

然后使用 Audacity 编辑和优化。

## 🔧 音频文件要求

### 格式要求
- **支持格式**: WAV, MP3, WMA, M4A
- **推荐格式**: WAV（无损）或 MP3（压缩）

### 质量要求
- **采样率**: 44.1 kHz 或 48 kHz
- **比特率**: 128 kbps 以上（MP3）
- **声道**: 单声道或立体声
- **时长**: 5-30 秒

### 音量要求
- **音量**: 适中，不要过大或过小
- **标准化**: 建议使用 Audacity 标准化音量

## 📝 文件命名规范

### 默认声音命名格式
```
alarm_[编号]_[描述].wav
```

**示例**：
- `alarm_01_classic_bell.wav`
- `alarm_02_electronic_beep.wav`
- `alarm_03_gentle_piano.wav`

### 自定义声音命名
用户可以使用任意文件名，但建议：
- 使用有意义的名称
- 避免使用特殊字符
- 使用英文或拼音

## 🛠️ 音频编辑工具

### Audacity（推荐）
- **网址**: https://www.audacityteam.org/
- **功能**: 剪辑、音量调整、格式转换
- **平台**: Windows, Mac, Linux
- **价格**: 免费开源

### 常用编辑操作

#### 1. 剪辑音频
1. 打开音频文件
2. 选择需要的部分
3. 编辑 → 删除其他部分

#### 2. 调整音量
1. 选择全部音频（Ctrl+A）
2. 效果 → 标准化
3. 设置最大振幅为 -1.0 dB

#### 3. 添加淡入淡出
1. 选择开头部分
2. 效果 → 淡入
3. 选择结尾部分
4. 效果 → 淡出

#### 4. 格式转换
1. 文件 → 导出 → 导出为 WAV/MP3
2. 选择合适的质量设置
3. 保存文件

## 📂 使用方法

### 在应用中使用默认声音

1. **打开闹钟编辑对话框**
   - 创建新闹钟或编辑现有闹钟

2. **选择声音文件**
   - 在"🔊 闹钟声音"区域
   - 点击"浏览..."按钮
   - 导航到 `sounds/defaults/` 目录
   - 选择想要的声音文件

3. **试听声音**
   - 点击"试听"按钮
   - 确认声音效果

4. **保存设置**
   - 点击"确定"按钮
   - 闹钟将使用选定的声音

### 添加自定义声音

1. **准备音频文件**
   - 确保格式为 WAV, MP3, WMA 或 M4A
   - 建议时长 5-30 秒

2. **放置文件**
   - 将文件复制到 `sounds/custom/` 目录
   - 或放在任意位置

3. **在应用中选择**
   - 点击"浏览..."按钮
   - 选择自定义声音文件
   - 试听并保存

## ⚖️ 版权说明

### 使用免费音效时请注意

1. **检查许可证**
   - 确认是否允许商业使用
   - 确认是否需要署名

2. **推荐许可类型**
   - ✅ CC0 (Public Domain) - 完全免费
   - ✅ CC BY - 免费使用，需署名
   - ✅ Pixabay License - 可商用
   - ⚠️ CC BY-NC - 仅限非商业使用

3. **避免版权问题**
   - 不要使用受版权保护的音乐
   - 不要使用商业软件的音效
   - 优先使用 CC0 或 Public Domain 音效

## 🎯 快速开始

### 5分钟快速设置

1. **访问 Pixabay**
   - 打开 https://pixabay.com/sound-effects/search/alarm/

2. **下载5个音效**
   - 选择喜欢的闹钟声音
   - 点击"免费下载"
   - 保存到 `sounds/defaults/` 目录

3. **重命名文件**
   - 按照命名规范重命名
   - 例如：`alarm_01_classic_bell.wav`

4. **在应用中测试**
   - 打开 AdvancedClock
   - 创建测试闹钟
   - 选择并试听声音

5. **完成！**
   - 现在你有了默认的闹钟声音集

## 💡 推荐音效组合

### 组合1：经典风格
- 机械闹钟铃声
- 电子哔哔声
- 数字闹钟声音

### 组合2：自然风格
- 鸟鸣声
- 海浪声
- 流水声

### 组合3：温和唤醒
- 柔和钢琴
- 轻音乐
- 风铃声

### 组合4：强力唤醒
- 紧急警报
- 公鸡打鸣
- 军号声

### 组合5：创意风格
- 游戏音效
- 动物叫声
- 电影配乐

## 🔗 相关文档

- **详细获取指南**: [DEFAULT_SOUNDS_GUIDE.md](DEFAULT_SOUNDS_GUIDE.md)
- **自定义声音功能**: [../docs/CUSTOM_SOUND_GUIDE.md](../docs/CUSTOM_SOUND_GUIDE.md)
- **项目主文档**: [../README.md](../README.md)

## 📞 需要帮助？

如果你在获取或使用声音文件时遇到问题：

1. 查看 [DEFAULT_SOUNDS_GUIDE.md](DEFAULT_SOUNDS_GUIDE.md) 获取详细指南
2. 查看 [CUSTOM_SOUND_GUIDE.md](../docs/CUSTOM_SOUND_GUIDE.md) 了解功能说明
3. 在项目 Issues 中提问

## 🎉 贡献

如果你有好的免费音效资源推荐，欢迎：
1. 提交 Pull Request
2. 在 Issues 中分享
3. 更新本文档

---

**版本**: v1.0  
**日期**: 2025-12-19  

**祝你找到完美的闹钟声音！** 🎵⏰
