namespace darts.Core.Games;

public class HighScoreGame : Game
{
    protected override int GetPlayerScore(int player)
    {
        if(Score.TryGetPlayerScore(player, out var score))
            return score;

        return 0;
    }
    public HighScoreGame(string[] players) : base(new DartScore(players))
    {
        OnScoreChanged += (cell) => ExecuteOnTotalScoreChanged(cell.player);
    }
}
