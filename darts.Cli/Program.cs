using darts.Cli.Commands;
using darts.Cli.Infrastructure;
using Spectre.Console.Cli;

var registrar = new TypeRegistrar();

var app = new CommandApp(registrar);

app.Configure(config =>
{
	config.AddCommand<HelloCommand>("hello");
    config.AddCommand<NewGameCommand>("new");
});

app.Run(args);

