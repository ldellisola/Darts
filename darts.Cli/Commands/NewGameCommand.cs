using System.ComponentModel;
using System.Globalization;
using darts.Core;
using darts.Core.Games;
using Spectre.Console;
using Spectre.Console.Cli;

namespace darts.Cli.Commands;

public class NewGameSettings : CommandSettings
{
    [CommandOption("-g|--game <GAME>")]
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
        var gameName = settings.Game ?? GetGame();
        var players = settings.Players ?? GetPlayers();
        AnsiConsole.MarkupLine($"Starting [green]{gameName}[/]");

        var game = gameName.ToLowerInvariant() switch
        {
            "201" => Start201(players),
            "501" => Start501(players),
            "knockout" => StartKnockout(players),
            "high score" => StartHighScore(players),
            _ => throw new NotSupportedException($"Game {gameName} is not supported")
        };


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
    private static Game StartHighScore(string[] players)
    {
        var game = new HighScoreGame(players);
        DartBoard.Initialize(game,players);
        return game;
    }


    private static Game StartKnockout(string[] players)
    {
        var game = new KnockoutGame(players);
        DartBoard.Initialize(game,players);
        return game;
    }
    private static Game Start201(string[] players)
    {
        var _scores = new DartScore(players);
        var game = new CountDownGame(_scores,201);
        DartBoard.Initialize(game,players);
        return game;
    }

    private static Game Start501(string[] players)
    {
        var _scores = new DartScore(players);
        var game = new CountDownGame(_scores,501);
        DartBoard.Initialize(game,players);
        return game;
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
    private static string GetGame()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .PageSize(10)
                .Title("Select a game")
                .AddChoices(new[] { "201", "501", "Knockout", "High Score"})
        );
    }
}
