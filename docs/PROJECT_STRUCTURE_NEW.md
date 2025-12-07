# 项目结构说明

## 整理后的项目结构

```
AdvancedClock/
├── src/                          # 源代码目录
│   ├── *.cs                      # C# 源代码文件
│   ├── *.xaml                    # WPF 界面文件
│   └── *.xaml.cs                 # WPF 代码后置文件
├── docs/                         # 文档目录
│   ├── ALERT_TEST_GUIDE.md       # 闹钟测试指南
│   ├── DEBUG_QUICKSTART.md       # 调试快速开始
│   ├── GITHUB_ACTIONS_GUIDE.md   # GitHub Actions 指南
│   ├── ICON_FILES.md             # 图标文件说明
│   ├── ICON_GUIDE.md             # 图标使用指南
│   ├── PROJECT_STRUCTURE.md      # 原项目结构说明
│   ├── PROJECT_STRUCTURE_NEW.md  # 新项目结构说明
│   ├── RELEASE_EXAMPLE.md        # 发布示例
│   ├── RELEASE_GUIDE.md          # 发布指南
│   └── VSCODE_DEBUG_GUIDE.md     # VS Code 调试指南
├── assets/                       # 资源文件目录
├── scripts/                      # 脚本文件目录
├── .github/                      # GitHub 配置目录
├── .vscode/                      # VS Code 配置目录
├── bin/                          # 编译输出目录
├── obj/                          # 编译临时目录
├── AdvancedClock.csproj          # 项目文件
├── AdvancedClock.sln             # 解决方案文件
├── README.md                     # 项目说明文件
├── .gitignore                    # Git 忽略文件
└── .editorconfig                 # 编辑器配置文件
```

## 主要改进

### 1. 源代码组织
- 所有 C# 源代码文件 (`*.cs`) 移动到 `src/` 目录
- 所有 XAML 文件 (`*.xaml` 和 `*.xaml.cs`) 移动到 `src/` 目录
- 项目文件已更新以支持新的目录结构

### 2. 文档整理
- 所有 Markdown 文档文件移动到 `docs/` 目录
- 保持 `README.md` 在根目录作为项目入口文档

### 3. 项目配置更新
- 更新了 `AdvancedClock.csproj` 文件以正确引用 `src/` 目录中的文件
- 禁用了默认的文件包含规则，使用显式的文件包含配置

## 构建和运行

项目结构整理后，构建和运行方式保持不变：

```bash
# 构建项目
dotnet build

# 运行项目
dotnet run

# 发布项目
dotnet publish -c Release
```

## VS Code 调试

VS Code 的调试配置文件 (`.vscode/launch.json`) 仍然有效，因为它使用相对路径引用编译输出。

## 文件分类

### 源代码文件 (src/)
- `AlarmDataService.cs` - 数据持久化服务
- `AlarmEditDialog.xaml/.cs` - 闹钟编辑对话框
- `AlarmModel.cs` - 闹钟数据模型
- `AlarmService.cs` - 闹钟服务
- `App.xaml/.cs` - 应用程序入口
- `AppSettings.cs` - 应用设置
- `AssemblyInfo.cs` - 程序集信息
- `IconHelper.cs` - 图标辅助类
- `MainWindow.xaml/.cs` - 主窗口
- `StartupService.cs` - 启动服务
- `StrongAlertWindow.xaml/.cs` - 强提醒窗口
- `ValueConverters.cs` - 值转换器

### 文档文件 (docs/)
- 各种 `.md` 文档文件，提供项目的详细说明和指南

### 配置文件 (根目录)
- `AdvancedClock.csproj` - 项目配置
- `AdvancedClock.sln` - 解决方案配置
- `.gitignore` - Git 忽略规则
- `.editorconfig` - 编辑器配置

这种结构使项目更加清晰和易于维护，符合 .NET 项目的最佳实践。