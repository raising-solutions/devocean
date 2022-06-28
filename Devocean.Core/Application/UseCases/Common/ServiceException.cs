using Microsoft.AspNetCore.Http;

namespace Devocean.Core.Application.UseCases.Common;
public class ServiceException : Exception
{
    public string Title { get; set; }
    public int Status { get; set; }
    public string Detail { get; set; }
    public List<string> Errors { get; set; }

    public ServiceException()
    {
        Errors = new List<string>();
    }

    public ServiceException(string title, int status = 500, string detail = "", List<string>? errors = default) : base(title)
    {
        Title = title;
        Status = status;
        Detail = detail;
        Errors = errors ?? new List<string>();
    }
}