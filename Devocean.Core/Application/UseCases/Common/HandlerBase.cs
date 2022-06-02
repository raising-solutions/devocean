using AutoMapper;
using Devocean.Core.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Devocean.Core.Application.UseCases.Common;

public abstract class HandlerBase<TRequest, TResponse, TDbContext> : IRequestHandler<TRequest, TResponse?> 
    where TRequest : IRequest<TResponse?>
    where TDbContext : DbContext, IDbContext
{
    protected readonly TDbContext _dbContext;
    protected readonly IMapper _mapper;

    protected HandlerBase(TDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public abstract Task<TResponse?> Handle(TRequest request, CancellationToken cancellationToken);
}

public abstract class HandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse?> 
    where TRequest : IRequest<TResponse?>
{
    protected readonly IMapper _mapper;

    protected HandlerBase(IMapper mapper)
    {
        _mapper = mapper;
    }
    public abstract Task<TResponse?> Handle(TRequest request, CancellationToken cancellationToken);
}