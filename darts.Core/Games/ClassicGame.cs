namespace Darts.Core.Games;

public class ClassicGame : Game
{
    private readonly int maxScore;

    public ClassicGame(string[] players, int maxScore) : base(new DartScore(players))
    {
        this.maxScore = maxScore;
        OnScoreChanged += (cell) => ExecuteOnTotalScoreChanged(cell.player);
    }

    protected override int GetPlayerScore(int player)
    {
        if(Score.TryGetPlayerScore(player, out var score))
            return maxScore - score;
        return maxScore;
    }

    protected override Dictionary<string, object?> GetGameState()
        => new()
           {
               {"MaxScore", maxScore}
           } ;

    private bool CheckWinner()
    {
        var winners = Score.Players
            .WithIndex()
            .Where(player => Score.TryGetPlayerScore(player.Index, out var score) && score == maxScore)
            .Select(t=> t.Index)
            .ToArray();

        if (winners.Any())
        {
            Winner = winners.First();
            return true;
        }

        return false;
    }

    protected override void CreateNewRound()
    {
        if (CheckWinner() is false)
        {
            base.CreateNewRound();
        }
    }

}
