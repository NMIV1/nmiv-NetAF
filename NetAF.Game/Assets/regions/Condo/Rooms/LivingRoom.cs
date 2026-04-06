using NetAF.Assets.Locations;
using NetAF.Commands;
using NetAF.Interpretation;
using NetAF.MyGame;
using NetAF.Utilities;
using NetAF.Game.Assets.Regions.Condo.Items;

namespace NetAF.Game.Assets.Regions.Condo.Rooms
{
    public class LivingRoom : IAssetTemplate<Room>
    {
        private const string Name = "Living Room";
        private const string Description = "A cramped living room with a couch and a small desk. A laptop computer sits on the desk, ready to browse your cases. To the north is the Hallway.";

        public Room Instantiate()
        {
            // Test command for the day clock
            var testClockCommand = new CustomCommand(
                new CommandHelp("TestClock", "Test the day clock (-10 clicks).", CommandCategory.Custom, colorHint: "255-165-0"),
                true,
                false,
                (game, args) =>
                {
                    DayClock.SkipNextAutoSpend = true;
                    var spent = DayClock.Spend(10);
                    var result = DayClock.ShowReaction ? ReactionResult.Inform : ReactionResult.Silent;
                    return new Reaction(result, $"You wasted some time. (-{spent} clicks, {DayClock.Remaining} remaining)");
                });

            var room = new Room(Name, Description, [ new Exit(Direction.East), new Exit(Direction.South), new Exit(Direction.North) ], commands: [testClockCommand]);
            room.AddItem(new Couch().Instantiate());
            room.AddItem(new Desk().Instantiate());
            room.AddItem(new Lamp().Instantiate());
            // Add the case computer NPC for browsing and starting cases
            room.AddCharacter(new CaseComputer().Instantiate());

            return room;
        }
    }
}
