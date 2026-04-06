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

namespace NetAF.Game.Assets.Items
{
    /// <summary>
    /// A phone item the player carries. UsePhone opens a conversation to view active cases.
    /// </summary>
    public class Phone : IAssetTemplate<Item>
    {
        public const string Name = "Phone";

        public Item Instantiate()
        {
            // Hidden NPC that serves as the IConverser for the phone conversation.
            // The conversation is rebuilt each time UsePhone is invoked to reflect current active cases.
            NonPlayableCharacter phoneNpc = null;

            var usePhoneCommand = new CustomCommand(
                new CommandHelp("UsePhone", "Open your phone to view cases.", CommandCategory.Custom, "Phone"),
                true,
                true,
                (game, args) =>
                {
                    GameVars.Instance.IsReturningFromMode = true;
                    var conversation = BuildPhoneConversation();
                    phoneNpc = new NonPlayableCharacter(
                        new Identifier("Phone"),
                        new Description("Your phone screen."),
                        conversation: conversation
                    );
                    phoneNpc.IsPlayerVisible = false;

                    conversation.Next(game);
                    var interpreter = game.Configuration.InterpreterProvider.Find(typeof(ConversationMode));
                    game.ChangeMode(new ConversationMode(phoneNpc, interpreter));
                    return new Reaction(ReactionResult.Silent, "");
                }
            );

            var phone = new Item(
                new Identifier(Name),
                new Description("Your phone. Use it to review active cases."),
                isTakeable: false,
                commands: [usePhoneCommand],
                examination: req => new Examination("A cracked smartphone. It still works well enough to check your cases.")
            );

            return phone;
        }

        private static Conversation BuildPhoneConversation()
        {
            var cases = GameState.GetActiveCaseDetails();
            var paragraphs = new List<Paragraph>();

            // Paragraph 0: Main menu
            var mainMenuResponses = new List<Response>();
            if (cases.Count > 0)
                mainMenuResponses.Add(new Response("View Cases", new GoTo(1)));

            paragraphs.Add(new Paragraph(
                cases.Count > 0
                    ? "Phone unlocked. What would you like to do?"
                    : "Phone unlocked. No active cases.",
                "MainMenu")
            { Responses = mainMenuResponses.ToArray() });

            if (cases.Count == 0)
                return new Conversation(paragraphs.ToArray());

            // Paragraph 1: Case list
            var listText = "Active Cases:\n";
            var listResponses = new List<Response>();
            int caseParaStart = 2;

            for (int i = 0; i < cases.Count; i++)
            {
                var c = cases[i];
                listText += $"\n  {i + 1}) {c.title}";
                listResponses.Add(new Response($"View {i + 1}", new GoTo(caseParaStart + i)));
            }

            listResponses.Add(new Response("Back", new GoTo(0)));

            paragraphs.Add(new Paragraph(listText, "CaseList") { Responses = listResponses.ToArray() });

            // Paragraphs 2+: One detail paragraph per case
            for (int i = 0; i < cases.Count; i++)
            {
                var c = cases[i];
                var detailText =
                    $"\n" +
                    $"          ═══════════════════════════════\n" +
                    $"                  {c.title}\n" +
                    $"          ═══════════════════════════════\n" +
                    $"\n" +
                    $"          {c.description}\n" +
                    $"\n" +
                    $"          Reward: {c.reward}\n" +
                    (string.IsNullOrEmpty(c.region) ? "" : $"          Location: {c.region}\n") +
                    $"\n" +
                    $"          ═══════════════════════════════";

                var detailResponses = new Response[]
                {
                    new Response("Back", new GoTo(1))
                };

                paragraphs.Add(new Paragraph(detailText, $"Detail_{c.id}") { Responses = detailResponses });
            }

            return new Conversation(paragraphs.ToArray());
        }
    }
}
