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

            var regionMaker = new RegionMaker(Name, Description)
            {
                [1,1,0] = living,
                [1,0,0] = kitchen,
                [0,1,0] = bedroom,
                [2,1,0] = bathroom
            };

            return regionMaker.Make(1,1,0);
        }
    }
}
