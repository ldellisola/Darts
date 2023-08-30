using darts.Entities;
using Action = darts.Core.Hello.Action;

namespace darts.Core;

public class DartScore
{
    private readonly List<List<int?>> _scores;
    private readonly int _playersCount;


    private int currentPlayer = 0;
    private int currentRound = -1;
    private int totalRounds = 0;

    public event Action<ScoreCell>? OnScoreChanged;
    public event Action<(ScoreCell previousCell, ScoreCell newCell)>? OnSelectionChanged;
    public event Action<(int row, int size)>? OnNewRound;
    public event Action<(int player, int score)>? OnTotalScoreChanged;

    public DartScore(int count)
    {
        _playersCount = count;
        _scores = new(Enumerable.Range(0, _playersCount).Select(_ => new List<int?>()));
    }

    public void NewRound()
    {
        foreach (var playerScore in _scores)
            playerScore.Add(null);
        currentRound++;
        totalRounds++;
        OnNewRound?.Invoke((totalRounds, _playersCount));
    }

    public void UpdatePartialScore(int score)
    {
        if (_scores[currentPlayer][currentRound] is null)
            _scores[currentPlayer][currentRound] = score;
        else
            _scores[currentPlayer][currentRound] = (_scores[currentPlayer][currentRound] * 10) + score;

        OnScoreChanged?.Invoke(new (currentRound, currentPlayer, _scores[currentPlayer][currentRound]));
        OnTotalScoreChanged?.Invoke((currentPlayer, _scores[currentPlayer].Select(t=> t.GetValueOrDefault()).Sum()));
    }

    public void NextPlayer()
    {
        var previousCell = new ScoreCell(currentRound, currentPlayer, _scores[currentPlayer][currentRound]);
        currentPlayer++;
        if (currentPlayer >= _playersCount)
        {
            currentPlayer = 0;
        }
        OnSelectionChanged?.Invoke((previousCell, new(currentRound, currentPlayer, _scores[currentPlayer][currentRound])));
    }

    public void PreviousPlayer()
    {
        var previousCell = new ScoreCell(currentRound, currentPlayer, _scores[currentPlayer][currentRound]);
        currentPlayer--;
        if (currentPlayer < 0)
        {
            currentPlayer = _playersCount - 1;
        }
        OnSelectionChanged?.Invoke((previousCell, new(currentRound, currentPlayer, _scores[currentPlayer][currentRound])));
    }

    public void NextRound()
    {
        if (currentRound < totalRounds - 1)
        {
            var previousCell = new ScoreCell(currentRound, currentPlayer, _scores[currentPlayer][currentRound]);
            currentRound++;
            OnSelectionChanged?.Invoke((previousCell, new(currentRound, currentPlayer, _scores[currentPlayer][currentRound])));
        }
    }

    public void PreviousRound()
    {
        if (currentRound > 0)
        {
            var previousCell = new ScoreCell(currentRound, currentPlayer, _scores[currentPlayer][currentRound]);
            currentRound--;
            OnSelectionChanged?.Invoke((previousCell, new(currentRound, currentPlayer, _scores[currentPlayer][currentRound])));

        }
    }

    public void DeleteScore()
    {
        _scores[currentPlayer][currentRound] = null;
        OnScoreChanged?.Invoke(new (currentRound, currentPlayer, null));
    }

}
