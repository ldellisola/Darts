using System.Text.Json;
using Darts.Entities;

namespace Darts.Core.Games;

public abstract class Game
{
    public DartScore Score { get; }

    protected int Winner
    {
        set => OnPlayerWon?.Invoke(value);
    }

    private string? _winner;

    public event Action<int>? OnPlayerWon;
    public event Action<ScoreCell>? OnScoreSelected;
    public event Action<ScoreCell>? OnScoreDeselected;
    public event Action<ScoreCell>? OnScoreChanged;
    public event Action<int, int>? OnRoundAdded;
    public event Action<int, int>? OnTotalScoreChanged;

    public Game(DartScore score)
    {
        Score = score;
        OnPlayerWon += i => _winner = Score.Players[i];
    }

    public virtual void Start()
    {
        var totalRounds = Score.NewRound();
        OnRoundAdded?.Invoke(totalRounds, Score.Players.Length);
        OnScoreSelected?.Invoke(Score.CurrentComputed);

        foreach(var (i, _) in Score.Players.WithIndex())
        {
            ExecuteOnTotalScoreChanged(i);
        }
    }

    public virtual void Consume(ConsoleKeyInfo keyInfo)
    {
        switch(keyInfo)
        {
            case { Key: ConsoleKey.LeftArrow }:
            {
                GoToPreviousPlayer();
                break;
            }
            case { Key: ConsoleKey.RightArrow }:
            {
                GoToNextPlayer();
                break;
            }
            case { Key: ConsoleKey.UpArrow }:
            {
                GoToPreviousRound();
                break;
            }
            case { Key: ConsoleKey.DownArrow }:
            {
                GoToNextRound();
                break;
            }
            case { Key: ConsoleKey.Spacebar }:
            {
                OnScoreChanged?.Invoke(Score.CurrentRaw);
                break;
            }
            case { Key: ConsoleKey.Enter }:
            {
                CreateNewRound();
                break;
            }
            case { KeyChar: '+' or '*' }:
            case { KeyChar: >= '0' and <= '9' }:
            {
                Score.UpdatePartialScore(keyInfo.KeyChar);
                OnScoreChanged?.Invoke(Score.CurrentRaw);
                break;
            }
            case { Key: ConsoleKey.Backspace }:
            {
                Score.DeleteScore();
                OnScoreChanged?.Invoke(Score.CurrentRaw);
                break;
            }
        }
    }

    protected virtual void CreateNewRound()
    {
        if(Score.Players.WithIndex().Aggregate(true, (b, tuple) => b && !Score.TryGetPlayerScore(tuple.Index, Score.TotalRounds - 1, out _)))
        {
            return;
        }

        OnScoreDeselected?.Invoke(Score.CurrentComputed);
        var totalRounds = Score.NewRound();
        OnRoundAdded?.Invoke(totalRounds, Score.Players.Length);
        OnScoreSelected?.Invoke(Score.CurrentComputed);
    }

    protected virtual void GoToNextRound()
    {
        OnScoreDeselected?.Invoke(Score.CurrentComputed);
        Score.NextRound();
        OnScoreSelected?.Invoke(Score.CurrentComputed);
    }

    protected virtual void GoToPreviousRound()
    {
        OnScoreDeselected?.Invoke(Score.CurrentComputed);
        Score.PreviousRound();
        OnScoreSelected?.Invoke(Score.CurrentComputed);
    }

    protected virtual void GoToNextPlayer()
    {
        OnScoreDeselected?.Invoke(Score.CurrentComputed);
        Score.NextPlayer();
        OnScoreSelected?.Invoke(Score.CurrentComputed);
    }

    protected virtual void GoToPreviousPlayer()
    {
        OnScoreDeselected?.Invoke(Score.CurrentComputed);
        Score.PreviousPlayer();
        OnScoreSelected?.Invoke(Score.CurrentComputed);
    }

    protected abstract int GetPlayerScore(int player);

    protected virtual int ExecuteOnTotalScoreChanged(int player)
    {
        var playerScore = GetPlayerScore(player);
        OnTotalScoreChanged?.Invoke(player, playerScore);
        return playerScore;
    }

    public string ToJson()
    {
        var options = new JsonSerializerOptions
                      {
                          WriteIndented = true
                      };

        var state = new Dictionary<string, object?>
                    {
                        { "Players", Score.Players },
                        { "Winner", _winner },
                        { "Score", Score.GetScores() },
                        { "Type", GetType().Name },
                        { "Date", DateTime.Now }
                    };

        return JsonSerializer.Serialize(GetGameState().Union(state).ToDictionary(t => t.Key, t => t.Value), options);
    }

    protected abstract Dictionary<string, object?> GetGameState();
}
