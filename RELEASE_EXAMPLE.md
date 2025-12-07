# 自动发布示例

本文档展示如何使用 GitHub Actions 自动发布 AdvancedClock 的完整流程。

## 📋 发布前准备

### 1. 确认代码状态
```powershell
# 检查当前状态
git status

# 确保所有更改已提交
git add .
git commit -m "准备发布 v1.0.0 版本"
git push origin main
```

### 2. 测试构建
```powershell
# 本地测试构建
dotnet build AdvancedClock.sln --configuration Release
dotnet publish AdvancedClock.csproj --configuration Release --output ./test-publish
```

## 🚀 自动发布流程

### 方法一：使用自动化脚本（推荐）

```powershell
# 在项目根目录运行
.\scripts\create-release.ps1 -Version "v1.0.0" -Message "AdvancedClock v1.0.0 正式发布"
```

**脚本执行过程**：
1. ✅ 验证版本格式（v1.0.0）
2. ✅ 检查工作区状态
3. ✅ 确认当前在主分支
4. ✅ 拉取最新代码
5. ✅ 创建标签 `v1.0.0`
6. ✅ 推送标签到远程仓库
7. ✅ 显示构建状态链接

### 方法二：手动创建标签

```bash
# 创建带注释的标签
git tag -a v1.0.0 -m "AdvancedClock v1.0.0 正式发布

主要更新：
- 新增强提醒功能
- 修复系统托盘问题
- 优化界面体验
- 提升性能稳定性"

# 推送标签
git push origin v1.0.0
```

## 🔄 GitHub Actions 自动化过程

### 1. 触发构建
推送标签后，GitHub Actions 自动开始：

```yaml
# 触发条件
on:
  push:
    tags: [ "v*" ]  # 匹配 v1.0.0, v2.1.3 等
```

### 2. 构建阶段
```
📦 构建阶段
├── 检出代码
├── 设置 .NET 7.0
├── 恢复 NuGet 包
├── 构建 Debug 版本
├── 构建 Release 版本
├── 发布 Release 应用
├── 创建 ZIP 包
└── 上传构建产物
```

### 3. 发布阶段
```
🚀 发布阶段
├── 下载构建产物
├── 获取版本信息
├── 生成更新日志
├── 创建 GitHub Release
└── 上传安装包
```

## 📦 发布结果

### 生成的文件
- **安装包**：`AdvancedClock-v1.0.0-windows-x64.zip`
- **包含内容**：
  - `AdvancedClock.exe` - 主程序
  - 依赖的 DLL 文件
  - 运行时配置文件

### GitHub Release 页面
自动创建的 Release 包含：

```markdown
## 🚀 v1.0.0 版本更新

### 📦 下载说明
- **Windows 用户**：下载 `AdvancedClock-v1.0.0-windows-x64.zip`
- 解压后运行 `AdvancedClock.exe` 即可使用
- 需要安装 .NET 7.0 Runtime（如果系统中没有）

### 🔄 更新内容
- 新增强提醒功能 (a1b2c3d)
- 修复系统托盘问题 (e4f5g6h)
- 优化界面体验 (i7j8k9l)

### 💡 使用提示
- 首次运行会创建示例闹钟
- 数据自动保存到用户目录：`%AppData%\AdvancedClock`
- 支持开机自启动和系统托盘功能

### 🐛 问题反馈
如遇到问题，请在 Issues 中反馈。
```

## 🔍 监控发布过程

### 1. 查看构建状态
访问：`https://github.com/用户名/AdvancedClock/actions`

### 2. 构建日志示例
```
Run dotnet build AdvancedClock.sln --configuration Release --no-restore
Microsoft (R) Build Engine version 17.0.0+c9eb9dd64 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  AdvancedClock -> f:\code\AdvancedClock\bin\Release\net7.0-windows\AdvancedClock.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:02.34
```

### 3. 发布成功通知
GitHub 会发送邮件通知发布成功，包含：
- Release 页面链接
- 下载链接
- 发布说明

## 🎯 版本发布示例

### v1.0.0 - 首个正式版本
```powershell
.\scripts\create-release.ps1 -Version "v1.0.0" -Message "AdvancedClock 首个正式版本发布

主要功能：
✅ 多种循环模式（不循环、每天、每月、每年）
✅ 数据持久化和自动保存
✅ 开机自动启动
✅ 强提醒和弱提醒
✅ 系统托盘支持
✅ 用户友好界面"
```

### v1.1.0 - 功能增强版本
```powershell
.\scripts\create-release.ps1 -Version "v1.1.0" -Message "AdvancedClock v1.1.0 功能增强版本

新增功能：
🆕 多语言支持
🆕 主题切换功能
🆕 声音提醒选择
🆕 闹钟分组管理

优化改进：
🔧 提升启动速度
🔧 优化内存使用
🔧 改进界面响应"
```

### v1.0.1 - Bug 修复版本
```powershell
.\scripts\create-release.ps1 -Version "v1.0.1" -Message "AdvancedClock v1.0.1 Bug修复版本

修复问题：
🐛 修复闹钟不触发的问题
🐛 修复托盘图标显示异常
🐛 修复数据保存失败问题
🐛 修复界面布局错位"
```

## 🛠️ 故障排除

### 常见问题

#### 1. 构建失败
**问题**：.NET SDK 版本不匹配
```
error NETSDK1045: The current .NET SDK does not support targeting .NET 7.0.
```

**解决**：检查 GitHub Actions 中的 .NET 版本配置

#### 2. 标签推送失败
**问题**：标签已存在
```
error: tag 'v1.0.0' already exists
```

**解决**：
```bash
# 删除本地标签
git tag -d v1.0.0
# 删除远程标签
git push origin --delete v1.0.0
# 重新创建
git tag -a v1.0.0 -m "新的发布消息"
git push origin v1.0.0
```

#### 3. Release 创建失败
**问题**：权限不足
```
Error: Resource not accessible by integration
```

**解决**：检查仓库的 Actions 权限设置

### 调试技巧

#### 1. 本地测试发布脚本
```powershell
# 测试模式（不实际推送）
$env:DRY_RUN = "true"
.\scripts\create-release.ps1 -Version "v1.0.0-test"
```

#### 2. 查看详细构建日志
在 GitHub Actions 页面点击具体的构建步骤查看详细输出

#### 3. 下载构建产物测试
在构建成功后下载 Artifacts 进行本地测试

## 📈 发布统计

### 版本发布历史
- `v1.0.0` - 2025-12-07 - 首个正式版本
- `v1.0.1` - 2025-12-08 - Bug修复版本
- `v1.1.0` - 2025-12-15 - 功能增强版本

### 下载统计
GitHub Release 页面会显示每个版本的下载次数统计

## 🔗 相关链接

- [GitHub Actions 工作流](../.github/workflows/dotnet-desktop.yml)
- [发布脚本](../scripts/create-release.ps1)
- [详细发布指南](RELEASE_GUIDE.md)
- [项目主页](../README.md)

---

**最后更新**：2025-12-07  
**版本**：1.0