using NetAF.Assets;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.Condo.Items
{
    public class Kettle : IAssetTemplate<Item>
    {
        private const string Name = "Kettle";

        public Item Instantiate()
        {
            return new Item(new Identifier(Name), new Description("An old electric kettle."));
        }
    }
}
