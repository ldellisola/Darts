namespace Darts.Core.Games;

public class BestOfGame : Game
{
    private readonly int _rounds;

    public BestOfGame(int rounds, string[] players) : base(new DartScore(players))
    {
        _rounds = rounds;
        OnScoreChanged += _ => UpdateAllPlayerScores();
    }

    protected override int GetPlayerScore(int player)
    {
        var roundsWon = 0;
        for (var round = 0; round < Score.TotalRounds; round++)
        {
            var bestScore = (Score: 0, Player: -1);
            for (var p = 0; p < Score.Players.Length; p++)
            {
                if (Score.TryGetPlayerScore(p, round, out var score) && score > bestScore.Score)
                {
                    bestScore = (score, p);
                }
            }
            if (bestScore.Player == player)
                roundsWon++;
        }

        return roundsWon;
    }

    private void UpdateAllPlayerScores()
    {
        var roundsWon = Score.Players.Select((_, i) => ExecuteOnTotalScoreChanged(i)).WithIndex().ToArray();
        var maxWins = (_rounds / 2) + 1;

        foreach (var (i, score) in roundsWon)
        {
            if (score >= maxWins)
            {
                Winner = i;
                break;
            }

            var remaining = _rounds - roundsWon.Sum(t=> t.Value);
            var canAnyPlayerSurpass = false;
            for (var j = 0; j < roundsWon.Length; j++)
            {
                if (i != j && roundsWon[j].Value + remaining > roundsWon[i].Value)
                {
                    canAnyPlayerSurpass = true;
                    break;
                }
            }

            if (!canAnyPlayerSurpass)
            {
                Winner = i;
                // break;
            }
        }
    }

    public override void CreateNewRound()
    {
        if (Score.TotalRounds < _rounds)
        {
            base.CreateNewRound();
        }
    }

    protected override Dictionary<string, object?> GetGameState()
        => new()
        {
            {"Rounds", _rounds}
        };
}
