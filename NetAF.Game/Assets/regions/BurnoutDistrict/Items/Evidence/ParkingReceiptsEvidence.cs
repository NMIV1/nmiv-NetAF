using NetAF.Assets;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict.Items.Evidence
{
    /// <summary>
    /// Takeable evidence: digital parking receipts from Silas's tablet.
    /// </summary>
    public class ParkingReceiptsEvidence : IAssetTemplate<Item>
    {
        public const string ItemName = "Parking Receipts";

        public Item Instantiate()
        {
            return new Item(
                new Identifier(ItemName),
                new Description("A printout of digital parking receipts. Seventeen consecutive Fridays at 3:00 AM, U-District Campus, Lot C. He never gets out of the car."),
                isTakeable: true,
                examination: _ => new Examination("Every Friday. 3:00 AM. Same lot. He drives there, parks, sits for almost three hours, and drives home. The campus security logs show no entry scan, no building access. He never gets out of the car."));
        }
    }
}
