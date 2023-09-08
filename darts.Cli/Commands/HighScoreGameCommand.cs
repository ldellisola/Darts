using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Darts.Cli.Commands;

public class HighScoreGameSettings : NewGameSettings
{
    [ CommandOption("-r|--rounds <ROUNDS>") ]
    [ DefaultValue(3) ]
    public int? Rounds { get; set; }
}

public class HighScoreGameCommand : BaseCommand<HighScoreGameSettings>
{
    protected override (Game, Table, Panel?) InitializeGame(HighScoreGameSettings settings)
    {
        settings.Rounds ??= GetRounds();
        AnsiConsole.MarkupLine($"Playing for [green] highest score in {settings.Rounds} rounds[/] with [green]{settings.Players!.Length}[/] players");
        var game = new HighScoreGame(settings.Players, settings.Rounds.Value);
        DartBoard.Initialize(game, settings.Players);
        return (game, DartBoard.Table, null);
    }

    private static int GetRounds() => AnsiConsole.Prompt(new TextPrompt<int>("How many rounds?"));
}
