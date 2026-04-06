using NetAF.Assets.Locations;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.Condo.Rooms
{
    public class Hallway : IAssetTemplate<Room>
    {
        private const string Name = "Hallway";
        private const string Description = "A narrow hallway outside your condo. The fluorescent light flickers overhead. The elevator is to the north.";

        public Room Instantiate()
        {
            return new Room(Name, Description, [new Exit(Direction.South), new Exit(Direction.North)]);
        }
    }
}
