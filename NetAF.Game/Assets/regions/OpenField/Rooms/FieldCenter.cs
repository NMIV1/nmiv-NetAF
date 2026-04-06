using NetAF.Assets.Locations;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.OpenField.Rooms
{
    public class FieldCenter : IAssetTemplate<Room>
    {
        private const string Name = "Field Center";
        private const string Description = "You stand in the center of a wide open field. A path leads north and south.";

        public Room Instantiate()
        {
            var room = new Room(Name, Description, [ new Exit(Direction.North), new Exit(Direction.South) ]);
            return room;
        }
    }
}
