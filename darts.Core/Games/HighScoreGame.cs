using Darts.Entities.GameState;

namespace Darts.Core.Games;

public class HighScoreGame : DartsGame<HighScoreGame>
{
    public int Rounds { get; }
    public bool IsTied { get; private set; }

    public HighScoreGame(IReadOnlyCollection<string> players, bool isTournament, int rounds) : base(players, isTournament)
    {
        Rounds = rounds;
    }

    public HighScoreGame(GameState state) : base(state)
    {
        Rounds = int.Parse(state.GameSpecific["Rounds"].ToString()!);
    }

    public override GameState Export()
    {
        var state = base.Export();
        state.GameSpecific["Rounds"] = Rounds;
        return state;
    }

    protected override int? SelectWinner()
    {
        IsTied = false;
        if (TotalRounds < Rounds)
            return null;

        var scores = Players
            .Select((_, p) => GetPlayerScore(p))
            .ToArray();

        var bestScore = scores.Max();

        if (scores.Count(t => t == bestScore) is 1)
            return scores.WithIndex().First(t => t.Value == bestScore).Index;

        IsTied = true;
        return null;
    }

    protected override void CreateNewRound()
    {
        if (TotalRounds < Rounds || IsTied)
            base.CreateNewRound();
    }
}
