using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Darts.Cli.Commands;

public class RoundsGameSettings : NewGameSettings
{
    [ CommandOption("-r|--rounds <ROUNDS>") ]
    [ DefaultValue(3) ]
    public int? Rounds { get; set; }
}

public class RoundsGameCommand : BaseCommand<RoundsGameSettings>
{
    protected override (Game, Table) InitializeGame(RoundsGameSettings settings)
    {
        settings.Rounds ??= GetRounds();
        AnsiConsole.MarkupLine($"Starting [green]Rounds darts to {settings.Rounds}[/] with [green]{settings.Players!.Length}[/] players");
        var game = new RoundsGame(settings.Players, settings.Rounds.Value);
        DartBoard.Initialize(game, settings.Players);
        return (game, DartBoard.Table);
    }

    private static int GetRounds() => AnsiConsole.Prompt(new TextPrompt<int>("How many rounds?"));
}
