namespace Darts.Entities.GameState;

public class GameState
{
    public GameState(string gameType, Common common, Dictionary<string, object> gameSpecific)
    {
        GameType = gameType;
        Common = common;
        GameSpecific = gameSpecific;
    }

    public GameState()
    {
        GameType = "";
        Common = new Common();
        GameSpecific = new Dictionary<string, object>();
    }

    public string GameType { get; set; }
    public Common Common { get; set; }
    public Dictionary<string, object> GameSpecific { get; set; }
}


public class Common
{
    public bool IsTournament { get; set; }
    public DateTime Date { get; set; }
    public string[] Players { get; set; }
    public string[] Scores { get; set; }
    public int Rounds { get; set; }
    public int? Winner { get; set; }

    public Common(string[] Players, string[] Scores, int Rounds, int? Winner, bool IsTournament, DateTime Date)
    {
        this.IsTournament = IsTournament;
        this.Date = Date;
        this.Players = Players;
        this.Scores = Scores;
        this.Rounds = Rounds;
        this.Winner = Winner;
    }

    public Common()
    {
        IsTournament = false;
        Date = DateTime.Now;
        Players = Array.Empty<string>();
        Scores = Array.Empty<string>();
        Rounds = 0;
        Winner = null;
    }
}
