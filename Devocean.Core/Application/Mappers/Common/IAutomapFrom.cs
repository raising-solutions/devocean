using AutoMapper;

namespace Devocean.Core.Application.Mappers.Common;

public interface IAutomapFrom<T>
{
    void Mapping(Profile profile);
}