using NetAF.Assets;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.Condo.Items
{
    public class Bed : IAssetTemplate<Item>
    {
        private const string Name = "Bed";

        public Item Instantiate()
        {
            return new Item(new Identifier(Name), new Description("A lumpy single bed."));
        }
    }
}
