using AutoMapper;
using Devocean.Core.Application.UseCases.Common;
using Devocean.Core.Extensions;
using Devocean.Core.Infrastructure.Services.Email;
using DevoceanExample.WebApi.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using System.ComponentModel;

namespace DevoceanExample.WebApi.Notification.UseCases;

public class GetWeatherForecastAndSend : IRequest<WeatherForecastEntity>
{
    public Guid IdNotificationControl { get; }

    public GetWeatherForecastAndSend(Guid idNotificationControl)
    {
        IdNotificationControl = idNotificationControl;
    }

    public class Validator : AbstractValidator<GetWeatherForecastAndSend>
    {
        public Validator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(v => v.IdNotificationControl)
                .NotEmpty()
                .WithMessageEnum(ValidationError.IdNotificationControlRequired);
        }
    }

    public enum ValidationError
    {
        [Description("Id de notificação é obrigatório")]
        IdNotificationControlRequired,
    }

    public class Handler : HandlerBase<GetWeatherForecastAndSend, WeatherForecastEntity, ApplicationDbContext>
    {
        private readonly ILogger<Handler> _logger;
        private readonly ISender _mediator;
        private readonly EmailService _mailService;

        public Handler(ILogger<Handler> logger,
            ApplicationDbContext context, IMapper mapper, ISender mediator, EmailService mailService)
            : base(context, mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mailService = mailService;
        }

        public override async Task<WeatherForecastEntity> Handle(GetWeatherForecastAndSend request,
            CancellationToken cancellationToken)
        {
            var notificationControl = await _dbContext.Notification.
                FindAsync(new object[] { request.IdNotificationControl }, cancellationToken);

            if (notificationControl == null)
            {
                throw new ResourceNotFoundException<WeatherForecastEntity>();
            }

            _logger.LogInformation($"Sending Notification: {JsonConvert.SerializeObject(notificationControl)}");
            notificationControl = await _mediator.Send(new SendNotification(notificationControl));

            return notificationControl;
        }
    }
}