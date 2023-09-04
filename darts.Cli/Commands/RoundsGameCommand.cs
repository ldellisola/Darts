using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Darts.Cli.Commands;

public class RoundsGameSettings : NewGameSettings
{
    [CommandOption("-r|--rounds <ROUNDS>")]
    [DefaultValue(3)]
    public int? Rounds { get; set; }
}

public class RoundsGameCommand : Command<RoundsGameSettings>
{

    public override int Execute(CommandContext context, RoundsGameSettings settings)
    {
        settings.Players ??= GetPlayers();
        settings.Rounds ??= GetRounds();

        AnsiConsole.MarkupLine($"Starting [green]Rounds darts to {settings.Rounds}[/] with [green]{settings.Players.Length}[/] players");
        var game = new RoundsGame(settings.Players, settings.Rounds.Value);
        DartBoard.Initialize(game,settings.Players);
        game.Start();
        var liveDisplay = AnsiConsole.Live(DartBoard.Table);

        liveDisplay.Start(ct =>
        {
            ct.Refresh();

            while (true)
            {
                ct.Refresh();
                var ch = Console.ReadKey(true);
                if (ch is {KeyChar: 'q'})
                {
                    break;
                }

                game.Consume(ch);
            }
        });

        return 1;
    }
    private static int GetRounds() => AnsiConsole.Prompt(new TextPrompt<int>("How many rounds?"));
    private static string[] GetPlayers()
    {
        AnsiConsole.MarkupLine("Add players to the game. Type 'done' when finished.");
        var players = new List<string>();
        while (true)
        {
            var readLine = Console.ReadLine();
            if (readLine is null)
                continue;
            if (readLine.ToLowerInvariant() is "done")
                break;
            players.Add(readLine);
        }

        return players.ToArray();
    }
}
