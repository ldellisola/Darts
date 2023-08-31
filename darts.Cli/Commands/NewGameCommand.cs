using System.ComponentModel;
using System.Globalization;
using darts.Core;
using Spectre.Console;
using Spectre.Console.Cli;

namespace darts.Cli.Commands;

public class NewGameSettings : CommandSettings
{
    [CommandOption( "-g|--game <GAME>")]
    [DefaultValue(null)]
    public string? Game { get; set; }

    [CommandOption("-p|--players <PLAYERS>")]
    [DefaultValue(null)]
    public string[]? Players { get; set; }
}
public class NewGameCommand : Command<NewGameSettings>
{

    public override int Execute(CommandContext context, NewGameSettings settings)
    {
        // var game = settings.Game ?? GetName();
        var players = settings.Players ?? GetPlayers();

        var _scores = new DartScore(players);
        var game = new CountDownGame(_scores,201);

        AnsiConsole.MarkupLine($"Starting [green]{game}[/]");
        DartBoard.Initialize(game,players);

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
    private static string GetName()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("What is the name of the game?");
            var readLine = Console.ReadLine();
            if (readLine is not null)
                return readLine;
        }
    }
}
