using Darts.Entities.GameState;
using Spectre.Console;
using Spectre.Console.Cli;
using YamlDotNet.Serialization;

namespace Darts.Cli.Commands;

public abstract class BaseCommand<TSettings,TGame> : Command<TSettings> where TSettings : NewGameSettings  where TGame : DartsGame<TGame>
{
    protected BaseCommand(ISerializer serializer) => _serializer = serializer;

    private readonly ISerializer _serializer;
    protected Layout Layout { get; set; } = new("root");
    protected TGame Game { get; private set; } = null!;
    protected bool ShowRawScore { get; private set; }

    public virtual int Execute(GameState state) => ExecuteGame(InitializeGame(state));

    public override int Execute(CommandContext context, TSettings settings)
    {
        settings.Players ??= GetPlayers();
        return ExecuteGame(InitializeGame(settings));
    }

    protected virtual int ExecuteGame(TGame game)
    {
        Game = game;
        AnsiConsole.Live(Layout).Start(ct =>
        {
            do
            {
                DrawGame();
                ct.Refresh();
            }
            while(GameLoop(Console.ReadKey(true)));
        });
        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, $"game_{DateTime.Now:yy-MM-dd-hh-mm-ss}.yml"), Export());
        return 0;
    }

    protected virtual bool GameLoop(ConsoleKeyInfo ch)
    {
        ShowRawScore = ch.Key == ConsoleKey.Spacebar
                       || char.IsDigit(ch.KeyChar)
                       || ch.Key == ConsoleKey.Backspace
                       || ch.Key == ConsoleKey.Multiply
                       || ch.Key == ConsoleKey.Add;

        return Game.Consume(ch.Key);
    }

    protected abstract TGame InitializeGame(TSettings settings);
    protected abstract TGame InitializeGame(GameState state);

    protected abstract void DrawGame();

    private static string[] GetPlayers()
    {
        AnsiConsole.MarkupLine("Add players to the game. Type 'done' when finished.");
        var players = new List<string>();
        while(true)
        {
            var readLine = Console.ReadLine();
            if(readLine is null)
                continue;
            if(readLine.ToLowerInvariant() is "done")
                break;
            players.Add(readLine);
        }

        return players.ToArray();
    }

    private string Export()
    {
        var state = Game.Export();
        return _serializer.Serialize(state);
    }
}
