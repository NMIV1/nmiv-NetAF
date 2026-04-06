using System;

namespace NetAF.MyGame.Skills
{
    /// <summary>
    /// The skill types used for discovering hidden objects.
    /// </summary>
    public enum SkillType
    {
        /// <summary>
        /// Intuition about physical materials and objects.
        /// </summary>
        MaterialIntuition,

        /// <summary>
        /// Ability to read the history of objects through time.
        /// </summary>
        ChronologicalEcho,

        /// <summary>
        /// Logical deduction from forensic evidence.
        /// </summary>
        ForensicLogic
    }

    /// <summary>
    /// Difficulty levels for skill checks.
    /// </summary>
    public enum Difficulty
    {
        /// <summary>
        /// Easy check — always succeeds.
        /// </summary>
        Easy,

        /// <summary>
        /// Medium check — moderate chance of success.
        /// </summary>
        Medium,

        /// <summary>
        /// Hard check — low chance of success.
        /// </summary>
        Hard
    }

    /// <summary>
    /// Resolves skill checks against difficulty thresholds.
    /// </summary>
    public static class SkillCheck
    {
        private static readonly Random random = new();

        /// <summary>
        /// Attempt a skill check.
        /// </summary>
        /// <param name="skill">The skill being tested.</param>
        /// <param name="difficulty">The difficulty of the check.</param>
        /// <returns>True if the check passes.</returns>
        public static bool Attempt(SkillType skill, Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => true,
                Difficulty.Medium => random.Next(100) < 60,
                Difficulty.Hard => random.Next(100) < 30,
                _ => false
            };
        }
    }
}
