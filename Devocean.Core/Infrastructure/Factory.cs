namespace Devocean.Core.Infrastructure;

public class Factory<T>
{
    private readonly Func<T> _factoryFunction;

    public Factory(Func<T> factoryFunction)
    {
        _factoryFunction = factoryFunction;
    }

    public T Get() => _factoryFunction();
    }