using NetAF.Assets.Locations;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.OpenField.Rooms
{
    public class SouthPath : IAssetTemplate<Room>
    {
        private const string Name = "South Path";
        private const string Description = "A worn track heading south. The field center is to the north.";

        public Room Instantiate()
        {
            var room = new Room(Name, Description, [ new Exit(Direction.North) ]);
            return room;
        }
    }
}
