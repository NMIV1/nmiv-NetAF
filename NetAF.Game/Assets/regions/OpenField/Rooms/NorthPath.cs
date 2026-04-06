using NetAF.Assets.Locations;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.OpenField.Rooms
{
    public class NorthPath : IAssetTemplate<Room>
    {
        private const string Name = "North Path";
        private const string Description = "A narrow path heading north through the grass. The field center is to the south.";

        public Room Instantiate()
        {
            var room = new Room(Name, Description, [ new Exit(Direction.South) ]);
            return room;
        }
    }
}
