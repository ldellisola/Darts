using System.ComponentModel;
using System.Globalization;
using darts.Core;
using darts.Entities;
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
        var game = settings.Game ?? GetName();
        var players = settings.Players ?? GetPlayers();

        var _scores = new DartScore(players.Length);

        _scores.OnScoreChanged += (tuple) => UpdateTableCell(tuple.round, tuple.player, tuple.value);
        _scores.OnNewRound += tuple => AddTableRow(tuple.row,tuple.size);
        _scores.OnSelectionChanged += (tuple) => SetTableCursor(tuple.previousCell, tuple.newCell);
        _scores.OnTotalScoreChanged += tuple => UpdateScore(tuple.player, tuple.score);

        AnsiConsole.MarkupLine($"Starting [green]{game}[/]");
        AnsiConsole.MarkupLine($"Players: {string.Join(", ", players)}");

        _table.AddColumn("Round");
        foreach (var player in players)
        {
            _table.AddColumn(player);
        }

        _table.AddRow(players.Select(_ => new Markup("   ")).Prepend(new Markup("[bold]Score[/]")));

        var liveDisplay = AnsiConsole.Live(_table);

        _scores.NewRound();
        liveDisplay.Start(ct =>
        {
            ct.Refresh();

            while (true)
            {
                var ch = Console.ReadKey(true);

                switch (ch)
                {
                    case { Key: ConsoleKey.LeftArrow}:
                    {
                        _scores.PreviousPlayer();
                        break;
                    }
                    case { Key: ConsoleKey.RightArrow}:
                    {
                        _scores.NextPlayer();
                        break;
                    }
                    case { Key: ConsoleKey.UpArrow}:
                    {
                        _scores.PreviousRound();
                        break;
                    }
                    case { Key: ConsoleKey.DownArrow}:
                    {
                        _scores.NextRound();
                        break;
                    }
                    case { Key: ConsoleKey.Enter}:
                    {
                        _scores.NewRound();
                        break;
                    }
                    case { KeyChar: >= '0' and <= '9'}:
                    {
                        _scores.UpdatePartialScore(ch.KeyChar - '0');
                        break;
                    }
                    case { Key: ConsoleKey.Backspace}:
                    {
                        _scores.DeleteScore();
                        break;
                    }
                    case { Key: ConsoleKey.Escape}:
                    {
                        return;
                    }
                }


                ct.Refresh();
            }
        });

        return 1;
    }
    private void SetTableCursor(ScoreCell previousCell, ScoreCell newCell)
    {
        _ = _table.UpdateCell(previousCell.round, previousCell.player + 1, previousCell.value?.ToString(CultureInfo.InvariantCulture) ?? "  ");
        _ = _table.UpdateCell(newCell.round, newCell.player + 1, $"[reverse]{newCell.value?.ToString(CultureInfo.InvariantCulture) ?? "  "}[/]");
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
