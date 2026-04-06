using NetAF.Assets;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.Condo.Items
{
    public class Desk : IAssetTemplate<Item>
    {
        private const string Name = "Desk";

        public Item Instantiate()
        {
            return new Item(new Identifier(Name), new Description("A cramped desk with scattered papers."));
        }
    }
}
