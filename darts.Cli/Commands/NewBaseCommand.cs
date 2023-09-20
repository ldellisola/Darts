﻿using Spectre.Console;
using Spectre.Console.Cli;

namespace Darts.Cli.Commands;

public abstract class NewBaseCommand<T> : Command<T> where T : NewGameSettings
{
    protected Layout Layout { get; } = new("root");
    protected DartsGame Game { get; private set; } = null!;
    protected bool ShowRawScore { get; private set; }

    public override int Execute(CommandContext context, T settings)
    {
        settings.Players ??= GetPlayers();
        Game = InitializeGame(settings);

        AnsiConsole.Live(Layout).Start(ct =>
                                       {
                                           do
                                           {
                                               DrawGame();
                                               ct.Refresh();
                                           }
                                           while(GameLoop(Console.ReadKey(true)));
                                       });
        // File.WriteAllText(Path.Combine(Environment.CurrentDirectory, $"game_{DateTime.Now:yy-MM-dd-hh-mm-ss}.json"), Game.ToJson());
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

    protected abstract DartsGame
        InitializeGame(T settings);

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
}
