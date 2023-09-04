using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Darts.Cli.Commands;

public class ClassicGameSettings : NewGameSettings
{
    [CommandOption("-s|--score <SCORE>")]
    [DefaultValue(201)]
    public int? Score { get; set; }
}

public class ClassicGameCommand : Command<ClassicGameSettings>
{
    public override int Execute(CommandContext context, ClassicGameSettings settings)
    {
        settings.Players ??= GetPlayers();
        settings.Score ??= GetScore();

        AnsiConsole.MarkupLine($"Starting [green]Classic darts to {settings.Score}[/] with [green]{settings.Players.Length}[/] players");
        var game = new ClassicGame(settings.Players, settings.Score.Value);
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

    private static int GetScore()
        => AnsiConsole.Prompt(new TextPrompt<int>("What is the score to play to?"));

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
