using NetAF.Assets;
using NetAF.Assets.Characters;
using NetAF.Extensions;
using NetAF.Utilities;

namespace NetAF.MyGame.Assets.Player
{
    public class Player : IAssetTemplate<PlayableCharacter>
    {
        #region Constants

        private const string Name = "Ben";
        private const string Description = "You are a 25 year old man, dressed in shorts, a t-shirt and flip-flops.";

        #endregion

        #region Implementation of IAssetTemplate<PlayableCharacter>

        /// <summary>
        /// Instantiate a new instance of the asset.
        /// </summary>
        /// <returns>The asset.</returns>
        public PlayableCharacter Instantiate()
        {
            var player = new PlayableCharacter(Name, Description, [], interaction: i =>
            {
                return new(InteractionResult.NoChange, i);
            });

            return player;
        }

        #endregion
    }
}
