# 闹钟声音资源说明

## 📁 声音文件夹

本文件夹用于存放闹钟的自定义声音文件。

## 🔊 支持的音频格式

- **WAV** (推荐) - 无损音质，兼容性最好
- **MP3** - 压缩格式，文件较小
- **WMA** - Windows Media Audio
- **M4A** - MPEG-4 Audio

## 📝 使用方法

### 1. 添加自定义声音

1. 将你的音频文件复制到此文件夹
2. 在闹钟编辑对话框中点击"浏览..."按钮
3. 选择你想要的声音文件
4. 点击"试听"按钮测试声音效果

### 2. 推荐的声音文件

你可以从以下来源获取免费的闹钟声音：

- **Freesound** - https://freesound.org/
- **Zapsplat** - https://www.zapsplat.com/
- **Mixkit** - https://mixkit.co/free-sound-effects/alarm/

### 3. 声音文件命名建议

为了方便管理，建议使用有意义的文件名：

```
alarm-gentle.wav        # 轻柔的闹钟声
alarm-loud.wav          # 响亮的闹钟声
alarm-birds.mp3         # 鸟鸣声
alarm-bell.wav          # 铃声
alarm-music.mp3         # 音乐闹钟
```

## 🎵 示例声音文件

本项目不包含预置的声音文件，但你可以：

1. **使用系统默认声音** - 不选择任何文件，程序会使用 Windows 系统声音
2. **下载免费声音** - 从上述网站下载免费的闹钟声音
3. **使用自己的音乐** - 使用你喜欢的音乐作为闹钟声音

## ⚙️ 技术说明

### 音频播放实现

- **WAV 文件** - 使用 `System.Media.SoundPlayer` 播放（更可靠）
- **MP3/WMA/M4A** - 使用 `System.Windows.Media.MediaPlayer` 播放

### 文件大小建议

- 建议单个声音文件不超过 **10MB**
- 闹钟声音通常 **10-30秒** 即可
- 过长的音频文件会占用更多内存

### 音质建议

- **采样率**: 44.1kHz 或 48kHz
- **比特率**: 128kbps 以上（MP3）
- **声道**: 单声道或立体声均可

## 📂 文件夹结构示例

```
sounds/
├── README.md                    # 本说明文件
├── gentle/                      # 轻柔的声音
│   ├── soft-chime.wav
│   ├── gentle-bell.wav
│   └── morning-birds.mp3
├── loud/                        # 响亮的声音
│   ├── loud-alarm.wav
│   ├── emergency.wav
│   └── siren.mp3
└── music/                       # 音乐类
    ├── classical.mp3
    ├── jazz.mp3
    └── pop.mp3
```

## 🔧 故障排除

### 声音无法播放

1. **检查文件格式** - 确保是支持的格式（WAV, MP3, WMA, M4A）
2. **检查文件路径** - 确保文件没有被移动或删除
3. **检查文件权限** - 确保程序有读取文件的权限
4. **尝试系统默认** - 清除自定义声音，使用系统默认声音

### 声音播放不完整

- 某些 MP3 文件可能存在兼容性问题
- 建议转换为 WAV 格式以获得最佳兼容性

### 声音太小或太大

- 使用音频编辑软件（如 Audacity）调整音量
- 或者在 Windows 音量混合器中调整程序音量

## 🎨 创建自己的闹钟声音

### 使用 Audacity（免费开源）

1. 下载 Audacity: https://www.audacityteam.org/
2. 录制或导入音频
3. 编辑音频（剪切、淡入淡出等）
4. 导出为 WAV 或 MP3 格式

### 简单的闹钟声音制作

```
1. 打开 Audacity
2. 生成 -> 音调 -> 选择频率（如 440Hz）
3. 效果 -> 淡入淡出
4. 文件 -> 导出 -> 导出为 WAV
```

## 📞 相关文档

- [自定义声音功能说明](../docs/CUSTOM_SOUND_GUIDE.md)
- [项目主文档](../README.md)

## 💡 提示

- 不同的闹钟可以设置不同的声音
- 重要的闹钟可以使用更响亮的声音
- 早晨闹钟可以使用轻柔的声音，避免惊醒

---

**享受个性化的闹钟体验！** 🎵⏰
