using NetAF.Assets;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.Condo.Items
{
    public class Lamp : IAssetTemplate<Item>
    {
        private const string Name = "Lamp";

        public Item Instantiate()
        {
            return new Item(new Identifier(Name), new Description("A flickering desk lamp."));
        }
    }
}
