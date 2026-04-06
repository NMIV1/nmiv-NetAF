using NetAF.Assets;
using NetAF.Assets.Characters;
using NetAF.Conversations;
using NetAF.Conversations.Instructions;
using NetAF.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace NetAF.Game.Assets.Regions.Condo.Items
{
    /// A computer NPC that lets you browse and start cases via conversation
    public class CaseComputer : IAssetTemplate<NonPlayableCharacter>
    {
        public const string Name = "Computer";

        private record CaseEntry(string id, string title, string description, string reward);

        public NonPlayableCharacter Instantiate()
        {
            var conversation = CreateCaseConversation();
            return new NonPlayableCharacter(new Identifier(Name), new Description("A laptop interface for browsing cases."), conversation: conversation, 
                examination: req => new Examination("A laptop computer. You can talk to it to browse cases."));
        }

        private Conversation CreateCaseConversation()
        {
            var cases = LoadCases();
            var paragraphs = new List<Paragraph>();

            // Paragraph 0: Main menu
            var mainMenuResponses = new List<Response>();
            mainMenuResponses.Add(new Response("List Cases", new Jump(1)));
            mainMenuResponses.Add(new Response("Exit Computer", null)); // null ends conversation
            
            paragraphs.Add(new Paragraph("What would you like to do?", "MainMenu") { Responses = mainMenuResponses.ToArray() });

            // Paragraph 1: List cases header
            var listText = "Available Cases:\n";
            foreach (var c in cases)
                listText += $"{c.id}) {c.title} - Reward: {c.reward}\n";
            
            var listResponses = new List<Response>();
            int caseIndex = 2; // start of case-specific paragraphs
            foreach (var c in cases)
            {
                listResponses.Add(new Response($"View {c.id}", new Jump(caseIndex)));
                caseIndex += 2; // each case takes 2 paragraphs (view + start)
            }
            listResponses.Add(new Response("Back", new Jump(0)));
            
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
                    new Response("Start Case", new Jump(nextIndex)),
                    new Response("Back", new Jump(1))
                };
                paragraphs.Add(new Paragraph(viewText, $"View_{caseData.id}") { Responses = viewResponses });

                // Start case paragraph - saves case when entered
                var startText = $"Case '{caseData.title}' started. Return to your investigation.";
                var startResponses = new Response[]
                {
                    new Response("Back", new Jump(0))
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
