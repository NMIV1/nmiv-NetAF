using NetAF.Assets;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.Condo.Items
{
    public class Mirror : IAssetTemplate<Item>
    {
        private const string Name = "Mirror";

        public Item Instantiate()
        {
            return new Item(new Identifier(Name), new Description("A small foggy mirror."));
        }
    }
}
