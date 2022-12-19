using AutoMapper;

namespace Devocean.Core.Application.Mappers.Common;

public interface IAutomapperProfile
{
    void Setup(Profile profile);
}