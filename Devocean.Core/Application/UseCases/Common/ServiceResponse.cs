namespace Devocean.Core.Application.UseCases.Common;

public static class ServiceResponse
{
    public static ServiceException ThrowGeneric(string title, int status = 500, string detail = "", List<string>? errors = default)
    {
        throw new ServiceException(title, status, detail, errors);
    }
}