using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Darts.Cli.Commands;

public class ClassicGameSettings : NewGameSettings
{
    [ CommandOption("-s|--score <SCORE>") ]
    [ DefaultValue(201) ]
    public int? Score { get; set; }
}

public class ClassicGameCommand : BaseCommand<ClassicGameSettings>
{
    protected override (Game, Table) InitializeGame(ClassicGameSettings settings)
    {
        settings.Score ??= GetScore();

        AnsiConsole.MarkupLine($"Starting [green]Classic darts to {settings.Score}[/] with [green]{settings.Players!.Length}[/] players");
        var game = new ClassicGame(settings.Players, settings.Score.Value);
        DartBoard.Initialize(game, settings.Players);
        return (game, DartBoard.Table);
    }

    private static int GetScore()
        => AnsiConsole.Prompt(new TextPrompt<int>("What is the score to play to?"));
}
