using Darts.Entities.GameState;

namespace Darts.Core.Games;

public class KnockoutGame : DartsGame<KnockoutGame>
{
    public int DropLast { get; }
    public bool[] PlayerStatus { get; private set; }

    public KnockoutGame(IReadOnlyCollection<string> players, bool isTournament, int dropLast) : base(players, isTournament)
    {
        DropLast = dropLast;
        PlayerStatus = new bool[players.Count];
    }

    public KnockoutGame(GameState state) : base(state)
    {
        PlayerStatus = new bool[state.Common.Players.Length];
        DropLast = int.TryParse(state.GameSpecific["DropLast"].ToString(), out var dropLast) ? dropLast : 1;
    }

    public override GameState Export()
    {
        var state = base.Export();
        state.GameSpecific["DropLast"] = DropLast;
        return state;
    }

    protected override void NextPlayer()
    {
        do
        {
            base.NextPlayer();
        } while (IsPlayerEliminated(CurrentPlayer) && !Score.TryGetRawScore(CurrentPlayer, CurrentRound, out _));
    }

    protected override void PreviousPlayer()
    {
        do
        {
            base.PreviousPlayer();
        } while (IsPlayerEliminated(CurrentPlayer) && !Score.TryGetRawScore(CurrentPlayer, CurrentRound, out _));
    }

    protected override void NextRound()
    {
        base.NextRound();
        if (IsPlayerEliminated(CurrentPlayer) && !Score.TryGetRawScore(CurrentPlayer, CurrentRound, out _))
            PreviousRound();
    }

    protected override void AddPlayer()
    {
        base.AddPlayer();
        PlayerStatus = PlayerStatus.Append(false).ToArray();
        Winner = SelectWinner();
    }

    public bool IsPlayerEliminated(int player) => PlayerStatus[player];

    protected override int? SelectWinner()
    {
        PlayerStatus = new bool[Players.Count];

        for (var round = 0; round < TotalRounds-1; round++)
        {
            var droppedPlayers = Players
                .Select((_, i) => Score.TryGetComputedScore(i, round, out var score) ? int.Parse(score!) : (int?)null)
                .WithIndex()
                .Where(t => t.Value is not null && !IsPlayerEliminated(t.Index))
                .OrderByDescending(t => t.Value)
                .TakeLast(DropLast);

            foreach (var (i,_) in droppedPlayers)
            {
                PlayerStatus[i] = true;
            }
        }

        return PlayerStatus.Count(t => !t) is 1 ? PlayerStatus.WithIndex().First(t => !t.Value).Index : null;
    }
}
