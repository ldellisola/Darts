using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Darts.Cli.Commands;

public class TotalerGameSettings : NewGameSettings
{
    [ CommandOption("-r|--rounds <ROUNDS>") ]
    [ DefaultValue(3) ]
    public int? Rounds { get; set; }
}

public class TotalerGameCommand : BaseCommand<TotalerGameSettings>
{
    protected override (Game, Table) InitializeGame(TotalerGameSettings settings)
    {
        settings.Rounds ??= GetRounds();
        AnsiConsole.MarkupLine($"Starting [green]Rounds darts to {settings.Rounds}[/] with [green]{settings.Players!.Length}[/] players");
        var game = new TotalerGame(settings.Players, settings.Rounds.Value);
        DartBoard.Initialize(game, settings.Players);
        return (game, DartBoard.Table);
    }

    private static int GetRounds() => AnsiConsole.Prompt(new TextPrompt<int>("How many rounds?"));
}
