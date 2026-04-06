using NetAF.Assets;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict.Items.Evidence
{
    /// <summary>
    /// Takeable evidence: the unworn platform shoes from under the bed.
    /// </summary>
    public class UnwornPlatformShoesEvidence : IAssetTemplate<Item>
    {
        public const string ItemName = "Unworn Platform Shoes";

        public Item Instantiate()
        {
            return new Item(
                new Identifier(ItemName),
                new Description("Pristine white platform shoes in a careful box. Never worn outside, but the carpet by the mirror tells a different story."),
                isTakeable: true,
                examination: _ => new Examination("Expensive, fashionable, completely unworn outdoors. But the oval track worn into the carpet in front of the cracked mirror says he puts them on regularly. Pacing. Rehearsing. Getting ready for something that never happens."));
        }
    }
}
