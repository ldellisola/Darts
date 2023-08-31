using darts.Entities;

namespace darts.Core;

public abstract class Game
{
    public  DartScore Score { get; }
    protected int Winner {
        set => OnPlayerWon?.Invoke(value);
    }

    public event Action<int>? OnPlayerWon;
    public event Action<ScoreCell>? OnScoreSelected;
    public event Action<ScoreCell>? OnScoreDeselected;
    public event Action<ScoreCell>? OnScoreChanged;
    public event Action<int, int>? OnRoundAdded;
    public event Action<int, int>? OnTotalScoreChanged;
    public Game(DartScore score)
    {
        Score = score;
    }

    public void Start()
    {
        var totalRounds = Score.NewRound();
        OnRoundAdded?.Invoke(totalRounds, Score.Players.Length);
        OnScoreSelected?.Invoke(Score.CurrentComputed);

        foreach (var (i,_) in Score.Players.WithIndex())
        {
            ExecuteOnTotalScoreChanged(i);
        }
    }

    public virtual void Consume(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo)
        {
            case { Key: ConsoleKey.LeftArrow}:
            {
                OnScoreDeselected?.Invoke(Score.CurrentComputed);
                Score.PreviousPlayer();
                OnScoreSelected?.Invoke(Score.CurrentComputed);
                break;
            }
            case { Key: ConsoleKey.RightArrow}:
            {
                OnScoreDeselected?.Invoke(Score.CurrentComputed);
                Score.NextPlayer();
                OnScoreSelected?.Invoke(Score.CurrentComputed);
                break;
            }
            case { Key: ConsoleKey.UpArrow}:
            {
                OnScoreDeselected?.Invoke(Score.CurrentComputed);
                Score.PreviousRound();
                OnScoreSelected?.Invoke(Score.CurrentComputed);
                break;
            }
            case { Key: ConsoleKey.DownArrow}:
            {
                OnScoreDeselected?.Invoke(Score.CurrentComputed);
                Score.NextRound();
                OnScoreSelected?.Invoke(Score.CurrentComputed);
                break;
            }
            case { Key: ConsoleKey.Enter}:
            {
                OnScoreDeselected?.Invoke(Score.CurrentComputed);
                var totalRounds = Score.NewRound();
                OnRoundAdded?.Invoke(totalRounds, Score.Players.Length);
                OnScoreSelected?.Invoke(Score.CurrentComputed);
                break;
            }
            case {KeyChar: '+' or '*'}:
            case { KeyChar: >= '0' and <= '9'}:
            {
                Score.UpdatePartialScore(keyInfo.KeyChar);
                OnScoreChanged?.Invoke(Score.CurrentRaw);
                break;
            }
            case { Key: ConsoleKey.Backspace}:
            {
                Score.DeleteScore();
                OnScoreChanged?.Invoke(Score.CurrentRaw);
                break;
            }
        }
    }

    protected abstract int GetPlayerScore(int player);
    protected void ExecuteOnTotalScoreChanged(int player)
    {
        OnTotalScoreChanged?.Invoke(player, GetPlayerScore(player));
    }
}
