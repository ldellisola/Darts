namespace Darts.Core.Games;

public class TotalerGame : Game
{
    private readonly int _rounds;
    protected override int GetPlayerScore(int player)
    {
        if(Score.TryGetPlayerScore(player, out var score))
            return score;

        return 0;
    }

    protected override Dictionary<string, object?> GetGameState()
        => new()
           {
               {"Rounds", _rounds}
           };

    public TotalerGame(string[] players, int rounds) : base(new DartScore(players))
    {
        _rounds = rounds;
        OnScoreChanged += (cell) => ExecuteOnTotalScoreChanged(cell.player);
    }

    protected override void CreateNewRound()
    {
        if (Score.TotalRounds < _rounds)
        {
           base.CreateNewRound();
        }
    }
}
