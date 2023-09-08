using Spectre.Console;

namespace Darts.Cli.Commands;

public class UnlimitedGameCommand: BaseCommand<NewGameSettings>
{

    protected override (Game, Table, Panel?) InitializeGame(NewGameSettings settings)
    {
        AnsiConsole.MarkupLine($"Playing [green] unlimited rounds[/] with [green]{settings.Players!.Length}[/] players");
        var game = new HighScoreGame(settings.Players, 1000);
        DartBoard.Initialize(game, settings.Players);
        return (game, DartBoard.Table, null);
    }
}
