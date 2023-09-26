using System.Text;
using Darts.Core.Expression;
using Darts.Entities;

namespace Darts.Core;

public class DartScore
{
    private readonly List<List<string?>> _scores;
    public string[] Players { get; }
    private readonly int _playersCount;

    private int _currentPlayer;
    private int _currentRound = -1;
    public int TotalRounds { get; private set; }

    public ScoreCell CurrentRaw => new (_currentRound, _currentPlayer, _scores[_currentPlayer][_currentRound]);
    public ScoreCell CurrentComputed => new(_currentRound, _currentPlayer, SimpleMathInterpreter.TryResolve(_scores[_currentPlayer][_currentRound] ?? string.Empty, out var score) ? score.ToString() : null);

    public DartScore(string[] players)
    {
        Players = players;
        _playersCount = players.Length;
        _scores = new(players.Select(_ => new List<string?>()));
    }

    /// <summary>
    /// Adds a new row to the table
    /// </summary>
    /// <returns>The total amount of rounds </returns>
    public int NewRound()
    {
        foreach (var playerScore in _scores)
            playerScore.Add(null);
        _currentRound++;
        TotalRounds++;
        return TotalRounds;
    }

    /// <summary>
    /// Updates the score of the current player in the current round
    /// </summary>
    /// <param name="score">Value to add</param>
    /// <returns>Player/round modified with the new value</returns>
    public ScoreCell UpdatePartialScore(char score)
    {
        if (_scores[_currentPlayer][_currentRound] is null)
            _scores[_currentPlayer][_currentRound] = "";

        _scores[_currentPlayer][_currentRound] += score;

        return new(_currentRound, _currentPlayer, _scores[_currentPlayer][_currentRound]);
    }


    /// <summary>
    /// Moves to the next player
    /// </summary>
    /// <returns>Player/round modified with the current value</returns>
    public ScoreCell NextPlayer()
    {
        _currentPlayer++;
                if (_currentPlayer >= _playersCount)
                {
                    _currentPlayer = 0;
                }
        return new(_currentRound, _currentPlayer, _scores[_currentPlayer][_currentRound]);
    }

    /// <summary>
    /// Moves to the previous player
    /// </summary>
    /// <returns>Player/round modified with the current value</returns>
    public ScoreCell PreviousPlayer()
    {
        _currentPlayer--;
        if (_currentPlayer < 0)
        {
            _currentPlayer = _playersCount - 1;
        }
        return new(_currentRound, _currentPlayer, _scores[_currentPlayer][_currentRound]);
    }

    /// <summary>
    /// Moves to the next round
    /// </summary>
    /// <returns>Player/round modified with the current value</returns>
    public ScoreCell? NextRound()
    {
        if (_currentRound < TotalRounds - 1)
        {
            _currentRound++;
            return new(_currentRound, _currentPlayer, _scores[_currentPlayer][_currentRound]);
        }

        return null;
    }

    /// <summary>
    /// Moves to the previous round
    /// </summary>
    /// <returns>Player/round modified with the current value</returns>
    public ScoreCell? PreviousRound()
    {
        if (_currentRound > 0)
        {
            _currentRound--;
            return new(_currentRound, _currentPlayer, _scores[_currentPlayer][_currentRound]);
        }

        return null;
    }

    /// <summary>
    /// Delete the score of a player
    /// </summary>
    /// <returns>Player/round modified with the current value</returns>
    public ScoreCell DeleteScore()
    {
        if (_scores[_currentPlayer][_currentRound] is not null && _scores[_currentPlayer][_currentRound]!.Length > 0)
            _scores[_currentPlayer][_currentRound] = _scores[_currentPlayer][_currentRound]?[..^1];
        return new(_currentRound, _currentPlayer, _scores[_currentPlayer][_currentRound]);
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
            scores[i] = new int[TotalRounds];
            for (var j = 0; j < TotalRounds; j++)
            {
                if (TryGetPlayerScore(i, j, out var score))
                {
                    scores[i][j] = score;
                }
            }
        }

        return scores;
    }
}
