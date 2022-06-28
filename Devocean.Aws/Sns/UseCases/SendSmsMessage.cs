using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AutoMapper;
using Devocean.Core.Application.UseCases.Common;
using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Extensions;
using System.ComponentModel;

namespace Devocean.Aws.Sns.UseCases;

public class SendSmsMessage : IRequest<SendSmsMessage.Response>
{
    public string Number { get; }
    public string Message { get; }
   
    public SendSmsMessage(string? nextToken, string number, string message)
    {
        Number = number;
        Message = message;
    }
   
    public class Response
    {
        public Response()
        {

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
        [Description("Message is required")]
        Message,
        [Description("Number is required")]
        Number
    }

    public class Handler : HandlerBase<SendSmsMessage, Response>
    {
        private readonly IAmazonSimpleNotificationService _notificationService;

        public Handler(IMapper mapper, IAmazonSimpleNotificationService notificationService) : base(mapper)
        {
            _notificationService = notificationService;
        }
        public override async Task<Response?> Handle(SendSmsMessage request, CancellationToken cancellationToken)
        {
            var publishRequest = new PublishRequest
            {
                Message = request.Message,
                PhoneNumber = request.Number
            };

            var response = await _notificationService.PublishAsync(publishRequest, cancellationToken);

            Console.WriteLine("Message sent to " + request.Number + ":");
            Console.WriteLine(request.Message);

            return new Response();
        }
    }
}
