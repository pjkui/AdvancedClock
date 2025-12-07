# VS Code 调试快速入门

本文档帮助您快速开始在 VS Code 中调试 AdvancedClock 项目。

## 🚀 5分钟快速开始

### 1. 打开项目
```bash
# 在项目根目录运行
code .
```

### 2. 安装扩展
VS Code 会自动提示安装推荐扩展，点击"安装"即可。

必需扩展：
- **C# for Visual Studio Code**
- **.NET Install Tool**
- **XML**

### 3. 开始调试
1. 按 `F5` 键
2. 选择"启动 AdvancedClock (Debug)"
3. 程序会自动构建并启动

## 🎯 常用调试场景

### 场景1：调试闹钟添加功能

1. **设置断点**
   ```csharp
   // 在 MainWindow.xaml.cs 第 X 行设置断点
   private void AddAlarmButton_Click(object sender, RoutedEventArgs e)
   {
       var dialog = new AlarmEditDialog(); // ← 点击此行左侧设置断点
       if (dialog.ShowDialog() == true)
       {
           _alarmService.AddAlarm(dialog.AlarmModel);
       }
   }
   ```

2. **启动调试**
   - 按 `F5` 启动调试
   - 在程序中点击"添加闹钟"按钮
   - 程序会在断点处暂停

3. **检查变量**
   - 在"变量"窗口查看 `dialog` 对象
   - 在"监视"窗口添加 `dialog.AlarmModel`

### 场景2：调试闹钟触发逻辑

1. **设置断点**
   ```csharp
   // 在 AlarmService.cs 中设置断点
   private void CheckAlarms()
   {
       var now = DateTime.Now; // ← 断点1
       
       foreach (var alarm in _alarms.Where(a => a.IsEnabled))
       {
           if (ShouldTriggerAlarm(alarm, now)) // ← 断点2
           {
               TriggerAlarm(alarm); // ← 断点3
           }
       }
   }
   ```

2. **使用监视表达式**
   在"监视"窗口添加：
   - `now`
   - `alarm.AlarmTime`
   - `alarm.IsEnabled`

### 场景3：调试数据保存问题

1. **设置断点**
   ```csharp
   // 在 AlarmDataService.cs 中设置断点
   public void SaveAlarms(IEnumerable<AlarmModel> alarms)
   {
       try
       {
           var json = JsonSerializer.Serialize(alarms, _options); // ← 断点
           File.WriteAllText(DataFilePath, json);
       }
       catch (Exception ex)
       {
           Debug.WriteLine($"保存失败: {ex.Message}"); // ← 异常断点
       }
   }
   ```

2. **检查序列化结果**
   - 查看 `json` 变量内容
   - 检查 `DataFilePath` 路径
   - 监视 `alarms` 集合

## ⌨️ 常用快捷键

### 调试控制
- `F5` - 开始调试/继续执行
- `F9` - 设置/取消断点
- `F10` - 单步跳过（不进入函数）
- `F11` - 单步进入（进入函数内部）
- `Shift+F11` - 单步跳出（退出当前函数）
- `Shift+F5` - 停止调试

### 编辑器
- `Ctrl+K, Ctrl+C` - 注释选中代码
- `Ctrl+K, Ctrl+U` - 取消注释
- `F12` - 转到定义
- `Shift+F12` - 查找所有引用

## 🔧 调试面板功能

### 1. 变量窗口
- **局部变量**：当前方法中的变量
- **监视**：自定义监视的表达式
- **调用堆栈**：方法调用链

### 2. 断点窗口
- 查看所有断点
- 启用/禁用断点
- 设置条件断点

### 3. 调试控制台
- 执行即时表达式
- 查看调试输出

## 🎨 调试技巧

### 1. 条件断点
右键断点 → "编辑断点" → 添加条件：
```csharp
// 只在特定条件下停止
i > 10
alarm.Name == "测试闹钟"
DateTime.Now.Hour == 9
```

### 2. 日志断点
右键断点 → "编辑断点" → 勾选"日志消息"：
```
当前时间：{DateTime.Now}，闹钟数量：{_alarms.Count}
```

### 3. 监视复杂表达式
在监视窗口中添加：
```csharp
_alarms.Where(a => a.IsEnabled).Count()
DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
_alarmService?.IsRunning ?? false
```

## 🐛 常见问题

### 问题1：断点不生效（空心圆）
**原因**：使用了 Release 配置或 PDB 文件缺失
**解决**：
1. 确保选择"启动 AdvancedClock (Debug)"配置
2. 重新构建项目：`Ctrl+Shift+P` → "Tasks: Run Task" → "build-debug"

### 问题2：无法查看变量值
**原因**：编译器优化或变量超出作用域
**解决**：
1. 使用 Debug 配置
2. 在变量声明处设置断点
3. 检查变量是否在当前作用域内

### 问题3：调试器启动失败
**原因**：.NET SDK 未安装或版本不匹配
**解决**：
1. 检查 .NET SDK：`dotnet --version`
2. 安装 .NET 7.0 SDK
3. 重启 VS Code

## 📋 调试检查清单

### 开始调试前
- [ ] 已安装必需的 VS Code 扩展
- [ ] 项目可以正常构建（`dotnet build`）
- [ ] 选择了正确的调试配置

### 调试过程中
- [ ] 断点设置在正确的位置
- [ ] 检查变量值是否符合预期
- [ ] 使用调用堆栈了解执行路径
- [ ] 利用监视窗口跟踪关键变量

### 调试结束后
- [ ] 移除不必要的断点
- [ ] 记录发现的问题和解决方案
- [ ] 提交代码修改

## 🎯 实战练习

### 练习1：添加调试日志
在关键方法中添加调试输出：
```csharp
System.Diagnostics.Debug.WriteLine($"[DEBUG] 方法 {nameof(AddAlarm)} 被调用");
```

### 练习2：性能调试
使用 Stopwatch 测量方法执行时间：
```csharp
var sw = System.Diagnostics.Stopwatch.StartNew();
// 你的代码
sw.Stop();
Debug.WriteLine($"执行时间：{sw.ElapsedMilliseconds}ms");
```

### 练习3：异常调试
在调试面板中设置异常断点，捕获所有异常。

## 🔗 进阶资源

- [完整调试指南](docs/VSCODE_DEBUG_GUIDE.md)
- [VS Code C# 文档](https://code.visualstudio.com/docs/languages/csharp)
- [.NET 调试技巧](https://docs.microsoft.com/en-us/dotnet/core/diagnostics/)

---

**开始调试愉快！** 🎉