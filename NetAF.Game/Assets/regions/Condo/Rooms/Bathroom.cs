using NetAF.Assets.Locations;
using NetAF.Utilities;
using NetAF.Game.Assets.Regions.Condo.Items;

namespace NetAF.Game.Assets.Regions.Condo.Rooms
{
    public class Bathroom : IAssetTemplate<Room>
    {
        private const string Name = "Bathroom";
        private const string Description = "A cramped bathroom with a shower and a mirror.";

        public Room Instantiate()
        {
            var room = new Room(Name, Description, [ new Exit(Direction.West) ]);
            room.AddItem(new Mirror().Instantiate());
            room.AddItem(new Shower().Instantiate());
            return room;
        }
    }
}
