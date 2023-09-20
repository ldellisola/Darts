using Darts.Cli.Commands;
using darts.Cli.Infrastructure;
using Spectre.Console.Cli;

var registrar = new TypeRegistrar();

var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.AddCommand<NewGameCommand>("new");
    config.AddCommand<NewClassicGameCommand>("classic");
    config.AddCommand<KnockoutGameCommand>("knockout");
    config.AddCommand<HighScoreGameCommand>("high-score");
    config.AddCommand<BestOfGameCommand>("best-of");
    config.AddCommand<UnlimitedGameCommand>("unlimited");
});

app.Run(args);

