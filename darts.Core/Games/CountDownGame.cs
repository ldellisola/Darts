namespace darts.Core.Games;

public class CountDownGame : Game
{
    private readonly int maxScore;

    public CountDownGame(DartScore score, int maxScore) : base(score)
    {
        this.maxScore = maxScore;
        OnScoreChanged += (cell) => ExecuteOnTotalScoreChanged(cell.player);
    }

    public override void Consume(ConsoleKeyInfo keyInfo)
    {
        base.Consume(keyInfo);
        if (keyInfo.Key == ConsoleKey.Enter)
            CheckWinner();
    }
    protected override int GetPlayerScore(int player)
    {
        if(Score.TryGetPlayerScore(player, out var score))
            return maxScore - score;
        return maxScore;
    }
    private void CheckWinner()
    {
        var winners = Score.Players
            .WithIndex()
            .Where(player => Score.TryGetPlayerScore(player.Index, out var score) && score == maxScore)
            .Select(t=> t.Index)
            .ToArray();

        if (winners.Any())
        {
            Winner = winners.First();
        }
    }
}
