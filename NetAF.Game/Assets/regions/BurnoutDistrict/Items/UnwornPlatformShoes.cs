using NetAF.Assets;
using NetAF.Assets.Locations;
using NetAF.Game.Assets.Regions.BurnoutDistrict.Items.Evidence;
using NetAF.MyGame;
using NetAF.MyGame.Skills;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict.Items
{
    /// <summary>
    /// OBJ-C: Unworn platform shoes in a pristine box under the bed.
    /// Never worn outside, but someone has paced around the room in them recently.
    /// </summary>
    public class UnwornPlatformShoes : IAssetTemplate<Item>
    {
        private const string Name = "Bed";
        private const string BaseDescription = "A narrow single bed shoved against the wall, sheets tangled and half on the floor. The mattress sags in the middle like it remembers the shape of someone who doesn't sleep well. There's a gap underneath — dark, dusty, but not empty.";

        private readonly Room room;

        /// <summary>
        /// Initializes a new instance of the UnwornPlatformShoes class.
        /// </summary>
        /// <param name="room">The room to spawn evidence into on discovery.</param>
        public UnwornPlatformShoes(Room room)
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
                    if (NodeMapState.HasFlag(NodeMapState.ObjCFound))
                        return new Examination("The shoe box is still under the bed. Pristine white platforms — expensive, fashionable, completely unworn. No scuff marks on the soles, no creases in the leather. But the carpet tells a different story: a worn oval track in front of the cracked mirror. He puts them on. Paces. Looks at himself. Takes them off. Puts them back in the box. Over and over. Getting ready for something that never happens.");

                    if (SkillCheck.Attempt(SkillType.ChronologicalEcho, Difficulty.Easy))
                    {
                        NodeMapState.SetFlag(NodeMapState.ObjCFound);
                        room.AddItem(new UnwornPlatformShoesEvidence().Instantiate());
                        return new Examination("[Chronological Echo — Easy: PASSED] You kneel down and reach under the bed. Your hand finds a shoe box — not beaten up and forgotten like everything else in this apartment, but pristine. Cared for. Inside: white platform shoes, still wrapped in tissue paper. The soles are immaculate. Never touched pavement, never felt rain. But something nags at you. You look at the carpet near the mirror — there's a faint oval track worn into the fibers. Someone has been pacing in these shoes. Recently. Repeatedly. Walking back and forth in front of that mirror like they're rehearsing for a night out that never comes. The shoes go back in the box. The box goes back under the bed. And the cycle starts again.");
                    }

                    return new Examination("An unmade bed. Tangled sheets smell like stale sweat. There might be something under there, but nothing about it screams importance. Just a sad bed in a sad room.");
                });
        }
    }
}
