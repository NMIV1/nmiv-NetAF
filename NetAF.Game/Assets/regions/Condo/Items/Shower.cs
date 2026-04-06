using NetAF.Assets;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.Condo.Items
{
    public class Shower : IAssetTemplate<Item>
    {
        private const string Name = "Shower";

        public Item Instantiate()
        {
            return new Item(new Identifier(Name), new Description("A cramped shower cubicle."));
        }
    }
}
