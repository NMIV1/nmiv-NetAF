using NetAF.Assets;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.Condo.Items
{
    public class Plate : IAssetTemplate<Item>
    {
        private const string Name = "Plate";

        public Item Instantiate()
        {
            return new Item(new Identifier(Name), new Description("A dirty plate with dried sauce."));
        }
    }
}
