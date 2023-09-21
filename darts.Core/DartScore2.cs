using System.Text;
using Darts.Core.Expression;

namespace Darts.Core;

public class DartScore2
{
    private readonly List<List<string?>> _scores;
    private int _playersCount;
    public int Rounds { get; private set; }
    public DartScore2(int players)
    {
        _playersCount = players;
        _scores = new(Enumerable.Range(0,players).Select(_ => new List<string?>()));
    }

    public void NewRound()
    {
        foreach (var playerScore in _scores)
            playerScore.Add(null);
        Rounds++;
    }

    public void DeleteScore(int player, int round)
    {
        if (_scores[player][round] is not null && _scores[player][round]!.Length > 0)
            _scores[player][round] = _scores[player][round]?[..^1];
    }


    public bool TryGetRawScore(int player, int round, out string? score)
    {
        score = null;
        if (player >= _playersCount  || player < 0 || round >= Rounds || round < 0)
            return false;

        if (_scores[player][round] is null)
            return false;

        score = _scores[player][round]!;
        return true;
    }

    public bool TryGetComputedScore(int player, int round, out string? score)
    {
        score = null;
        if (player >= _playersCount  || player < 0 || round >= Rounds || round < 0)
            return false;

        if (_scores[player][round] is null)
            return false;

        if (!SimpleMathInterpreter.TryResolve(_scores[player][round]!, out var result))
            return false;

        score = result.ToString();
        return true;
    }


    public bool TryGetPlayerScore(int player, out int score)
    {
        var exp = new StringBuilder()
            .AppendJoin('+', _scores[player].Select(s => string.IsNullOrWhiteSpace(s) ? "0" : s.Trim('+')))
            .ToString();

        if (SimpleMathInterpreter.TryResolve(exp, out int result))
        {
            score = result;
            return true;
        }
        score = -1;
        return false;
    }

    public void UpdatePartialScore(int player, int round, char c)
    {
        _scores[player][round] ??= string.Empty;
        _scores[player][round] += c;
    }
    public void AddPlayer()
    {
        _playersCount++;
        _scores.Add(Enumerable.Range(0,Rounds).Select(_ => (string?)null).ToList());
    }
}
