namespace Darts.Cli.Commands;

public class NewClassicGameCommand : NewBaseCommand<ClassicGameSettings>
{
    protected override Game InitializeGame(ClassicGameSettings settings) => new ClassicGame(settings.Players!,settings.Score!.Value);

    protected override void DrawGame(Game game)
    {

    }
}
