# 闹钟声音增强功能说明

## 📋 概述

本次更新为 AdvancedClock 添加了声音选择和播放控制的增强功能，包括：
1. 自动枚举 `sounds/defaults` 目录下的默认声音文件
2. 下拉选择框快速选择预设声音
3. 保留自定义声音文件选择功能
4. 循环播放控制，可设置最大播放时长

## ✨ 新增功能

### 1. 默认声音库支持

#### 功能描述
- 自动扫描 `sounds/defaults` 目录下的所有音频文件
- 支持 WAV, MP3, WMA, M4A 格式
- 按文件名自动排序
- 在下拉框中显示所有可用的默认声音

#### 使用方法
1. 将音频文件放入 `sounds/defaults` 目录
2. 打开闹钟编辑对话框
3. 选择"使用默认声音库"单选按钮
4. 从下拉框中选择想要的声音
5. 点击"试听"按钮预览效果

#### 目录结构
```
AdvancedClock/
├── sounds/
│   ├── defaults/              # 默认声音库目录
│   │   ├── alarm_01_classic_bell.wav
│   │   ├── alarm_02_electronic_beep.wav
│   │   ├── alarm_03_gentle_piano.wav
│   │   └── ...
│   └── custom/                # 用户自定义声音目录（可选）
```

### 2. 双模式声音选择

#### 模式一：使用默认声音库
- **优点**：快速选择，无需浏览文件系统
- **适用场景**：使用预设的常用闹钟声音
- **操作步骤**：
  1. 选择"使用默认声音库"单选按钮
  2. 从下拉框选择声音
  3. 点击"试听"测试效果

#### 模式二：自定义声音文件
- **优点**：可以使用任意位置的音频文件
- **适用场景**：使用个人收藏的音乐或特殊音效
- **操作步骤**：
  1. 选择"自定义声音文件"单选按钮
  2. 点击"浏览..."按钮选择文件
  3. 点击"试听"测试效果

### 3. 循环播放时长控制

#### 功能描述
- 声音文件会自动循环播放
- 可设置最大播放时长（5-600秒）
- 默认播放时长为 60 秒（1分钟）
- 到达设定时长后自动停止播放

#### 时长设置
- **最小值**：5 秒
- **最大值**：600 秒（10分钟）
- **默认值**：60 秒（1分钟）
- **推荐值**：
  - 短暂提醒：30-60 秒
  - 标准闹钟：60-120 秒
  - 重要事项：120-300 秒

#### 使用场景
```
场景1：早晨起床闹钟
- 播放时长：120秒（2分钟）
- 声音选择：温和的钢琴曲或鸟鸣声
- 效果：温和唤醒，避免惊吓

场景2：工作提醒
- 播放时长：30秒
- 声音选择：简短的电子提示音
- 效果：快速提醒，不打扰工作

场景3：重要会议提醒
- 播放时长：300秒（5分钟）
- 声音选择：响亮的警报声
- 效果：确保不会错过重要事项
```

## 🔧 技术实现

### 1. 数据模型更新

#### AlarmModel.cs
```csharp
/// <summary>
/// 最大播放时长（秒），默认60秒（1分钟）
/// </summary>
public int MaxPlayDurationSeconds
{
    get => _maxPlayDurationSeconds;
    set
    {
        _maxPlayDurationSeconds = Math.Max(5, Math.Min(600, value)); // 限制在5-600秒之间
        OnPropertyChanged(nameof(MaxPlayDurationSeconds));
    }
}
```

### 2. 音频服务增强

#### AudioService.cs 新增功能

##### 循环播放支持
```csharp
/// <summary>
/// 播放闹钟声音
/// </summary>
/// <param name="customSoundPath">自定义声音文件路径</param>
/// <param name="isStrongAlert">是否为强提醒</param>
/// <param name="maxDurationSeconds">最大播放时长（秒），默认60秒</param>
public void PlayAlarmSound(string? customSoundPath, bool isStrongAlert = false, int maxDurationSeconds = 60)
```

##### 自动停止机制
- 使用 `DispatcherTimer` 实现定时停止
- MediaPlayer 的 `MediaEnded` 事件实现循环播放
- 到达设定时长后自动调用 `Stop()` 方法

##### 默认声音枚举
```csharp
/// <summary>
/// 获取 sounds/defaults 目录下的所有音频文件
/// </summary>
public static List<string> GetDefaultSounds()
{
    // 扫描 sounds/defaults 目录
    // 支持 WAV, MP3, WMA, M4A 格式
    // 按文件名排序返回
}
```

### 3. UI 界面更新

#### AlarmEditDialog.xaml 新增控件

##### 声音选择模式切换
```xml
<RadioButton x:Name="UseDefaultSoundRadio" Content="使用默认声音库"
             IsChecked="True" Checked="SoundSourceChanged"/>
<RadioButton x:Name="UseCustomSoundRadio" Content="自定义声音文件"
             Checked="SoundSourceChanged"/>
```

##### 默认声音下拉框
```xml
<ComboBox x:Name="DefaultSoundComboBox" 
          SelectionChanged="DefaultSoundComboBox_SelectionChanged"/>
```

##### 播放时长设置
```xml
<TextBox x:Name="MaxPlayDurationTextBox"
         Text="{Binding MaxPlayDurationSeconds, UpdateSourceTrigger=PropertyChanged}"/>
```

### 4. 逻辑控制

#### AlarmEditDialog.xaml.cs 新增方法

##### 初始化声音选择
```csharp
private void InitializeSoundSelection()
{
    // 加载默认声音列表
    LoadDefaultSounds();
    
    // 初始化播放时长
    MaxPlayDurationTextBox.Text = AlarmModel.MaxPlayDurationSeconds.ToString();
    
    // 根据当前声音路径设置选择状态
    // ...
}
```

##### 加载默认声音列表
```csharp
private void LoadDefaultSounds()
{
    // 清空下拉框
    DefaultSoundComboBox.Items.Clear();
    
    // 添加系统默认选项
    // 获取 sounds/defaults 目录下的所有声音文件
    // 填充到下拉框
}
```

##### 验证播放时长
```csharp
private bool ValidateAndUpdateSoundDuration()
{
    // 验证播放时长在 5-600 秒之间
    // 更新 AlarmModel.MaxPlayDurationSeconds
}
```

## 📝 使用指南

### 快速开始

#### 步骤1：准备默认声音文件
1. 打开 `sounds/defaults` 目录
2. 将音频文件（WAV, MP3, WMA, M4A）复制到该目录
3. 建议使用有意义的文件名，如：
   - `alarm_01_classic_bell.wav`
   - `alarm_02_gentle_piano.mp3`
   - `alarm_03_bird_chirping.wav`

#### 步骤2：创建或编辑闹钟
1. 打开 AdvancedClock
2. 点击"添加闹钟"或编辑现有闹钟
3. 滚动到"🔊 闹钟声音"区域

#### 步骤3：选择声音
**方式A：使用默认声音库**
1. 选择"使用默认声音库"单选按钮
2. 从下拉框选择想要的声音
3. 点击"试听"按钮（播放5秒）

**方式B：自定义声音文件**
1. 选择"自定义声音文件"单选按钮
2. 点击"浏览..."按钮
3. 选择任意位置的音频文件
4. 点击"试听"按钮（播放5秒）

#### 步骤4：设置播放时长
1. 在"循环播放时长"输入框中输入秒数
2. 范围：5-600 秒
3. 参考：
   - 60 秒 = 1 分钟
   - 120 秒 = 2 分钟
   - 300 秒 = 5 分钟
   - 600 秒 = 10 分钟

#### 步骤5：保存设置
1. 点击"确定"按钮保存
2. 闹钟触发时会按设置循环播放声音

### 高级技巧

#### 技巧1：组织默认声音库
```
sounds/defaults/
├── 01_温和唤醒/
│   ├── gentle_piano.mp3
│   ├── bird_chirping.wav
│   └── ocean_waves.mp3
├── 02_标准提醒/
│   ├── classic_bell.wav
│   ├── electronic_beep.mp3
│   └── digital_clock.wav
└── 03_强力警报/
    ├── emergency_alert.wav
    ├── loud_alarm.mp3
    └── siren.wav
```

#### 技巧2：根据场景设置时长
- **快速提醒**（会议、任务）：30-60 秒
- **日常闹钟**（起床、休息）：60-120 秒
- **重要事项**（考试、面试）：120-300 秒
- **紧急警报**（药物提醒）：300-600 秒

#### 技巧3：声音文件优化
- **格式选择**：WAV（无损）或 MP3（压缩）
- **时长建议**：5-30 秒（会自动循环）
- **音量标准化**：使用 Audacity 标准化音量
- **淡入淡出**：添加淡入淡出效果更舒适

## 🎯 功能特点

### 优势

1. **便捷性**
   - ✅ 下拉选择，无需浏览文件系统
   - ✅ 自动扫描，无需手动配置
   - ✅ 双模式支持，灵活切换

2. **智能化**
   - ✅ 自动循环播放
   - ✅ 定时自动停止
   - ✅ 文件格式自动识别

3. **可控性**
   - ✅ 精确控制播放时长
   - ✅ 试听功能（5秒预览）
   - ✅ 随时切换声音

4. **兼容性**
   - ✅ 支持多种音频格式
   - ✅ 保留原有功能
   - ✅ 向后兼容旧数据

### 与原有功能的对比

| 功能 | 原有实现 | 新增实现 | 改进 |
|------|---------|---------|------|
| 声音选择 | 仅支持浏览文件 | 下拉选择 + 浏览文件 | ⭐⭐⭐⭐⭐ |
| 播放控制 | 播放一次 | 循环播放 + 时长控制 | ⭐⭐⭐⭐⭐ |
| 默认声音 | 系统默认 | 系统默认 + 预设库 | ⭐⭐⭐⭐ |
| 试听功能 | 完整播放 | 5秒预览 | ⭐⭐⭐⭐ |
| 用户体验 | 一般 | 优秀 | ⭐⭐⭐⭐⭐ |

## 🔍 常见问题

### Q1: 默认声音库在哪里？
**A**: 在应用程序目录下的 `sounds/defaults` 文件夹中。

### Q2: 支持哪些音频格式？
**A**: 支持 WAV, MP3, WMA, M4A 格式。

### Q3: 如何添加新的默认声音？
**A**: 直接将音频文件复制到 `sounds/defaults` 目录，重新打开编辑对话框即可看到。

### Q4: 播放时长最长可以设置多久？
**A**: 最长 600 秒（10分钟），最短 5 秒。

### Q5: 声音会一直循环播放吗？
**A**: 会循环播放，但到达设定的最大播放时长后会自动停止。

### Q6: 试听功能播放多久？
**A**: 试听固定播放 5 秒，用于快速预览效果。

### Q7: 可以使用自己的音乐文件吗？
**A**: 可以，选择"自定义声音文件"模式，然后浏览选择任意位置的音频文件。

### Q8: 如果 defaults 目录为空会怎样？
**A**: 下拉框会显示"（没有找到默认声音文件）"，可以使用系统默认声音或自定义文件。

### Q9: 原有的闹钟声音设置会受影响吗？
**A**: 不会，完全向后兼容，原有设置保持不变。

### Q10: 如何恢复使用系统默认声音？
**A**: 点击"清除（使用系统默认）"按钮，或在下拉框中选择"系统默认声音"。

## 📊 性能说明

### 资源占用
- **内存占用**：增加约 1-2 MB（取决于声音文件数量）
- **CPU 占用**：几乎无影响
- **磁盘占用**：取决于用户添加的声音文件

### 性能优化
- ✅ 延迟加载：仅在打开编辑对话框时扫描文件
- ✅ 缓存机制：避免重复扫描
- ✅ 异步播放：不阻塞 UI 线程
- ✅ 自动释放：播放结束后自动释放资源

## 🚀 未来计划

### 短期计划
- [ ] 添加音量控制滑块
- [ ] 支持声音淡入淡出效果
- [ ] 添加声音波形预览
- [ ] 支持声音分类管理

### 长期计划
- [ ] 在线声音库
- [ ] 声音编辑器集成
- [ ] AI 生成闹钟声音
- [ ] 社区声音分享

## 📚 相关文档

- [默认声音获取指南](../sounds/DEFAULT_SOUNDS_GUIDE.md)
- [自定义声音功能指南](CUSTOM_SOUND_GUIDE.md)
- [声音目录说明](../sounds/README.md)
- [项目主文档](../README.md)

## 🎉 总结

本次更新为 AdvancedClock 带来了更强大、更灵活的声音控制功能：

### 核心改进
1. ✅ **默认声音库** - 快速选择预设声音
2. ✅ **双模式选择** - 默认库 + 自定义文件
3. ✅ **循环播放** - 自动循环，无需手动重复
4. ✅ **时长控制** - 精确控制播放时长（5-600秒）
5. ✅ **智能停止** - 到达时长自动停止

### 用户体验提升
- 🎵 更方便的声音选择
- ⏱️ 更灵活的播放控制
- 🎧 更好的试听体验
- 🔧 更强大的自定义能力

### 技术亮点
- 🏗️ 模块化设计
- 🔄 循环播放机制
- ⏰ 定时器控制
- 📁 自动文件扫描

---

**版本**: v2.5  
**日期**: 2025-12-19  
**状态**: ✅ 已完成

**享受更智能的闹钟声音体验！** 🎵⏰
