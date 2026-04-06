using NetAF.Assets;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict.Items.Evidence
{
    /// <summary>
    /// Takeable evidence: the shadow roommate AI server from the hollow PC tower.
    /// </summary>
    public class ShadowRoommateServerEvidence : IAssetTemplate<Item>
    {
        public const string ItemName = "Shadow Roommate Server";

        public Item Instantiate()
        {
            return new Item(
                new Identifier(ItemName),
                new Description("A compact server board pulled from a hollowed-out PC tower. Completely air-gapped — a localized AI trained to speak like a real person."),
                isTakeable: true,
                examination: _ => new Examination("An air-gapped AI server. The chat logs show a personality that uses slang, humor, and warmth. It says things like 'rough day, huh?' and 'I'm not going anywhere.' Someone built this from scratch and trained it to sound like a specific person. The Shadow Roommate."));
        }
    }
}
