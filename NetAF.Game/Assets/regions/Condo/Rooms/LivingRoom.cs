using NetAF.Assets.Locations;
using NetAF.Utilities;
using NetAF.Game.Assets.Regions.Condo.Items;

namespace NetAF.Game.Assets.Regions.Condo.Rooms
{
    public class LivingRoom : IAssetTemplate<Room>
    {
        private const string Name = "Living Room";
        private const string Description = "A cramped living room with a couch and a small desk. The work computer sits on the desk.";

        public Room Instantiate()
        {
            var room = new Room(Name, Description, [ new Exit(Direction.East), new Exit(Direction.South) ]);
            room.AddItem(new Couch().Instantiate());
            room.AddItem(new Desk().Instantiate());
            room.AddItem(new Lamp().Instantiate());
            // Add the case computer NPC for browsing and starting cases
            room.AddCharacter(new CaseComputer().Instantiate());
            return room;
        }
    }
}
