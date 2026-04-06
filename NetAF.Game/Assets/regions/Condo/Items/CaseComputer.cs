using NetAF.Assets;
using NetAF.Assets.Characters;
using NetAF.Commands;
using NetAF.Conversations;
using NetAF.Conversations.Instructions;
using NetAF.Logic;
using NetAF.Logic.Modes;
using NetAF.MyGame;
using NetAF.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Navigation note: Jump(n) is a RELATIVE delta, GoTo(n) is an ABSOLUTE index, ToName(s) jumps by paragraph name.

namespace NetAF.Game.Assets.Regions.Condo.Items
{
    /// A computer NPC that lets you browse and start cases via conversation
    public class CaseComputer : IAssetTemplate<NonPlayableCharacter>
    {
        public const string Name = "Computer";

        private record CaseEntry(string id, string title, string description, string reward, string region);

        public NonPlayableCharacter Instantiate()
        {
            var conversation = CreateCaseConversation();
            
            NonPlayableCharacter character = null; // will be set after creation
            
            var useCommand = new CustomCommand(
                new CommandHelp("UseComputer", "Use the computer to browse cases", CommandCategory.Custom),
                true,
                true,
                (game, args) =>
                {
                    GameVars.Instance.IsReturningFromMode = true;
                    if (conversation == null || character == null)
                        return new Reaction(ReactionResult.Error, "No computer interface available.");
                    
                    conversation.Next(game);
                    var interpreter = game.Configuration.InterpreterProvider.Find(typeof(ConversationMode));
                    game.ChangeMode(new ConversationMode(character, interpreter));
                    return new Reaction(ReactionResult.Silent, "");
                }
            );
            
            character = new NonPlayableCharacter(
                new Identifier(Name), 
                new Description("A laptop interface for browsing cases."), 
                conversation: conversation, 
                commands: [useCommand],
                examination: req => new Examination("A laptop computer. You can use it to browse your cases.")
            );
            character.IsPlayerVisible = true;
            return character;
        }

        private Conversation CreateCaseConversation()
        {
            var cases = LoadCases();
            var paragraphs = new List<Paragraph>();

            // Paragraph 0: Main menu
            var mainMenuResponses = new List<Response>();
            mainMenuResponses.Add(new Response("List Cases", new GoTo(1)));
            
            paragraphs.Add(new Paragraph("What would you like to do?", "MainMenu") { Responses = mainMenuResponses.ToArray() });

            // Paragraph 1: List cases header
            var listText = "Available Cases:\n";
            foreach (var c in cases)
                listText += $"{c.id}) {c.title} - Reward: {c.reward}\n";
            
            var listResponses = new List<Response>();
            int caseIndex = 2; // start of case-specific paragraphs
            foreach (var c in cases)
            {
                listResponses.Add(new Response($"View {c.id}", new GoTo(caseIndex)));
                caseIndex += 2; // each case takes 2 paragraphs (view + start)
            }
            listResponses.Add(new Response("Back", new GoTo(0)));
            
            paragraphs.Add(new Paragraph(listText + "\nChoose a case or go back.", "CaseList") { Responses = listResponses.ToArray() });

            // Paragraphs 2+: For each case, create view + start
            int currentIndex = 2;
            foreach (var caseData in cases)
            {
                var nextIndex = currentIndex + 1;

                // View case paragraph
                var viewText = $"{caseData.title}\n\n{caseData.description}\n\nReward: {caseData.reward}";
                var viewResponses = new Response[]
                {
                    new Response("Start Case", new GoTo(nextIndex)),
                    new Response("Back", new GoTo(1))
                };
                paragraphs.Add(new Paragraph(viewText, $"View_{caseData.id}") { Responses = viewResponses });

                // Start case paragraph - saves case when entered
                var startText = $"Case '{caseData.title}' started. Return to your investigation.";
                var startResponses = new Response[]
                {
                    new Response("Back", new GoTo(0))
                };
                var startPara = new Paragraph(startText, $"Start_{caseData.id}") { Responses = startResponses };
                startPara.Action = g => NetAF.MyGame.GameState.AddActiveCase(caseData.id);
                paragraphs.Add(startPara);

                currentIndex += 2;
            }

            return new Conversation(paragraphs.ToArray());
        }

        private List<CaseEntry> LoadCases()
        {
            try
            {
                var casesPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Cases", "cases.json");
                if (!File.Exists(casesPath))
                    return new List<CaseEntry>();

                var json = File.ReadAllText(casesPath);
                return JsonSerializer.Deserialize<List<CaseEntry>>(json) ?? new List<CaseEntry>();
            }
            catch
            {
                return new List<CaseEntry>();
            }
        }
    }
}
