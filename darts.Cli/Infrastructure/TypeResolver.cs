using Spectre.Console.Cli;

namespace darts.Cli.Infrastructure;

internal sealed class TypeResolver : ITypeResolver
{

	private readonly IServiceProvider _provider;

    public TypeResolver(IServiceProvider provider) => _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    public object? Resolve(Type? type) => type is null ? null : _provider.GetService(type);
}
