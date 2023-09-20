using System.Text;
using Darts.Core.Expression;
using Darts.Entities;

namespace Darts.Core;

public class DartScore2
{
    private readonly List<List<string?>> _scores;
    private readonly int _playersCount;
    public int Rounds { get; private set; }
    public DartScore2(int players)
    {
        _playersCount = players;
        _scores = new(Enumerable.Range(0,players).Select(_ => new List<string?>()));
    }

    /// <summary>
    /// Adds a new row to the table
    /// </summary>
    /// <returns>The total amount of rounds </returns>
    public int NewRound()
    {
        foreach (var playerScore in _scores)
            playerScore.Add(null);
        Rounds++;
        return Rounds;
    }

    /// <summary>
    /// Updates the score of the current player in the current round
    /// </summary>
    /// <param name="score">Value to add</param>
    /// <returns>Player/round modified with the new value</returns>
    public ScoreCell UpdatePartialScore(char score)
    {
        i
    }


    /// <summary>
    /// Delete the score of a player
    /// </summary>
    /// <returns>Player/round modified with the current value</returns>
    public ScoreCell DeleteScore()
    {
        if (_scores[currentPlayer][currentRound] is not null && _scores[currentPlayer][currentRound]!.Length > 0)
            _scores[currentPlayer][currentRound] = _scores[currentPlayer][currentRound]?[..^1];
        return new(currentRound, currentPlayer, _scores[currentPlayer][currentRound]);
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

    public bool TryGetPlayerScore(int player, int round, out int score)
    {
        if (_scores[player][round] is null)
        {
            score = -1;
            return false;
        }

        if (SimpleMathInterpreter.TryResolve(_scores[player][round]!, out score))
        {
            return true;
        }
        score = -1;
        return false;
    }

    public int[][] GetScores()
    {
        var scores = new int[_playersCount][];
        for (var i = 0; i < _playersCount; i++)
        {
            scores[i] = new int[Rounds];
            for (var j = 0; j < Rounds; j++)
            {
                if (TryGetPlayerScore(i, j, out var score))
                {
                    scores[i][j] = score;
                }
            }
        }

        return scores;
    }

    public void UpdatePartialScore(int player, int round, char c)
    {
        _scores[player][round] ??= "";
        _scores[player][round] += c;
    }
}
