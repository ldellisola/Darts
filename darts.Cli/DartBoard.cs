using Spectre.Console;

namespace Darts.Cli;

public static class DartBoard
{
    public static Table Table { get; } = new();

    public static void Initialize(KnockoutGame game, string[] players)
    {
        game.OnPlayerEliminated += (player) => _ = Table.UpdateCell(player.Round, player.Player + 1, $"[red]{player.Value}[/]");
        game.OnScoreChanged += (tuple) => UpdateTableCell(tuple.Round, tuple.Player, tuple.Value);
        game.OnRoundAdded += AddTableRow;
        game.OnScoreSelected += cell => SelectCell(cell.Round,cell.Player, cell.Value is not null && game.IsPlayerEliminated(cell.Player) ? $"[red]{cell.Value}[/]": cell.Value);
        game.OnScoreDeselected += cell => DeselectCell(cell.Round,cell.Player, cell.Value is not null && game.IsPlayerEliminated(cell.Player) ? $"[red]{cell.Value}[/]": cell.Value);

        _ = Table.AddColumn("Round");
        foreach (var player in players)
        {
            _ = Table.AddColumn(player);
        }
    }

    public static void Initialize(Game game, string[] players)
    {
        game.OnScoreChanged += (tuple) => UpdateTableCell(tuple.Round, tuple.Player, tuple.Value);
        game.OnRoundAdded += AddTableRow;
        game.OnScoreSelected += cell => SelectCell(cell.Round,cell.Player, cell.Value);
        game.OnScoreDeselected += cell => DeselectCell(cell.Round,cell.Player, cell.Value);
        game.OnPlayerWon += MarkPlayerAsWinner;

        _ = Table.AddColumn("Round");
        foreach (var player in players)
        {
            _ = Table.AddColumn(player);
        }
        game.OnTotalScoreChanged += UpdateScore;
        _ = Table.AddRow(players.Select(_ => new Markup("   ")).Prepend(new Markup("[bold]Total Score[/]")));
    }

    private static void MarkPlayerAsWinner(int column) => Table.UpdateCell(Table.Rows.Count - 1, column + 1, $"[bold][green]WINNER[/][/]");
    private static void DeselectCell(int row, int column, string? value) => Table.UpdateCell(row, column + 1, value ?? "  ");
    private static void SelectCell(int row, int column, string? value) => Table.UpdateCell(row, column + 1, $"[reverse]{value ?? "  "}[/]");
    private static void AddTableRow(int row, int size)
    {
        if (row >= Table.Rows.Count)
        {
            _ = Table.InsertRow(row-1,Enumerable.Repeat(new Markup("  "), size).Prepend(new Markup($"[bold]{row}[/]")));
        }
    }

    private static void UpdateTableCell(int row, int column, string? value) => Table.UpdateCell(row, column + 1, $"[reverse]{value ?? "  "}[/]");

    private static void UpdateScore(int column, int score)
        => Table.UpdateCell(Table.Rows.Count - 1, column + 1, $"[bold]{score}[/]");
}
