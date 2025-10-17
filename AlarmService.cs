using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Quadono
{
    public static class AlarmService
    {
        static readonly string AlarmFile = "alarms.txt";

        public static void Set(string time, string note)
        {
            File.AppendAllText(AlarmFile, $"{time}|{note}\n");
            Console.WriteLine($"已设置闹钟 {time}  {note}");
        }

        // 阻塞运行，供宿主线程调用
        public static void Run(CancellationToken token)
        {
            if (!File.Exists(AlarmFile))
                File.WriteAllText(AlarmFile, "");

            while (!token.IsCancellationRequested)
            {
                CheckAlarms();
                Thread.Sleep(1000);
            }
        }

        static void CheckAlarms()
        {
            var now = DateTime.Now.ToString("HH:mm");
            var lines = File.ReadAllLines(AlarmFile).ToList();
            bool changed = false;

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var sp = lines[i].Split('|');
                if (sp.Length == 2 && sp[0] == now)
                {
                    Console.WriteLine($"\n【闹钟】{sp[1]}");
                    try { Console.Beep(1000, 500); } catch { }
                    lines.RemoveAt(i);
                    changed = true;
                }
            }

            if (changed)
                File.WriteAllLines(AlarmFile, lines);
        }
    }
}