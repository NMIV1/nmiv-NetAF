using NetAF.Assets;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict.Items.Evidence
{
    /// <summary>
    /// Takeable evidence: the burner hard drive found under the mini-fridge.
    /// </summary>
    public class BurnerHardDriveEvidence : IAssetTemplate<Item>
    {
        public const string ItemName = "Burner Hard Drive";

        public Item Instantiate()
        {
            return new Item(
                new Identifier(ItemName),
                new Description("A small external hard drive, still warm from the fridge compressor. Contains a directory called 'LEXICON_BANNED' — hundreds of slang words flagged for corporate deletion."),
                isTakeable: true,
                examination: _ => new Examination("The drive holds a spreadsheet of banned words — every slang term, colloquialism, and piece of internet shorthand that Silas was paid to erase from the corporate AI. He kept a backup of every single one."));
        }
    }
}
