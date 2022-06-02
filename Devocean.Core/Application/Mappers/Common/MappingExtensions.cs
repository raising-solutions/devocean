using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Devocean.Core.Application.Mappers.Common;

public static class MappingExtensions
{
    public static Task<List<TDestination>> MapToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configurationProvider)
        => queryable.ProjectTo<TDestination>(configurationProvider).ToListAsync();
}