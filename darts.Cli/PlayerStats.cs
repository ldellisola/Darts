using System.Text;
using Spectre.Console;

namespace Darts.Cli;

public class PlayerStats
{

    private static readonly Table Table = new();
    public static readonly Panel Panel = new(Table);

    public static void Initialize(ClassicGame game)
    {
        Table.AddColumn("Possible Throws");
        Table.ShowHeaders = false;
        Table.NoBorder();
        Panel.Header("Possible throws");

        game.OnScoreChanged += (tuple) =>
        {
            Panel.Header($"Possible throws for {game.GetPlayerName(tuple.player)}");
            Table.Rows.Clear();
            foreach (var possibleThrow in game.GetPossibleThrows(tuple.player, tuple.value))
            {
                Table.AddRow(new StringBuilder().AppendJoin(" + ", possibleThrow.Select(t => t.Multiplier switch
                    {
                        1 => $"{t.Score}".Trim(),
                        _ => $"{t.Score}x{t.Multiplier}".Trim()
                    })).ToString());
            }
        };
    }
}
