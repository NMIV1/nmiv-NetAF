using NetAF.Assets;
using NetAF.Assets.Locations;
using NetAF.Game.Assets.Regions.BurnoutDistrict.Items;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict.Rooms
{
    /// <summary>
    /// Silas's micro-unit apartment. Contains 5 hidden node objects for the player to discover.
    /// Silas is not here — he's at the Dive Bar.
    /// </summary>
    public class SilasApartment : IAssetTemplate<Room>
    {
        private const string Name = "Silas's Apartment";
        private const string Description = "A wrecked micro-unit. The door wasn't locked — or the lock was broken so long ago nobody bothered to fix it. Takeout containers in various stages of decomposition cover the counter. Empty bottles line the windowsill like a glass fence. The air is stale, warm, and sour. Silas isn't here. The bed is unmade, the sink is full, and the ceiling fan turns in slow, useless circles. But the room is talking. Everything in here is a sentence fragment — you just have to learn how to read it.";

        public Room Instantiate()
        {
            var room = new Room(Name, Description, [new Exit(Direction.West)]);

            // OBJ-A: Burner Hard Drive — taped under the mini-fridge
            room.AddItem(new BurnerHardDrive(room).Instantiate());

            // OBJ-B: Cracked VR Headset — wedged behind the radiator
            room.AddItem(new CrackedVRHeadset(room).Instantiate());

            // OBJ-C: Unworn Platform Shoes — in a pristine box under the bed
            room.AddItem(new UnwornPlatformShoes(room).Instantiate());

            // OBJ-D: Parking Receipts — digital trace on his unlocked tablet
            room.AddItem(new ParkingReceipts(room).Instantiate());

            // OBJ-E: Shadow Roommate Server — inside a hollowed-out PC tower case
            room.AddItem(new ShadowRoommateServer(room).Instantiate());

            // Ambient items (non-node, atmospheric)
            room.AddItem(new Item("Bottles", "A row of empty bottles on the windowsill, arranged with unsettling precision.",
                examination: req => new Examination("Cheap whiskey, mostly. A couple of off-brand vodka bottles. The labels on three of them have been methodically scratched off with a thumbnail — not in anger, but in the slow, repetitive way someone does when they're thinking about something else entirely. The bottles are arranged by height. Left to right, tallest to shortest. There's an order here that the rest of the apartment doesn't share.")));
            room.AddItem(new Item("Mirror", "A cracked mirror mounted on the wall near the bed.",
                examination: req => new Examination("The mirror has a long diagonal crack running through it, but it hasn't been replaced or removed. The carpet in front of it is worn — a small oval track, as if someone stands here regularly and shifts their weight back and forth. Rehearsing. Checking themselves. Looking for someone they used to be.")));
            room.AddItem(new Item("Sink", "A kitchen sink overflowing with dishes and murky water.",
                examination: req => new Examination("The dishes have been here long enough to develop a kind of ecosystem. But the single mug on the counter is clean — recently washed, carefully placed. A mug that says 'World's Okayest Roommate.' The humor stings differently when you look around and realize he lives alone.")));

            return room;
        }
    }
}
