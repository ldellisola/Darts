using Darts.Entities.GameState;
using Stateless;

namespace Darts.Core.Games;


public abstract class DartsGame<T>
{
    protected enum States
    {
        WaitingForInput,
        AddPlayer,
        Finished
    }

    private readonly bool _isTournament;
    protected StateMachine<States, ConsoleKey> Machine { get; }
    public int CurrentPlayer { get; private set; }
    public int CurrentRound { get; private set; }
    public int TotalRounds => Score.Rounds;
    public List<string> Players { get; }
    public DartScore2 Score { get; private set; }
    public int? Winner { get; protected set; }

    private readonly string _gameType = typeof(T).Name;

    public DartsGame(IReadOnlyCollection<string> players, bool isTournament)
    : this()
    {
        _isTournament = isTournament;
        Players = players.ToList();
        Score = new DartScore2(players.Count);
        Score.NewRound();
    }

    public DartsGame(GameState state)
    : this()
    {
        _isTournament = state.Common.IsTournament;
        Players = state.Common.Players.ToList();
        Winner = state.Common.Winner;
        Score = DartScore2.Import(state.Common.Scores, state.Common.Players.Length, state.Common.Rounds);
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private DartsGame()
    {
        Machine = new StateMachine<States, ConsoleKey>(States.WaitingForInput);

        Machine.Configure(States.WaitingForInput)
            .InternalTransition(ConsoleKey.RightArrow, NextPlayer)
            .InternalTransition(ConsoleKey.LeftArrow, PreviousPlayer)
            .InternalTransition(ConsoleKey.DownArrow, NextRound)
            .InternalTransition(ConsoleKey.UpArrow, PreviousRound)
            .InternalTransition(ConsoleKey.Enter, CreateNewRound)
            .Permit(ConsoleKey.Q, States.Finished)
            .InternalTransition(ConsoleKey.D1, () => Score!.UpdatePartialScore(CurrentPlayer, CurrentRound, '1'))
            .InternalTransition(ConsoleKey.D2, () => Score!.UpdatePartialScore(CurrentPlayer, CurrentRound, '2'))
            .InternalTransition(ConsoleKey.D3, () => Score!.UpdatePartialScore(CurrentPlayer, CurrentRound, '3'))
            .InternalTransition(ConsoleKey.D4, () => Score!.UpdatePartialScore(CurrentPlayer, CurrentRound, '4'))
            .InternalTransition(ConsoleKey.D5, () => Score!.UpdatePartialScore(CurrentPlayer, CurrentRound, '5'))
            .InternalTransition(ConsoleKey.D6, () => Score!.UpdatePartialScore(CurrentPlayer, CurrentRound, '6'))
            .InternalTransition(ConsoleKey.D7, () => Score!.UpdatePartialScore(CurrentPlayer, CurrentRound, '7'))
            .InternalTransition(ConsoleKey.D8, () => Score!.UpdatePartialScore(CurrentPlayer, CurrentRound, '8'))
            .InternalTransition(ConsoleKey.D9, () => Score!.UpdatePartialScore(CurrentPlayer, CurrentRound, '9'))
            .InternalTransition(ConsoleKey.D0, () => Score!.UpdatePartialScore(CurrentPlayer, CurrentRound, '0'))
            .InternalTransition(ConsoleKey.Backspace, () => Score!.DeleteScore(CurrentPlayer, CurrentRound))
            .InternalTransition(ConsoleKey.Multiply, () => Score!.UpdatePartialScore(CurrentPlayer, CurrentRound, '*'))
            .InternalTransition(ConsoleKey.Add, () => Score!.UpdatePartialScore(CurrentPlayer, CurrentRound, '+'))
            .Permit(ConsoleKey.P, States.AddPlayer)
            ;

        Machine.Configure(States.AddPlayer)
            .OnEntry(AddPlayer)
            .InternalTransition(ConsoleKey.Backspace, () => Players![^1] = Players[^1][..^1])
            .InternalTransition(ConsoleKey.A, () => Players![^1] += "a")
            .InternalTransition(ConsoleKey.B, () => Players![^1] += "b")
            .InternalTransition(ConsoleKey.C, () => Players![^1] += "c")
            .InternalTransition(ConsoleKey.D, () => Players![^1] += "d")
            .InternalTransition(ConsoleKey.E, () => Players![^1] += "e")
            .InternalTransition(ConsoleKey.F, () => Players![^1] += "f")
            .InternalTransition(ConsoleKey.G, () => Players![^1] += "g")
            .InternalTransition(ConsoleKey.H, () => Players![^1] += "h")
            .InternalTransition(ConsoleKey.I, () => Players![^1] += "i")
            .InternalTransition(ConsoleKey.J, () => Players![^1] += "j")
            .InternalTransition(ConsoleKey.K, () => Players![^1] += "k")
            .InternalTransition(ConsoleKey.L, () => Players![^1] += "l")
            .InternalTransition(ConsoleKey.M, () => Players![^1] += "m")
            .InternalTransition(ConsoleKey.N, () => Players![^1] += "n")
            .InternalTransition(ConsoleKey.O, () => Players![^1] += "o")
            .InternalTransition(ConsoleKey.P, () => Players![^1] += "p")
            .InternalTransition(ConsoleKey.Q, () => Players![^1] += "q")
            .InternalTransition(ConsoleKey.R, () => Players![^1] += "r")
            .InternalTransition(ConsoleKey.S, () => Players![^1] += "s")
            .InternalTransition(ConsoleKey.T, () => Players![^1] += "t")
            .InternalTransition(ConsoleKey.U, () => Players![^1] += "u")
            .InternalTransition(ConsoleKey.V, () => Players![^1] += "v")
            .InternalTransition(ConsoleKey.W, () => Players![^1] += "w")
            .InternalTransition(ConsoleKey.X, () => Players![^1] += "x")
            .InternalTransition(ConsoleKey.Y, () => Players![^1] += "y")
            .InternalTransition(ConsoleKey.Z, () => Players![^1] += "z")
            .InternalTransition(ConsoleKey.Spacebar, () => Players![^1] += " ")
            .Permit(ConsoleKey.Enter, States.WaitingForInput)
            ;
    }

    protected virtual void AddPlayer()
    {
        Players.Add("");
        Score.AddPlayer();
    }

    public virtual bool Consume(ConsoleKey key)
    {
        if (Machine.CanFire(key))
        {
            Machine.Fire(key);
            Winner = SelectWinner();
        }

        return Machine.State != States.Finished;
    }

    protected virtual void CreateNewRound()
    {
        if(Players.WithIndex().Aggregate(true, (b, tuple) => b && !Score.TryGetRawScore(tuple.Index, TotalRounds - 1, out _)))
            return;
        Score.NewRound();
        NextRound();
    }


    /// <summary>
    /// It will return the id of the winner, if there's one
    /// </summary>
    /// <returns></returns>
    protected abstract int? SelectWinner();

    /// <summary>
    /// It returns the score of a player
    /// </summary>
    /// <param name="player">Index of the player</param>
    /// <returns></returns>
    public virtual int GetPlayerScore(int player)
        => Score.TryGetPlayerScore(player, out var score) ? score : 0;

    protected virtual void PreviousRound()
    {
        if (CurrentRound > 0)
            CurrentRound--;
    }

    protected virtual void NextRound()
    {
        if (CurrentRound < TotalRounds - 1)
            CurrentRound++;
    }

    protected virtual void NextPlayer()
    {
        if(++CurrentPlayer >= Players.Count)
            CurrentPlayer = 0;
    }

    protected virtual void PreviousPlayer()
    {
        if(--CurrentPlayer < 0)
            CurrentPlayer = Players.Count - 1;
    }

    public virtual GameState Export()
    {
        return new GameState(_gameType,
            new Common(
                Players.ToArray(),
                Score.Export(),
                TotalRounds,
                Winner,
                _isTournament,
                DateTime.Now
                ),
            new Dictionary<string, object>()
            );
    }
}

