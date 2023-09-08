using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Darts.Cli.Commands;

public class NewGameSettings : CommandSettings
{
    [CommandOption("-p|--players <PLAYERS>")]
    [DefaultValue(null)]
    public string[]? Players { get; set; }
}
public class NewGameCommand : Command<NewGameSettings>
{
    public override int Execute(CommandContext context, NewGameSettings settings)
    {
        var gameName = GetGame();
        AnsiConsole.MarkupLine($"Starting [green]{gameName}[/]");

        return gameName.ToLowerInvariant() switch
        {
            "classic" => new ClassicGameCommand().Execute(context, new ClassicGameSettings { Players = settings.Players }),
            "knockout" => new KnockoutGameCommand().Execute(context, new KnockoutGameSettings { Players = settings.Players }),
            "highscore" => new HighScoreGameCommand().Execute(context, new HighScoreGameSettings { Players = settings.Players }),
            "best-of" => new BestOfGameCommand().Execute(context,new BestOfGameSettings {Players = settings.Players }),
            "unlimited" => new UnlimitedGameCommand().Execute(context, new NewGameSettings { Players = settings.Players }),
            _ => throw new NotSupportedException($"Game {gameName} is not supported")
        };
    }
    private static string GetGame()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .PageSize(10)
                .Title("Select a game")
                .AddChoices("Classic", "Knockout", "Rounds")
        );
    }
}
