# 默认闹钟声音获取指南

## 📋 概述

本指南将帮助你为 AdvancedClock 添加默认的闹钟声音文件。

## 🎵 推荐的免费音效资源网站

### 1. Freesound.org
- **网址**: https://freesound.org/
- **特点**: 大量免费音效，需要注册
- **搜索关键词**: "alarm clock", "alarm sound", "morning alarm", "beep"
- **许可**: 多种 Creative Commons 许可

### 2. Pixabay
- **网址**: https://pixabay.com/sound-effects/
- **特点**: 完全免费，无需注册
- **搜索关键词**: "alarm", "bell", "notification"
- **许可**: Pixabay License（可商用）

### 3. Zapsplat
- **网址**: https://www.zapsplat.com/
- **特点**: 免费音效库，需要注册
- **搜索关键词**: "alarm clock", "wake up"
- **许可**: 免费使用（需署名）

### 4. Mixkit
- **网址**: https://mixkit.co/free-sound-effects/alarm/
- **特点**: 高质量免费音效
- **搜索关键词**: "alarm", "notification"
- **许可**: Mixkit License（可商用）

### 5. SoundBible
- **网址**: https://soundbible.com/
- **特点**: 简单易用，分类清晰
- **搜索关键词**: "alarm", "clock"
- **许可**: 多种许可类型

## 🎯 推荐的闹钟声音类型

### 经典闹钟声音
1. **传统机械闹钟** - 经典的"铃铃铃"声音
2. **电子哔哔声** - 简单的电子提示音
3. **数字闹钟** - 现代数字闹钟的声音

### 温和唤醒声音
4. **鸟鸣声** - 清晨鸟叫声
5. **海浪声** - 舒缓的海浪拍打声
6. **轻音乐** - 柔和的钢琴或吉他旋律

### 强力唤醒声音
7. **紧急警报** - 强烈的警报声
8. **公鸡打鸣** - 响亮的公鸡叫声
9. **军号声** - 起床号角

### 创意声音
10. **游戏音效** - 经典游戏提示音
11. **动物叫声** - 猫叫、狗叫等
12. **自然声音** - 雨声、风声等

## 📁 文件命名规范

建议使用以下命名格式：

```
alarm_01_classic_bell.wav
alarm_02_electronic_beep.wav
alarm_03_digital_clock.wav
alarm_04_bird_chirping.wav
alarm_05_ocean_waves.wav
alarm_06_gentle_piano.wav
alarm_07_emergency_alert.wav
alarm_08_rooster_crow.wav
alarm_09_bugle_call.wav
alarm_10_game_sound.wav
```

**命名规则**：
- 前缀：`alarm_`
- 编号：`01-99`
- 描述：简短的英文描述
- 格式：`.wav` 或 `.mp3`

## 📂 文件存放位置

将下载的音频文件放在以下目录：

```
E:/code/AdvancedClock/sounds/defaults/
```

## 🔧 音频文件要求

### 格式要求
- **推荐格式**: WAV（无损）或 MP3（压缩）
- **支持格式**: WAV, MP3, WMA, M4A

### 质量要求
- **采样率**: 44.1 kHz 或 48 kHz
- **比特率**: 128 kbps 以上（MP3）
- **声道**: 单声道或立体声
- **时长**: 5-30 秒

### 音量要求
- **音量**: 适中，不要过大或过小
- **标准化**: 建议使用音频编辑软件标准化音量

## 🛠️ 音频编辑工具

如果需要编辑音频文件，可以使用以下免费工具：

### 1. Audacity（推荐）
- **网址**: https://www.audacityteam.org/
- **功能**: 剪辑、淡入淡出、音量调整、格式转换
- **平台**: Windows, Mac, Linux

### 2. Ocenaudio
- **网址**: https://www.ocenaudio.com/
- **功能**: 简单易用的音频编辑器
- **平台**: Windows, Mac, Linux

### 3. WavePad
- **网址**: https://www.nch.com.au/wavepad/
- **功能**: 专业音频编辑
- **平台**: Windows, Mac

## 📝 使用步骤

### 步骤1：下载音频文件
1. 访问上述推荐网站
2. 搜索合适的闹钟声音
3. 下载音频文件（WAV 或 MP3 格式）

### 步骤2：编辑音频（可选）
1. 使用 Audacity 打开音频文件
2. 剪辑到合适的长度（5-30秒）
3. 调整音量（效果 → 标准化）
4. 添加淡入淡出效果（可选）
5. 导出为 WAV 或 MP3 格式

### 步骤3：重命名文件
按照上述命名规范重命名文件

### 步骤4：放置文件
将文件复制到 `sounds/defaults/` 目录

### 步骤5：在应用中使用
1. 打开 AdvancedClock
2. 创建或编辑闹钟
3. 点击"浏览"按钮
4. 选择默认声音文件
5. 点击"试听"测试效果

## 🎨 创建自定义声音集

### 方案A：使用在线工具生成
1. **Beepbox** (https://www.beepbox.co/)
   - 在线音乐创作工具
   - 可以创建简单的电子音效

2. **SFXR** (https://sfxr.me/)
   - 8位游戏音效生成器
   - 适合创建复古风格的闹钟声音

### 方案B：录制真实声音
1. 使用手机或录音设备
2. 录制真实的闹钟声音
3. 使用 Audacity 编辑和优化
4. 导出为合适的格式

### 方案C：使用文本转语音
1. 使用 TTS 工具生成语音提醒
2. 例如："该起床了！"、"Good morning!"
3. 可以使用 Windows 自带的语音合成

## 📦 预设声音包推荐

### 基础包（5个声音）
1. `alarm_01_classic_bell.wav` - 经典铃声
2. `alarm_02_electronic_beep.wav` - 电子哔哔声
3. `alarm_03_gentle_piano.wav` - 柔和钢琴
4. `alarm_04_bird_chirping.wav` - 鸟鸣声
5. `alarm_05_emergency_alert.wav` - 紧急警报

### 完整包（10个声音）
基础包 + 以下5个：
6. `alarm_06_digital_clock.wav` - 数字闹钟
7. `alarm_07_ocean_waves.wav` - 海浪声
8. `alarm_08_rooster_crow.wav` - 公鸡打鸣
9. `alarm_09_game_sound.wav` - 游戏音效
10. `alarm_10_bugle_call.wav` - 军号声

## 🔍 快速搜索链接

### Freesound.org 直达链接
- [Alarm Clock Sounds](https://freesound.org/search/?q=alarm+clock)
- [Morning Alarm Sounds](https://freesound.org/search/?q=morning+alarm)
- [Beep Sounds](https://freesound.org/search/?q=beep+alarm)

### Pixabay 直达链接
- [Alarm Sound Effects](https://pixabay.com/sound-effects/search/alarm/)
- [Bell Sound Effects](https://pixabay.com/sound-effects/search/bell/)

### Mixkit 直达链接
- [Alarm Sound Effects](https://mixkit.co/free-sound-effects/alarm/)

## ⚖️ 版权注意事项

### 使用免费音效时请注意：
1. **检查许可证** - 确认是否允许商业使用
2. **署名要求** - 某些许可需要署名原作者
3. **修改限制** - 某些许可不允许修改
4. **分发限制** - 某些许可限制再分发

### 推荐许可类型：
- ✅ **CC0 (Public Domain)** - 完全免费，无需署名
- ✅ **CC BY** - 免费使用，需要署名
- ✅ **Pixabay License** - 可商用，无需署名
- ⚠️ **CC BY-NC** - 仅限非商业使用
- ❌ **All Rights Reserved** - 需要购买许可

## 🎯 实施建议

### 短期方案（快速实现）
1. 从 Pixabay 下载 3-5 个免费音效
2. 使用 Audacity 简单编辑
3. 放入 `sounds/defaults/` 目录
4. 在应用中测试

### 长期方案（完整实现）
1. 收集 10-15 个不同风格的音效
2. 统一音量和格式
3. 创建音效预览图标
4. 在应用中添加默认音效选择器
5. 允许用户在首次使用时选择默认音效

## 🚀 下一步开发建议

### 功能增强
1. **内置音效库** - 将默认音效打包到应用中
2. **音效预览** - 在选择界面显示音效波形
3. **音效分类** - 按类型分类（温和、强力、创意等）
4. **音量调节** - 为每个闹钟单独调节音量
5. **渐强效果** - 闹钟声音逐渐增大

### 代码实现示例

```csharp
// 在 AudioService.cs 中添加默认音效管理
public class AudioService
{
    private static readonly string DefaultSoundsPath = 
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sounds", "defaults");
    
    public static List<string> GetDefaultSounds()
    {
        if (!Directory.Exists(DefaultSoundsPath))
            return new List<string>();
            
        return Directory.GetFiles(DefaultSoundsPath, "*.wav")
            .Concat(Directory.GetFiles(DefaultSoundsPath, "*.mp3"))
            .ToList();
    }
    
    public static string GetDefaultSoundName(string filePath)
    {
        // 从文件名提取友好名称
        // alarm_01_classic_bell.wav -> "Classic Bell"
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var parts = fileName.Split('_');
        if (parts.Length >= 3)
        {
            return string.Join(" ", parts.Skip(2))
                .Replace("_", " ")
                .ToTitleCase();
        }
        return fileName;
    }
}
```

## 📚 参考资源

### 音效设计教程
- [How to Create Alarm Sounds](https://www.youtube.com/results?search_query=how+to+create+alarm+sounds)
- [Audacity Tutorial](https://manual.audacityteam.org/)

### 音效理论
- [Psychology of Alarm Sounds](https://www.sleepfoundation.org/bedroom-environment/best-alarm-sounds)
- [Effective Wake-Up Sounds](https://www.healthline.com/health/healthy-sleep/best-alarm-sound)

## 💡 创意建议

### 个性化音效
1. **录制家人的声音** - "该起床了！"
2. **使用喜欢的歌曲片段** - 注意版权
3. **创建主题音效** - 例如星球大战主题
4. **季节性音效** - 春天鸟鸣、冬天铃铛

### 智能音效
1. **根据时间选择** - 早晨温和，下午强力
2. **根据天气选择** - 晴天鸟鸣，雨天雨声
3. **渐进式唤醒** - 音量逐渐增大
4. **智能重复** - 未关闭时逐渐加强

## 🎉 总结

通过本指南，你可以：
1. ✅ 找到高质量的免费闹钟音效
2. ✅ 编辑和优化音频文件
3. ✅ 创建统一的音效库
4. ✅ 为用户提供多样化的选择
5. ✅ 遵守版权和许可要求

**建议从 Pixabay 开始**，因为它完全免费且无需署名，非常适合快速实现！

---

**版本**: v1.0  
**日期**: 2025-12-19  
**作者**: AdvancedClock Team

**祝你找到完美的闹钟声音！** 🎵⏰
