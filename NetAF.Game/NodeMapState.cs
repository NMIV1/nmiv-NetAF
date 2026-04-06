using NetAF.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace NetAF.MyGame
{
    /// <summary>
    /// Truth-table style dependency matrix for tracking game state.
    /// Maintains base facts (directly set/cleared) and derived facts (auto-computed from rules).
    /// Derived facts are recomputed whenever base facts change, so they become false when
    /// their prerequisites are removed.
    /// </summary>
    public static class NodeMapState
    {
        #region Constants

        // Object discovery flags (base facts — set when a skill check passes, permanent)
        public const string ObjAFound = "OBJ_A_FOUND";
        public const string ObjBFound = "OBJ_B_FOUND";
        public const string ObjCFound = "OBJ_C_FOUND";
        public const string ObjDFound = "OBJ_D_FOUND";
        public const string ObjEFound = "OBJ_E_FOUND";

        // Evidence possession flags (base facts — set when player takes, cleared when player drops)
        public const string HasEvidenceHardDrive = "HAS_EVIDENCE_HARDDRIVE";
        public const string HasEvidenceVR = "HAS_EVIDENCE_VR";
        public const string HasEvidenceShoes = "HAS_EVIDENCE_SHOES";
        public const string HasEvidenceReceipts = "HAS_EVIDENCE_RECEIPTS";
        public const string HasEvidenceServer = "HAS_EVIDENCE_SERVER";

        // Derived connection flags (auto-computed from OBJ_* prerequisites)
        public const string ConnLexicon = "CONN_LEXICON";
        public const string ConnHaunting = "CONN_HAUNTING";
        public const string ConnPhantomDate = "CONN_PHANTOM_DATE";

        #endregion

        #region Fields

        // Base facts: directly set or cleared by game events
        private static readonly HashSet<string> baseFacts = [];

        // Derived facts: recomputed from connectionRules whenever baseFacts change
        private static readonly HashSet<string> derivedFacts = [];

        // Dependency rules: derived flag -> set of required base flags (all must be true = AND)
        private static readonly Dictionary<string, HashSet<string>> connectionRules = new()
        {
            // OBJ-A (Burner Hard Drive) + OBJ-E (Shadow Roommate Server) = The Lexicon
            [ConnLexicon] = [ObjAFound, ObjEFound],
            // OBJ-B (Cracked VR Headset) + OBJ-E (Shadow Roommate Server) = The Haunting
            [ConnHaunting] = [ObjBFound, ObjEFound],
            // OBJ-C (Unworn Platform Shoes) + OBJ-D (Parking Receipts) = The Phantom Date
            [ConnPhantomDate] = [ObjCFound, ObjDFound]
        };

        // Map: evidence item name -> HAS_EVIDENCE_* flag (used by event subscriptions)
        private static readonly Dictionary<string, string> evidenceItemFlags = new()
        {
            ["Burner Hard Drive"] = HasEvidenceHardDrive,
            ["Cracked VR Headset"] = HasEvidenceVR,
            ["Unworn Platform Shoes"] = HasEvidenceShoes,
            ["Parking Receipts"] = HasEvidenceReceipts,
            ["Shadow Roommate Server"] = HasEvidenceServer
        };

        #endregion

        #region Static Constructor

        static NodeMapState()
        {
            // Subscribe to item take/drop events to keep HAS_EVIDENCE_* flags in sync
            EventBus.Subscribe<ItemReceived>(OnItemReceived);
            EventBus.Subscribe<ItemRemoved>(OnItemRemoved);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the total number of active flags (base + derived).
        /// </summary>
        public static int FlagCount => baseFacts.Count + derivedFacts.Count;

        #endregion

        #region Methods

        /// <summary>
        /// Set a base fact and recompute all derived facts.
        /// </summary>
        /// <param name="flag">The flag to set.</param>
        public static void SetFlag(string flag)
        {
            if (string.IsNullOrEmpty(flag))
                return;

            baseFacts.Add(flag);
            EvaluateDerived();
        }

        /// <summary>
        /// Set or clear a base fact and recompute all derived facts.
        /// </summary>
        /// <param name="flag">The flag to modify.</param>
        /// <param name="value">True to set the flag; false to clear it.</param>
        public static void SetFlag(string flag, bool value)
        {
            if (value)
                SetFlag(flag);
            else
                ClearFlag(flag);
        }

        /// <summary>
        /// Clear a base fact and recompute all derived facts.
        /// Derived facts whose prerequisites are no longer satisfied will be removed.
        /// </summary>
        /// <param name="flag">The flag to clear.</param>
        public static void ClearFlag(string flag)
        {
            if (string.IsNullOrEmpty(flag))
                return;

            baseFacts.Remove(flag);
            EvaluateDerived();
        }

        /// <summary>
        /// Check whether a flag is active (either a base fact or a derived fact).
        /// </summary>
        /// <param name="flag">The flag to check.</param>
        /// <returns>True if the flag is active.</returns>
        public static bool HasFlag(string flag)
        {
            return baseFacts.Contains(flag) || derivedFacts.Contains(flag);
        }

        /// <summary>
        /// Check whether a named connection is active.
        /// Returns true if the connection was derived from its prerequisites,
        /// or was explicitly set as a base fact.
        /// </summary>
        /// <param name="connectionName">The connection name.</param>
        /// <returns>True if the connection is active.</returns>
        public static bool HasConnection(string connectionName)
        {
            return derivedFacts.Contains(connectionName) || baseFacts.Contains(connectionName);
        }

        /// <summary>
        /// Get all active derived (connection) flag names.
        /// </summary>
        /// <returns>An array of active derived flag names.</returns>
        public static string[] GetActiveConnections()
        {
            return [.. derivedFacts];
        }

        /// <summary>
        /// Get all active derived fact names.
        /// </summary>
        /// <returns>An array of all currently active derived flags.</returns>
        public static string[] GetActiveDerived()
        {
            return [.. derivedFacts];
        }

        /// <summary>
        /// Get the full dependency rule schema as a read-only dictionary.
        /// Returns all possible derived flags and their required prerequisites,
        /// regardless of current state. Suitable for JSON export without playing the game.
        /// </summary>
        /// <returns>A dictionary mapping each derived flag to its required base flags.</returns>
        public static IReadOnlyDictionary<string, string[]> GetRuleSchema()
        {
            return connectionRules.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToArray());
        }

        /// <summary>
        /// Export the dependency rule schema to a JSON file.
        /// The schema reflects all possible dependencies regardless of current game state.
        /// </summary>
        /// <param name="path">Destination file path.</param>
        public static void ExportSchemaJson(string path)
        {
            var schema = new
            {
                nodes = GetAllKnownNodes(),
                rules = connectionRules.Select(kvp => new
                {
                    target = kvp.Key,
                    requires = kvp.Value.ToArray()
                }).ToArray()
            };

            var json = JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(path, json);
        }

        /// <summary>
        /// Export the current runtime state to a JSON file.
        /// </summary>
        /// <param name="path">Destination file path.</param>
        public static void ExportStateJson(string path)
        {
            var state = new
            {
                baseFacts = baseFacts.OrderBy(f => f).ToArray(),
                derivedFacts = derivedFacts.OrderBy(f => f).ToArray()
            };

            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(path, json);
        }

        /// <summary>
        /// Reset all base and derived facts. Does not affect event subscriptions.
        /// </summary>
        public static void Reset()
        {
            baseFacts.Clear();
            derivedFacts.Clear();
        }

        /// <summary>
        /// Recompute all derived facts from scratch using fixpoint iteration.
        /// Each newly derived fact is immediately available as a prerequisite for
        /// subsequent rules, so derived facts can chain: A → B → C is supported.
        /// Facts that no longer have all prerequisites satisfied are removed.
        /// </summary>
        private static void EvaluateDerived()
        {
            derivedFacts.Clear();

            // Build a working set that starts from base facts and grows as new
            // derived facts are added.  Iterate until no new facts are produced.
            var available = new HashSet<string>(baseFacts);
            bool changed;

            do
            {
                changed = false;

                foreach (var rule in connectionRules)
                {
                    if (!derivedFacts.Contains(rule.Key) && rule.Value.IsSubsetOf(available))
                    {
                        derivedFacts.Add(rule.Key);
                        available.Add(rule.Key);
                        changed = true;
                    }
                }
            }
            while (changed);
        }

        /// <summary>
        /// Returns all known node IDs (base fact keys + derived fact keys) for schema export.
        /// </summary>
        private static object[] GetAllKnownNodes()
        {
            var baseNodes = new[]
            {
                ObjAFound, ObjBFound, ObjCFound, ObjDFound, ObjEFound,
                HasEvidenceHardDrive, HasEvidenceVR, HasEvidenceShoes,
                HasEvidenceReceipts, HasEvidenceServer
            }.Select(id => (object)new { id, kind = "base" });

            var derivedNodes = connectionRules.Keys
                .Select(id => (object)new { id, kind = "derived" });

            return [.. baseNodes.Concat(derivedNodes)];
        }

        #endregion

        #region Event Handlers

        private static void OnItemReceived(ItemReceived e)
        {
            var itemName = e?.Item?.Identifier?.Name;
            if (itemName != null && evidenceItemFlags.TryGetValue(itemName, out var flag))
                SetFlag(flag);
        }

        private static void OnItemRemoved(ItemRemoved e)
        {
            var itemName = e?.Item?.Identifier?.Name;
            if (itemName != null && evidenceItemFlags.TryGetValue(itemName, out var flag))
                ClearFlag(flag);
        }

        #endregion
    }
}
