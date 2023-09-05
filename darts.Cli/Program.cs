using Darts.Cli.Commands;
using darts.Cli.Infrastructure;
using Spectre.Console.Cli;

var registrar = new TypeRegistrar();

var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.AddCommand<NewGameCommand>("new");
    config.AddCommand<ClassicGameCommand>("classic");
    config.AddCommand<KnockoutGameCommand>("knockout");
    config.AddCommand<TotalerGameCommand>("totaler");
    config.AddCommand<RounderGameCommand>("rounder");
});

app.Run(args);

