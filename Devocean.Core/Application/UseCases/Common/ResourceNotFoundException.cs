namespace Devocean.Core.Application.UseCases.Common;

public class ResourceNotFoundException<T> : ResourceNotFoundException
{
    public ResourceNotFoundException() : base($"O recurso para {typeof(T).Name} não foi encontrado")
    {
    }

    public ResourceNotFoundException(Exception inner) : base($"O recurso para {typeof(T).Name} não foi encontrado", inner)
    {
    }
}
public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException()
    {
    }

    public ResourceNotFoundException(string message) : base(message)
    {
    }

    public ResourceNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }
}