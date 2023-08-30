using System.ComponentModel;
using System.Globalization;
using darts.Core;
using Spectre.Console;
using Spectre.Console.Cli;

namespace darts.Cli.Commands;

public class NewGameSettings : CommandSettings
{
    [CommandOption( "-g|--game <GAME>")]
    [DefaultValue(null)]
    public string? Game { get; set; }

    [CommandOption("-p|--players <PLAYERS>")]
    [DefaultValue(null)]
    public string[]? Players { get; set; }
}
public class NewGameCommand : Command<NewGameSettings>
{
    private readonly Table _table = new();

    public override int Execute(CommandContext context, NewGameSettings settings)
    {
        // var game = settings.Game ?? GetName();
        var players = settings.Players ?? GetPlayers();

        var _scores = new DartScore(players);
        var game = new CountDownGame(_scores,201);

        game.OnScoreChanged += (tuple) => UpdateTableCell(tuple.round, tuple.player, tuple.value);
        game.OnRoundAdded += AddTableRow;
        game.OnScoreSelected += cell => SelectCell(cell.round,cell.player, cell.value);
        game.OnScoreDeselected += cell => DeselectCell(cell.round,cell.player, cell.value);
        game.OnTotalScoreChanged += UpdateScore;
        game.OnPlayerWon +=  MarkPlayerAsWinner;

        AnsiConsole.MarkupLine($"Starting [green]{game}[/]");
        AnsiConsole.MarkupLine($"Players: {string.Join(", ", players)}");

        _ = _table.AddColumn("Round");
        foreach (var player in players)
        {
            _ = _table.AddColumn(player);
        }

        _ = _table.AddRow(players.Select(_ => new Markup("   ")).Prepend(new Markup("[bold]Score[/]")));

        game.Start();

        var liveDisplay = AnsiConsole.Live(_table);

        liveDisplay.Start(ct =>
        {
            ct.Refresh();

            while (true)
            {
                ct.Refresh();
                var ch = Console.ReadKey(true);
                if (ch is {KeyChar: 'q'})
                    break;
                game.Consume(ch);
            }
        });

        return 1;
    }
    private void MarkPlayerAsWinner(int column)
    {
        _ = _table.UpdateCell(_table.Rows.Count-1, column + 1, $"[bold][green]WINNER[/][/]");
    }
    private void DeselectCell(int row, int column, int? value)
    {
        _ = _table.UpdateCell(row, column + 1, value?.ToString(CultureInfo.InvariantCulture) ?? "  ");
    }
    private void SelectCell(int row, int column, int? value)
    {
        _ = _table.UpdateCell(row, column + 1, $"[reverse]{value?.ToString(CultureInfo.InvariantCulture) ?? "  "}[/]");

    }
    private void AddTableRow(int row, int size)
    {
        if (row >= _table.Rows.Count)
        {
            _ = _table.InsertRow(row-1,Enumerable.Repeat(new Markup("  "), size).Prepend(new Markup($"[bold]{row}[/]")));
        }
    }

    private void UpdateTableCell(int row, int column, int? value)
    {
        _ = _table.UpdateCell(row, column + 1, $"[reverse]{value?.ToString(CultureInfo.InvariantCulture) ?? "  "}[/]");
    }

    private void UpdateScore(int column, int score)
    {
        _ = _table.UpdateCell(_table.Rows.Count-1, column + 1, $"[bold]{score}[/]");
    }

    private static string[] GetPlayers()
    {
        AnsiConsole.MarkupLine("Add players to the game. Type 'done' when finished.");
        var players = new List<string>();
        while (true)
        {
            var readLine = Console.ReadLine();
            if (readLine is null)
                continue;
            if (readLine.ToLowerInvariant() is "done")
                break;
            players.Add(readLine);
        }

        return players.ToArray();
    }
    private static string GetName()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("What is the name of the game?");
            var readLine = Console.ReadLine();
            if (readLine is not null)
                return readLine;
        }
    }
}
