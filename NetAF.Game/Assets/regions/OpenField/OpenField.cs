using NetAF.Assets;
using NetAF.Assets.Locations;
using NetAF.Game.Assets.Regions.OpenField.Rooms;
using NetAF.Extensions;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.OpenField
{
    public class OpenField : IAssetTemplate<Region>
    {
        private const string Name = "OpenField";
        private const string Description = "A wide open grassy field with a worn path running north and south.";

        public Region Instantiate()
        {
            var center = new FieldCenter().Instantiate();
            var north = new NorthPath().Instantiate();
            var south = new SouthPath().Instantiate();

            var regionMaker = new RegionMaker(Name, Description)
            {
                [1, 1, 0] = center,
                [1, 0, 0] = north,
                [1, 2, 0] = south
            };

            return regionMaker.Make(1, 1, 0);
        }
    }
}
