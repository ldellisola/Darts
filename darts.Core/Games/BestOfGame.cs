using Darts.Entities.GameState;

namespace Darts.Core.Games;

public class BestOfGame : DartsGame<BestOfGame>
{
    public int Rounds { get; }
    public bool IsTied { get; private set; }

    public BestOfGame(IReadOnlyCollection<string> players, bool isTournament, int rounds) : base(players, isTournament)
    {
        Rounds = rounds;
    }

    public BestOfGame(GameState state) : base(state)
    {
        Rounds = int.TryParse(state.GameSpecific["Rounds"].ToString(), out var rounds)
            ? rounds
            : 3;
    }

    public override GameState Export()
    {
        var state = base.Export();
        state.GameSpecific["Rounds"] = Rounds;
        return state;
    }

    protected override void CreateNewRound()
    {
        if (TotalRounds < Rounds || IsTied)
            base.CreateNewRound();
    }

    public override int GetPlayerScore(int player)
    {
        var roundsWon = 0;
        for (var round = 0; round < TotalRounds; round++)
        {
            var scores = Players
                .Select((_, i) => Score.TryGetComputedScore(i, round, out var score) ? int.Parse(score!) : 0)
                .ToArray();

            if (scores[player] is not 0 && scores[player] == scores.Max())
                roundsWon++;
        }

        return roundsWon;
    }


    protected override int? SelectWinner()
    {
        IsTied = false;
        var playerScores = Players.WithIndex()
            .Select(t => GetPlayerScore(t.Index))
            .ToArray();

        var maxWins = (TotalRounds / 2) + 1;
        var maxScore = playerScores.WithIndex().MaxBy(t=> t.Value);

        if (maxScore.Value >= maxWins)
            return maxScore.Index;

        var remainingRounds = Math.Min(0,Rounds - TotalRounds);

        var possibleWinners = Players
            .WithIndex()
            .Where(t =>
                playerScores
                    .WithIndex()
                    .All(r => r.Index == t.Index || playerScores[t.Index] >= (r.Value + remainingRounds))
            )
            .Select(t=> (t.Index, Score: base.GetPlayerScore(t.Index)))
            .OrderByDescending(t=> t.Score)
            .ToArray();


        if (possibleWinners.Length is 0)
            return null;

        possibleWinners = possibleWinners
            .TakeWhile(t => t.Score == possibleWinners.First().Score)
            .ToArray();

        if (possibleWinners.Length is 1)
            return possibleWinners.First().Index;

        IsTied = true;
        return null;
    }
}
