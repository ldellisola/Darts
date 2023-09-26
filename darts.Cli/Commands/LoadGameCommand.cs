using Darts.Entities.GameState;
using Spectre.Console;
using Spectre.Console.Cli;
using YamlDotNet.Serialization;

namespace Darts.Cli.Commands;

public class LoadGameSettings : CommandSettings
{
    [CommandArgument(0, "<FILE>")]
    public string File { get; set; } = null!;

    public override ValidationResult Validate() =>
        System.IO.File.Exists(File) ?
            ValidationResult.Success() :
            ValidationResult.Error("File does not exists");
}

public class LoadGameCommand : Command<LoadGameSettings>
{
    private readonly IDeserializer _deserializer;
    private readonly ISerializer _serializer;

    public LoadGameCommand(IDeserializer deserializer, ISerializer serializer)
    {
        _deserializer = deserializer;
        _serializer = serializer;
    }

    public override int Execute(CommandContext context, LoadGameSettings settings)
    {
        var content = File.ReadAllText(settings.File);
        var state = _deserializer.Deserialize<GameState>(content);

        return state.GameType switch
        {
            nameof(ClassicGame) => new ClassicGameCommand(_serializer).Execute(state),
            nameof(HighScoreGame) => new HighScoreGameCommand(_serializer).Execute(state),
            _ => throw new NotSupportedException($"Game {state.GameType} is not supported")
        };
    }
}
