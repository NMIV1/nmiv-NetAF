using NetAF.Assets;
using NetAF.Assets.Locations;
using NetAF.Game.Assets.Regions.BurnoutDistrict.Items.Evidence;
using NetAF.MyGame;
using NetAF.MyGame.Skills;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict.Items
{
    /// <summary>
    /// OBJ-B: A cracked VR headset wedged behind the radiator.
    /// Holds a localized, un-deleted voicemail from someone named "Rye."
    /// </summary>
    public class CrackedVRHeadset : IAssetTemplate<Item>
    {
        private const string Name = "Radiator";
        private const string BaseDescription = "A cast-iron radiator bolted to the wall beneath the window, paint flaking in long curls. It ticks and clanks with residual heat. The gap behind it is dark, but you can see the edge of something plastic — cracked, wedged in tight, like someone shoved it there in a hurry.";

        private readonly Room room;

        /// <summary>
        /// Initializes a new instance of the CrackedVRHeadset class.
        /// </summary>
        /// <param name="room">The room to spawn evidence into on discovery.</param>
        public CrackedVRHeadset(Room room)
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
                    if (NodeMapState.HasFlag(NodeMapState.ObjBFound))
                        return new Examination("The cracked VR headset sits where you left it, half-pulled from behind the radiator. You already heard the voicemail. A voice — warm, teasing, a little tired — saying: 'Hey. It's Rye. I know you're screening. I just... I miss the way you used to laugh. Before all this. Call me back, okay? Please.' The message is un-deleted. Localized storage — not on any cloud. He kept it where no one could take it from him. This is the only place Rye still exists.");

                    if (SkillCheck.Attempt(SkillType.MaterialIntuition, Difficulty.Medium))
                    {
                        NodeMapState.SetFlag(NodeMapState.ObjBFound);
                        room.AddItem(new CrackedVRHeadsetEvidence().Instantiate());
                        return new Examination("[Material Intuition — Medium: PASSED] You work your arm behind the radiator, skin scraping against hot iron. Your fingers close around something plastic. You pull it free — a VR headset, screen cracked down the middle like a lightning strike. The strap is worn thin. He's used this a thousand times. You power it on. No apps, no games, no media library. Just one file: an audio voicemail, stored locally. The voice is soft, familiar with him in a way that makes you feel like an intruder: 'Hey. It's Rye. I know you're screening. I just... I miss the way you used to laugh. Before all this. Call me back, okay? Please.' The timestamp is eleven months old. He never called back.");
                    }

                    return new Examination("The radiator clanks and hisses. There's definitely something wedged behind it — you can see the corner of cracked plastic — but the angle is awkward and the iron is hot. You'd need steadier hands or a different approach to pull it free.");
                });
        }
    }
}
