using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace NetAF.MyGame
{
    public static class GameState
    {
        private static readonly string ActiveCasesPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Cases", "active_cases.json");
        private static readonly string CasesPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Cases", "cases.json");

        public static string CurrentCaseId { get; set; } = string.Empty;

        private class ActiveCasesFile
        {
            public List<string> Active { get; set; } = new();
        }

        private class CaseEntry
        {
            public string id { get; set; } = string.Empty;
            public string title { get; set; } = string.Empty;
            public string description { get; set; } = string.Empty;
            public string reward { get; set; } = string.Empty;
            public string region { get; set; } = string.Empty;
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

        /// <summary>
        /// Get titles of active cases by resolving IDs against cases.json.
        /// </summary>
        public static IReadOnlyList<string> GetActiveCaseTitles()
        {
            try
            {
                var activeIds = GetActiveCases();
                if (activeIds.Count == 0)
                    return new List<string>();

                var allCases = LoadAllCases();
                var titles = new List<string>();

                foreach (var id in activeIds)
                {
                    var match = allCases.FirstOrDefault(c => c.id == id);
                    titles.Add(match != null ? match.title : $"Case {id}");
                }

                return titles;
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Get full details of all active cases by resolving IDs against cases.json.
        /// </summary>
        public static IReadOnlyList<(string id, string title, string description, string reward, string region)> GetActiveCaseDetails()
        {
            try
            {
                var activeIds = GetActiveCases();
                if (activeIds.Count == 0)
                    return new List<(string, string, string, string, string)>();

                var allCases = LoadAllCases();
                var details = new List<(string, string, string, string, string)>();

                foreach (var id in activeIds)
                {
                    var match = allCases.FirstOrDefault(c => c.id == id);
                    if (match != null)
                        details.Add((match.id, match.title, match.description, match.reward, match.region));
                    else
                        details.Add((id, $"Case {id}", "Unknown case.", "Unknown", ""));
                }

                return details;
            }
            catch
            {
                return new List<(string, string, string, string, string)>();
            }
        }

        private static List<CaseEntry> LoadAllCases()
        {
            try
            {
                if (!File.Exists(CasesPath))
                    return new List<CaseEntry>();

                var json = File.ReadAllText(CasesPath);
                return JsonSerializer.Deserialize<List<CaseEntry>>(json) ?? new List<CaseEntry>();
            }
            catch
            {
                return new List<CaseEntry>();
            }
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
