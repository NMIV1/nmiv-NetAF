namespace NetAF.MyGame
{
    /// <summary>
    /// Centralized game variables. Use GameVars.Instance to access.
    /// </summary>
    public class GameVars
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static GameVars Instance { get; } = new();

        /// <summary>
        /// When true, the next scene render will skip the automatic -1 click spend.
        /// Set this when entering a conversation or other mode so that exiting doesn't cost a click.
        /// </summary>
        public bool IsReturningFromMode { get; set; }
    }
}
