using NetAF.Assets;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.Condo.Items
{
    public class Couch : IAssetTemplate<Item>
    {
        private const string Name = "Couch";

        public Item Instantiate()
        {
            return new Item(new Identifier(Name), new Description("A small, sagging couch."));
        }
    }
}
