# Git 标签快速参考

## 📌 常用命令

### 查看标签
```bash
# 列出所有标签
git tag

# 列出特定模式的标签
git tag -l "v2.*"

# 按版本号排序
git tag --sort=-version:refname

# 查看标签详情
git show v2.5.2
```

### 创建标签
```bash
# 轻量级标签
git tag v2.5.2

# 附注标签
git tag -a v2.5.2 -m "Release v2.5.2"

# 为历史提交创建标签
git tag v2.5.2 <commit-hash>
```

### 推送标签
```bash
# 推送单个标签
git push origin v2.5.2

# 推送所有标签
git push origin --tags
```

### 删除标签
```bash
# 删除本地标签
git tag -d v2.5.2

# 删除远程标签
git push origin --delete v2.5.2
```

### 检出标签
```bash
# 检出标签（分离头指针）
git checkout v2.5.2

# 基于标签创建分支
git checkout -b branch-name v2.5.2
```

## 🚀 使用脚本

### 自动化标签创建
```powershell
# 基本用法
.\scripts\create-tag.ps1 -Version "2.5.3"

# 创建附注标签
.\scripts\create-tag.ps1 -Version "2.5.3" -Annotated -Message "Release message"

# 创建并推送
.\scripts\create-tag.ps1 -Version "2.5.3" -Push

# 完整示例
.\scripts\create-tag.ps1 -Version "2.5.3" -Annotated -Message "Release v2.5.3" -Push
```

## 📋 版本号规范

### 语义化版本 (Semantic Versioning)
```
v主版本号.次版本号.修订号

v2.5.2
│ │ │
│ │ └─ PATCH: Bug 修复
│ └─── MINOR: 新增功能（向下兼容）
└───── MAJOR: 重大更新（可能不兼容）
```

### 示例
```
v1.0.0 - 初始版本
v1.0.1 - Bug 修复
v1.1.0 - 新增功能
v2.0.0 - 重大更新
```

## 🔍 当前标签

```
v1.0.0
v1.0.1
v1.0.2
v1.0.3
v1.1.0
v2.5.2  ← 最新
```

## 🔗 快速链接

- **仓库**: https://github.com/pjkui/AdvancedClock
- **标签**: https://github.com/pjkui/AdvancedClock/tags
- **发布**: https://github.com/pjkui/AdvancedClock/releases

## 📚 详细文档

- [Git 标签管理指南](GIT_TAG_GUIDE.md)
- [v2.5.2 创建总结](TAG_v2.5.2_SUMMARY.md)

---

**快速参考** | **AdvancedClock** | **2025-12-20**
