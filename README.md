# Quadono - 四象限任务管理工具

<div align="center">

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Version](https://img.shields.io/badge/version-β0.3.0-orange.svg)](CHANGELOG.md)

**基于四象限法则的现代化任务管理工具，集成番茄工作法、节日管理和重要日提醒**

[English](#english) | 中文

</div>

## 🌟 功能特性

### 📋 四象限任务管理
- **艾森豪威尔矩阵**：按重要性和紧急性科学分类任务
- **智能任务追踪**：支持任务完成历史记录和时间预估
- **灵活任务操作**：添加、删除、完成、查询一站式管理
- **数据持久化**：JSON格式存储，确保数据安全可靠

### 🍅 番茄工作法集成
- **标准番茄钟**：25分钟专注工作 + 5分钟休息
- **任务绑定**：番茄钟可与具体任务关联
- **智能提醒**：倒计时显示和完成提醒
- **历史记录**：自动生成工作效率报告

### 🔔 闹钟提醒系统
- **定时提醒**：基于时间的智能提醒功能
- **后台监控**：持续检查并自动触发闹钟
- **灵活配置**：支持自定义提醒内容和频率

### 🎉 节日管理系统
- **丰富节日库**：内置30+国际节日、中国传统节日、民间节日
- **农历支持**：智能农历转换和农历节日计算
- **自定义节日**：支持添加个人或企业特殊节日
- **智能提醒**：即将到来的节日提前通知

### 📅 重要日管理
- **考试日历**：内置中考、高考、各类竞赛信息
- **报名提醒**：智能报名截止日期提醒
- **多维分类**：按类型和重要程度科学分类
- **费用信息**：提供考试费用和报名网址

## 🚀 快速开始

### 环境要求
- **.NET 8.0 SDK** 或更高版本
- **Windows/Linux/macOS** 操作系统
- **1GB** 可用内存
- **50MB** 磁盘空间

### 安装步骤

#### 方式一：直接下载
```bash
# 下载最新版本
git clone https://github.com/dhjs0000/quadono.git
cd quadono

# 构建项目
dotnet build -c Release

# 运行程序
dotnet run -- help
```

#### 方式二：发布版本
```bash
# 发布为独立可执行文件
dotnet publish -c Release -r win-x64 --self-contained

# 单文件发布
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

### 基本用法

#### 任务管理
```bash
# 添加任务（象限1-4，预估时间分钟）
quadono add "完成项目报告" 1 120
quadono add "回复邮件" 2 15
quadono add "团队会议" 3 60
quadono add "整理桌面" 4 10

# 查看任务列表
quadono list

# 标记任务完成
quadono done <任务ID>

# 删除任务
quadono del <任务ID>
```

#### 番茄工作法
```bash
# 开始标准番茄钟
quadono pom

# 绑定任务的番茄钟
quadono pom --task <任务ID>
```

#### 节日管理
```bash
# 查看所有节日
quadono holidays

# 查看即将到来的节日（默认30天）
quadono holidays upcoming
quadono holidays upcoming 60  # 未来60天

# 按类型查看节日
quadono holidays international  # 国际节日
quadono holidays chinese        # 中国节日
quadono holidays folk          # 民间节日
quadono holidays community     # 社区节日

# 管理自定义节日
quadono holidays add "公司周年庆" community 6 18 "成立纪念日"
quadono holidays del "公司周年庆"
```

#### 重要日管理
```bash
# 查看所有重要日
quadono important

# 查看即将到来的重要日
quadono important upcoming
quadono important upcoming 60  # 未来60天

# 按类型筛选
quadono important exam      # 考试类
quadono important contest   # 竞赛类
quadono important language  # 语言考试
quadono important critical  # 极其重要

# 搜索重要日
quadono important search 高考
quadono important search CSP

# 管理自定义重要日
quadono important add "数学建模比赛" contest veryimportant 9 10 "全国大学生数学建模竞赛"
quadono important del "数学建模比赛"
```

#### 闹钟功能
```bash
# 设置闹钟
quadono alarm 09:00 "晨会提醒"
quadono alarm 14:30 "下午茶时间"
quadono alarm 18:00 "下班提醒"
```

## 📊 四象限法则说明

根据艾森豪威尔矩阵，任务按重要性和紧急性分为四个象限：

| 象限 | 特征 | 处理策略 | 示例 |
|------|------|----------|------|
| **第一象限** | 重要且紧急 | 立即执行 | 项目截止日期、紧急客户投诉 |
| **第二象限** | 重要不紧急 | 计划执行 | 学习新技能、健康检查、关系维护 |
| **第三象限** | 不重要但紧急 | 委托他人 | 某些会议、临时请求 |
| **第四象限** | 不重要不紧急 | 减少或消除 | 无意义社交、过度娱乐 |

## 🎯 最佳实践

### 任务管理建议
1. **每日回顾**：每天开始和结束时查看任务列表
2. **合理分类**：准确判断任务的重要性和紧急性
3. **时间预估**：为任务设置合理的完成时间
4. **优先级调整**：根据实际情况动态调整任务象限

### 番茄工作法使用技巧
1. **专注单一任务**：每个番茄钟只处理一个任务
2. **避免干扰**：番茄钟期间关闭不必要的通知
3. **适当休息**：严格遵守休息时间，让大脑放松
4. **记录分析**：定期查看番茄钟历史，优化工作模式

### 节日和重要日管理
1. **提前规划**：利用节日和重要日提醒功能提前安排
2. **自定义扩展**：根据个人需求添加特殊日期
3. **报名提醒**：关注需要报名的重要考试和竞赛
4. **费用准备**：提前了解相关费用并做好准备

## 🛠️ 技术架构

### 核心技术栈
- **.NET 8.0** - 跨平台运行时和框架
- **C# 12** - 现代编程语言
- **System.Text.Json** - 高性能JSON处理
- **Microsoft.Extensions.Hosting** - 后台服务托管

### 项目结构
```
quadono/
├── Program.cs              # 主程序入口和命令行处理
├── TaskStore.cs            # 任务存储和管理
├── PomodoroService.cs      # 番茄工作法服务
├── AlarmService.cs         # 闹钟服务
├── HolidayService.cs       # 节日管理服务
├── HolidayModels.cs        # 节日数据模型
├── ImportantDateService.cs # 重要日管理服务
├── ImportantDateModels.cs  # 重要日数据模型
├── QuadonoHost.cs          # 后台服务宿主
├── custom_holidays.json    # 自定义节日存储
├── custom_important_dates.json # 自定义重要日存储
└── quadono.json           # 任务数据存储
```

### 数据存储
- **任务数据**：`quadono.json` - 持久化任务信息
- **历史记录**：`history.log` - 番茄钟完成历史
- **闹钟设置**：`alarms.txt` - 闹钟配置
- **自定义节日**：`custom_holidays.json` - 用户自定义节日
- **自定义重要日**：`custom_important_dates.json` - 用户自定义重要日

## 🎨 界面预览

### 任务列表显示
```
📋 四象限任务列表
══════════════════════════════════════

🔥 第一象限 - 重要且紧急
├─ [1] 完成项目报告 (预估: 120分钟)
└─ [2] 处理客户投诉 (预估: 30分钟)

📈 第二象限 - 重要不紧急
├─ [3] 学习新技术 (预估: 60分钟)
└─ [4] 制定年度计划 (预估: 90分钟)

📞 第三象限 - 不重要但紧急
├─ [5] 参加周例会 (预估: 45分钟)
└─ [6] 回复常规邮件 (预估: 15分钟)

🎮 第四象限 - 不重要不紧急
├─ [7] 整理文件资料 (预估: 20分钟)
└─ [8] 更新社交媒体 (预估: 10分钟)
```

### 番茄工作法界面
```
🍅 番茄工作法开始！
══════════════════════════════════════
任务：学习新技术
状态：工作中 (25:00)
进度：████████████████████████

按 Ctrl+C 提前结束...
```

## 🔧 开发指南

### 环境搭建
```bash
# 克隆项目
git clone https://github.com/dhjs0000/quadono.git
cd quadono

# 还原依赖
dotnet restore

# 构建项目
dotnet build

# 运行测试
dotnet test
```

### 代码规范
- **命名约定**：采用PascalCase和camelCase标准
- **代码风格**：使用隐式全局using和可空引用类型
- **注释规范**：中文注释，注重解释"为什么"而非"是什么"
- **错误处理**：完善的异常处理和用户友好的错误提示

### 贡献指南
1. **Fork项目**到个人仓库
2. **创建功能分支** (`git checkout -b feature/AmazingFeature`)
3. **提交更改** (`git commit -m 'Add some AmazingFeature'`)
4. **推送到分支** (`git push origin feature/AmazingFeature`)
5. **创建Pull Request**

## 📈 版本历史

### β0.3.0 (当前版本)
- ✅ 新增重要日管理系统
- ✅ 内置30+考试和竞赛信息
- ✅ 四级重要程度分类
- ✅ 智能报名提醒功能
- ✅ 支持考试费用和报名网址信息
- ✅ 多维度搜索和筛选功能

### β0.2.0
- ✅ 新增节日管理系统
- ✅ 支持农历节日转换
- ✅ 自定义节日功能
- ✅ 即将到来的节日提醒
- ✅ 中英文节日名称对照

### β0.1.0
- ✅ 基础四象限任务管理
- ✅ 番茄工作法集成
- ✅ 闹钟提醒功能
- ✅ 任务数据持久化

## 🤝 贡献者

感谢所有为Quadono项目做出贡献的开发者：

- **Ethernos Studio** - 核心开发团队
- **社区贡献者** - 功能建议和问题反馈

## 📄 许可证

本项目采用 **MIT许可证** 开源，详见 [LICENSE](LICENSE) 文件。

## 🆘 支持与帮助

### 常见问题
**Q: 数据文件存储在哪里？**
A: 数据文件存储在程序运行目录下，包括quadono.json、history.log等。

**Q: 如何备份我的数据？**
A: 定期备份运行目录下的.json和.log文件即可。

**Q: 支持多语言吗？**
A: 目前支持中文界面，节日和重要日支持中英文显示。

### 获取帮助
- 📧 **邮件支持**：通过GitHub Issues提交问题
- 💬 **社区讨论**：参与GitHub Discussions
- 📖 **文档查看**：查看项目Wiki文档

### 报告问题
如发现Bug或有功能建议，请在 [GitHub Issues](https://github.com/dhjs0000/quadono/issues) 页面提交。

---

<div align="center">

**⭐ 如果这个项目对你有帮助，请给个Star支持一下！**

Made with ❤️ by [Ethernos Studio](https://github.com/dhjs0000)

</div>

---

## English

# Quadono - Quadrant Task Management Tool

A modern task management tool based on the Eisenhower Matrix (Important-Urgent quadrants), integrated with Pomodoro Technique, holiday management, and important date reminders.

## Key Features

- **Quadrant Task Management**: Scientifically categorize tasks by importance and urgency
- **Pomodoro Integration**: 25-minute focused work + 5-minute break cycles
- **Holiday Management**: Built-in database of 30+ holidays with lunar calendar support
- **Important Date Tracking**: Exam calendars and competition information with registration reminders
- **Smart Reminders**: Intelligent notifications for upcoming events and deadlines

## Quick Start

```bash
# Clone and build
git clone https://github.com/dhjs0000/quadono.git
cd quadono
dotnet build -c Release

# Run
./quadono add "Complete project report" 1 120
./quadono list
./quadono pom
```

See the Chinese section above for detailed usage instructions and examples.