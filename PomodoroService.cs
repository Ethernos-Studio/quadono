using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Quadono
{
    public static class PomodoroService
    {
        public static async Task Start(string? taskId)
        {
            var task = taskId == null ? null : TaskStore.Find(taskId);
            if (taskId != null && task == null)
            {
                Console.WriteLine("未找到指定任务");
                return;
            }

            int work = 25 * 60, brk = 5 * 60;
            Console.WriteLine($"【番茄钟】工作 25 分钟，任务：{task?.Title ?? "无"}");
            await CountDown(work, "工作");

            Console.WriteLine("【番茄钟】休息 5 分钟");
            await CountDown(brk, "休息");

            if (task != null)
            {
                TaskStore.Done(task.Id);
                Console.WriteLine("任务已自动标记完成");
            }

            // 写历史日志
            File.AppendAllText("history.log",
                $"{DateTime.Now:yyyy-MM-dd HH:mm} 完成任务 {task?.Title ?? "无"} {work / 60} 分钟\n");
        }

        static async Task CountDown(int seconds, string phase)
        {
            var end = DateTime.Now.AddSeconds(seconds);
            while (DateTime.Now < end)
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    Console.WriteLine("用户提前终止");
                    return;
                }
                var left = end - DateTime.Now;
                Console.Write($"\r剩余 {left:mm\\:ss}  ");
                await Task.Delay(1000);
            }
            Console.WriteLine();
            Beep();
        }

        static void Beep()
        {
            try { Console.Beep(800, 300); Console.Beep(1000, 300); } catch { }
        }
    }
}