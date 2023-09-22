using Darts.Entities;

namespace Darts.Core.Games;

public class KnockoutGame : Game
{
    private readonly int _dropLast;

    public event Action<ScoreCell>? OnPlayerEliminated;
    private bool[] _playerStatus;

    public KnockoutGame(string[] players, int dropLast) : base(new DartScore(players))
    {
        _dropLast = dropLast;
        _playerStatus = new bool[players.Length];
    }

    public bool IsPlayerEliminated(int player) => _playerStatus[player];

    private void RefreshEliminatedPlayers(int totalRounds, int playersLength)
    {
        _playerStatus = new bool[playersLength];

        for (int round = 0; round < totalRounds; round++)
        {
            var playerScores = new List<(int Player, int Score)>();

            foreach (var (i, _) in Score.Players.WithIndex())
            {
                if (Score.TryGetPlayerScore(i, round, out var score))
                {
                    playerScores.Add((i, score));
                }
                else if (!IsPlayerEliminated(i))
                {
                    playerScores.Clear();
                    break;
                }
            }

            playerScores.Sort((a, b) => a.Score.CompareTo(b.Score)); // Sort in ascending order of scores

            // Take the first N players (lowest scores) to eliminate
            foreach (var eliminated in playerScores.Take(Math.Min(_dropLast, playerScores.Count-1)))
            {
                OnPlayerEliminated?.Invoke(new ScoreCell(round, eliminated.Player, eliminated.Score.ToString()));
                _playerStatus[eliminated.Player] = true;
            }
        }

        if (_playerStatus.Count(t=> !t) == 1)
            Winner = _playerStatus.ToList().IndexOf(false);
    }

    public override void Consume(ConsoleKeyInfo keyInfo)
    {
        base.Consume(keyInfo);
        if (keyInfo.Key == ConsoleKey.Enter)
        {
            RefreshEliminatedPlayers(Score.TotalRounds, Score.Players.Length);
        }
    }

    protected override void GoToNextPlayer()
    {
        do
        {
            base.GoToNextPlayer();
        }
        while (IsPlayerEliminated(Score.CurrentRaw.player) && Score.CurrentRaw.value is null);
    }

    protected override void GoToPreviousPlayer()
    {
        do
        {
            base.GoToPreviousPlayer();
        }
        while (IsPlayerEliminated(Score.CurrentRaw.player) && Score.CurrentRaw.value is null);
    }


    protected override void GoToNextRound()
    {
        base.GoToNextRound();
        if (IsPlayerEliminated(Score.CurrentRaw.player) && Score.CurrentRaw.value is null)
        {
            base.GoToPreviousRound();
        }
    }

    protected override int GetPlayerScore(int player)
    {
        // throw new NotSupportedException("Knockout game does not support player score");
        return 0;
    }

    protected override int ExecuteOnTotalScoreChanged(int player)
    {
        return 0;
    }

    protected override Dictionary<string, object?> GetGameState()
        => new()
           {
               {"DropLast", _dropLast},
           } ;
}
