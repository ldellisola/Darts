using Spectre.Console;
using Spectre.Console.Cli;

namespace darts.Cli.Commands;


public class HelloCommandSettings: CommandSettings
{
    [CommandArgument(0, "<name>")]
    internal string Name { get; set; } = string.Empty;

    public override ValidationResult Validate()
    {
        return Name != "lucas" ? ValidationResult.Success() : ValidationResult.Error("Error message");
    }
}

public class HelloCommand : Command<HelloCommandSettings>
{
    public override int Execute(CommandContext context, HelloCommandSettings settings)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var name =new Core.Hello.Action().Execute(settings.Name);
		AnsiConsole.MarkupLine($"Hello [green]{name}[/]!");
		return 0;
	}

}
