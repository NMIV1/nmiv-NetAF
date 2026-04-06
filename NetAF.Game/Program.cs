using NetAF.Assets.Locations;
using NetAF.Assets;
using NetAF.Commands;
using NetAF.Logic;
using NetAF.MyGame.Assets.Player;
using NetAF.Game.Assets.Regions.OpenField;
using NetAF.Game.Assets.Regions.Condo;
using NetAF.Extensions;

namespace NetAF.MyGame
{
    public class MyGame
    {
        private static EndCheckResult DetermineIfGameHasCompleted(NetAF.Logic.Game game)
        {
            var atDestination = false;

            if (!atDestination)
                return EndCheckResult.NotEnded;

            return new EndCheckResult(true, "Game Over", "You have reached the end of the game, thanks for playing!");
        }

        private static EndCheckResult DetermineIfGameOver(NetAF.Logic.Game game)
        {
            if (game.Player.IsAlive)
                return EndCheckResult.NotEnded;

            return new EndCheckResult(true, "Game Over", "You are dead!");
        }

        public static GameCreator Create(GameConfiguration configuration)
        {
            static Overworld overworldCreator()
            {
                var regions = new List<Region> { new Condo().Instantiate(), new OpenField().Instantiate() };

                CustomCommand[] commands =
                [
                    // add a hidden custom command to the overworld that allows jumping around a region for debugging purposes
                    new(new("Jump", "Jump to a location in a region."), false, true, (g, a) =>
                        {
                            var x = 0;
                            var y = 0;
                            var z = 0;

                            if (a?.Length >= 3)
                            {
                                _ = int.TryParse(a[0], out x);
                                _ = int.TryParse(a[1], out y);
                                _ = int.TryParse(a[2], out z);
                            }

                            var result = g.Overworld.CurrentRegion.JumpToRoom(new Point3D(x, y, z));

                            if (result.Result == ReactionResult.Error)
                                return result;

                            return new(ReactionResult.Inform, $"Jumped to {x} {y} {z}.");
                        }),
                    new(new("StartCase", "Start a case by id."), false, true, (g, a) =>
                        {
                            if (a == null || a.Length == 0)
                                return new(ReactionResult.Error, "Usage: startcase <id>");

                            GameState.CurrentCaseId = a[0];
                            return new(ReactionResult.Inform, $"Started case {a[0]}");
                        }),
                    new(new("CurrentCase", "Show current started case"), false, true, (g, a) =>
                        {
                            var id = string.IsNullOrEmpty(GameState.CurrentCaseId) ? "(none)" : GameState.CurrentCaseId;
                            return new(ReactionResult.Inform, $"Current case: {id}");
                        }),
                    new(new("ClearActiveCases", "Clear active cases (debug)."), false, true, (g, a) =>
                        {
                            NetAF.MyGame.GameState.ClearActiveCases();
                            return new(ReactionResult.Inform, "Active cases cleared.");
                        })
                ];

                var overworld = new Overworld("Demo", "A demo of NetAF.", commands);

                foreach (var region in regions)
                    overworld.AddRegion(region);

                return overworld;
            }

            var about = "This is a short demo of NetAF made up from test chunks of games that were build to test different features during development.";
            return NetAF.Logic.Game.Create(new("NetAF Demo", about, "By Ben Pollard 2011 - 2026."), about, AssetGenerator.Custom(overworldCreator, new Player().Instantiate), new GameEndConditions(DetermineIfGameHasCompleted, DetermineIfGameOver), configuration);
        }
    }

}
