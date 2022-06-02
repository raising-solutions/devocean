using Devocean.Core.Application.Interfaces;

namespace Devocean.Core.Infrastructure.Services;

public class DateTimeService: IDateTime
{
    public DateTime Now => DateTime.Now.SetKindUtc();
}