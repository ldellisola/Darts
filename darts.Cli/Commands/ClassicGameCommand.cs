using System.ComponentModel;
using System.Globalization;
using Darts.Entities.GameState;
using Spectre.Console;
using Spectre.Console.Cli;
using YamlDotNet.Serialization;

namespace Darts.Cli.Commands;

public class ClassicGameSettings : NewGameSettings
{
    [CommandOption("-s|--score <SCORE>")]
    [DefaultValue(201)]
    public int? Score { get; set; }
}

public class ClassicGameCommand : BaseCommand<ClassicGameSettings, ClassicGame>
{
    private Table _scoreTable = null!;
    private readonly Table _statsTable = new();
    private bool _isUiInitialized;
    protected override ClassicGame InitializeGame(ClassicGameSettings settings) => new(settings.Players!, settings.Score!.Value,settings.IsTournament);
    protected override ClassicGame InitializeGame(GameState state) => new(state);
    protected override void DrawGame()
    {
        if (!_isUiInitialized)
        {
            _isUiInitialized = true;
            _scoreTable = new Table();
            Layout.SplitColumns(
                new Layout("game", new Panel(_scoreTable).Header("Classic Game to [bold]"+Game.Goal+"[/]")).Ratio(3),
                new Layout("stats", new Panel(_statsTable.AddColumn("").HideHeaders().NoBorder()).Expand().Header("Possible Throws"))
                );
        }

        if (Game.Players.Count != _scoreTable.Columns.Count - 1)
        {
            _scoreTable = Game.Players.Aggregate(
                                           new Table().AddColumn("Round", t=> t.Footer("[bold]Total Score[/]")),
                                           (t, player) => t.AddColumn(player, c => c.Footer(Game.Goal.ToString(CultureInfo.InvariantCulture)))
                                           );
            Layout.GetLayout("game").Update(new Panel(_scoreTable).Header($"Classic Game to [bold]{Game.Goal}[/]"));
            _scoreTable.ShowFooters();
        }

        _statsTable.Rows.Clear();
        Layout.GetLayout("stats").Update(new Panel(_statsTable).Header($"Possible Throws for [bold]{Game.Players[Game.CurrentPlayer]}[/]").Expand()).Invisible();
        if (Game.TryGetPossibleThrows(Game.CurrentPlayer, Game.CurrentRound, out var dartsThrows))
        {
            Layout.GetLayout("stats").Visible();
            _ = dartsThrows.Aggregate(_statsTable, (t, possibleThrow) => t.AddRow(possibleThrow.ToString()));
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

            _scoreTable.Columns[1+p].Header = new Text(Game.Players[p]);
            if (Game.Winner is not null && Game.Winner.Value == p)
                _scoreTable.Columns[1 + Game.Winner.Value].Footer = new Markup("[bold][green]WINNER[/][/]");
            else
                _scoreTable.Columns[1+p].Footer = new Markup(Game.GetPlayerScore(p).ToString(CultureInfo.InvariantCulture));
        }
    }
    public ClassicGameCommand(ISerializer serializer) : base(serializer)
    {
    }
}
