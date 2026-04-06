using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace NetAF.MyGame
{
    public static class GameState
    {
        private static readonly string ActiveCasesPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Cases", "active_cases.json");

        public static string CurrentCaseId { get; set; } = string.Empty;

        private class ActiveCasesFile
        {
            public List<string> Active { get; set; } = new();
        }

        public static IReadOnlyList<string> GetActiveCases()
        {
            try
            {
                if (!File.Exists(ActiveCasesPath))
                {
                    SaveActiveCases(new List<string>());
                    return new List<string>();
                }

                var json = File.ReadAllText(ActiveCasesPath);
                var doc = JsonSerializer.Deserialize<ActiveCasesFile>(json) ?? new ActiveCasesFile();
                return doc.Active ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        public static void AddActiveCase(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            var list = new List<string>(GetActiveCases());
            if (!list.Contains(id))
            {
                list.Add(id);
                SaveActiveCases(list);
            }

            CurrentCaseId = id;
        }

        public static void ClearActiveCases()
        {
            SaveActiveCases(new List<string>());
            CurrentCaseId = string.Empty;
        }

        private static void SaveActiveCases(IEnumerable<string> ids)
        {
            try
            {
                var dir = Path.GetDirectoryName(ActiveCasesPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var obj = new ActiveCasesFile { Active = new List<string>(ids) };
                var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ActiveCasesPath, json);
            }
            catch
            {
                // ignore
            }
        }
    }
}
