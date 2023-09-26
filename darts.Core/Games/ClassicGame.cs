using Darts.Entities;
using Darts.Entities.GameState;

namespace Darts.Core.Games;

public class ClassicGame : DartsGame<ClassicGame>
{
    public int Goal { get; }
    public ClassicGame(IReadOnlyCollection<string> players, int goal, bool isTournament) : base(players, isTournament)
    {
        Goal = goal;
    }

    public ClassicGame(GameState state) : base(state)
    {
        Goal = int.Parse(state.GameSpecific["Goal"].ToString()!);
    }

    public override GameState Export()
    {
        var state = base.Export();
        state.GameSpecific.Add("Goal", Goal);
        return state;
    }

    protected override int? SelectWinner()
    {
        return Players
            .WithIndex()
            .Where(player => GetPlayerScore(player.Index) is 0)
            .Select(t => t.Index)
            .Cast<int?>()
            .FirstOrDefault();
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

    public override int GetPlayerScore(int player) => Goal - (Score.TryGetPlayerScore(player, out var score) ? score : 0);
}
