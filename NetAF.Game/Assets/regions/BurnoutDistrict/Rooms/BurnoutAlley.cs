using NetAF.Assets.Locations;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict.Rooms
{
    public class BurnoutAlley : IAssetTemplate<Room>
    {
        private const string Name = "Burnout Alley";
        private const string Description = "A grimy alley littered with burnt-out trash cans and faded graffiti. The air smells like smoke. A doorway leads east into an apartment building.";

        public Room Instantiate()
        {
            return new Room(Name, Description, [new Exit(Direction.East), new Exit(Direction.North)]);
        }
    }
}
