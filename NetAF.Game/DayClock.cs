namespace NetAF.MyGame
{
    /// <summary>
    /// Tracks the day/night cycle as a click counter.
    /// Each action costs clicks. When clicks run out, the day ends.
    /// </summary>
    public static class DayClock
    {
        /// <summary>
        /// The maximum number of clicks per day.
        /// </summary>
        public const int MaxClicks = 300;

        /// <summary>
        /// Get or set whether spending clicks shows a reaction screen.
        /// When false, reactions are silent (no popup).
        /// </summary>
        public static bool ShowReaction { get; set; } = true;

        /// <summary>
        /// Get or set the remaining clicks for the current day.
        /// </summary>
        public static int Remaining { get; private set; } = MaxClicks;

        /// <summary>
        /// When true, the next scene render will skip the automatic -1 spend.
        /// Orange commands that handle their own cost should set this to true.
        /// It resets to false after each render.
        /// </summary>
        public static bool SkipNextAutoSpend { get; set; }

        /// <summary>
        /// Spend a number of clicks. Returns the actual amount spent.
        /// </summary>
        /// <param name="amount">The number of clicks to spend.</param>
        /// <returns>The actual amount spent (clamped to remaining).</returns>
        public static int Spend(int amount)
        {
            if (amount <= 0) return 0;
            var actual = amount > Remaining ? Remaining : amount;
            Remaining -= actual;
            return actual;
        }

        /// <summary>
        /// Reset the clock to the start of a new day.
        /// </summary>
        public static void Reset()
        {
            Remaining = MaxClicks;
        }

        /// <summary>
        /// Called each scene render. Spends 1 click automatically unless skipped.
        /// Returns the display text.
        /// </summary>
        public static string TickAndGetDisplayText()
        {
            if (SkipNextAutoSpend || GameVars.Instance.IsReturningFromMode)
            {
                GameVars.Instance.IsReturningFromMode = false;
            }
            else
            {
                Spend(1);
            }

            SkipNextAutoSpend = false;
            return $"Day: {Remaining}/{MaxClicks}";
        }

        /// <summary>
        /// Get a display string for the current clock state without spending.
        /// </summary>
        public static string GetDisplayText()
        {
            return $"Day: {Remaining}/{MaxClicks}";
        }
    }
}
