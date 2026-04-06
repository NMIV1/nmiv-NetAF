using NetAF.Assets;
using NetAF.Assets.Locations;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict.Rooms
{
    public class ApartmentHallway : IAssetTemplate<Room>
    {
        private const string Name = "Apartment Hallway";
        private const string Description = "A dimly lit hallway with peeling wallpaper. Most doors are boarded up. One door at the end of the hall is slightly ajar — apartment 4B.";

        public Room Instantiate()
        {
            var room = new Room(Name, Description, [new Exit(Direction.West), new Exit(Direction.East)]);
            room.AddItem(new Item("Notice", "A faded eviction notice pinned to the wall. It's addressed to 'Silas Vance'.",
                examination: req => new Examination("An eviction notice dated three months ago. Silas Vance, apartment 4B. Overdue rent.")));
            return room;
        }
    }
}
