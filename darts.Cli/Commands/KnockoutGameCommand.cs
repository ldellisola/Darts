using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Darts.Cli.Commands;

public class KnockoutGameSettings : NewGameSettings
{
    [CommandOption("-d|--drop-last <DropLast>")]
    [DefaultValue(1)]
    public int? DropLast { get; set; }
}

public class KnockoutGameCommand : Command<KnockoutGameSettings>
{
    public override int Execute(CommandContext context, KnockoutGameSettings settings)
    {
        settings.Players ??= GetPlayers();
        settings.DropLast ??= GetDropLast();
        AnsiConsole.MarkupLine(
            $"Starting [green]Knockout darts with {settings.DropLast} drop last[/] with [green]{settings.Players.Length}[/] players");

        var game = new KnockoutGame(settings.Players, settings.DropLast.Value);
        DartBoard.Initialize(game, settings.Players);
        game.Start();
        var liveDisplay = AnsiConsole.Live(DartBoard.Table);

        liveDisplay.Start(ct =>
        {
            ct.Refresh();

            while (true)
            {
                ct.Refresh();
                var ch = Console.ReadKey(true);
                if (ch is { KeyChar: 'q' })
                {
                    break;
                }

                game.Consume(ch);
            }
        });

        return 1;
    }
    private static int GetDropLast()
        => AnsiConsole.Prompt(new TextPrompt<int>("How many players to drop each round?"));
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
