using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Darts.Cli.Commands;

public class KnockoutGameSettings : NewGameSettings
{
    [ CommandOption("-d|--drop-last <DropLast>") ]
    [ DefaultValue(1) ]
    public int? DropLast { get; set; }
}

public class KnockoutGameCommand : BaseCommand<KnockoutGameSettings>
{
    protected override (Game, Table, Panel?) InitializeGame(KnockoutGameSettings settings)
    {
        settings.DropLast ??= GetDropLast();
        AnsiConsole.MarkupLine($"Starting [green]Knockout darts with {settings.DropLast} drop last[/] with [green]{settings.Players!.Length}[/] players");

        var game = new KnockoutGame(settings.Players, settings.DropLast.Value);
        DartBoard.Initialize(game, settings.Players);
        return (game, DartBoard.Table, null);
    }

    private static int GetDropLast()
        => AnsiConsole.Prompt(new TextPrompt<int>("How many players to drop each round?"));
}
