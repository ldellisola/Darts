using Spectre.Console;
using Spectre.Console.Cli;

namespace Darts.Cli.Commands;

public abstract class BaseCommand<T> : Command<T> where T : NewGameSettings
{
    public override int Execute(CommandContext context, T settings)
    {
        settings.Players ??= GetPlayers();

        var (game, table) = InitializeGame(settings);
        game.Start();
        AnsiConsole.Live(table)
                   .Start(ct =>
                                      {
                                          ct.Refresh();

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

        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, $"game_{DateTime.Now}.json"), game.ToJson());

       // AnsiConsole.Write(
       //                   new Panel( new JsonText(game.ToJson()))
       //                       .Header("Game State")
       //                       .Expand()
       //                       .RoundedBorder()
       //                   );
        return 0;
    }

    protected abstract (Game, Table) InitializeGame(T settings);

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
