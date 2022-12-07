using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AutoMapper;
using Devocean.Core.Application.UseCases.Common;
using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Extensions;
using System.ComponentModel;

namespace Devocean.Aws.Sns.UseCases;

public class ListTopics : IRequest<ListTopics.Response>
{
    public string? NextToken { get; }
   
    public ListTopics(string? nextToken)
    {
        NextToken = nextToken;
    }
   
    public class Response
    {
        public List<Topic> ResultTopics { get; }
        public string ResultNextToken { get; }
        public Response(List<Topic> resultTopics, string resultNextToken)
        {
            ResultTopics = resultTopics;
            ResultNextToken = resultNextToken;
        }
    }

    public class Validator : AbstractValidator<ListTopics>
    {
        public Validator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            When(v => v.NextToken != null,
                () => RuleFor(v => v.NextToken)
                    .NotEmpty()
                    .WithMessage(Error.NextToken.GetAttributeOfType<DescriptionAttribute>().Description));

        }
    }

    public enum Error
    {
        [Description("NextToken can't be empty")]
        NextToken
    }

    public class Handler : HandlerBase<ListTopics, Response>
    {
        private readonly IAmazonSimpleNotificationService _notificationService;

        public Handler(IMapper mapper, IAmazonSimpleNotificationService notificationService) : base(mapper)
        {
            _notificationService = notificationService;
        }
        public override async Task<Response?> Handle(ListTopics request, CancellationToken cancellationToken)
        {
            var result = await _notificationService.ListTopicsAsync(cancellationToken);
            
            return new Response(result.Topics, result.NextToken);
        }
    }
    
}
