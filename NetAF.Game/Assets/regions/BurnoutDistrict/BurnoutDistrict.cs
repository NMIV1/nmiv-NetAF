using NetAF.Assets;
using NetAF.Assets.Locations;
using NetAF.Game.Assets.Regions.BurnoutDistrict.Rooms;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict
{
    public class BurnoutDistrict : IAssetTemplate<Region>
    {
        private const string Name = "Burnout District";
        private const string Description = "A run-down neighborhood on the edge of the city. This is where Silas Vance lives.";

        public Region Instantiate()
        {
            var alley = new BurnoutAlley().Instantiate();
            var hallway = new ApartmentHallway().Instantiate();
            var silasApt = new SilasApartment().Instantiate();
            var diveBar = new DiveBar().Instantiate();

            // Layout:
            //  [0,1,0] Dive Bar
            //      |
            //  [0,0,0] Alley -- [1,0,0] Hallway -- [2,0,0] Silas's Apartment
            var regionMaker = new RegionMaker(Name, Description)
            {
                [0,0,0] = alley,
                [1,0,0] = hallway,
                [2,0,0] = silasApt,
                [0,1,0] = diveBar
            };

            return regionMaker.Make(0,0,0);
        }
    }
}
