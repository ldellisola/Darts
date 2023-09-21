using System.ComponentModel;
using System.Globalization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Darts.Cli.Commands;

public class ClassicGameSettings : NewGameSettings
{
    [CommandOption("-s|--score <SCORE>")]
    [DefaultValue(201)]
    public int? Score { get; set; }
}

public class ClassicGameCommand : NewBaseCommand<ClassicGameSettings, ClassicGame>
{
    private Table scoreTable = null!;
    private readonly Table statsTable = new();
    private bool isUIInitialized;
    protected override ClassicGame InitializeGame(ClassicGameSettings settings) => new (settings.Players!, settings.Score!.Value);
    protected override void DrawGame()
    {
        if (!isUIInitialized)
        {
            isUIInitialized = true;
            scoreTable = new Table();
            Layout.SplitColumns(
                new Layout("game", new Panel(scoreTable).Header("Classic Game to [bold]"+Game.Goal+"[/]")).Ratio(3),
                new Layout("stats", new Panel(statsTable.AddColumn("").HideHeaders().NoBorder()).Expand().Header("Possible Throws"))
                );
        }

        if (Game.Players.Count != scoreTable.Columns.Count - 1)
        {
            scoreTable = Game.Players.Aggregate(
                                           new Table().AddColumn("Round", t=> t.Footer("[bold]Total Score[/]")),
                                           (t, player) => t.AddColumn(player, c => c.Footer(Game.Goal.ToString(CultureInfo.InvariantCulture)))
                                           );
            Layout.GetLayout("game").Update(new Panel(scoreTable).Header($"Classic Game to [bold]{Game.Goal}[/]"));
            scoreTable.ShowFooters();
        }

        statsTable.Rows.Clear();
        Layout.GetLayout("stats").Update(new Panel(statsTable).Header($"Possible Throws for [bold]{Game.Players[Game.CurrentPlayer]}[/]").Expand()).Invisible();
        if (Game.TryGetPossibleThrows(Game.CurrentPlayer, Game.CurrentRound, out var dartsThrows))
        {
            Layout.GetLayout("stats").Visible();
            _ = dartsThrows.Aggregate(statsTable, (t, possibleThrow) => t.AddRow(possibleThrow.ToString()));
        }


        for(var p = 0; p < Game.Players.Count; p++)
        {
            scoreTable.Columns[1+p].Header = new Text(Game.Players[p]);
            scoreTable.Columns[1+p].Footer = new Markup(Game.GetPlayerScore(p).ToString(CultureInfo.InvariantCulture));
        }

        while (scoreTable.Rows.Count < Game.TotalRounds)
            scoreTable.AddRow(Enumerable.Repeat(new Markup(""), Game.Players.Count));

        for (var p = 0; p < Game.Players.Count; p++)
        {
            for (var r = 0; r < Game.TotalRounds; r++)
            {
                scoreTable.UpdateCell(r, 0, $"[bold]{r + 1}[/]");
                if (p == Game.CurrentPlayer && r == Game.CurrentRound)
                {
                    string? score;
                    if (ShowRawScore)
                        Game.Score.TryGetRawScore(p, r, out score);
                    else
                        Game.Score.TryGetComputedScore(p, r, out score);

                    scoreTable.UpdateCell(r, p + 1, $"[reverse]{score ?? "   "}[/]");
                }
                else
                {
                    scoreTable.UpdateCell(r, p + 1, Game.Score.TryGetComputedScore(p, r, out var score) ? score ?? "   " : "  ");
                }
            }
        }
    }
}
