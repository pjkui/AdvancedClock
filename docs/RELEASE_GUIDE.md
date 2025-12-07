# GitHub Actions 自动发布指南

本指南详细说明如何使用 GitHub Actions 自动构建并发布 AdvancedClock 应用程序到 GitHub Release。

## 🚀 自动发布流程概述

### 触发条件
1. **推送版本标签**：当推送格式为 `v*` 的标签时（如 `v1.0.0`）
2. **自动构建**：GitHub Actions 自动构建 Release 版本
3. **自动发布**：构建成功后自动创建 GitHub Release 并上传安装包

### 工作流程
```
推送版本标签 → 触发 GitHub Actions → 构建应用 → 打包文件 → 创建 Release → 上传安装包
```

## 📋 发布步骤

### 方法一：使用自动化脚本（推荐）

1. **运行发布脚本**
   ```powershell
   # 在项目根目录运行
   .\scripts\create-release.ps1 -Version "v1.0.0" -Message "版本 1.0.0 正式发布"
   ```

2. **脚本功能**
   - ✅ 验证版本格式
   - ✅ 检查工作区状态
   - ✅ 确认当前分支
   - ✅ 拉取最新代码
   - ✅ 创建并推送标签
   - ✅ 提供构建状态链接

### 方法二：手动创建标签

1. **确保代码已提交**
   ```bash
   git add .
   git commit -m "准备发布 v1.0.0"
   git push origin main
   ```

2. **创建版本标签**
   ```bash
   git tag -a v1.0.0 -m "版本 1.0.0 发布"
   git push origin v1.0.0
   ```

3. **查看构建状态**
   - 访问仓库的 Actions 页面查看构建进度

## 🔧 GitHub Actions 配置详解

### 工作流文件
- **位置**：`.github/workflows/dotnet-desktop.yml`
- **触发条件**：推送到 main/master 分支或推送版本标签

### 构建矩阵
```yaml
strategy:
  matrix:
    configuration: [Debug, Release]
```
- 同时构建 Debug 和 Release 版本
- 只有 Release 版本会被打包和发布

### 环境变量配置
```yaml
env:
  PROJECT_NAME: AdvancedClock          # 项目名称
  SOLUTION_FILE: AdvancedClock.sln     # 解决方案文件
  PROJECT_FILE: AdvancedClock.csproj   # 项目文件
  DOTNET_VERSION: 7.0.x                # .NET 版本
  PUBLISH_OUTPUT_DIR: ./publish        # 输出目录
  RELEASE_CONFIGURATION: Release       # 发布配置
```

### 构建步骤
1. **检出代码**：获取仓库代码
2. **设置 .NET**：安装指定版本的 .NET SDK
3. **恢复依赖**：下载 NuGet 包
4. **构建项目**：编译应用程序
5. **发布应用**：创建可执行文件
6. **创建发布包**：打包为 ZIP 文件
7. **上传产物**：保存构建结果

### 自动发布步骤
1. **下载构建产物**：获取打包文件
2. **生成更新日志**：基于 Git 提交记录
3. **创建 GitHub Release**：自动发布到 Release 页面

## 📦 发布包格式

### 文件命名
- **标签发布**：`AdvancedClock-v1.0.0-windows-x64.zip`
- **提交发布**：`AdvancedClock-{commit-hash}-windows-x64.zip`

### 包含内容
- `AdvancedClock.exe` - 主程序
- `*.dll` - 依赖库文件
- `AdvancedClock.runtimeconfig.json` - 运行时配置
- 其他必要的运行时文件

## 🏷️ 版本标签规范

### 标签格式
- **正式版本**：`v1.0.0`, `v2.1.3`
- **预发布版本**：`v1.0.0-alpha`, `v1.0.0-beta`, `v1.0.0-rc1`

### 版本号规则
遵循 [语义化版本](https://semver.org/lang/zh-CN/) 规范：
- **主版本号**：不兼容的 API 修改
- **次版本号**：向下兼容的功能性新增
- **修订号**：向下兼容的问题修正

### 示例
```bash
# 主要功能更新
git tag -a v1.0.0 -m "首个正式版本"

# 功能增强
git tag -a v1.1.0 -m "新增系统托盘功能"

# Bug 修复
git tag -a v1.1.1 -m "修复闹钟触发问题"

# 预发布版本
git tag -a v1.2.0-beta -m "v1.2.0 测试版"
```

## 📝 Release 说明自动生成

### 更新日志来源
- 基于 Git 提交记录自动生成
- 显示自上次标签以来的所有提交
- 格式：`- 提交消息 (提交哈希)`

### Release 说明模板
```markdown
## 🚀 v1.0.0 版本更新

### 📦 下载说明
- **Windows 用户**：下载 `AdvancedClock-v1.0.0-windows-x64.zip`
- 解压后运行 `AdvancedClock.exe` 即可使用
- 需要安装 .NET 7.0 Runtime（如果系统中没有）

### 🔄 更新内容
- 新增强提醒功能 (a1b2c3d)
- 修复托盘图标显示问题 (e4f5g6h)
- 优化界面布局 (i7j8k9l)

### 💡 使用提示
- 首次运行会创建示例闹钟
- 数据自动保存到用户目录：`%AppData%\AdvancedClock`
- 支持开机自启动和系统托盘功能

### 🐛 问题反馈
如遇到问题，请在 Issues 中反馈。
```

## 🔍 监控和调试

### 查看构建状态
1. **Actions 页面**：`https://github.com/用户名/AdvancedClock/actions`
2. **构建日志**：点击具体的工作流运行查看详细日志
3. **构建产物**：在成功的构建中下载 Artifacts

### 常见问题排查

#### 1. 构建失败
- 检查 .NET 版本是否匹配
- 确认项目文件路径正确
- 查看构建日志中的错误信息

#### 2. 发布失败
- 确认标签格式正确（以 `v` 开头）
- 检查 GitHub Token 权限
- 确认 Release 权限设置

#### 3. 包上传失败
- 检查文件路径是否正确
- 确认包大小不超过限制
- 查看网络连接状态

### 调试技巧
```yaml
# 在工作流中添加调试步骤
- name: Debug Info
  run: |
    echo "Current directory: $(pwd)"
    echo "Files in publish directory:"
    ls -la ${{ env.PUBLISH_OUTPUT_DIR }}
    echo "Environment variables:"
    env | grep GITHUB
```

## 🛠️ 自定义配置

### 修改发布配置
编辑 `.github/workflows/dotnet-desktop.yml` 文件：

```yaml
# 修改目标框架
DOTNET_VERSION: 8.0.x

# 修改发布配置
- name: Publish
  run: dotnet publish ${{ env.PROJECT_FILE }} 
    --configuration Release 
    --output ${{ env.PUBLISH_OUTPUT_DIR }}/Release 
    --self-contained true 
    --runtime win-x64
```

### 添加多平台支持
```yaml
strategy:
  matrix:
    os: [windows-latest, ubuntu-latest, macos-latest]
    configuration: [Release]
```

### 自定义 Release 说明
创建 `.github/release-template.md` 文件自定义模板。

## 📊 最佳实践

### 1. 版本管理
- ✅ 使用语义化版本号
- ✅ 为每个版本写清楚的提交消息
- ✅ 在发布前充分测试
- ✅ 保持版本标签的一致性

### 2. 自动化流程
- ✅ 使用脚本创建发布
- ✅ 设置自动化测试
- ✅ 监控构建状态
- ✅ 及时修复构建问题

### 3. 文档维护
- ✅ 更新 CHANGELOG.md
- ✅ 维护 README.md
- ✅ 记录重要变更
- ✅ 提供使用说明

## 🔗 相关链接

- [GitHub Actions 文档](https://docs.github.com/en/actions)
- [.NET 发布指南](https://docs.microsoft.com/en-us/dotnet/core/deploying/)
- [语义化版本规范](https://semver.org/lang/zh-CN/)
- [GitHub Release 文档](https://docs.github.com/en/repositories/releasing-projects-on-github)

## 📞 技术支持

如果在使用过程中遇到问题：

1. **查看文档**：首先查看本指南和相关文档
2. **检查日志**：查看 GitHub Actions 构建日志
3. **搜索问题**：在 GitHub Issues 中搜索类似问题
4. **提交 Issue**：如果问题未解决，请提交新的 Issue

---

**最后更新**：2025-12-07  
**版本**：1.0