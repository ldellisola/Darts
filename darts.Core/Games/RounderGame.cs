namespace Darts.Core.Games;

public class RounderGame : Game
{
    private readonly int _rounds;

    public RounderGame(int rounds, string[] players) : base(new DartScore(players))
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
        for (var player = 0; player < Score.Players.Length; player++)
        {
            ExecuteOnTotalScoreChanged(player);
        }
    }

    protected override void CreateNewRound()
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
