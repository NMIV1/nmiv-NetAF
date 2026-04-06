using NetAF.Assets;
using NetAF.Assets.Locations;
using NetAF.Game.Assets.Regions.Condo.Rooms;
using NetAF.Game.Assets.Regions.Condo.Items;
using NetAF.Extensions;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.Condo
{
    public class Condo : IAssetTemplate<Region>
    {
        private const string Name = "Condo";
        private const string Description = "A small, dim condo where the player lives and works.";

        public Region Instantiate()
        {
            var living = new LivingRoom().Instantiate();
            var kitchen = new Kitchen().Instantiate();
            var bedroom = new Bedroom().Instantiate();
            var bathroom = new Bathroom().Instantiate();
            var hallway = new Hallway().Instantiate();
            var elevator = new Elevator().Instantiate();
            var lobby = new Lobby().Instantiate();
            var outside = new Outside().Instantiate();

            // Layout (z=0 is condo floor, z=-1 is ground floor):
            //
            //  z=0:  [0,1,0] Bedroom -- [1,1,0] Living Room -- [2,1,0] Bathroom
            //                                |
            //                           [1,0,0] Kitchen
            //
            //                           [1,2,0] Hallway
            //                                |
            //                           [1,3,0] Elevator
            //
            //  z=-1: [1,3,-1] Lobby
            //                  |
            //            [1,2,-1] Outside

            var regionMaker = new RegionMaker(Name, Description)
            {
                [1,1,0] = living,
                [1,0,0] = kitchen,
                [0,1,0] = bedroom,
                [2,1,0] = bathroom,
                [1,2,0] = hallway,
                [1,3,0] = elevator,
                [1,3,-1] = lobby,
                [1,2,-1] = outside
            };

            return regionMaker.Make(1,1,0);
        }
    }
}
