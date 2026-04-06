using NetAF.Assets;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict.Items.Evidence
{
    /// <summary>
    /// Takeable evidence: the cracked VR headset with Rye's voicemail.
    /// </summary>
    public class CrackedVRHeadsetEvidence : IAssetTemplate<Item>
    {
        public const string ItemName = "Cracked VR Headset";

        public Item Instantiate()
        {
            return new Item(
                new Identifier(ItemName),
                new Description("A VR headset with a cracked screen. Contains a single locally-stored voicemail from someone named Rye."),
                isTakeable: true,
                examination: _ => new Examination("The voicemail plays again: 'Hey. It's Rye. I know you're screening. I just... I miss the way you used to laugh. Before all this. Call me back, okay? Please.' Eleven months old. He never called back."));
        }
    }
}
