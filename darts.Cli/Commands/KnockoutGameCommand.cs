using System.ComponentModel;
using Darts.Entities.GameState;
using Spectre.Console;
using Spectre.Console.Cli;
using YamlDotNet.Serialization;

namespace Darts.Cli.Commands;

public class KnockoutGameSettings : NewGameSettings
{
    [ CommandOption("-d|--drop-last <DropLast>") ]
    [ DefaultValue(1) ]
    public int DropLast { get; set; }
}

public class KnockoutGameCommand : BaseCommand<KnockoutGameSettings,KnockoutGame>
{

    public KnockoutGameCommand(ISerializer serializer) : base(serializer)
    {
    }

    protected override KnockoutGame InitializeGame(KnockoutGameSettings settings)
        => new(settings.Players!,settings.IsTournament,settings.DropLast);

    protected override KnockoutGame InitializeGame(GameState state)
        => new(state);

    private Table _scoreTable = null!;
    private bool _isUiInitialized;

    protected override void DrawGame()
    {
        if (!_isUiInitialized)
        {
            _isUiInitialized = true;
            _scoreTable = new Table();
            Layout.SplitColumns(
                new Layout("game", new Panel(_scoreTable).Header($"Knockout game dropping [bold]{Game.DropLast}[/] players per round")).Ratio(3)
            );
        }

        if (Game.Players.Count != _scoreTable.Columns.Count - 1)
        {
            _scoreTable = Game.Players.Aggregate(
                new Table().AddColumn("Round"),
                (t, player) => t.AddColumn(player)
            );
            Layout.GetLayout("game").Update(new Panel(_scoreTable).Header($"Knockout game dropping [bold]{Game.DropLast}[/] players per round"));
            _scoreTable.ShowFooters();
        }

        while (_scoreTable.Rows.Count < Game.TotalRounds)
            _scoreTable.AddRow(Enumerable.Repeat(new Markup(""), Game.Players.Count));

        for (var p = 0; p < Game.Players.Count; p++)
        {
            for (var r = 0; r < Game.TotalRounds; r++)
            {
                _scoreTable.UpdateCell(r, 0, $"[bold]{r + 1}[/]");
                if (p == Game.CurrentPlayer && r == Game.CurrentRound)
                {
                    string? score;
                    if (ShowRawScore)
                        Game.Score.TryGetRawScore(p, r, out score);
                    else
                        Game.Score.TryGetComputedScore(p, r, out score);

                    if (Game.IsPlayerEliminated(p))
                        _scoreTable.UpdateCell(r, p + 1, $"[red][reverse]{score ?? "   "}[/][/]");
                    else if (Game.Winner == p)
                        _scoreTable.UpdateCell(r, p + 1, $"[green][reverse]{score ?? "   "}[/][/]");
                    else
                        _scoreTable.UpdateCell(r, p + 1, $"[reverse]{score ?? "   "}[/]");
                }
                else
                {
                    Game.Score.TryGetComputedScore(p, r, out var score);
                    if (Game.IsPlayerEliminated(p))
                        _scoreTable.UpdateCell(r, p + 1, $"[red]{score ?? "   "}[/]");
                    else if (Game.Winner == p)
                        _scoreTable.UpdateCell(r, p + 1, $"[green]{score ?? "   "}[/]");
                    else
                        _scoreTable.UpdateCell(r, p + 1, $"{score ?? "   "}");
                }
            }
            _scoreTable.Columns[1 + p].Header = new Text(Game.Players[p]);
        }
    }
}
