using NetAF.Assets.Locations;
using NetAF.Utilities;
using NetAF.Game.Assets.Regions.Condo.Items;

namespace NetAF.Game.Assets.Regions.Condo.Rooms
{
    public class Kitchen : IAssetTemplate<Room>
    {
        private const string Name = "Kitchen";
        private const string Description = "A tiny kitchenette with a kettle and a few dirty dishes.";

        public Room Instantiate()
        {
            var room = new Room(Name, Description, [ new Exit(Direction.North) ]);
            room.AddItem(new Kettle().Instantiate());
            room.AddItem(new Plate().Instantiate());
            return room;
        }
    }
}
