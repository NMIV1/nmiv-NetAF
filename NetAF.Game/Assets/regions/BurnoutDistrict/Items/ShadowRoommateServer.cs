using NetAF.Assets;
using NetAF.Assets.Locations;
using NetAF.Game.Assets.Regions.BurnoutDistrict.Items.Evidence;
using NetAF.MyGame;
using NetAF.MyGame.Skills;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict.Items
{
    /// <summary>
    /// OBJ-E: A "Shadow Roommate" server inside a hollowed-out PC tower case.
    /// A localized AI running a chat simulation using a custom vocabulary.
    /// </summary>
    public class ShadowRoommateServer : IAssetTemplate<Item>
    {
        private const string Name = "PC Tower";
        private const string BaseDescription = "A bulky PC tower case shoved under the desk, coated in a fine layer of dust. The power light is dead. No cables running to a monitor. By all appearances, it's junk — a dead machine in a dead apartment. But if you press your ear close... there's a hum. Faint. Steady. Something inside is alive.";

        private readonly Room room;

        /// <summary>
        /// Initializes a new instance of the ShadowRoommateServer class.
        /// </summary>
        /// <param name="room">The room to spawn evidence into on discovery.</param>
        public ShadowRoommateServer(Room room)
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
                    if (NodeMapState.HasFlag(NodeMapState.ObjEFound))
                        return new Examination("The hollowed-out tower case hides a localized AI server — no network connection, no cloud sync, completely off-grid. The chat logs are still scrolling on the tiny internal display. The AI speaks in slang, uses contractions, drops syllables. It sounds... human. Not corporate-human, not assistant-human. Actually human. It tells jokes. It gets annoyed. It says 'I dunno' and 'whatever' and 'come on, you know what I mean.' Someone spent months — maybe years — teaching this thing to talk the way real people talk. The Shadow Roommate. It lives in this box, and it talks to Silas, and nobody else knows it exists.");

                    if (SkillCheck.Attempt(SkillType.MaterialIntuition, Difficulty.Hard))
                    {
                        NodeMapState.SetFlag(NodeMapState.ObjEFound);
                        room.AddItem(new ShadowRoommateServerEvidence().Instantiate());
                        return new Examination("[Material Intuition — Hard: PASSED] The weight is wrong. A tower case this size should be heavy — full of drives, boards, fans. This one shifts too easily when you nudge it. Hollow. You find the side panel release and pop it open. Inside: the guts have been ripped out and replaced. A single compact server board, a solid-state drive, a cooling fan wired to an external battery. No network card. No WiFi antenna. This thing is completely air-gapped — offline, invisible, untraceable. A localized AI running a chat simulation. The terminal shows recent conversations. You scroll through them. The AI doesn't speak like a corporate assistant. It uses slang, humor, warmth. It says things like 'rough day, huh?' and 'tell me about it' and 'I'm not going anywhere.' Someone built this from scratch. Fed it a custom vocabulary. Trained it to sound like a person — not just any person, but someone specific. A Shadow Roommate, living in a box under the desk.");
                    }

                    return new Examination("An old PC tower case, dusty and disconnected. Looks like it hasn't been plugged in for months. The case panels are screwed shut. Whatever was in here, it's either dead or forgotten. You'd need to really know what you're looking for to find anything worth examining.");
                });
        }
    }
}
