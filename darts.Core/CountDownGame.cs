using darts.Entities;

namespace darts.Core;

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
        return maxScore - Score.GetPlayerScore(player);
    }
    private void CheckWinner()
    {
        var winners = Score.Players
            .WithIndex()
            .Where((player) => Score.GetPlayerScore(player.Index) == maxScore)
            .Select(t=> t.Index)
            .ToArray();

        if (winners.Any())
        {
            Winner = winners.First();
        }
    }
}
