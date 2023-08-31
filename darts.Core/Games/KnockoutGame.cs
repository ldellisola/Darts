using darts.Entities;

namespace darts.Core.Games;

public class KnockoutGame : Game
{

    public event Action<ScoreCell>? OnPlayerEliminated;
    private bool[] _playerStatus;

    public KnockoutGame(string[] players) : base(new DartScore(players))
    {
        _playerStatus = new bool[players.Length];
    }

    public bool IsPlayerEliminated(int player) => _playerStatus[player];

    private void RefreshEliminatedPlayers(int totalRounds, int playersLength)
    {
        _playerStatus = new bool[playersLength];

        for (int round = 0; round < totalRounds; round++)
        {
            var lowestScore = (Player: -1, Score: int.MaxValue);
            foreach (var (i,p) in Score.Players.WithIndex())
            {
                if (Score.TryGetPlayerScore(i, round, out var score))
                {
                    if (score < lowestScore.Score)
                        lowestScore = (i, score);

                }
                else if (!IsPlayerEliminated(i))
                {
                    lowestScore = (-1, int.MaxValue);
                    break;
                }
            }

            if (lowestScore.Player is not -1)
            {
                OnPlayerEliminated?.Invoke(new ScoreCell(round, lowestScore.Player, lowestScore.Score.ToString()));
                _playerStatus[lowestScore.Player] = true;
            }
        }
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
        throw new NotSupportedException("Knockout game does not support player score");
    }
}
