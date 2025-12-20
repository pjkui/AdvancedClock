# Git 标签管理指南

## 📋 当前标签列表

### 已发布的版本标签

| 标签 | 发布日期 | 说明 |
|------|---------|------|
| **v2.5.2** | 2025-12-20 | 修复声音文件显示问题，优化声音选择功能 |
| v1.1.0 | - | 早期版本 |
| v1.0.3 | - | 早期版本 |
| v1.0.2 | - | 早期版本 |
| v1.0.1 | - | 早期版本 |
| v1.0.0 | - | 初始版本 |

## 🏷️ v2.5.2 版本详情

### 发布信息
- **版本号**: v2.5.2
- **发布日期**: 2025-12-20
- **提交哈希**: 0712268
- **标签类型**: 轻量级标签 (Lightweight Tag)

### 主要更新内容

#### 🐛 Bug 修复
1. **修复声音文件不显示问题**
   - 在项目文件中添加配置，将 `sounds` 目录复制到输出目录
   - 现在下拉列表可以正确显示所有默认声音文件

#### ✨ 功能增强
1. **声音选择功能优化**
   - 自动枚举 `sounds/defaults` 目录下的所有声音文件
   - 支持从下拉列表选择预设声音
   - 支持选择自定义位置的音乐文件

2. **声音播放增强**
   - 音乐默认循环播放 1 分钟后自动停止
   - 播放时长可自定义（5-600 秒）
   - 支持多种音频格式（WAV、MP3、WMA、M4A）

#### 📚 文档更新
- 创建声音文件显示问题修复说明文档
- 创建声音文件部署验证脚本
- 更新 README.md 添加版本记录

### 技术改进
1. **项目配置优化**
   ```xml
   <ItemGroup>
       <None Include="sounds\**\*.*">
           <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
           <Link>sounds\%(RecursiveDir)%(Filename)%(Extension)</Link>
       </None>
   </ItemGroup>
   ```

2. **音频服务增强**
   - 添加 `GetDefaultSounds()` 方法自动扫描默认声音
   - 实现循环播放和定时停止功能
   - 优化音频播放器资源管理

### 相关文件
- `AdvancedClock.csproj` - 项目配置文件
- `src/AudioService.cs` - 音频服务
- `src/AlarmEditDialog.xaml.cs` - 编辑对话框
- `docs/SOUND_FILES_FIX.md` - 修复说明文档
- `scripts/verify-sound-deployment.ps1` - 验证脚本

## 🔧 Git 标签操作指南

### 查看标签

```bash
# 查看所有标签
git tag

# 查看标签详细信息
git show v2.5.2

# 查看特定模式的标签
git tag -l "v2.*"
```

### 创建标签

#### 轻量级标签（Lightweight Tag）
```bash
# 创建轻量级标签
git tag v2.5.2

# 为特定提交创建标签
git tag v2.5.2 <commit-hash>
```

#### 附注标签（Annotated Tag）
```bash
# 创建附注标签（推荐用于发布版本）
git tag -a v2.5.2 -m "Release v2.5.2: Fix sound files display issue"

# 为特定提交创建附注标签
git tag -a v2.5.2 <commit-hash> -m "Release message"
```

### 推送标签

```bash
# 推送单个标签到远程仓库
git push origin v2.5.2

# 推送所有标签到远程仓库
git push origin --tags

# 推送所有标签（包括轻量级和附注标签）
git push --follow-tags
```

### 删除标签

```bash
# 删除本地标签
git tag -d v2.5.2

# 删除远程标签
git push origin --delete v2.5.2

# 或者使用这种方式删除远程标签
git push origin :refs/tags/v2.5.2
```

### 检出标签

```bash
# 检出标签（会进入分离头指针状态）
git checkout v2.5.2

# 基于标签创建新分支
git checkout -b branch-name v2.5.2
```

## 📦 版本发布流程

### 1. 准备发布

```bash
# 确保工作区干净
git status

# 查看最近的提交
git log --oneline -5

# 确保所有更改已提交
git add .
git commit -m "feat: 准备发布 v2.5.2"
```

### 2. 创建标签

```bash
# 创建版本标签
git tag v2.5.2

# 或创建带注释的标签（推荐）
git tag -a v2.5.2 -m "Release v2.5.2: 修复声音文件显示问题"
```

### 3. 推送到远程

```bash
# 推送代码
git push origin main

# 推送标签
git push origin v2.5.2
```

### 4. 创建 GitHub Release

1. 访问 GitHub 仓库页面
2. 点击 "Releases" 标签
3. 点击 "Create a new release"
4. 选择标签 `v2.5.2`
5. 填写发布说明
6. 上传编译好的二进制文件（可选）
7. 点击 "Publish release"

## 🎯 版本号规范

### 语义化版本（Semantic Versioning）

格式：`MAJOR.MINOR.PATCH`

- **MAJOR（主版本号）**: 不兼容的 API 修改
- **MINOR（次版本号）**: 向下兼容的功能性新增
- **PATCH（修订号）**: 向下兼容的问题修正

### 示例

```
v1.0.0 - 初始版本
v1.0.1 - Bug 修复
v1.1.0 - 新增功能
v2.0.0 - 重大更新（可能不兼容）
```

### 预发布版本

```
v2.5.2-alpha   - Alpha 测试版
v2.5.2-beta    - Beta 测试版
v2.5.2-rc.1    - Release Candidate 候选版本
```

## 📝 标签命名建议

### 推荐格式

```bash
# 正式版本
v2.5.2

# 预发布版本
v2.5.2-alpha
v2.5.2-beta.1
v2.5.2-rc.1

# 带日期的版本
v2.5.2-20251220
```

### 不推荐的格式

```bash
# 避免使用
2.5.2          # 缺少 'v' 前缀
version-2.5.2  # 过于冗长
release_2.5.2  # 使用下划线
```

## 🔍 标签查询技巧

### 查找特定版本

```bash
# 查找所有 v2.x 版本
git tag -l "v2.*"

# 查找所有 beta 版本
git tag -l "*beta*"

# 按时间排序显示标签
git tag --sort=-creatordate
```

### 比较标签

```bash
# 比较两个标签之间的差异
git diff v2.5.1..v2.5.2

# 查看两个标签之间的提交
git log v2.5.1..v2.5.2

# 查看两个标签之间的文件变更
git log --stat v2.5.1..v2.5.2
```

## 🛠️ 常见问题

### Q1: 如何修改已推送的标签？

```bash
# 1. 删除本地标签
git tag -d v2.5.2

# 2. 删除远程标签
git push origin --delete v2.5.2

# 3. 重新创建标签
git tag v2.5.2

# 4. 推送新标签
git push origin v2.5.2
```

### Q2: 如何为历史提交创建标签？

```bash
# 查找提交哈希
git log --oneline

# 为特定提交创建标签
git tag v2.5.1 <commit-hash>

# 推送标签
git push origin v2.5.1
```

### Q3: 轻量级标签 vs 附注标签？

**轻量级标签**：
- 只是一个指向提交的引用
- 不包含额外信息
- 适合临时标记

**附注标签**（推荐用于发布）：
- 包含标签创建者信息
- 包含标签日期
- 包含标签说明
- 可以被 GPG 签名

```bash
# 创建附注标签
git tag -a v2.5.2 -m "Release v2.5.2"

# 查看附注标签信息
git show v2.5.2
```

## 📊 版本历史

### v2.x 系列（当前）

- **v2.5.2** (2025-12-20) - 修复声音文件显示问题
- **v2.5.1** (2025-12-20) - 修复空引用异常
- **v2.5.0** (2025-12-20) - 声音增强功能
- **v2.4.0** (2025-12-20) - 滚动功能优化
- **v2.3.0** (2025-12-20) - 自定义声音功能
- **v2.2.0** (2025-12-20) - 循环闹钟永不过期
- **v2.1.0** (2025-12-20) - 触发动作系统

### v1.x 系列（早期版本）

- **v1.1.0** - 功能增强
- **v1.0.3** - Bug 修复
- **v1.0.2** - Bug 修复
- **v1.0.1** - Bug 修复
- **v1.0.0** - 初始发布

## 🔗 相关链接

- **GitHub 仓库**: https://github.com/pjkui/AdvancedClock
- **发布页面**: https://github.com/pjkui/AdvancedClock/releases
- **标签列表**: https://github.com/pjkui/AdvancedClock/tags
- **语义化版本规范**: https://semver.org/lang/zh-CN/

## 📚 参考资源

### Git 标签文档
- [Git 官方文档 - 标签](https://git-scm.com/book/zh/v2/Git-%E5%9F%BA%E7%A1%80-%E6%89%93%E6%A0%87%E7%AD%BE)
- [GitHub 文档 - 管理发布](https://docs.github.com/zh/repositories/releasing-projects-on-github)

### 版本管理最佳实践
- [语义化版本 2.0.0](https://semver.org/lang/zh-CN/)
- [Keep a Changelog](https://keepachangelog.com/zh-CN/1.0.0/)

---

**文档版本**: v1.0  
**最后更新**: 2025-12-20  
**维护者**: AdvancedClock 开发团队
