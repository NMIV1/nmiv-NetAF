using System;
using System.Collections.Generic;
using System.Linq;
using NetAF.Commands;
using NetAF.Interpretation;
using NetAF.Logic;
using NetAF.Logic.Modes;
using NetAF.Assets.Locations;
using NetAF.Rendering;
using NetAF.Rendering.FrameBuilders;
using NetAF.Utilities;

namespace NetAF.Game.Modes
{
    // A simple interactive mode for "using" a computer: exposes custom commands from player-visible examinables
    public sealed class ComputerInteractionMode : IGameMode
    {
        public IInterpreter Interpreter { get; }

        public GameModeType Type => GameModeType.Interactive;

        public ComputerInteractionMode(IInterpreter sceneInterpreter)
        {
            // compose a computer-specific interpreter first, then fall back to the scene interpreter
            Interpreter = new InputInterpreter(new ComputerModeInterpreter(), sceneInterpreter);
        }

        public void Render(NetAF.Logic.Game game)
        {
            CommandHelp[] commands = FrameProperties.CommandListType switch
            {
                CommandListType.All => Interpreter?.GetContextualCommandHelp(game).ToArray() ?? Array.Empty<CommandHelp>(),
                CommandListType.Minimal => Interpreter?.GetContextualCommandHelp(game).ToArray() ?? Array.Empty<CommandHelp>(),
                CommandListType.None => Array.Empty<CommandHelp>(),
                _ => throw new NotImplementedException()
            };

            var frame = game.Configuration.FrameBuilders.GetFrameBuilder<ISceneFrameBuilder>()
                .Build(game.Overworld.CurrentRegion.CurrentRoom, ViewPoint.Create(game.Overworld.CurrentRegion), game.Player, commands, FrameProperties.ShowMapInScenes, FrameProperties.KeyType, game.Configuration.DisplaySize);
            game.Configuration.Adapter.RenderFrame(frame);
        }

        // A small interpreter that exposes custom commands (including an ExitComputer command) regardless of the current mode.
        private sealed class ComputerModeInterpreter : IInterpreter
        {
            public CommandHelp[] SupportedCommands { get; } = Array.Empty<CommandHelp>();

            private static bool TryFindCommand(string input, CustomCommand[] commands, out CustomCommand command, out string matchingInput)
            {
                var match = Array.Find(commands, x => IsMatch(input, x.Help.Command));
                if (match != null)
                {
                    command = match;
                    matchingInput = match.Help.Command;
                    return true;
                }

                match = Array.Find(commands, x => IsMatch(input, x.Help.Shortcut));
                if (match != null)
                {
                    command = match;
                    matchingInput = match.Help.Shortcut;
                    return true;
                }

                command = null;
                matchingInput = string.Empty;
                return false;
            }

            private static bool IsMatch(string input, string command)
            {
                if (!input.StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
                    return false;

                return input.Equals(command, StringComparison.InvariantCultureIgnoreCase) || (input.Length > command.Length && input[command.Length] == ' ');
            }

            public InterpretationResult Interpret(string input, NetAF.Logic.Game game)
            {
                if (string.IsNullOrEmpty(input))
                    return InterpretationResult.Fail;

                input = StringUtilities.PreenInput(input);

                List<CustomCommand> commands = new();

                // add an exit computer command
                var exit = new CustomCommand(new CommandHelp("ExitComputer", "Exit the computer"), true, true, (g, args) =>
                {
                    // return to the normal scene mode
                    var sceneInterpreter = g.Configuration.InterpreterProvider.Find(typeof(SceneMode));
                    g.ChangeMode(new SceneMode(sceneInterpreter));
                    return new Reaction(ReactionResult.GameModeChanged, string.Empty);
                });

                commands.Add(exit);

                // compile commands from player visible examinables (like the WorkComputer)
                foreach (var examinable in game.GetAllPlayerVisibleExaminables().Where(x => x.Commands != null))
                    commands.AddRange(examinable.Commands.Where(x => x.IsPlayerVisible || x.InterpretIfNotPlayerVisible));

                if (commands.Count == 0)
                    return InterpretationResult.Fail;

                if (!TryFindCommand(input, commands.ToArray(), out var command, out var matchingInput))
                    return InterpretationResult.Fail;

                input = input[matchingInput.Length..];

                var clonedCommand = command.Clone() as CustomCommand;
                clonedCommand.Arguments = StringUtilities.PreenInput(input).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new InterpretationResult(true, clonedCommand);
            }

            public CommandHelp[] GetContextualCommandHelp(NetAF.Logic.Game game)
            {
                List<CommandHelp> help = new();

                // expose exit computer command first
                help.Add(new CommandHelp("ExitComputer", "Exit the computer"));

                foreach (var examinable in game.GetAllPlayerVisibleExaminables().Where(x => x.Commands != null))
                    help.AddRange(examinable.Commands.Where(x => x.IsPlayerVisible).Select(c => c.Help));

                return help.ToArray();
            }
        }
    }
}
