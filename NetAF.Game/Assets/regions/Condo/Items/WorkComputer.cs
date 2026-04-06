using NetAF.Assets;
using NetAF.Assets.Locations;
using NetAF.Commands;
using NetAF.Utilities;
using NetAF.Narratives;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace NetAF.Game.Assets.Regions.Condo.Items
{
    public class WorkComputer : IAssetTemplate<Item>
    {
        public const string Name = "Work Computer";

        private record CaseEntry(string id, string title, string description, string reward);

        public Item Instantiate()
        {
            // Commands exposed by the Work Computer: list cases and start a case by id.
            var commands = new CustomCommand[]
            {
                new CustomCommand(new CommandHelp("Computer Cases", "List cases on the laptop."), true, true, (game, args) =>
                {
                    var casesPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Cases", "cases.json");
                    var output = "Available Cases:\n";

                    try
                    {
                        if (File.Exists(casesPath))
                        {
                            var json = File.ReadAllText(casesPath);
                            var list = JsonSerializer.Deserialize<List<CaseEntry>>(json) ?? new List<CaseEntry>();
                            foreach (var c in list)
                                output += $"{c.id}) {c.title} - Reward: {c.reward}\n";
                        }
                        else
                        {
                            output += "(no cases file found)\n";
                        }
                    }
                    catch
                    {
                        output += "(failed to load cases)\n";
                    }

                    return new Reaction(ReactionResult.Inform, output);
                }),

                new CustomCommand(new CommandHelp("StartCase", "Start a case by id."), true, true, (game, args) =>
                {
                    if (args == null || args.Length == 0)
                        return new Reaction(ReactionResult.Error, "Usage: startcase <id>");

                    NetAF.MyGame.GameState.CurrentCaseId = args[0];
                    return new Reaction(ReactionResult.Inform, $"Started case {args[0]}");
                })
            };

            var item = new Item(new Identifier(Name), new Description("A battered laptop where you check your Cases."), true, commands, interaction: i =>
            {
                // Default interaction shows the list of cases (same as the Computer Cases command)
                var casesPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Cases", "cases.json");
                var output = "Available Cases:\n";

                try
                {
                    if (File.Exists(casesPath))
                    {
                        var json = File.ReadAllText(casesPath);
                        var list = JsonSerializer.Deserialize<List<CaseEntry>>(json) ?? new List<CaseEntry>();
                        foreach (var c in list)
                            output += $"{c.id}) {c.title} - Reward: {c.reward}\n";
                    }
                    else
                    {
                        output += "(no cases file found)\n";
                    }
                }
                catch
                {
                    output += "(failed to load cases)\n";
                }

                return new Interaction(InteractionResult.NoChange, i, output);
            });

            return item;
        }
    }
}
