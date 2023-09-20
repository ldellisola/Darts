using Stateless;

namespace Darts.Core.Games;

public enum GameStates
{
    WaitingForInput,
    UpdatingScore,
    Finished
}

// public enum GameTriggers
// {
//     NextPlayer,
//     PreviousPlayer,
//     NextRound,
//     PreviousRound,
//     NewRound,
//     ScoreUpdate,
//     End,
//     Start
// }

public class DartsGame
{
    private readonly StateMachine<GameStates, ConsoleKey> _machine;
    public int CurrentPlayer { get; private set; }
    public int CurrentRound { get; private set; }
    public int TotalRounds => Score.Rounds;
    public List<string> Players { get; }

    public DartScore2 Score { get; }

    public DartsGame(IReadOnlyCollection<string> players)
    {
        Players = players.ToList();
        Score = new DartScore2(players.Count);
        _machine = new StateMachine<GameStates, ConsoleKey>(GameStates.WaitingForInput);


        _machine.Configure(GameStates.WaitingForInput)
                .InternalTransition(ConsoleKey.RightArrow, NextPlayer)
                .InternalTransition(ConsoleKey.LeftArrow, PreviousPlayer)
                .InternalTransition(ConsoleKey.DownArrow, NextRound)
                .InternalTransition(ConsoleKey.UpArrow, PreviousRound)
                .InternalTransition(ConsoleKey.Enter, CreateNewRound)
                .Permit(ConsoleKey.Q, GameStates.Finished)
                .InternalTransition(ConsoleKey.D1, () => Score.UpdatePartialScore(CurrentPlayer, CurrentRound, '1'))
                .InternalTransition(ConsoleKey.D2, () => Score.UpdatePartialScore(CurrentPlayer, CurrentRound, '2'))
                .InternalTransition(ConsoleKey.D3, () => Score.UpdatePartialScore(CurrentPlayer, CurrentRound, '3'))
                .InternalTransition(ConsoleKey.D4, () => Score.UpdatePartialScore(CurrentPlayer, CurrentRound, '4'))
                .InternalTransition(ConsoleKey.D5, () => Score.UpdatePartialScore(CurrentPlayer, CurrentRound, '5'))
                . InternalTransition(ConsoleKey.D6, () => Score.UpdatePartialScore(CurrentPlayer, CurrentRound, '6'))
                .InternalTransition(ConsoleKey.D7, () => Score.UpdatePartialScore(CurrentPlayer, CurrentRound, '7'))
                . InternalTransition(ConsoleKey.D8, () => Score.UpdatePartialScore(CurrentPlayer, CurrentRound, '8'))
                . InternalTransition(ConsoleKey.D9, () => Score.UpdatePartialScore(CurrentPlayer, CurrentRound, '9'))
                . InternalTransition(ConsoleKey.D0, () => Score.UpdatePartialScore(CurrentPlayer, CurrentRound, '0'))
                . InternalTransition(ConsoleKey.Backspace, () => Score.DeleteScore(CurrentPlayer, CurrentRound))
                . InternalTransition(ConsoleKey, () => Score.UpdatePartialScore(CurrentPlayer, CurrentRound, '*'))


    }

    private void UpdatePartialScore(char c)
    {

    }

    private void CreateNewRound()
    {
        if(Players.WithIndex().Aggregate(true, (b, tuple) => b && !Score.TryGetPlayerScore(tuple.Index, TotalRounds - 1, out _)))
            return;
        Score.NewRound();
        NextRound();
    }

    private void PreviousRound()
    {
        if (CurrentRound > 0)
            CurrentRound--;
    }

    private void NextRound()
    {
        if (CurrentRound < TotalRounds - 1)
            CurrentRound++;
    }

    private void NextPlayer()
    {
        if(++CurrentPlayer >= Players.Count)
            CurrentPlayer = 0;
    }

    private void PreviousPlayer()
    {
        if(--CurrentPlayer < 0)
            CurrentPlayer = Players.Count - 1;
    }
}
