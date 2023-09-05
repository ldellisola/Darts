using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Darts.Cli.Commands;

public class RounderGameSettings : NewGameSettings
{
    [ CommandOption("-r|--rounds <ROUNDS>") ]
    [ DefaultValue(3) ]
    public int? Rounds { get; set; }
}

public class RounderGameCommand : BaseCommand<RounderGameSettings>
{

    protected override (Game, Table) InitializeGame(RounderGameSettings settings)
    {
        settings.Rounds ??= GetRounds();
        AnsiConsole.MarkupLine($"Starting [green]Rounds darts to {settings.Rounds}[/] with [green]{settings.Players!.Length}[/] players");
        var game = new RounderGame(settings.Rounds.Value,settings.Players);
        DartBoard.Initialize(game, settings.Players);
        return (game, DartBoard.Table);
    }
    private static int GetRounds()
        => AnsiConsole.Ask<int>("How many rounds?");
}
