using Darts.Cli.Commands;
using darts.Cli.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var registrar = new TypeRegistrar();

registrar.RegisterInstance(typeof(ISerializer),
    new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build()
    );

registrar.RegisterInstance(typeof(IDeserializer),
    new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build()
    );

var app = new CommandApp(registrar);

app.Configure(config =>
{

    config.AddCommand<NewGameCommand>("new");
    config.AddCommand<ClassicGameCommand>("classic");

    config.AddCommand<KnockoutGameCommand>("knockout");
    config.AddCommand<HighScoreGameCommand>("high-score");
    config.AddCommand<BestOfGameCommand>("best-of");
    config.AddCommand<UnlimitedGameCommand>("unlimited");

    config.AddCommand<LoadGameCommand>("load");

    config.SetExceptionHandler(t => AnsiConsole.WriteException(t));
});

app.Run(args);

