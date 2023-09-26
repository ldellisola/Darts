using Darts.Entities.GameState;

namespace Darts.Core.Games;

public class HighScoreGame : DartsGame<HighScoreGame>
{
    public int Rounds { get; }

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
        if (TotalRounds < Rounds)
            return null;

        return Players
            .WithIndex()
            .Select(t=> (Player: t.Index, Score: GetPlayerScore(t.Index)))
            .OrderByDescending(t=> t.Score)
            .FirstOrDefault()
            .Player;
    }

    protected override void CreateNewRound()
    {
        if (TotalRounds < Rounds)
            base.CreateNewRound();
    }
}
