using NetAF.Assets;
using NetAF.Assets.Locations;
using NetAF.Game.Assets.Regions.BurnoutDistrict.Items.Evidence;
using NetAF.MyGame;
using NetAF.MyGame.Skills;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict.Items
{
    /// <summary>
    /// OBJ-A: A burner hard drive taped under the mini-fridge.
    /// Contains a list of "Banned Words" Silas is supposed to delete at his job.
    /// </summary>
    public class BurnerHardDrive : IAssetTemplate<Item>
    {
        private const string Name = "Mini-Fridge";
        private const string BaseDescription = "A squat mini-fridge wedged into the corner, its compressor rattling like a bad cough. A faded takeout menu is magneted to the door. The gap between the fridge and the floor is just wide enough to see... something.";

        private readonly Room room;

        /// <summary>
        /// Initializes a new instance of the BurnerHardDrive class.
        /// </summary>
        /// <param name="room">The room to spawn evidence into on discovery.</param>
        public BurnerHardDrive(Room room)
        {
            this.room = room;
        }

        public Item Instantiate()
        {
            return new Item(
                new Identifier(Name),
                new Description(BaseDescription),
                examination: req =>
                {
                    if (NodeMapState.HasFlag(NodeMapState.ObjAFound))
                        return new Examination("The burner hard drive is still taped to the underside of the fridge. You already pulled the file list — hundreds of words, slang terms, neologisms, all flagged for corporate deletion. 'Vibe.' 'Doomscroll.' 'Ghosted.' The language of actual people, marked for erasure. Silas was supposed to scrub these from the system. Instead, he kept a copy. The question isn't why he saved them — it's what he's doing with them.");

                    if (SkillCheck.Attempt(SkillType.MaterialIntuition, Difficulty.Easy))
                    {
                        NodeMapState.SetFlag(NodeMapState.ObjAFound);
                        room.AddItem(new BurnerHardDriveEvidence().Instantiate());
                        return new Examination("[Material Intuition — Easy: PASSED] Something about the weight distribution is off. You crouch down and run your fingers along the underside of the fridge. There — gaffer tape, warm from the compressor heat. You peel it back and a small external hard drive drops into your palm. Still warm. You plug it into your phone. The drive contains a single directory: 'LEXICON_BANNED.' Inside, a spreadsheet — hundreds of rows. Slang words. Colloquialisms. Internet shorthand. Every entry tagged with a deletion date and a corporate mandate reference. These are the words Silas is paid to erase from the company AI's vocabulary. He kept a backup of every single one.");
                    }

                    return new Examination("A mini-fridge. The compressor rattles. A couple of expired energy drinks visible through the gap in the door. You don't notice anything else — just the hum and the smell of old food.");
                });
        }
    }
}
