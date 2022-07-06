using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AutoMapper;
using Devocean.Core.Application.UseCases.Common;
using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Extensions;
using System.ComponentModel;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Devocean.Aws.Sns.UseCases;

public class SendSmsMessage : IRequest<SendSmsMessage.Response>
{
    public string Number { get; }
    public string Message { get; }
    
    public SmsType SmsType { get; }

    public SendSmsMessage(string number, string message, SmsType smsType)
    {
        Number = number;
        Message = message;
        SmsType = smsType;
    }

    public class Response
    {
        public HttpStatusCode StatusCode { get; }

        public Response(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }

    public class Validator : AbstractValidator<SendSmsMessage>
    {
        public Validator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(v => v.Message)
                .NotEmpty()
                .WithMessage(Error.Message.GetAttributeOfType<DescriptionAttribute>().Description);

            RuleFor(v => v.Number)
                .NotEmpty()
                .WithMessage(Error.Number.GetAttributeOfType<DescriptionAttribute>().Description);
        }
    }

    public enum Error
    {
        [Description("Message is required")] Message,
        [Description("Number is required")] Number
    }

    public class Handler : HandlerBase<SendSmsMessage, SendSmsMessage.Response>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IAmazonSimpleNotificationService _notificationService;

        public Handler(IMapper mapper,
            ILogger<Handler> logger,
            IAmazonSimpleNotificationService notificationService) : base(mapper)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        public override async Task<Response?> Handle(SendSmsMessage request, CancellationToken cancellationToken)
        {
            var publishRequest = new PublishRequest
            {
                Message = request.Message,
                PhoneNumber = request.Number,
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    {
                        "DefaultSMSType",
                        new MessageAttributeValue { DataType = "String", StringValue = Enum.GetName(request.SmsType) }
                    },
                    {
                        "DefaultSenderID",
                        new MessageAttributeValue { DataType = "String", StringValue = "SolAgora" }
                    },
                }
            };

            var response = await _notificationService.PublishAsync(publishRequest, cancellationToken);

            _logger.LogInformation(
                $"An SMS Message was sent with status {response.HttpStatusCode} to {request.Number}");

            return new Response(response.HttpStatusCode);
        }
    }
}