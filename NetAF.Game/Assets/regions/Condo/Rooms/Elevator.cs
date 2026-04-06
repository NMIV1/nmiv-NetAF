using NetAF.Assets.Locations;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.Condo.Rooms
{
    public class Elevator : IAssetTemplate<Room>
    {
        private const string Name = "Elevator";
        private const string Description = "A small, creaky elevator. Press the button to go down to the lobby.";

        public Room Instantiate()
        {
            return new Room(Name, Description, [new Exit(Direction.South), new Exit(Direction.Down)]);
        }
    }
}
