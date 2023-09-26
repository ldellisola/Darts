
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace darts.Cli.Infrastructure;
/// <summary>
/// Dependency injection registrar.
/// </summary>
internal sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _builder;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeRegistrar"/> class.
    /// </summary>
    public TypeRegistrar() => _builder = new ServiceCollection();

    public ITypeResolver Build() => new TypeResolver(_builder.BuildServiceProvider());

    public void Register(Type service, Type implementation) => _builder.AddSingleton(service, implementation);

    public void RegisterInstance(Type service, object implementation) => _builder.AddSingleton(service, implementation);

    public void RegisterLazy(Type service, Func<object> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        _builder.AddSingleton(service, _ => func());
    }

    public TypeRegistrar Register(Action<IServiceCollection> action)
    {
        action(_builder);
        return this;
    }
}
