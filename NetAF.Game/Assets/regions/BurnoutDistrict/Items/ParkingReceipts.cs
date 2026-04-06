using NetAF.Assets;
using NetAF.Assets.Locations;
using NetAF.Game.Assets.Regions.BurnoutDistrict.Items.Evidence;
using NetAF.MyGame;
using NetAF.MyGame.Skills;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict.Items
{
    /// <summary>
    /// OBJ-D: Parking receipts — a digital trace on Silas's unlocked tablet.
    /// Shows he parks at the U-District Campus every Friday at 3:00 AM.
    /// </summary>
    public class ParkingReceipts : IAssetTemplate<Item>
    {
        private const string Name = "Tablet";
        private const string BaseDescription = "A cheap tablet lying face-up on the desk, screen still active. No lock screen — whoever lives here stopped caring about security a long time ago. Browser tabs are open, notifications unread. The digital life of someone running on autopilot.";

        private readonly Room room;

        /// <summary>
        /// Initializes a new instance of the ParkingReceipts class.
        /// </summary>
        /// <param name="room">The room to spawn evidence into on discovery.</param>
        public ParkingReceipts(Room room)
        {
            this.room = room;
        }

        public Item Instantiate()
        {
            return new Item(
                new Identifier(Name),
                new Description(BaseDescription),
                examination: req =>
                {
                    if (NodeMapState.HasFlag(NodeMapState.ObjDFound))
                        return new Examination("The parking receipts are still on screen. Seventeen consecutive Fridays. Arrival: 3:00 AM. Departure: 5:47 AM, give or take. U-District Campus, Lot C — the one furthest from any building, facing the old science quad. He drives there, parks, and sits. For almost three hours. Every week. The campus security logs show no entry scan, no building access. He never gets out of the car. You think about the shoes under the bed. The pacing. He gets dressed up. Drives to campus. And then... can't.");

                    if (SkillCheck.Attempt(SkillType.ForensicLogic, Difficulty.Medium))
                    {
                        NodeMapState.SetFlag(NodeMapState.ObjDFound);
                        room.AddItem(new ParkingReceiptsEvidence().Instantiate());
                        return new Examination("[Forensic Logic — Medium: PASSED] You swipe past the home screen and start pulling threads. Email — mostly spam. Browser history — cleared, but the auto-fill remembers a parking payment portal. You follow it. The account is still logged in. And there it is: a trail of digital parking receipts stretching back months. Every Friday. Arrival time: 3:00 AM. Same lot. U-District Campus, Lot C. The pattern is mechanical — obsessive. Seventeen weeks without a single deviation. 3:00 AM is not a class time. It's not a work shift. It's the hour when you do something you don't want anyone to see. Or the hour when you almost do something, and then drive home instead.");
                    }

                    return new Examination("You tap through the tablet — a few open browser tabs, unread notifications, nothing that jumps out immediately. The home screen is cluttered with apps. Could be something buried in here, but you'd need sharper eyes to find the pattern.");
                });
        }
    }
}
