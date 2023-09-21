using Spectre.Console;

namespace Darts.Cli.Commands;

public class NewClassicGameCommand : NewBaseCommand<ClassicGameSettings>
{
    private Table table = null!;
    private bool isUIInitialized;
    protected override DartsGame InitializeGame(ClassicGameSettings settings) => new (settings.Players!);
    protected override void DrawGame()
    {
        if (!isUIInitialized)
        {
            isUIInitialized = true;
            table = Game.Players.Aggregate(
                                           new Table().AddColumn("Round", t=> t.Footer("[bold]Total Score[/]")),
                                           (t, player) => t.AddColumn(player, c => c.Footer("0"))
                                           );
            Layout.SplitColumns(new Layout("game", new Panel(table).Header("Classic")));
            table.ShowFooters();
        }

        for(var p = 0; p < Game.Players.Count; p++)
        {
            if (p >= table.Columns.Count)
                table.AddColumn(Game.Players[p]);
            else if (p < table.Columns.Count)
                table.Columns[p].Header = new Text(Game.Players[p]);

            table.Columns[p].Footer = new Markup(Game.Score.TryGetPlayerScore(p, out var score) ? score.ToString() : "0");
        }

        while (table.Rows.Count - 1 < Game.TotalRounds)
            table.InsertRow(Game.TotalRounds - 1, Enumerable.Repeat(new Markup(""), Game.Players.Count));

        for (var p = 0; p < Game.Players.Count; p++)
        {
            for (var r = 0; r < Game.TotalRounds; r++)
            {
                table.UpdateCell(r, 0, $"[bold]{r + 1}[/]");
                if (p == Game.CurrentPlayer && r == Game.CurrentRound)
                {
                    string? score;
                    if (ShowRawScore)
                        Game.Score.TryGetRawScore(p, r, out score);
                    else
                        Game.Score.TryGetComputedScore(p, r, out score);

                    table.UpdateCell(r, p + 1, $"[reverse]{score ?? "   "}[/]");
                }
                else
                {
                    table.UpdateCell(r, p + 1, Game.Score.TryGetComputedScore(p, r, out var score) ? score ?? "   " : "  ");
                }
            }
            table.UpdateCell(Game.TotalRounds, p + 1, $"[bold]{(Game.Score.TryGetPlayerScore(p,out var totalScore) ? totalScore : "   ")}[/]");
        }
    }
}
