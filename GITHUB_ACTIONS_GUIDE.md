# GitHub Actions 自动化构建指南

## 概述

本项目已配置 GitHub Actions 自动化工作流，用于在代码推送或 Pull Request 时自动构建和测试 AdvancedClock WPF 应用程序。

## 修复内容

### 原配置的问题

1. ❌ **使用了占位符变量**：`your-solution-name`、`your-test-project-path` 等未替换
2. ❌ **.NET 版本不匹配**：配置使用 .NET 8.0，但项目使用 .NET 7.0
3. ❌ **复杂的打包流程**：配置针对 Windows 应用打包项目（WAP），但本项目是简单的 WPF 应用
4. ❌ **签名证书步骤**：包含了不必要的证书签名步骤
5. ❌ **硬编码配置**：项目名称、版本等信息直接硬编码在步骤中

### 新配置的改进

✅ **简化的工作流**：移除了不必要的复杂步骤
✅ **正确的 .NET 版本**：使用 .NET 7.0.x 匹配项目配置
✅ **实际的项目名称**：使用 `AdvancedClock.sln` 和 `AdvancedClock.csproj`
✅ **标准的构建流程**：恢复 → 构建 → 发布 → 上传产物
✅ **支持多分支**：同时支持 `main` 和 `master` 分支
✅ **环境变量管理**：所有配置项提取到 `env` 中，便于维护和修改

## 工作流程说明

### 环境变量配置

工作流使用以下环境变量进行配置（在 `env` 部分定义）：

```yaml
env:
  # 项目配置
  PROJECT_NAME: AdvancedClock              # 项目名称
  SOLUTION_FILE: AdvancedClock.sln         # 解决方案文件
  PROJECT_FILE: AdvancedClock.csproj       # 项目文件
  
  # .NET 配置
  DOTNET_VERSION: 7.0.x                    # .NET SDK 版本
  
  # 构建配置
  PUBLISH_OUTPUT_DIR: ./publish            # 发布输出目录
  ARTIFACT_RETENTION_DAYS: 30              # 产物保留天数
```

**优势**：
- 🔧 **易于维护**：所有配置集中管理，修改时只需更新一处
- 🔄 **可重用性**：可以轻松复制到其他项目并修改变量
- 📝 **清晰明了**：配置项含义一目了然
- 🚀 **快速适配**：更换项目名称或版本时无需修改多处

### 触发条件

工作流在以下情况下自动触发：
- 推送代码到 `main` 或 `master` 分支
- 创建针对 `main` 或 `master` 分支的 Pull Request

### 构建矩阵

工作流会并行构建两个配置：
- **Debug**：调试版本
- **Release**：发布版本

### 构建步骤

1. **Checkout**：检出代码仓库
2. **Setup .NET 7.0**：安装 .NET 7.0 SDK
3. **Restore dependencies**：恢复 NuGet 包依赖
4. **Build**：编译项目
5. **Publish**：发布应用程序（生成可执行文件）
6. **Upload build artifacts**：上传构建产物到 GitHub

### 构建产物

构建完成后，可以在 GitHub Actions 页面下载构建产物：
- `AdvancedClock-Debug`：调试版本的可执行文件
- `AdvancedClock-Release`：发布版本的可执行文件

产物保留时间：30 天

## 如何查看构建结果

1. 访问 GitHub 仓库页面
2. 点击顶部的 **Actions** 标签
3. 查看最近的工作流运行记录
4. 点击具体的运行记录查看详细日志
5. 构建成功后，在 **Artifacts** 部分下载构建产物

## 构建状态徽章

可以在 README.md 中添加构建状态徽章：

```markdown
![Build Status](https://github.com/YOUR_USERNAME/AdvancedClock/workflows/.NET%20WPF%20Desktop%20Build/badge.svg)
```

将 `YOUR_USERNAME` 替换为你的 GitHub 用户名。

## 本地测试

在推送代码前，可以在本地测试构建流程：

```powershell
# 恢复依赖
dotnet restore AdvancedClock.sln

# 构建 Debug 版本
dotnet build AdvancedClock.sln --configuration Debug --no-restore

# 构建 Release 版本
dotnet build AdvancedClock.sln --configuration Release --no-restore

# 发布 Release 版本
dotnet publish AdvancedClock.csproj --configuration Release --output ./publish/Release
```

## 故障排查

### 构建失败

如果构建失败，请检查：
1. 代码是否有编译错误
2. 所有依赖包是否正确引用
3. .NET 版本是否匹配（项目使用 .NET 7.0）

### 产物上传失败

如果产物上传失败，请检查：
1. 发布路径是否正确
2. GitHub Actions 权限设置

## 自定义配置

如果需要修改配置，只需编辑 `.github/workflows/dotnet-desktop.yml` 文件中的 `env` 部分：

### 修改项目名称

```yaml
env:
  PROJECT_NAME: YourProjectName           # 修改为你的项目名称
  SOLUTION_FILE: YourProject.sln          # 修改为你的解决方案文件
  PROJECT_FILE: YourProject.csproj        # 修改为你的项目文件
```

### 修改 .NET 版本

```yaml
env:
  DOTNET_VERSION: 8.0.x                   # 修改为你需要的版本
```

### 修改构建配置

```yaml
env:
  PUBLISH_OUTPUT_DIR: ./output            # 修改输出目录
  ARTIFACT_RETENTION_DAYS: 90             # 修改保留天数（1-90）
```

### 添加更多构建配置

如果需要添加更多构建配置（如 x86、x64、ARM64），可以修改构建矩阵：

```yaml
strategy:
  matrix:
    configuration: [Debug, Release]
    platform: [x64, x86, ARM64]           # 添加平台配置
```

## 未来改进

可以考虑添加以下功能：
- 🔄 自动化测试（单元测试、集成测试）
- 📦 自动创建 Release 和上传安装包
- 🔐 代码签名（需要配置签名证书）
- 📊 代码质量检查（SonarQube、CodeQL）
- 🚀 自动部署到发布平台
- 🌍 多平台构建（Windows、Linux、macOS）

## 参考资料

- [GitHub Actions 文档](https://docs.github.com/en/actions)
- [.NET CLI 文档](https://docs.microsoft.com/en-us/dotnet/core/tools/)
- [WPF 应用部署](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/deployment/)
