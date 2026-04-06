using System.Collections.Generic;
using System.Linq;

namespace NetAF.MyGame
{
    /// <summary>
    /// Tracks discovered object flags and auto-detects connections between paired objects.
    /// Uses a set for state flags and a dictionary for connection dependency rules.
    /// </summary>
    public static class NodeMapState
    {
        #region Constants

        public const string ObjAFound = "OBJ_A_FOUND";
        public const string ObjBFound = "OBJ_B_FOUND";
        public const string ObjCFound = "OBJ_C_FOUND";
        public const string ObjDFound = "OBJ_D_FOUND";
        public const string ObjEFound = "OBJ_E_FOUND";

        public const string ConnLexicon = "CONN_LEXICON";
        public const string ConnHaunting = "CONN_HAUNTING";
        public const string ConnPhantomDate = "CONN_PHANTOM_DATE";

        #endregion

        #region Fields

        private static readonly HashSet<string> flags = [];

        private static readonly Dictionary<string, HashSet<string>> connectionRules = new()
        {
            // OBJ-A (Burner Hard Drive) + OBJ-E (Shadow Roommate Server) = The Lexicon
            [ConnLexicon] = [ObjAFound, ObjEFound],
            // OBJ-B (Cracked VR Headset) + OBJ-E (Shadow Roommate Server) = The Haunting
            [ConnHaunting] = [ObjBFound, ObjEFound],
            // OBJ-C (Unworn Platform Shoes) + OBJ-D (Parking Receipts) = The Phantom Date
            [ConnPhantomDate] = [ObjCFound, ObjDFound]
        };

        #endregion

        #region Properties

        /// <summary>
        /// Get the number of flags currently set.
        /// </summary>
        public static int FlagCount => flags.Count;

        #endregion

        #region Methods

        /// <summary>
        /// Set a flag and automatically check if any connections are now complete.
        /// </summary>
        /// <param name="flag">The flag to set.</param>
        public static void SetFlag(string flag)
        {
            if (string.IsNullOrEmpty(flag))
                return;

            flags.Add(flag);
            EvaluateConnections();
        }

        /// <summary>
        /// Check whether a flag is set.
        /// </summary>
        /// <param name="flag">The flag to check.</param>
        /// <returns>True if the flag is set.</returns>
        public static bool HasFlag(string flag)
        {
            return flags.Contains(flag);
        }

        /// <summary>
        /// Check whether a named connection has been unlocked.
        /// </summary>
        /// <param name="connectionName">The connection name.</param>
        /// <returns>True if the connection is active.</returns>
        public static bool HasConnection(string connectionName)
        {
            return flags.Contains(connectionName);
        }

        /// <summary>
        /// Get all active connection names.
        /// </summary>
        /// <returns>An array of active connection names.</returns>
        public static string[] GetActiveConnections()
        {
            return connectionRules.Keys.Where(flags.Contains).ToArray();
        }

        /// <summary>
        /// Reset all flags and connections.
        /// </summary>
        public static void Reset()
        {
            flags.Clear();
        }

        /// <summary>
        /// Evaluate all connection rules and set connection flags where requirements are met.
        /// </summary>
        private static void EvaluateConnections()
        {
            foreach (var rule in connectionRules)
            {
                if (rule.Value.IsSubsetOf(flags))
                    flags.Add(rule.Key);
            }
        }

        #endregion
    }
}
