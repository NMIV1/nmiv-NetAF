using NetAF.Assets.Locations;
using NetAF.Utilities;
using NetAF.Game.Assets.Regions.Condo.Items;

namespace NetAF.Game.Assets.Regions.Condo.Rooms
{
    public class Bedroom : IAssetTemplate<Room>
    {
        private const string Name = "Bedroom";
        private const string Description = "A small bedroom with a bed and a box of old shoes under it.";

        public Room Instantiate()
        {
            var room = new Room(Name, Description, [ new Exit(Direction.East) ]);
            room.AddItem(new Bed().Instantiate());
            room.AddItem(new ShoesBox().Instantiate());
            return room;
        }
    }
}
