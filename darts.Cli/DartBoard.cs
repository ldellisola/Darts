using Spectre.Console;

namespace Darts.Cli;

public static class DartBoard
{
    public static Table Table { get; } = new();

    public static void Initialize(KnockoutGame game, string[] players)
    {
        game.OnPlayerEliminated += (player) => _ = Table.UpdateCell(player.round, player.player + 1, $"[red]{player.value}[/]");
        game.OnScoreChanged += (tuple) => UpdateTableCell(tuple.round, tuple.player, tuple.value);
        game.OnRoundAdded += AddTableRow;
        game.OnScoreSelected += cell => SelectCell(cell.round,cell.player, cell.value is not null && game.IsPlayerEliminated(cell.player) ? $"[red]{cell.value}[/]": cell.value);
        game.OnScoreDeselected += cell => DeselectCell(cell.round,cell.player, cell.value is not null && game.IsPlayerEliminated(cell.player) ? $"[red]{cell.value}[/]": cell.value);

        _ = Table.AddColumn("Round");
        foreach (var player in players)
        {
            _ = Table.AddColumn(player);
        }
    }

    public static void Initialize(Game game, string[] players)
    {
        game.OnScoreChanged += (tuple) => UpdateTableCell(tuple.round, tuple.player, tuple.value);
        game.OnRoundAdded += AddTableRow;
        game.OnScoreSelected += cell => SelectCell(cell.round,cell.player, cell.value);
        game.OnScoreDeselected += cell => DeselectCell(cell.round,cell.player, cell.value);
        game.OnPlayerWon += MarkPlayerAsWinner;

        _ = Table.AddColumn("Round");
        foreach (var player in players)
        {
            _ = Table.AddColumn(player);
        }
        game.OnTotalScoreChanged += UpdateScore;
        _ = Table.AddRow(players.Select(_ => new Markup("   ")).Prepend(new Markup("[bold]Score[/]")));
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

    private static void UpdateScore(int column, int score) => Table.UpdateCell(Table.Rows.Count - 1, column + 1, $"[bold]{score}[/]");
}
