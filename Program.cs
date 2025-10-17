using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Quadono
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            // 如果没有任何参数，直接打印用法
            if (args.Length == 0)
            {
                PrintUsage();
                return 0;
            }

            // 子命令需要后台服务时，统一走 Host；纯查询可立即返回
            switch (args[0].ToLowerInvariant())
            {
                case "add":
                    if (args.Length < 4) goto default;
                    TaskStore.Add(args[1], int.Parse(args[2]), int.Parse(args[3]));
                    Console.WriteLine("任务已添加");
                    return 0;

                case "list":
                    TaskStore.List();
                    return 0;

                case "done":
                    if (args.Length < 2) goto default;
                    TaskStore.Done(args[1]);
                    Console.WriteLine("任务已标记完成");
                    return 0;

                case "del":
                    if (args.Length < 2) goto default;
                    TaskStore.Del(args[1]);
                    Console.WriteLine("任务已删除");
                    return 0;

                case "pom":
                case "pomodoro":
                case "25":
                    return await RunHostAsync(() => PomodoroService.Start(args.Length >= 3 && args[1] == "--task" ? args[2] : null));

                case "alarm":
                    if (args.Length < 3) goto default;
                    return await RunHostAsync(() => AlarmService.Set(args[1], string.Join(' ', args[2..])));

                case "holidays":
                case "holiday":
                case "节日":
                    return HandleHolidayCommand(args);

                case "important":
                case "dates":
                case "重要日":
                case "考试":
                    return HandleImportantDateCommand(args);

                default:
                    Console.WriteLine($"未知命令：{args[0]}");
                    PrintUsage();
                    return 1;
            }
        }

        static async Task<int> RunHostAsync(Action backgroundWork)
        {
            using var host = Host.CreateDefaultBuilder()
                .ConfigureServices(svc =>
                {
                    svc.AddSingleton<IHostedService>(_ => new QuadonoHost(backgroundWork));
                })
                .Build();
            await host.RunAsync();
            return 0;
        }

        static void PrintUsage()
        {
            Console.WriteLine("quadono β0.2.0 — Ethernos Studio");
            Console.WriteLine("用法：");
            Console.WriteLine("  quadono add <标题> <象限1-4> <预估分钟>");
            Console.WriteLine("  quadono list                 列出全部任务");
            Console.WriteLine("  quadono done <guid>          标记完成");
            Console.WriteLine("  quadono del <guid>           删除任务");
            Console.WriteLine("  quadono pom --task <guid>    绑定任务番茄钟");
            Console.WriteLine("  quadono alarm 09:00 \"Stand-up\"  设置闹钟");
            Console.WriteLine("  quadono holidays             显示所有节日");
            Console.WriteLine("  quadono holidays upcoming    显示即将到来的节日");
            Console.WriteLine("  quadono holidays add <名称> <类型> <月> <日> [描述] 添加自定义节日");
            Console.WriteLine("  quadono holidays del <名称>  删除自定义节日");
            //Console.WriteLine("  quadono important            显示所有重要日");
            //Console.WriteLine("  quadono important upcoming   显示即将到来的重要日");
            //Console.WriteLine("  quadono important add <名称> <类型> <级别> <月> <日> [描述] 添加重要日");
            //Console.WriteLine("  quadono important search <关键词> 搜索重要日");
            //Console.WriteLine("  quadono important del <名称> 删除重要日");
            //Console.WriteLine("  节日类型: international, chinese, folk, community");
            //Console.WriteLine("  重要日类型: exam, contest, language, professional, admission, other");
            //Console.WriteLine("  重要级别: normal, important, veryimportant, critical");
        }

        static int HandleHolidayCommand(string[] args)
        {
            if (args.Length == 1)
            {
                // 显示所有节日
                HolidayService.ListHolidays();
                return 0;
            }

            switch (args[1].ToLowerInvariant())
            {
                case "upcoming":
                case "coming":
                case "近期":
                    int daysAhead = 30;
                    if (args.Length >= 3 && int.TryParse(args[2], out int days))
                        daysAhead = days;
                    HolidayService.ShowUpcomingHolidays(daysAhead);
                    return 0;

                case "add":
                case "添加":
                    if (args.Length < 6)
                    {
                        Console.WriteLine("用法: quadono holidays add <名称> <类型> <月> <日> [描述]");
                        Console.WriteLine("节日类型: international, chinese, folk, community");
                        return 1;
                    }
                    
                    if (!Enum.TryParse<HolidayType>(args[3], true, out var type))
                    {
                        Console.WriteLine($"无效的节日类型: {args[3]}");
                        Console.WriteLine("可用类型: international, chinese, folk, community");
                        return 1;
                    }
                    
                    if (!int.TryParse(args[4], out int month) || month < 1 || month > 12)
                    {
                        Console.WriteLine($"无效的月份: {args[4]}");
                        return 1;
                    }
                    
                    if (!int.TryParse(args[5], out int day) || day < 1 || day > 31)
                    {
                        Console.WriteLine($"无效的日期: {args[5]}");
                        return 1;
                    }
                    
                    string description = args.Length >= 7 ? string.Join(" ", args[6..]) : "";
                    HolidayService.AddCustomHoliday(args[2], type, month, day, description);
                    return 0;

                case "del":
                case "delete":
                case "删除":
                    if (args.Length < 3)
                    {
                        Console.WriteLine("用法: quadono holidays del <名称>");
                        return 1;
                    }
                    HolidayService.RemoveCustomHoliday(args[2]);
                    return 0;

                case "international":
                case "国际":
                    HolidayService.ListHolidays(HolidayType.International);
                    return 0;

                case "chinese":
                case "中国":
                    HolidayService.ListHolidays(HolidayType.Chinese);
                    return 0;

                case "folk":
                case "民间":
                    HolidayService.ListHolidays(HolidayType.Folk);
                    return 0;

                case "community":
                case "社区":
                    HolidayService.ListHolidays(HolidayType.Community);
                    return 0;

                default:
                    Console.WriteLine($"未知的节日命令: {args[1]}");
                    Console.WriteLine("可用命令: upcoming, add, del, international, chinese, folk, community");
                    return 1;
            }
        }

        static int HandleImportantDateCommand(string[] args)
        {
            if (args.Length == 1)
            {
                // 显示所有重要日
                ImportantDateService.ListImportantDates();
                return 0;
            }

            switch (args[1].ToLowerInvariant())
            {
                case "upcoming":
                case "coming":
                case "近期":
                    int daysAhead = 90;
                    if (args.Length >= 3 && int.TryParse(args[2], out int days))
                        daysAhead = days;
                    ImportantDateService.ShowUpcomingImportantDates(daysAhead);
                    return 0;

                case "add":
                case "添加":
                    if (args.Length < 7)
                    {
                        Console.WriteLine("用法: quadono important add <名称> <类型> <级别> <月> <日> [描述]");
                        Console.WriteLine("重要日类型: exam, contest, language, professional, admission, other");
                        Console.WriteLine("重要级别: normal, important, veryimportant, critical");
                        return 1;
                    }
                    
                    if (!Enum.TryParse<ImportantDateType>(args[3], true, out var dateType))
                    {
                        Console.WriteLine($"无效的重要日类型: {args[3]}");
                        Console.WriteLine("可用类型: exam, contest, language, professional, admission, other");
                        return 1;
                    }
                    
                    if (!Enum.TryParse<ImportanceLevel>(args[4], true, out var level))
                    {
                        Console.WriteLine($"无效的重要级别: {args[4]}");
                        Console.WriteLine("可用级别: normal, important, veryimportant, critical");
                        return 1;
                    }
                    
                    if (!int.TryParse(args[5], out int month) || month < 1 || month > 12)
                    {
                        Console.WriteLine($"无效的月份: {args[5]}");
                        return 1;
                    }
                    
                    if (!int.TryParse(args[6], out int day) || day < 1 || day > 31)
                    {
                        Console.WriteLine($"无效的日期: {args[6]}");
                        return 1;
                    }
                    
                    string description = args.Length >= 8 ? string.Join(" ", args[7..]) : "";
                    ImportantDateService.AddCustomImportantDate(name: args[2], type: dateType, level: level, 
                        month: month, day: day, description: description);
                    return 0;

                case "del":
                case "delete":
                case "删除":
                    if (args.Length < 3)
                    {
                        Console.WriteLine("用法: quadono important del <名称>");
                        return 1;
                    }
                    ImportantDateService.RemoveCustomImportantDate(args[2]);
                    return 0;

                case "search":
                case "搜索":
                case "find":
                    if (args.Length < 3)
                    {
                        Console.WriteLine("用法: quadono important search <关键词>");
                        return 1;
                    }
                    string keyword = string.Join(" ", args[2..]);
                    ImportantDateService.SearchImportantDates(keyword);
                    return 0;

                case "exam":
                case "考试":
                    ImportantDateService.ListImportantDates(type: ImportantDateType.MiddleSchoolExam);
                    return 0;

                case "contest":
                case "竞赛":
                    ImportantDateService.ListImportantDates(type: ImportantDateType.ProgrammingContest);
                    return 0;

                case "language":
                case "语言":
                    ImportantDateService.ListImportantDates(type: ImportantDateType.LanguageExam);
                    return 0;

                case "critical":
                case "极其重要":
                    ImportantDateService.ListImportantDates(level: ImportanceLevel.Critical);
                    return 0;

                case "registration":
                case "报名":
                    ImportantDateService.ShowUpcomingImportantDates(365); // 显示全年需要报名的考试
                    return 0;

                default:
                    Console.WriteLine($"未知的重要日命令: {args[1]}");
                    Console.WriteLine("可用命令: upcoming, add, del, search, exam, contest, language, critical, registration");
                    return 1;
            }
        }
    }
}