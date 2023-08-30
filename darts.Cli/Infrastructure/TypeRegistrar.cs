namespace darts.Cli.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

/// <summary>
/// Dependency injection registrar.
/// </summary>
internal class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection builder;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeRegistrar"/> class.
    /// </summary>
    public TypeRegistrar()
    {
        builder = new ServiceCollection();
    }

    public ITypeResolver Build()
    {
        return new TypeResolver(builder.BuildServiceProvider());
    }

    public void Register(Type service, Type implementation)
    {
        builder.AddSingleton(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        builder.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        builder.AddSingleton(service, _ => func());
    }

    public TypeRegistrar Register(Action<IServiceCollection> action)
    {
        action(builder);
        return this;
    }
}