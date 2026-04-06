using NetAF.Assets;
using NetAF.Assets.Characters;
using NetAF.Assets.Locations;
using NetAF.Commands;
using NetAF.Conversations;
using NetAF.Logic.Modes;
using NetAF.MyGame;
using NetAF.Utilities;

namespace NetAF.Game.Assets.Regions.BurnoutDistrict.Rooms
{
    /// <summary>
    /// The dive bar where Silas can be found. The player's dialogue options depend on
    /// which node connections were unlocked in Silas's apartment, which evidence items
    /// the player carries, and skill check results.
    /// </summary>
    public class DiveBar : IAssetTemplate<Room>
    {
        private const string Name = "Dive Bar";
        private const string Description = "A grimy dive bar with no sign above the door — just a flickering neon tube that might have been a word once. Inside: sticky floors, smoke-stained ceiling tiles, and the low murmur of people who came here to stop thinking. The bartender doesn't look up when you walk in. Nobody does. Silas Vance is hunched in the corner booth, nursing something brown in a smudged glass. He hasn't noticed you yet.";

        public Room Instantiate()
        {
            var room = new Room(Name, Description, [new Exit(Direction.South)]);

            // Hidden NPC used as the IConverser for dynamically built conversations.
            NonPlayableCharacter silasConverser = null;

            var interrogateCommand = new CustomCommand(
                new CommandHelp("Talk", "Approach Silas and start a conversation.", CommandCategory.Conversation, "L"),
                true,
                false,
                (game, args) =>
                {
                    GameVars.Instance.IsReturningFromMode = true;
                    var conversation = SilasConversationBuilder.Build(game.Player);
                    silasConverser = new NonPlayableCharacter(
                        new Identifier("Silas"),
                        new Description("Silas Vance, hunched in the corner booth."),
                        conversation: conversation
                    );
                    silasConverser.IsPlayerVisible = false;

                    conversation.Next(game);
                    var interpreter = game.Configuration.InterpreterProvider.Find(typeof(ConversationMode));
                    game.ChangeMode(new ConversationMode(silasConverser, interpreter));
                    return new Reaction(ReactionResult.Silent, "");
                }
            );

            var silas = new NonPlayableCharacter(
                new Identifier("Silas"),
                new Description("A gaunt man in his early 30s folded into the corner booth like he's trying to disappear into the vinyl. Dark circles cut deep under his eyes. He rotates his glass slowly on the table — not drinking, just holding. His jacket is too big for him now. He's lost weight recently, and not on purpose."),
                commands: [interrogateCommand],
                examination: req => new Examination("Silas Vance. Up close, he's worse than the case file suggested. The tremor in his hands is constant — not alcohol, something deeper. His eyes move in quick, nervous sweeps across the room, tracking exits and strangers. He hasn't touched his drink in the five minutes you've been watching. He's here because this is where he goes. Not to drink. Just to sit somewhere that isn't that apartment.")
            );
            silas.IsPlayerVisible = true;
            room.AddCharacter(silas);

            return room;
        }
    }
}
