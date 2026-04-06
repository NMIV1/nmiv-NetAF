using NetAF.Assets.Locations;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.Condo.Rooms
{
    public class Lobby : IAssetTemplate<Room>
    {
        private const string Name = "Lobby";
        private const string Description = "The building lobby. A dusty plant sits in the corner. The front door leads outside to the south.";

        public Room Instantiate()
        {
            return new Room(Name, Description, [new Exit(Direction.Up), new Exit(Direction.South)]);
        }
    }
}
