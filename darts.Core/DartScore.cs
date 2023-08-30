using darts.Entities;

namespace darts.Core;

public class DartScore
{
    private readonly List<List<int?>> _scores;
    public string[] Players { get; }
    private readonly int _playersCount;

    private int currentPlayer;
    private int currentRound = -1;
    private int totalRounds;

    public ScoreCell Current  => new (currentRound, currentPlayer, _scores[currentPlayer][currentRound]);

    public DartScore(string[] players)
    {
        Players = players;
        _playersCount = players.Length;
        _scores = new(players.Select(_ => new List<int?>()));
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
    public ScoreCell UpdatePartialScore(int score)
    {
        if (_scores[currentPlayer][currentRound] is null)
            _scores[currentPlayer][currentRound] = score;
        else
            _scores[currentPlayer][currentRound] = (_scores[currentPlayer][currentRound] * 10) + score;

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


    public int GetPlayerScore(int player)
    {
        return _scores[player].Select(t=> t.GetValueOrDefault()).Sum();
    }

}
