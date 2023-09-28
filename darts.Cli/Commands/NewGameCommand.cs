using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using YamlDotNet.Serialization;

namespace Darts.Cli.Commands;

public class NewGameSettings : CommandSettings
{
    [CommandOption("-p|--players <PLAYERS>")]
    [DefaultValue(null)]
    public string[]? Players { get; set; }

    [CommandOption("-t|--tournament <TYPE>")]
    [DefaultValue(false)]
    public bool IsTournament { get; set; }
}
public class NewGameCommand : Command<NewGameSettings>
{
    private readonly ISerializer _serializer;

    public NewGameCommand(ISerializer serializer) => _serializer = serializer;

    public override int Execute(CommandContext context, NewGameSettings settings)
    {
        var gameName = GetGame();
        AnsiConsole.MarkupLine($"Starting [green]{gameName}[/]");

        return gameName.ToLowerInvariant() switch
        {
            "classic" => new ClassicGameCommand(_serializer).Execute(context, new ClassicGameSettings { Players = settings.Players }),
            "knockout" => new KnockoutGameCommand(_serializer).Execute(context, new KnockoutGameSettings { Players = settings.Players }),
            "highscore" => new HighScoreGameCommand(_serializer).Execute(context, new HighScoreGameSettings { Players = settings.Players }),
            "best-of" => new BestOfGameCommand(_serializer).Execute(context,new BestOfGameSettings {Players = settings.Players }),
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
