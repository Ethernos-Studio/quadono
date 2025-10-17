using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Quadono
{
    /// <summary>
    /// 重要日服务类
    /// </summary>
    public static class ImportantDateService
    {
        private static readonly List<ImportantDate> _importantDates = InitializeImportantDates();
        private static readonly string _customImportantDatesFile = "custom_important_dates.json";
        
        /// <summary>
        /// 获取所有重要日
        /// </summary>
        public static List<ImportantDate> GetAllImportantDates()
        {
            var customDates = LoadCustomImportantDates();
            return _importantDates.Concat(customDates).ToList();
        }
        
        /// <summary>
        /// 按类型获取重要日
        /// </summary>
        public static List<ImportantDate> GetImportantDatesByType(ImportantDateType type)
        {
            return GetAllImportantDates().Where(d => d.Type == type).ToList();
        }
        
        /// <summary>
        /// 按重要级别获取重要日
        /// </summary>
        public static List<ImportantDate> GetImportantDatesByLevel(ImportanceLevel level)
        {
            return GetAllImportantDates().Where(d => d.Level == level).ToList();
        }
        
        /// <summary>
        /// 获取即将到来的重要日
        /// </summary>
        public static List<ImportantDate> GetUpcomingImportantDates(int daysAhead = 90)
        {
            var today = DateTime.Today;
            var upcoming = new List<ImportantDate>();
            
            foreach (var date in GetAllImportantDates())
            {
                var daysUntil = date.DaysUntil(today.Year);
                if (daysUntil <= daysAhead && daysUntil >= 0)
                {
                    upcoming.Add(date);
                }
            }
            
            return upcoming.OrderBy(d => d.GetDate(today.Year)).ToList();
        }
        
        /// <summary>
        /// 获取需要报名提醒的重要日
        /// </summary>
        public static List<ImportantDate> GetRegistrationReminders(int currentYear)
        {
            return GetAllImportantDates()
                .Where(d => d.ShouldShowRegistrationReminder(currentYear))
                .OrderBy(d => d.DaysUntil(currentYear))
                .ToList();
        }
        
        /// <summary>
        /// 显示重要日列表
        /// </summary>
        public static void ListImportantDates(ImportantDateType? type = null, ImportanceLevel? level = null)
        {
            var dates = GetAllImportantDates();
            
            if (type.HasValue)
                dates = dates.Where(d => d.Type == type.Value).ToList();
                
            if (level.HasValue)
                dates = dates.Where(d => d.Level == level.Value).ToList();
            
            var currentYear = DateTime.Today.Year;
            
            if (!dates.Any())
            {
                Console.WriteLine("没有找到重要日。");
                return;
            }
            
            var grouped = dates.GroupBy(d => d.Type).OrderBy(g => g.Key);
            
            foreach (var group in grouped)
            {
                Console.WriteLine($"【{group.Key.GetTypeDisplayName()}】");
                foreach (var date in group.OrderBy(d => d.Month).ThenBy(d => d.Day))
                {
                    var dateValue = date.GetDate(currentYear);
                    var daysUntil = date.DaysUntil(currentYear);
                    
                    Console.Write($"  {date.GetLevelSymbol()} {date.Name}");
                    
                    if (!string.IsNullOrEmpty(date.EnglishName))
                        Console.Write($" ({date.EnglishName})");
                    
                    Console.Write($" - {date.Month}月{date.Day}日");
                    
                    if (date.IsDateRange && date.DurationDays > 1)
                        Console.Write($"({date.DurationDays}天)");
                    
                    if (daysUntil == 0)
                        Console.Write(" 【今天】");
                    else if (daysUntil == 1)
                        Console.Write(" 【明天】");
                    else if (daysUntil > 0 && daysUntil <= 7)
                        Console.Write($" 【还有{daysUntil}天】");
                    
                    Console.Write($" - {date.GetLevelDisplayName()}");
                    
                    if (date.RequiresRegistration && date.RegistrationAdvanceDays > 0)
                        Console.Write($" (需提前{date.RegistrationAdvanceDays}天报名)");
                    
                    Console.WriteLine();
                    
                    if (!string.IsNullOrEmpty(date.Description))
                        Console.WriteLine($"    {date.Description}");
                    
                    if (!string.IsNullOrEmpty(date.Fee))
                        Console.WriteLine($"    费用: {date.Fee}");
                    
                    if (date.ShouldShowRegistrationReminder(currentYear))
                    {
                        Console.WriteLine($"    ⚠️  报名提醒: 还有{daysUntil - date.RegistrationAdvanceDays}天截止报名！");
                        if (!string.IsNullOrEmpty(date.RegistrationUrl))
                            Console.WriteLine($"    报名网址: {date.RegistrationUrl}");
                    }
                }
                Console.WriteLine();
            }
        }
        
        /// <summary>
        /// 显示即将到来的重要日
        /// </summary>
        public static void ShowUpcomingImportantDates(int daysAhead = 90)
        {
            var upcoming = GetUpcomingImportantDates(daysAhead);
            var today = DateTime.Today;
            
            var registrationReminders = GetRegistrationReminders(today.Year);
            
            if (!upcoming.Any() && !registrationReminders.Any())
            {
                Console.WriteLine($"未来{daysAhead}天内没有重要日。");
                return;
            }
            
            // 显示报名提醒
            if (registrationReminders.Any())
            {
                Console.WriteLine($"【报名提醒】");
                foreach (var date in registrationReminders.Take(5))
                {
                    var daysUntil = date.DaysUntil(today.Year);
                    Console.WriteLine($"  ⚠️  {date.Name} - 还有{daysUntil}天考试，报名即将截止！");
                    if (!string.IsNullOrEmpty(date.RegistrationUrl))
                        Console.WriteLine($"     报名: {date.RegistrationUrl}");
                }
                Console.WriteLine();
            }
            
            // 显示即将到来的重要日
            if (upcoming.Any())
            {
                Console.WriteLine($"【未来{daysAhead}天内的重要日】");
                foreach (var date in upcoming.Take(15))
                {
                    var daysUntil = date.DaysUntil(today.Year);
                    
                    Console.Write($"  {date.GetLevelSymbol()} {date.Month}月{date.Day}日 {date.Name}");
                    
                    if (daysUntil == 0)
                        Console.Write(" 【今天】");
                    else if (daysUntil == 1)
                        Console.Write(" 【明天】");
                    else
                        Console.Write($" 【还有{daysUntil}天】");
                    
                    Console.Write($" - {date.GetTypeDisplayName()}");
                    Console.Write($"({date.GetLevelDisplayName()})");
                    
                    if (date.RequiresRegistration && !date.ShouldShowRegistrationReminder(today.Year))
                        Console.Write(" ✅报名截止");
                    else if (date.ShouldShowRegistrationReminder(today.Year))
                        Console.Write(" ⏰报名中");
                    
                    Console.WriteLine();
                    
                    if (!string.IsNullOrEmpty(date.Fee))
                        Console.WriteLine($"     费用: {date.Fee}");
                }
            }
        }
        
        /// <summary>
        /// 添加自定义重要日
        /// </summary>
        public static void AddCustomImportantDate(string name, ImportantDateType type, ImportanceLevel level, 
            int month, int day, string? description = null, string? fee = null, 
            bool requiresRegistration = true, int registrationAdvanceDays = 30,
            string? registrationUrl = null, string? website = null, 
            bool isDateRange = false, int durationDays = 1, int? year = null)
        {
            var customDates = LoadCustomImportantDates();
            
            var importantDate = new ImportantDate
            {
                Name = name,
                Type = type,
                Level = level,
                Month = month,
                Day = day,
                Description = description ?? "",
                Fee = fee ?? "",
                RequiresRegistration = requiresRegistration,
                RegistrationAdvanceDays = registrationAdvanceDays,
                RegistrationUrl = registrationUrl ?? "",
                Website = website ?? "",
                IsDateRange = isDateRange,
                DurationDays = durationDays,
                Year = year
            };
            
            customDates.Add(importantDate);
            SaveCustomImportantDates(customDates);
            
            Console.WriteLine($"已添加自定义重要日：{name}");
            
            if (requiresRegistration && registrationAdvanceDays > 0)
            {
                Console.WriteLine($"提醒：需要提前{registrationAdvanceDays}天报名");
            }
        }
        
        /// <summary>
        /// 删除自定义重要日
        /// </summary>
        public static void RemoveCustomImportantDate(string name)
        {
            var customDates = LoadCustomImportantDates();
            var removed = customDates.RemoveAll(d => d.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            
            if (removed > 0)
            {
                SaveCustomImportantDates(customDates);
                Console.WriteLine($"已删除 {removed} 个自定义重要日。");
            }
            else
            {
                Console.WriteLine("未找到匹配的自定义重要日。");
            }
        }
        
        /// <summary>
        /// 搜索重要日
        /// </summary>
        public static void SearchImportantDates(string keyword)
        {
            var dates = GetAllImportantDates()
                .Where(d => d.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                           d.EnglishName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                           d.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                           d.Tags.Any(t => t.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            
            if (!dates.Any())
            {
                Console.WriteLine($"未找到包含\"{keyword}\"的重要日。");
                return;
            }
            
            Console.WriteLine($"【搜索结果：\"{keyword}\"】");
            var currentYear = DateTime.Today.Year;
            
            foreach (var date in dates.OrderBy(d => d.Month).ThenBy(d => d.Day))
            {
                var daysUntil = date.DaysUntil(currentYear);
                Console.Write($"  {date.GetLevelSymbol()} {date.Name} - {date.Month}月{date.Day}日");
                
                if (daysUntil >= 0 && daysUntil <= 30)
                {
                    if (daysUntil == 0)
                        Console.Write(" 【今天】");
                    else if (daysUntil == 1)
                        Console.Write(" 【明天】");
                    else
                        Console.Write($" 【还有{daysUntil}天】");
                }
                
                Console.WriteLine($" ({date.GetTypeDisplayName()} - {date.GetLevelDisplayName()})");
            }
        }
        
        /// <summary>
        /// 加载自定义重要日
        /// </summary>
        private static List<ImportantDate> LoadCustomImportantDates()
        {
            if (!File.Exists(_customImportantDatesFile))
                return new List<ImportantDate>();
            
            try
            {
                var json = File.ReadAllText(_customImportantDatesFile);
                return JsonSerializer.Deserialize<List<ImportantDate>>(json) ?? new List<ImportantDate>();
            }
            catch
            {
                return new List<ImportantDate>();
            }
        }
        
        /// <summary>
        /// 保存自定义重要日
        /// </summary>
        private static void SaveCustomImportantDates(List<ImportantDate> dates)
        {
            var json = JsonSerializer.Serialize(dates, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_customImportantDatesFile, json);
        }
        
        /// <summary>
        /// 初始化内置重要日数据
        /// </summary>
        private static List<ImportantDate> InitializeImportantDates()
        {
            return new List<ImportantDate>
            {
                // 中学考试
                new ImportantDate
                {
                    Name = "中考",
                    EnglishName = "Senior High School Entrance Examination",
                    Type = ImportantDateType.MiddleSchoolExam,
                    Level = ImportanceLevel.Critical,
                    Month = 6, Day = 15, // 通常在6月中旬，各地略有不同
                    Description = "初中毕业升学考试，决定高中录取",
                    IsDateRange = true, DurationDays = 3,
                    RequiresRegistration = true, RegistrationAdvanceDays = 60,
                    Fee = "各地不同，约100-200元",
                    Tags = new List<string> { "中考", "升学", "初中毕业" }
                },
                new ImportantDate
                {
                    Name = "高考",
                    EnglishName = "National College Entrance Examination (Gaokao)",
                    Type = ImportantDateType.MiddleSchoolExam,
                    Level = ImportanceLevel.Critical,
                    Month = 6, Day = 7,
                    Description = "普通高等学校招生全国统一考试",
                    IsDateRange = true, DurationDays = 3,
                    RequiresRegistration = true, RegistrationAdvanceDays = 90,
                    Fee = "约150元",
                    Website = "https://gaokao.chsi.com.cn/",
                    Tags = new List<string> { "高考", "大学", "升学" }
                },
                
                // 编程竞赛
                new ImportantDate
                {
                    Name = "CSP-J/S 2025初赛",
                    EnglishName = "CSP-J/S First Round",
                    Type = ImportantDateType.ProgrammingContest,
                    Level = ImportanceLevel.VeryImportant,
                    Month = 9, Day = 20,
                    Description = "CCF计算机软件能力认证，青少年编程竞赛初赛",
                    RequiresRegistration = true, RegistrationAdvanceDays = 30,
                    Fee = "J组：100元，S组：200元",
                    Website = "https://www.ccf.org.cn/",
                    RegistrationUrl = "https://cspsj.ccf.org.cn/",
                    Tags = new List<string> { "CSP", "编程", "信奥" }
                },
                new ImportantDate
                {
                    Name = "CSP-J/S 2025复赛",
                    EnglishName = "CSP-J/S Second Round",
                    Type = ImportantDateType.ProgrammingContest,
                    Level = ImportanceLevel.VeryImportant,
                    Month = 11, Day = 1,
                    Description = "CCF计算机软件能力认证，青少年编程竞赛复赛",
                    RequiresRegistration = false, // 初赛通过自动进入复赛
                    Website = "https://www.ccf.org.cn/",
                    Tags = new List<string> { "CSP", "编程", "信奥", "复赛" }
                },
                new ImportantDate
                {
                    Name = "NOIP",
                    EnglishName = "National Olympiad in Informatics in Provinces",
                    Type = ImportantDateType.ProgrammingContest,
                    Level = ImportanceLevel.Critical,
                    Month = 11, Day = 15,
                    Description = "全国青少年信息学奥林匹克联赛，省级比赛",
                    RequiresRegistration = true, RegistrationAdvanceDays = 45,
                    Fee = "约300元",
                    Website = "https://www.ccf.org.cn/",
                    Tags = new List<string> { "NOIP", "信息学", "奥赛", "省级" }
                },
                new ImportantDate
                {
                    Name = "NOI省选",
                    EnglishName = "NOI Provincial Selection",
                    Type = ImportantDateType.ProgrammingContest,
                    Level = ImportanceLevel.Critical,
                    Month = 4, Day = 10,
                    Description = "全国青少年信息学奥林匹克竞赛省队选拔",
                    RequiresRegistration = true, RegistrationAdvanceDays = 30,
                    Website = "https://www.ccf.org.cn/",
                    Tags = new List<string> { "NOI", "省选", "信息学", "国赛" }
                },
                new ImportantDate
                {
                    Name = "NOI",
                    EnglishName = "National Olympiad in Informatics",
                    Type = ImportantDateType.ProgrammingContest,
                    Level = ImportanceLevel.Critical,
                    Month = 7, Day = 20,
                    Description = "全国青少年信息学奥林匹克竞赛，国家级比赛",
                    IsDateRange = true, DurationDays = 7,
                    RequiresRegistration = true, RegistrationAdvanceDays = 60,
                    Website = "https://www.ccf.org.cn/",
                    Tags = new List<string> { "NOI", "信息学", "奥赛", "国赛", "集训队" }
                },
                new ImportantDate
                {
                    Name = "IOI中国代表队选拔",
                    EnglishName = "IOI China Team Selection",
                    Type = ImportantDateType.ProgrammingContest,
                    Level = ImportanceLevel.Critical,
                    Month = 5, Day = 1,
                    Description = "国际信息学奥林匹克竞赛中国代表队选拔",
                    RequiresRegistration = true, RegistrationAdvanceDays = 90,
                    Website = "https://www.ccf.org.cn/",
                    Tags = new List<string> { "IOI", "国际奥赛", "国家队", "选拔" }
                },
                
                // 学科竞赛
                new ImportantDate
                {
                    Name = "数学竞赛初赛",
                    EnglishName = "Mathematics Competition First Round",
                    Type = ImportantDateType.SubjectContest,
                    Level = ImportanceLevel.Important,
                    Month = 9, Day = 10,
                    Description = "全国高中数学联赛初赛",
                    RequiresRegistration = true, RegistrationAdvanceDays = 30,
                    Fee = "约100元",
                    Tags = new List<string> { "数学", "竞赛", "五大学科" }
                },
                new ImportantDate
                {
                    Name = "数学竞赛复赛",
                    EnglishName = "Mathematics Competition Second Round",
                    Type = ImportantDateType.SubjectContest,
                    Level = ImportanceLevel.Critical,
                    Month = 10, Day = 15,
                    Description = "全国高中数学联赛复赛",
                    RequiresRegistration = false, // 初赛通过自动进入
                    Tags = new List<string> { "数学", "竞赛", "复赛", "五大学科" }
                },
                new ImportantDate
                {
                    Name = "物理竞赛初赛",
                    EnglishName = "Physics Competition First Round",
                    Type = ImportantDateType.SubjectContest,
                    Level = ImportanceLevel.Important,
                    Month = 9, Day = 5,
                    Description = "全国中学生物理竞赛初赛",
                    RequiresRegistration = true, RegistrationAdvanceDays = 30,
                    Fee = "约100元",
                    Tags = new List<string> { "物理", "竞赛", "五大学科" }
                },
                new ImportantDate
                {
                    Name = "物理竞赛复赛",
                    EnglishName = "Physics Competition Second Round",
                    Type = ImportantDateType.SubjectContest,
                    Level = ImportanceLevel.Critical,
                    Month = 9, Day = 25,
                    Description = "全国中学生物理竞赛复赛（理论+实验）",
                    RequiresRegistration = false,
                    IsDateRange = true, DurationDays = 2,
                    Tags = new List<string> { "物理", "竞赛", "复赛", "五大学科" }
                },
                new ImportantDate
                {
                    Name = "化学竞赛初赛",
                    EnglishName = "Chemistry Competition First Round",
                    Type = ImportantDateType.SubjectContest,
                    Level = ImportanceLevel.Important,
                    Month = 8, Day = 30,
                    Description = "中国化学奥林匹克初赛",
                    RequiresRegistration = true, RegistrationAdvanceDays = 30,
                    Fee = "约100元",
                    Tags = new List<string> { "化学", "竞赛", "五大学科" }
                },
                new ImportantDate
                {
                    Name = "化学竞赛复赛",
                    EnglishName = "Chemistry Competition Second Round",
                    Type = ImportantDateType.SubjectContest,
                    Level = ImportanceLevel.Critical,
                    Month = 10, Day = 5,
                    Description = "中国化学奥林匹克复赛",
                    RequiresRegistration = false,
                    Tags = new List<string> { "化学", "竞赛", "复赛", "五大学科" }
                },
                new ImportantDate
                {
                    Name = "生物竞赛初赛",
                    EnglishName = "Biology Competition First Round",
                    Type = ImportantDateType.SubjectContest,
                    Level = ImportanceLevel.Important,
                    Month = 4, Day = 20,
                    Description = "全国中学生生物学竞赛初赛",
                    RequiresRegistration = true, RegistrationAdvanceDays = 30,
                    Fee = "约100元",
                    Tags = new List<string> { "生物", "竞赛", "五大学科" }
                },
                new ImportantDate
                {
                    Name = "生物竞赛复赛",
                    EnglishName = "Biology Competition Second Round",
                    Type = ImportantDateType.SubjectContest,
                    Level = ImportanceLevel.Critical,
                    Month = 5, Day = 15,
                    Description = "全国中学生生物学竞赛复赛",
                    RequiresRegistration = false,
                    Tags = new List<string> { "生物", "竞赛", "复赛", "五大学科" }
                },
                
                // 语言考试
                new ImportantDate
                {
                    Name = "英语四级",
                    EnglishName = "CET-4",
                    Type = ImportantDateType.LanguageExam,
                    Level = ImportanceLevel.Important,
                    Month = 6, Day = 15,
                    Description = "大学英语四级考试",
                    RequiresRegistration = true, RegistrationAdvanceDays = 60,
                    Fee = "约30元",
                    Website = "http://cet.neea.edu.cn/",
                    Tags = new List<string> { "英语", "四级", "CET", "大学" }
                },
                new ImportantDate
                {
                    Name = "英语六级",
                    EnglishName = "CET-6",
                    Type = ImportantDateType.LanguageExam,
                    Level = ImportanceLevel.Important,
                    Month = 6, Day = 15,
                    Description = "大学英语六级考试",
                    RequiresRegistration = true, RegistrationAdvanceDays = 60,
                    Fee = "约35元",
                    Website = "http://cet.neea.edu.cn/",
                    Tags = new List<string> { "英语", "六级", "CET", "大学" }
                },
                new ImportantDate
                {
                    Name = "雅思",
                    EnglishName = "IELTS",
                    Type = ImportantDateType.LanguageExam,
                    Level = ImportanceLevel.VeryImportant,
                    Month = 1, Day = 1, // 每月多次，这里只显示提醒
                    Description = "雅思考试，每月多次考试",
                    IsAnnualFixed = false, // 不是固定年度日期
                    RequiresRegistration = true, RegistrationAdvanceDays = 30,
                    Fee = "2170元",
                    Website = "https://www.ielts.org/",
                    RegistrationUrl = "https://ielts.neea.cn/",
                    Tags = new List<string> { "雅思", "IELTS", "留学", "英语" }
                },
                new ImportantDate
                {
                    Name = "托福",
                    EnglishName = "TOEFL",
                    Type = ImportantDateType.LanguageExam,
                    Level = ImportanceLevel.VeryImportant,
                    Month = 1, Day = 1, // 每月多次
                    Description = "托福考试，每月多次考试",
                    IsAnnualFixed = false,
                    RequiresRegistration = true, RegistrationAdvanceDays = 30,
                    Fee = "2100元",
                    Website = "https://www.ets.org/toefl",
                    RegistrationUrl = "https://toefl.neea.cn/",
                    Tags = new List<string> { "托福", "TOEFL", "留学", "英语" }
                }
            };
        }
    }
}