using FluentValidation.Results;

namespace Devocean.Core.Application.UseCases.Common;

public class Response<T>
{
    private readonly IList<string> _messages = new List<string>();

    public T? Result { get; set; }
    public ValidationResult ValidationResult { get; private set; }
    
    // public IEnumerable<string> Errors { get; }
    // public Response() => Errors = new ReadOnlyCollection<string>(_messages);

    private Response()
    {
    }
    
    public Response(ValidationResult validationResult)
    {
        ValidationResult = validationResult;
    }
    public Response(T result, ValidationResult validationResult) : this()
    {
        Result = result;
        ValidationResult = validationResult;
    }

    public Response<T> AddError(string message)
    {
        _messages.Add(message);
        return this;
    }
}