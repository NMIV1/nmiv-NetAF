using NetAF.Assets.Locations;
using NetAF.Game.Assets.Regions.Condo.Items;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.Condo.Rooms
{
    public class Outside : IAssetTemplate<Room>
    {
        private const string Name = "Outside";
        private const string Description = "The street outside your building. Traffic hums in the distance. A taxi idles at the curb, waiting for a fare.";

        public Room Instantiate()
        {
            var room = new Room(Name, Description, [new Exit(Direction.North)]);
            room.AddCharacter(new Taxi().Instantiate());
            return room;
        }
    }
}
