using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Quadono
{
    public class TaskItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "";
        public int Quadrant { get; set; }
        public int EstimateMinutes { get; set; }
        public bool Done { get; set; }
    }

    public static class TaskStore
    {
        static readonly string FileName = "quadono.json";
        static List<TaskItem> _cache = new();
        static readonly object _lock = new();

        static TaskStore()
        {
            if (File.Exists(FileName))
            {
                var json = File.ReadAllText(FileName);
                _cache = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new();
            }
        }

        static void Save()
        {
            lock (_lock)
            {
                File.WriteAllText(FileName, JsonSerializer.Serialize(_cache, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        public static void Add(string title, int quadrant, int minutes)
        {
            lock (_lock) _cache.Add(new TaskItem { Title = title, Quadrant = quadrant, EstimateMinutes = minutes });
            Save();
        }

        public static void List()
        {
            // 重新加载，保证与磁盘一致
            lock (_lock)
            {
                if (File.Exists(FileName))
                    _cache = JsonSerializer.Deserialize<List<TaskItem>>(File.ReadAllText(FileName)) ?? new();
                else
                    _cache = new();

                var groups = _cache.Where(t => !t.Done)
                                   .GroupBy(t => t.Quadrant).OrderBy(g => g.Key);
                foreach (var g in groups)
                {
                    Console.WriteLine($"【第 {g.Key} 象限】");
                    foreach (var t in g)
                        Console.WriteLine($"  {t.Id[..8]}  {t.Title}  估{t.EstimateMinutes}分钟");
                }
            }
        }

        public static void Done(string id)
        {
            lock (_lock)
            {
                var t = _cache.FirstOrDefault(x =>
                    x.Id.StartsWith(id, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(x.Title, id, StringComparison.OrdinalIgnoreCase));
                if (t != null) t.Done = true;
            }
            Save();
        }

        public static void Del(string id)
        {
            lock (_lock)
            {
                _cache.RemoveAll(x =>
                    x.Id.StartsWith(id, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(x.Title, id, StringComparison.OrdinalIgnoreCase));
            }
            Save();
        }

        public static TaskItem? Find(string id)
        {
            lock (_lock)
                return _cache.FirstOrDefault(x =>
                         x.Id.StartsWith(id, StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(x.Title, id, StringComparison.OrdinalIgnoreCase));
        }
    }
}