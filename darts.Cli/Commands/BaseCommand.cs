using Spectre.Console;
using Spectre.Console.Cli;

namespace Darts.Cli.Commands;

public abstract class BaseCommand<T> : Command<T> where T : NewGameSettings
{
    public override int Execute(CommandContext context, T settings)
    {
        settings.Players ??= GetPlayers();

        var (game, table, panel) = InitializeGame(settings);

        var layout = new Layout("Root")
            .SplitColumns(
                new Layout("GameBoard", table).Ratio(3),
                new Layout("PlayerStats", Align.Left((panel ?? new("")).Expand()))
            );

        if (panel is null)
            layout["PlayerStats"].Invisible();

        game.Start();
        AnsiConsole.Live(layout)
                   .Start(ct =>
                   {
                                          while(true)
                                          {
                                              ct.Refresh();
                                              var ch = Console.ReadKey(true);
                                              if(ch is { KeyChar: 'q' })
                                              {
                                                  break;
                                              }

                                              game.Consume(ch);
                                          }

                   });
        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, $"game_{DateTime.Now:yy-MM-dd-hh-mm-ss}.json"), game.ToJson());

       // AnsiConsole.Write(
       //                   new Panel( new JsonText(game.ToJson()))
       //                       .Header("Game State")
       //                       .Expand()
       //                       .RoundedBorder()
       //                   );
        return 0;
    }

    protected abstract (Game, Table, Panel?) InitializeGame(T settings);

    private static string[] GetPlayers()
    {
        AnsiConsole.MarkupLine("Add players to the game. Type 'done' when finished.");
        var players = new List<string>();
        while(true)
        {
            var readLine = Console.ReadLine();
            if(readLine is null)
                continue;
            if(readLine.ToLowerInvariant() is "done")
                break;
            players.Add(readLine);
        }

        return players.ToArray();
    }
}
