using System.Text;
using darts.Core.Expression;
using darts.Entities;

namespace darts.Core;

public class DartScore
{
    private readonly List<List<string?>> _scores;
    private readonly SimpleMathInterpreter _mathInterpreter = new();
    public string[] Players { get; }
    private readonly int _playersCount;

    private int currentPlayer;
    private int currentRound = -1;
    private int totalRounds;

    public ScoreCell CurrentRaw => new (currentRound, currentPlayer, _scores[currentPlayer][currentRound]);
    public ScoreCell CurrentComputed => new(currentRound, currentPlayer, _mathInterpreter.TryResolve(_scores[currentPlayer][currentRound] ?? string.Empty, out var score) ? score.ToString() : null);

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
        currentRound++;
        totalRounds++;
        return totalRounds;
    }

    /// <summary>
    /// Updates the score of the current player in the current round
    /// </summary>
    /// <param name="score">Value to add</param>
    /// <returns>Player/round modified with the new value</returns>
    public ScoreCell UpdatePartialScore(char score)
    {
        if (_scores[currentPlayer][currentRound] is null)
            _scores[currentPlayer][currentRound] = "";

        _scores[currentPlayer][currentRound] += score;

        return new(currentRound, currentPlayer, _scores[currentPlayer][currentRound]);
    }


    /// <summary>
    /// Moves to the next player
    /// </summary>
    /// <returns>Player/round modified with the current value</returns>
    public ScoreCell NextPlayer()
    {
        currentPlayer++;
        if (currentPlayer >= _playersCount)
        {
            currentPlayer = 0;
        }
        return new(currentRound, currentPlayer, _scores[currentPlayer][currentRound]);
    }

    /// <summary>
    /// Moves to the previous player
    /// </summary>
    /// <returns>Player/round modified with the current value</returns>
    public ScoreCell PreviousPlayer()
    {
        currentPlayer--;
        if (currentPlayer < 0)
        {
            currentPlayer = _playersCount - 1;
        }
        return new(currentRound, currentPlayer, _scores[currentPlayer][currentRound]);
    }

    /// <summary>
    /// Moves to the next round
    /// </summary>
    /// <returns>Player/round modified with the current value</returns>
    public ScoreCell? NextRound()
    {
        if (currentRound < totalRounds - 1)
        {
            currentRound++;
            return new(currentRound, currentPlayer, _scores[currentPlayer][currentRound]);
        }

        return null;
    }

    /// <summary>
    /// Moves to the previous round
    /// </summary>
    /// <returns>Player/round modified with the current value</returns>
    public ScoreCell? PreviousRound()
    {
        if (currentRound > 0)
        {
            currentRound--;
            return new(currentRound, currentPlayer, _scores[currentPlayer][currentRound]);
        }

        return null;
    }

    /// <summary>
    /// Delete the score of a player
    /// </summary>
    /// <returns>Player/round modified with the current value</returns>
    public ScoreCell DeleteScore()
    {
        _scores[currentPlayer][currentRound] = null;
        return new(currentRound, currentPlayer, _scores[currentPlayer][currentRound]);
    }


    public bool TryGetPlayerScore(int player, out int score)
    {
        var exp = new StringBuilder()
                      .AppendJoin('+',player.se
                                  )
        var (success, value) = _mathInterpreter.Resolve(_scores[player].Aggregate(new StringBuilder(), (sb, s) => sb.Append('+').Append(s ?? "0")).ToString());
        if(success is false)
        {
            score = 0;
            return false;
        }

        score = value!.Value;
        return true;
    }
}
