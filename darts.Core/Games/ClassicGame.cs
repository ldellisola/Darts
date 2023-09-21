using Darts.Entities;

namespace Darts.Core.Games;

public class ClassicGame : DartsGame
{
    public int Goal { get; }


    private void CheckWinner()
    {
        Winner = Players
            .WithIndex()
            .Where(player => Score.TryGetPlayerScore(player.Index, out var score) && score == Goal)
            .Select(t => t.Index)
            .FirstOrDefault();
    }
    public ClassicGame(IReadOnlyCollection<string> players, int goal) : base(players)
    {
        Goal = goal;
    }

    public override bool Consume(ConsoleKey key)
    {
        var result = base.Consume(key);
        CheckWinner();
        return result;
    }

    public bool TryGetPossibleThrows(int player, int round, out List<DartsThrow> throws)
    {
        throws = new();
        if (player >= Players.Count || player < 0 || round >= Score.Rounds || round < 0)
            return false;

        var shotNumber = Score.TryGetRawScore(player, round, out var score) ? score?.Split('+').Length-1 : 0;
        var currentScore = Score.TryGetPlayerScore(player, out var playerScore) ? playerScore : 0;
        throws = DartsCombinations.FindCombinations(currentScore,Goal, shotNumber ?? 0, 15);
        return throws.Count > 0;
    }

    public int GetPlayerScore(int player) => Goal - (Score.TryGetPlayerScore(player, out var score) ? score : 0);
}
