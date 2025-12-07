# .NET 框架升级指南

## 升级概述

项目已从 .NET 7.0 升级到 .NET 8.0，以解决框架支持生命周期问题。

## 升级详情

### 升级前
- **目标框架**: `net7.0-windows`
- **System.Drawing.Common**: `7.0.0`
- **警告**: NETSDK1138 - .NET 7.0 不再受支持

### 升级后
- **目标框架**: `net8.0-windows` ✅
- **System.Drawing.Common**: `8.0.0` ✅
- **警告**: 已解决 ✅

## 升级内容

### 1. 项目文件更新 (`AdvancedClock.csproj`)
```xml
<PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <!-- 其他配置保持不变 -->
</PropertyGroup>

<ItemGroup>
    <PackageReference Include="System.Drawing.Common" Version="8.0.0" />
</ItemGroup>
```

### 2. VS Code 调试配置更新 (`.vscode/launch.json`)
所有调试配置的程序路径已更新：
- `${workspaceFolder}/bin/Debug/net7.0-windows/AdvancedClock.exe`
- ↓ 更新为 ↓
- `${workspaceFolder}/bin/Debug/net8.0-windows/AdvancedClock.exe`

### 3. 输出目录变更
- **Debug**: `bin/Debug/net8.0-windows/`
- **Release**: `bin/Release/net8.0-windows/`

## .NET 8.0 的优势

### 长期支持 (LTS)
- .NET 8.0 是长期支持版本，支持到 2026 年 11 月
- 获得持续的安全更新和 bug 修复

### 性能改进
- 更好的 JIT 编译器优化
- 改进的垃圾收集器性能
- 更快的启动时间

### 新功能
- 改进的 WPF 支持
- 更好的 Windows Forms 集成
- 增强的诊断和调试工具

## 兼容性

### 向后兼容
- 所有现有功能保持不变
- 用户数据和配置文件完全兼容
- 界面和用户体验无变化

### 系统要求
- Windows 10 版本 1607 或更高版本
- Windows 11（所有版本）
- .NET 8.0 运行时（如果独立部署则不需要）

## 验证升级

### 构建测试
```bash
# 清理旧版本
dotnet clean

# 还原包
dotnet restore

# 构建 Debug 版本
dotnet build

# 构建 Release 版本
dotnet build -c Release
```

### 运行测试
```bash
# 运行程序
dotnet run

# 或直接运行可执行文件
./bin/Debug/net8.0-windows/AdvancedClock.exe
```

## 故障排除

### 常见问题

1. **构建错误**: 如果遇到资产文件错误，运行 `dotnet restore`
2. **调试问题**: 确保 VS Code 配置文件中的路径正确
3. **运行时错误**: 确保安装了 .NET 8.0 运行时

### 回滚方案
如果需要回滚到 .NET 7.0：
1. 将 `TargetFramework` 改回 `net7.0-windows`
2. 将 `System.Drawing.Common` 版本改回 `7.0.0`
3. 更新 VS Code 配置文件中的路径
4. 运行 `dotnet restore` 和 `dotnet build`

## 升级日期
- **升级时间**: 2025年12月7日
- **升级版本**: v1.0.1 → v1.1.0
- **升级原因**: 解决 NETSDK1138 警告，使用长期支持版本

## 相关链接
- [.NET 8.0 发布说明](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [.NET 支持策略](https://dotnet.microsoft.com/platform/support/policy)
- [从 .NET 7 迁移到 .NET 8](https://docs.microsoft.com/en-us/dotnet/core/migration/)