using NetAF.Assets;
using NetAF.Assets.Characters;
using NetAF.Commands;
using NetAF.Conversations;
using NetAF.Conversations.Instructions;
using NetAF.Logic;
using NetAF.Logic.Modes;
using NetAF.MyGame;
using NetAF.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace NetAF.Game.Assets.Regions.Condo.Items
{
    /// <summary>
    /// A taxi NPC that lets you travel to case locations via conversation.
    /// </summary>
    public class Taxi : IAssetTemplate<NonPlayableCharacter>
    {
        public const string Name = "Taxi";

        public NonPlayableCharacter Instantiate()
        {
            NonPlayableCharacter character = null;

            var useTaxiCommand = new CustomCommand(
                new CommandHelp("UseTaxi", "Hail the taxi to travel somewhere.", CommandCategory.Custom, "Taxi", displayAs: "UseTaxi -20", colorHint: "255-165-0"),
                true,
                true,
                (game, args) =>
                {
                    DayClock.SkipNextAutoSpend = true;
                    GameVars.Instance.IsReturningFromMode = true;
                    DayClock.Spend(20);
                    var conversation = BuildTaxiConversation(game);
                    character = new NonPlayableCharacter(
                        new Identifier("Taxi Driver"),
                        new Description("A grizzled taxi driver."),
                        conversation: conversation
                    );
                    character.IsPlayerVisible = false;

                    conversation.Next(game);
                    var interpreter = game.Configuration.InterpreterProvider.Find(typeof(ConversationMode));
                    game.ChangeMode(new ConversationMode(character, interpreter));
                    return new Reaction(ReactionResult.Silent, "");
                }
            );

            character = new NonPlayableCharacter(
                new Identifier(Name),
                new Description("A yellow taxi idling at the curb."),
                commands: [useTaxiCommand],
                examination: req => new Examination("A beat-up yellow taxi. The driver looks bored.")
            );
            character.IsPlayerVisible = true;
            return character;
        }

        private static Conversation BuildTaxiConversation(NetAF.Logic.Game game)
        {
            var activeCases = GameState.GetActiveCaseDetails();
            var paragraphs = new List<Paragraph>();

            // Paragraph 0: Main menu
            var mainResponses = new List<Response>();

            // Build destination list from active cases that have a region
            var destinations = activeCases.Where(c => !string.IsNullOrEmpty(c.region)).ToList();

            if (destinations.Count > 0)
            {
                for (int i = 0; i < destinations.Count; i++)
                {
                    mainResponses.Add(new Response($"Go to {destinations[i].title}", new GoTo(1 + i)));
                }
            }

            // Always offer to go home if not already in Condo
            var currentRegion = game.Overworld.CurrentRegion?.Identifier?.Name ?? "";
            if (currentRegion != "Condo")
            {
                mainResponses.Add(new Response("Go Home", new GoTo(1 + destinations.Count)));
            }

            var menuText = destinations.Count > 0
                ? "\"Where to, pal?\""
                : "\"Where to, pal? ...Looks like you got nowhere to go.\"";

            paragraphs.Add(new Paragraph(menuText, "TaxiMenu") { Responses = mainResponses.ToArray() });

            // Paragraphs 1..N: One per destination
            for (int i = 0; i < destinations.Count; i++)
            {
                var dest = destinations[i];
                var travelPara = new Paragraph(
                    $"\"Alright, heading to {dest.title}. Hang on.\"",
                    $"Travel_{dest.id}")
                {
                    Responses = [] // no responses — will auto-end conversation
                };

                // Action: move to the target region
                var regionName = dest.region;
                travelPara.Action = g =>
                {
                    var targetRegion = g.Overworld.Regions.FirstOrDefault(
                        r => r.Identifier.Name.Replace(" ", "_").ToLower() == regionName.ToLower()
                            || r.Identifier.Name.ToLower() == regionName.ToLower());

                    if (targetRegion != null)
                        g.Overworld.Move(targetRegion);
                };

                paragraphs.Add(travelPara);
            }

            // "Go Home" paragraph (if applicable)
            if (currentRegion != "Condo")
            {
                var homePara = new Paragraph("\"Back home it is.\"", "GoHome")
                {
                    Responses = []
                };
                homePara.Action = g =>
                {
                    var condo = g.Overworld.Regions.FirstOrDefault(
                        r => r.Identifier.Name == "Condo");
                    if (condo != null)
                        g.Overworld.Move(condo);
                };
                paragraphs.Add(homePara);
            }

            return new Conversation(paragraphs.ToArray());
        }
    }
}
