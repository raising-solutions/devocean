using AutoMapper;
using Devocean.Aws.Sns;
using Devocean.Aws.Sns.UseCases;
using Devocean.Core.Application.UseCases.Common;
using Devocean.Core.Infrastructure.Services.Email;
using DevoceanExample.WebApi.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using System.ComponentModel;
using System.Net;

namespace DevoceanExample.WebApi.Notification.UseCases;

public class SendNotification : IRequest<NotificationEntity>
{
    public NotificationEntity Notification { get; }

    public SendNotification(NotificationEntity notificationControl)
    {
        Notification = notificationControl;
    }


    public class Validator : AbstractValidator<SendNotification>
    {
        public Validator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(v => v.Notification)
                .NotEmpty()
                .WithMessage(EnumUtil.GetDescription(ValidationError.NotificationControlRequired));
        }
    }

    public enum ValidationError
    {
        [Description("Não há notificação para enviar")]
        NotificationControlRequired,
    }

    public class Handler : HandlerBase<SendNotification, NotificationEntity, ApplicationDbContext>
    {
        private readonly ILogger<Handler> _logger;
        private readonly ISender _mediator;
        private readonly EmailService _mailService;
        private readonly IConfiguration _configuration;

        public Handler(ILogger<Handler> logger,
            ApplicationDbContext context, IMapper mapper, ISender mediator, EmailService mailService,
            IConfiguration configuration) : base(context, mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mailService = mailService;
            _configuration = configuration;
        }

        public override async Task<NotificationEntity> Handle(SendNotification request,
            CancellationToken cancellationToken)
        {
            try
            {
                request.Notification.Status = NotificationStatus.Processing;
                _dbContext.Notification.Update(request.Notification);
                await _dbContext.SaveChangesAsync(cancellationToken);

                var success = await SendMessage(request, cancellationToken);

                request.Notification.Status = success
                    ? NotificationStatus.Completed
                    : NotificationStatus.Error;
            }
            catch (Exception err)
            {
                _logger.LogError(err, "An error occurs while Sending Notification");
                request.Notification.Status = NotificationStatus.Error;
                await _dbContext.SaveChangesAsync(cancellationToken);
                throw;
            }

            var entry = _dbContext.Notification.Update(request.Notification);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entry.Entity;
        }

        private async Task<bool> SendMessage(SendNotification request, CancellationToken cancellationToken)
        {
            var fromEmail = _configuration.GetSection("AppSettings:NoReplyEmailAddress").Value;

            if (request.Notification.ChannelType == ChannelType.Email)
            {
                var serverResult = await _mailService.SendHtmlMessage(fromEmail,
                    request.Notification.Destination,
                    request.Notification.Subject,
                    request.Notification.Body,
                    cancellationToken);

                _logger.LogInformation($"Retorno do serviço de email -> {serverResult}");
                return serverResult.Contains("OK", StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                var sendSmsMessage = new SendSmsMessage(request.Notification.Destination,
                    request.Notification.Body,
                    SmsType.Transactional);

                var serverResult = await _mediator.Send(sendSmsMessage);
                return serverResult.StatusCode == HttpStatusCode.OK;
            }
        }
    }
}