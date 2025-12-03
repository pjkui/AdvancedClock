
# AdvancedClock 项目结构说明

本文档说明了项目的目录结构和文件组织方式。

## 📁 目录结构

```
AdvancedClock/
│
├── assets/                          # 图片和资源文件目录
│   ├── icon.svg                     # 应用图标（SVG矢量格式）
│   ├── icon-preview.html            # 图标预览页面
│   └── convert-icon.ps1             # 图标转换脚本
│
├── docs/                            # 文档目录
│   ├── ICON_GUIDE.md                # 图标使用指南
│   ├── ICON_FILES.md                # 图标文件清单
│   ├── GITHUB_ACTIONS_GUIDE.md      # GitHub Actions 使用指南
│   └── ALERT_TEST_GUIDE.md          # 闹钟测试指南
│
├── .github/                         # GitHub 配置目录
│   └── workflows/
│       └── dotnet-desktop.yml       # GitHub Actions 工作流配置
│
├── *.cs                             # C# 源代码文件
├── *.xaml                           # WPF 界面文件
├── *.csproj                         # 项目文件
├── *.sln                            # 解决方案文件
├── README.md                        # 项目说明文档（根目录）
└── icon.ico                         # Windows 应用图标（需转换生成）
```

## 📂 目录说明

### assets/ - 资源文件目录

存放所有图片、图标和相关资源文件。

**包含文件**：
- `icon.svg` - 应用图标源文件（矢量格式，256×256）
- `icon-preview.html` - 在浏览器中预览图标效果
- `convert-icon.ps1` - 自动化图标转换脚本

**用途**：
- 集中管理项目的视觉资源
- 便于图标的维护和更新
- 提供图标预览和转换工具

### docs/ - 文档目录

存放所有项目文档和使用指南。

**包含文件**：
- `ICON_GUIDE.md` - 图标设计说明和使用指南
- `ICON_FILES.md` - 图标文件清单和技术规格
- `GITHUB_ACTIONS_GUIDE.md` - CI/CD 自动化构建指南
- `ALERT_TEST_GUIDE.md` - 闹钟功能测试指南

**用途**：
- 提供详细的使用文档
- 记录技术规格和设计决策
- 帮助开发者快速上手

### .github/ - GitHub 配置

存放 GitHub 相关的配置文件。

**包含文件**：
- `workflows/dotnet-desktop.yml` - 自动化构建工作流

**用途**：
- 配置 CI/CD 自动化流程
- 自动构建和测试代码
- 生成发布产物

## 🔗 文件引用关系

### 图标文件引用

```
assets/icon.svg
    ↓ (转换)
icon.ico
    ↓ (引用)
AdvancedClock.csproj
    ↓ (编译)
AdvancedClock.exe (带图标)
```

### 文档引用关系

```
README.md (根目录)
    ├─→ docs/ICON_GUIDE.md
    ├─→ docs/GITHUB_ACTIONS_GUIDE.md
    └─→ assets/icon.svg

docs/ICON_GUIDE.md
    ├─→ assets/icon.svg
    ├─→ assets/convert-icon.ps1
    └─→ assets/icon-preview.html

docs/ICON_FILES.md
    ├─→ assets/icon.svg
    ├─→ assets/convert-icon.ps1
    ├─→ assets/icon-preview.html
    └─→ docs/ICON_GUIDE.md
```

## 📝 文件路径规范

### 相对路径使用

在项目内部引用文件时，使用相对路径：

**从根目录引用**：
```markdown
[图标指南](docs/ICON_GUIDE.md)
![图标](assets/icon.svg)
```

**从 docs/ 目录引用**：
```markdown
[图标文件](../assets/icon.svg)
```

**从 assets/ 目录引用**：
```markdown
[文档](../docs/ICON_GUIDE.md)
```

### PowerShell 脚本路径

在 PowerShell 脚本中使用路径：

```powershell
# 从根目录运行脚本
.\assets\convert-icon.ps1

# 脚本内部引用文件
$svgFile = "assets/icon.svg"
$icoFile = "icon.ico"
```

## 🎯 最佳实践

### 1. 文件组织原则

- ✅ **资源文件** → `assets/` 目录
- ✅ **文档文件** → `docs/` 目录
- ✅ **源代码** → 项目根目录
- ✅ **配置文件** → 相应的配置目录（如 `.github/`）

### 2. 命名规范

- **目录名**：小写，使用连字符（如 `assets`, `docs`）
- **文档文件**：大写，使用下划线（如 `ICON_GUIDE.md`）
- **资源文件**：小写，使用连字符（如 `icon-preview.html`）
- **源代码**：PascalCase（如 `MainWindow.xaml.cs`）

### 3. 路径引用

- 优先使用相对路径
- 避免硬编码绝对路径
- 在脚本中使用变量存储路径
- 确保跨平台兼容性（使用 `/` 而非 `\`）

### 4. 文档维护

- 更新文件位置时同步更新所有引用
- 在 README.md 中保持目录结构的最新状态
- 为新增的重要文件添加说明

## 🔄 迁移记录

### 2025-12-03 - 目录结构重组

**变更内容**：
- ✅ 创建 `assets/` 目录
- ✅ 创建 `docs/` 目录
- ✅ 移动图标文件到 `assets/`
- ✅ 移动文档文件到 `docs/`
- ✅ 更新所有文件引用路径

**移动的文件**：

**assets/ 目录**：
- `icon.svg` (根目录 → assets/)
- `icon-preview.html` (根目录 → assets/)
- `convert-icon.ps1` (根目录 → assets/)

**docs/ 目录**：
- `ICON_GUIDE.md` (根目录 → docs/)
- `ICON_FILES.md` (根目录 → docs/)
- `GITHUB_ACTIONS_GUIDE.md` (根目录 → docs/)
- `ALERT_TEST_GUIDE.md` (根目录 → docs/)

**更新的引用**：
- ✅ README.md - 更新图标和文档链接
- ✅ convert-icon.ps1 - 更新文件路径
- ✅ icon-preview.html - 更新文档链接
- ✅ ICON_GUIDE.md - 更新文件路径
- ✅ ICON_FILES.md - 更新文件路径和目录结构

## 📊 文件统计

### 目录统计

| 目录 | 文件数 | 说明 |
|------|--------|------|
| `assets/` | 3 | 图标和资源文件 |
| `docs/` | 4 | 项目文档 |
| `.github/workflows/` | 1 | CI/CD 配置 |
| 根目录 | ~20 | 源代码和配置文件 |

### 文件类型统计

| 类型 | 数量 | 位置 |
|------|------|------|
| Markdown 文档 | 5 | docs/ + 根目录 |
| SVG 图标 | 1 | assets/ |
| HTML 文件 | 1 | assets/ |
| PowerShell 脚本 | 1 | assets/ |
| C# 源文件 | ~10 | 根目录 |
| XAML 文件 | ~5 | 根目录 |

## 🚀 快速导航

### 查看图标

```powershell
# 在浏览器中预览图标
Start-Process assets\icon-preview.html
```

### 转换图标

```powershell
# 运行转换脚本
.\assets\convert-icon.ps1
```

### 查看文档

```powershell
# 在编辑器中打开文档
code docs\ICON_GUIDE.md
code docs\GITHUB_ACTIONS_GUIDE.md
```

### 构建项目

```powershell
# 构建项目
dotnet build

# 发布项目
dotnet publish --configuration Release
```

## 📞 相关文档

- [README.md](../README.md) - 项目总览
- [图标设计指南](docs/ICON_GUIDE.md) - 图标使用说明
- [GitHub Actions 指南](docs/GITHUB_ACTIONS_GUIDE.md) - CI/CD 配置
- [闹钟测试指南](docs/ALERT_TEST_GUIDE.md) - 功能测试

---

**最后更新**：2025-12-03  
**版本**：1.0
