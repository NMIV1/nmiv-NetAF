using NetAF.Assets;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.Condo.Items
{
    public class ShoesBox : IAssetTemplate<Item>
    {
        private const string Name = "Shoes Box";

        public Item Instantiate()
        {
            return new Item(new Identifier(Name), new Description("A cardboard box full of dusty shoes."));
        }
    }
}
