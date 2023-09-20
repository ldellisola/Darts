using Spectre.Console;
using Spectre.Console.Cli;
using Stateless;
using Stateless.Graph;

namespace Darts.Cli.Commands;

public abstract class NewBaseCommand<T> : Command<T> where T : NewGameSettings
{
    protected Layout Layout { get; } = new("root");
    protected Game Game { get; private set; } = null!;
    protected int PlayersCount => Game.Players.Count;
    protected int CurrentPlayer { get; set; }
    protected int CurrentRound { get; set; }

    public override int Execute(CommandContext context, T settings)
    {
        settings.Players ??= GetPlayers();
        Game = InitializeGame(settings);
        Game.Start();

        AnsiConsole.Live(Layout).Start(ct =>
                                       {
                                           do
                                           {
                                               DrawGame();
                                               ct.Refresh();
                                           }
                                           while(GameLoop(Console.ReadKey(true)));
                                       });
        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, $"game_{DateTime.Now:yy-MM-dd-hh-mm-ss}.json"), Game.ToJson());
        return 0;
    }

    protected virtual bool GameLoop(ConsoleKeyInfo ch)
    {
        switch(ch)
        {
            case { Key: ConsoleKey.LeftArrow }:
            {
                CurrentPlayer--;
                if(CurrentPlayer < 0)
                    CurrentPlayer = PlayersCount - 1;
                break;
            }
            case { Key: ConsoleKey.RightArrow }:
            {
                CurrentPlayer++;
                if(CurrentPlayer >= PlayersCount)
                    CurrentPlayer = 0;

                break;
            }
            case { Key: ConsoleKey.UpArrow }:
            {
                if(CurrentRound > 0)
                    CurrentRound--;
                break;
            }
            case { Key: ConsoleKey.DownArrow }:
            {
                if (CurrentRound < Game.Score.TotalRounds - 1)
                    CurrentRound++;
                break;
            }
            case { Key: ConsoleKey.Spacebar }:
            {
                // OnScoreChanged?.Invoke(Score.CurrentRaw);
                break;
            }
            case { Key: ConsoleKey.Enter }:
            {
                Game.CreateNewRound();
                break;
            }
            case { KeyChar: '+' or '*' }:
            case { KeyChar: >= '0' and <= '9' }:
            {
                Game.UpdatePartialScore(ch.KeyChar);
                break;
            }
            case { Key: ConsoleKey.Backspace }:
            {
                Game.DeleteScore();
                break;
            }
            case { Key: ConsoleKey.Q }:
            {
                return false;
            }
        }

        return true;
    }

    protected abstract Game InitializeGame(T settings);

    protected abstract void DrawGame();

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


    private void CreateStateMachine()
    {
        var game = new StateMachine<State, Trigger>(State.);
    }
}
