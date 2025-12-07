# VS Code 调试配置指南

本指南详细说明如何在 VS Code 中调试 AdvancedClock WPF 应用程序。

## 🛠️ 环境准备

### 必需的扩展
VS Code 会自动推荐以下扩展，请确保已安装：

#### 核心扩展
- **C# for Visual Studio Code** (`ms-dotnettools.csharp`) - C# 语言支持
- **.NET Install Tool** (`ms-dotnettools.vscode-dotnet-runtime`) - .NET 运行时管理
- **XML** (`redhat.vscode-xml`) - XAML 文件支持

#### 推荐扩展
- **GitLens** (`eamodio.gitlens`) - Git 增强功能
- **PowerShell** (`ms-vscode.powershell`) - PowerShell 脚本支持
- **Path Intellisense** (`christian-kohler.path-intellisense`) - 路径自动完成

### 系统要求
- .NET 7.0 SDK
- Windows 10/11（WPF 应用程序）
- VS Code 1.70+

## 🎯 调试配置详解

### 可用的调试配置

#### 1. **启动 AdvancedClock (Debug)** - 默认调试配置
```json
{
    "name": "启动 AdvancedClock (Debug)",
    "type": "coreclr",
    "request": "launch",
    "preLaunchTask": "build-debug"
}
```
- ✅ 自动构建 Debug 版本
- ✅ 启用 JIT 优化抑制
- ✅ 仅调试用户代码
- ✅ 最适合日常开发调试

#### 2. **启动 AdvancedClock (Release)** - 性能测试
```json
{
    "name": "启动 AdvancedClock (Release)",
    "type": "coreclr",
    "request": "launch",
    "preLaunchTask": "build-release"
}
```
- ✅ 自动构建 Release 版本
- ✅ 启用 JIT 优化
- ✅ 可调试框架代码
- ✅ 适合性能分析和发布前测试

#### 3. **附加到 AdvancedClock 进程** - 附加调试
```json
{
    "name": "附加到 AdvancedClock 进程",
    "type": "coreclr",
    "request": "attach",
    "processName": "AdvancedClock.exe"
}
```
- ✅ 附加到正在运行的进程
- ✅ 不重新启动应用程序
- ✅ 适合调试已运行的实例

#### 4. **启动 AdvancedClock (无预构建)** - 快速启动
```json
{
    "name": "启动 AdvancedClock (无预构建)",
    "type": "coreclr",
    "request": "launch"
}
```
- ✅ 跳过构建步骤
- ✅ 使用现有的编译文件
- ✅ 快速启动调试

#### 5. **启动 AdvancedClock (断点调试)** - 深度调试
```json
{
    "name": "启动 AdvancedClock (断点调试)",
    "type": "coreclr",
    "request": "launch",
    "stopAtEntry": true
}
```
- ✅ 在程序入口点停止
- ✅ 禁用步骤过滤
- ✅ 可调试所有代码
- ✅ 适合深度问题排查

## 🚀 使用方法

### 基本调试流程

#### 1. **设置断点**
```csharp
// 在代码行左侧点击设置断点
private void AddAlarmButton_Click(object sender, RoutedEventArgs e)
{
    var dialog = new AlarmEditDialog(); // ← 在此行设置断点
    if (dialog.ShowDialog() == true)
    {
        _alarmService.AddAlarm(dialog.AlarmModel);
    }
}
```

#### 2. **启动调试**
- 按 `F5` 或点击调试面板的"启动调试"
- 选择"启动 AdvancedClock (Debug)"配置
- 程序会自动构建并启动

#### 3. **调试操作**
- **继续执行**：`F5`
- **单步执行**：`F10`
- **步入函数**：`F11`
- **步出函数**：`Shift+F11`
- **停止调试**：`Shift+F5`

### 高级调试技巧

#### 1. **条件断点**
```csharp
// 右键断点 → 编辑断点 → 添加条件
for (int i = 0; i < alarms.Count; i++)
{
    // 条件：i == 5
    ProcessAlarm(alarms[i]); // 只在 i=5 时停止
}
```

#### 2. **日志断点**
```csharp
// 右键断点 → 编辑断点 → 日志消息
private void SaveAlarms()
{
    // 日志消息：保存 {_alarms.Count} 个闹钟
    _dataService.SaveAlarms(_alarms);
}
```

#### 3. **监视表达式**
在调试面板的"监视"窗口中添加：
```csharp
_alarms.Count
DateTime.Now
_alarmService.IsRunning
```

#### 4. **调用堆栈分析**
查看调试面板的"调用堆栈"了解方法调用路径。

## 🔧 任务配置详解

### 可用的构建任务

#### 1. **build-debug** - 调试构建
```bash
dotnet build --configuration Debug
```

#### 2. **build-release** - 发布构建
```bash
dotnet build --configuration Release
```

#### 3. **publish-debug** - 调试发布
```bash
dotnet publish --configuration Debug --output ./publish/Debug
```

#### 4. **publish-release** - 发布版本
```bash
dotnet publish --configuration Release --output ./publish/Release
```

#### 5. **clean** - 清理项目
```bash
dotnet clean
```

#### 6. **restore** - 恢复依赖
```bash
dotnet restore
```

#### 7. **watch** - 监视模式
```bash
dotnet watch run
```

### 运行任务
- 按 `Ctrl+Shift+P` 打开命令面板
- 输入 "Tasks: Run Task"
- 选择要执行的任务

## 🐛 常见调试场景

### 1. **闹钟不触发问题**
```csharp
// 在 AlarmService.cs 中设置断点
private void CheckAlarms()
{
    var now = DateTime.Now; // ← 断点1：检查当前时间
    
    foreach (var alarm in _alarms.Where(a => a.IsEnabled))
    {
        if (ShouldTriggerAlarm(alarm, now)) // ← 断点2：检查触发条件
        {
            TriggerAlarm(alarm); // ← 断点3：确认触发
        }
    }
}
```

### 2. **数据保存失败问题**
```csharp
// 在 AlarmDataService.cs 中设置断点
public void SaveAlarms(IEnumerable<AlarmModel> alarms)
{
    try
    {
        var json = JsonSerializer.Serialize(alarms, _options); // ← 断点1
        File.WriteAllText(DataFilePath, json); // ← 断点2
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"保存失败: {ex.Message}"); // ← 断点3
    }
}
```

### 3. **界面响应问题**
```csharp
// 在 MainWindow.xaml.cs 中设置断点
private void AddAlarmButton_Click(object sender, RoutedEventArgs e)
{
    var dialog = new AlarmEditDialog(); // ← 断点1：对话框创建
    if (dialog.ShowDialog() == true) // ← 断点2：对话框结果
    {
        _alarmService.AddAlarm(dialog.AlarmModel); // ← 断点3：添加闹钟
    }
}
```

### 4. **系统托盘问题**
```csharp
// 在 MainWindow.xaml.cs 中设置断点
private void InitializeNotifyIcon()
{
    var customIcon = IconHelper.GetApplicationIcon(); // ← 断点1：图标获取
    
    _notifyIcon = new WinForms.NotifyIcon
    {
        Icon = customIcon ?? System.Drawing.SystemIcons.Information, // ← 断点2
        Visible = true,
        Text = "高级闹钟"
    };
}
```

## 📊 调试面板功能

### 1. **变量窗口**
- **局部变量**：当前作用域的变量
- **监视**：自定义监视表达式
- **调用堆栈**：方法调用路径

### 2. **断点窗口**
- 查看所有断点
- 启用/禁用断点
- 编辑断点条件

### 3. **调试控制台**
- 查看调试输出
- 执行即时表达式
- 查看异常信息

### 4. **终端**
- 执行 dotnet 命令
- 查看构建输出
- 运行脚本

## ⚙️ 调试设置优化

### 1. **性能优化**
```json
// .vscode/settings.json
{
    "csharp.debug.justMyCode": true,        // 仅调试用户代码
    "csharp.debug.enableStepFiltering": true, // 启用步骤过滤
    "csharp.debug.suppressJITOptimizations": true // 抑制JIT优化
}
```

### 2. **文件排除**
```json
{
    "files.exclude": {
        "**/bin": true,
        "**/obj": true,
        "**/.vs": true
    }
}
```

### 3. **自动格式化**
```json
{
    "editor.formatOnSave": true,
    "editor.formatOnPaste": true,
    "[csharp]": {
        "editor.defaultFormatter": "ms-dotnettools.csharp"
    }
}
```

## 🔍 故障排除

### 常见问题

#### 1. **调试器无法启动**
**问题**：点击 F5 没有反应
**解决**：
```bash
# 检查 .NET SDK
dotnet --version

# 重新安装 C# 扩展
# 在扩展面板中卸载并重新安装 ms-dotnettools.csharp
```

#### 2. **断点不生效**
**问题**：断点显示为空心圆
**解决**：
- 确保使用 Debug 配置
- 检查 PDB 文件是否存在
- 重新构建项目

#### 3. **无法附加到进程**
**问题**：附加调试失败
**解决**：
- 确保进程正在运行
- 检查进程名称是否正确
- 以管理员身份运行 VS Code

#### 4. **XAML 文件无法调试**
**问题**：XAML 中的绑定错误无法调试
**解决**：
- 在输出窗口查看绑定错误
- 使用 PresentationTraceSources 跟踪
- 在代码后台设置断点

### 调试技巧

#### 1. **使用输出窗口**
```csharp
System.Diagnostics.Debug.WriteLine("调试信息");
Console.WriteLine("控制台输出");
```

#### 2. **异常设置**
- 在调试面板中配置异常断点
- 捕获特定类型的异常
- 在异常抛出时自动停止

#### 3. **内存调试**
- 使用诊断工具查看内存使用
- 检查对象生命周期
- 识别内存泄漏

## 📚 快捷键参考

### 调试快捷键
- `F5` - 开始调试/继续
- `Ctrl+F5` - 开始执行（不调试）
- `F9` - 切换断点
- `F10` - 单步跳过
- `F11` - 单步进入
- `Shift+F11` - 单步跳出
- `Shift+F5` - 停止调试
- `Ctrl+Shift+F5` - 重新启动调试

### 编辑快捷键
- `Ctrl+K, Ctrl+C` - 注释代码
- `Ctrl+K, Ctrl+U` - 取消注释
- `Ctrl+K, Ctrl+D` - 格式化文档
- `F12` - 转到定义
- `Shift+F12` - 查找所有引用

### 任务快捷键
- `Ctrl+Shift+P` - 命令面板
- `Ctrl+Shift+` ` - 新建终端
- `Ctrl+B` - 切换侧边栏

## 🔗 相关资源

- [VS Code C# 调试文档](https://code.visualstudio.com/docs/languages/csharp)
- [.NET 调试指南](https://docs.microsoft.com/en-us/dotnet/core/diagnostics/)
- [WPF 调试技巧](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/debugging-wpf)

---

**最后更新**：2025-12-07  
**版本**：1.0