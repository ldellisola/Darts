using System.ComponentModel;
using System.Globalization;
using Darts.Entities.GameState;
using Spectre.Console;
using Spectre.Console.Cli;
using YamlDotNet.Serialization;

namespace Darts.Cli.Commands;

public class BestOfGameSettings : NewGameSettings
{
    [ CommandOption("-r|--rounds <ROUNDS>") ]
    [ DefaultValue(3) ]
    public int Rounds { get; set; }
}

public class BestOfGameCommand : BaseCommand<BestOfGameSettings, BestOfGame>
{
    private Table _scoreTable = null!;
    private bool _isUiInitialized;
    public BestOfGameCommand(ISerializer serializer) : base(serializer)
    {
    }

    protected override BestOfGame InitializeGame(BestOfGameSettings settings)
        => new(settings.Players!, settings.IsTournament, settings.Rounds);

    protected override BestOfGame InitializeGame(GameState state)
        => new(state);

    protected override void DrawGame()
    {
        if (!_isUiInitialized)
        {
            _isUiInitialized = true;
            _scoreTable = new Table();
            Layout.SplitColumns(
                new Layout("game", new Panel(_scoreTable).Header($"Best of [bold]{Game.Rounds}[/] rounds")).Ratio(3)
            );
        }

        if (Game.Players.Count != _scoreTable.Columns.Count - 1)
        {
            _scoreTable = Game.Players.Aggregate(
                new Table().AddColumn("Round", t => t.Footer("[bold]Rounds Won[/]")),
                (t, player) => t.AddColumn(player, c => c.Footer(0.ToString(CultureInfo.InvariantCulture)))
            );
            Layout.GetLayout("game").Update(new Panel(_scoreTable).Header($"Best of [bold]{Game.Rounds}[/] rounds"));
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

                    _scoreTable.UpdateCell(r, p + 1, $"[reverse]{score ?? "   "}[/]");
                }
                else
                {
                    _scoreTable.UpdateCell(r, p + 1, Game.Score.TryGetComputedScore(p, r, out var score) ? score ?? "   " : "  ");
                }
            }

            _scoreTable.Columns[1 + p].Header = new Text(Game.Players[p]);
            _scoreTable.Columns[1 + p].Footer = Game.Winner is not null && Game.Winner.Value == p
                ? new Markup("[bold][green]WINNER[/][/]")
                : (Spectre.Console.Rendering.IRenderable)new Markup(Game.GetPlayerScore(p).ToString(CultureInfo.InvariantCulture));
        }
    }
}
