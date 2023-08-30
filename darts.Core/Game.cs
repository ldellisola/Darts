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
        OnScoreSelected?.Invoke(Score.Current);

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
                OnScoreDeselected?.Invoke(Score.Current);
                Score.PreviousPlayer();
                OnScoreSelected?.Invoke(Score.Current);
                break;
            }
            case { Key: ConsoleKey.RightArrow}:
            {
                OnScoreDeselected?.Invoke(Score.Current);
                Score.NextPlayer();
                OnScoreSelected?.Invoke(Score.Current);
                break;
            }
            case { Key: ConsoleKey.UpArrow}:
            {
                OnScoreDeselected?.Invoke(Score.Current);
                Score.PreviousRound();
                OnScoreSelected?.Invoke(Score.Current);
                break;
            }
            case { Key: ConsoleKey.DownArrow}:
            {
                OnScoreDeselected?.Invoke(Score.Current);
                Score.NextRound();
                OnScoreSelected?.Invoke(Score.Current);
                break;
            }
            case { Key: ConsoleKey.Enter}:
            {
                OnScoreDeselected?.Invoke(Score.Current);
                var totalRounds = Score.NewRound();
                OnRoundAdded?.Invoke(totalRounds, Score.Players.Length);
                OnScoreSelected?.Invoke(Score.Current);
                break;
            }
            case { KeyChar: >= '0' and <= '9'}:
            {
                Score.UpdatePartialScore(keyInfo.KeyChar - '0');
                OnScoreChanged?.Invoke(Score.Current);
                break;
            }
            case { Key: ConsoleKey.Backspace}:
            {
                Score.DeleteScore();
                OnScoreChanged?.Invoke(Score.Current);
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
