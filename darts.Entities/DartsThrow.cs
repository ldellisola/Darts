using System.Text;

namespace Darts.Entities;

public record DartsThrow
{
    private (int Score, int Multiplier)[] _darts;

    public DartsThrow(List<(int Score, int Multiplier)> darts)
    {
        _darts = darts.ToArray();
    }

    public override string ToString()
    {
        return new StringBuilder().AppendJoin(" + ", _darts.Select(t => t.Multiplier switch
        {
            1 => $"{t.Score}".Trim(),
            _ => $"{t.Score}x{t.Multiplier}".Trim()
        })
            ).ToString();
    }
}
