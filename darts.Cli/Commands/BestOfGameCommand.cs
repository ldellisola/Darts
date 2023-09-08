using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Darts.Cli.Commands;

public class BestOfGameSettings : NewGameSettings
{
    [ CommandOption("-r|--rounds <ROUNDS>") ]
    [ DefaultValue(3) ]
    public int? Rounds { get; set; }
}

public class BestOfGameCommand : BaseCommand<BestOfGameSettings>
{

    protected override (Game, Table, Panel?) InitializeGame(BestOfGameSettings settings)
    {
        settings.Rounds ??= GetRounds();
        AnsiConsole.MarkupLine($"Playing [green]best of {settings.Rounds} rounds [/] with [green]{settings.Players!.Length}[/] players");
        var game = new BestOfGame(settings.Rounds.Value,settings.Players);
        DartBoard.Initialize(game, settings.Players);
        return (game, DartBoard.Table, null);
    }
    private static int GetRounds()
        => AnsiConsole.Ask<int>("How many rounds?");
}
