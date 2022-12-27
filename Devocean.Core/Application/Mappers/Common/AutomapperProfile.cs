using System.Reflection;
using AutoMapper;

namespace Devocean.Core.Application.Mappers.Common;

public class AutomapperFactory
{
    public static Assembly ConfigureScan<T>()
    {
        AutomapperProfile.IncludeAssembly(typeof(T).Assembly);
        return typeof(AutomapperFactory).Assembly;
    }
}

public class AutomapperProfile : Profile
{
    private static readonly Dictionary<string, Assembly> IncludedAssemblies = new();

    public static bool IncludeAssembly(Assembly assembly)
    {
        return IncludedAssemblies.TryAdd(assembly.GetName().FullName, assembly);
    }

    public AutomapperProfile()
    {
        var types = IncludedAssemblies.Values
            .SelectMany(assembly => assembly
                .GetExportedTypes());

        var mappingInterfaces = types.Where(type =>
            (type.GetInterface("IAutomapFrom`1") ?? type.GetInterface("IAutomapperProfile")) is not null).AsParallel();
        
        Parallel.ForEach(mappingInterfaces, type =>
        {
            var instance = Activator.CreateInstance(type);
            var methodInfo = type.GetMethod("Mapping")
                             ?? type.GetInterface("IAutomapFrom`1")?.GetMethod("Mapping")
                             ?? type.GetMethod("Setup")
                             ?? type.GetInterface("IAutomapperProfile")?.GetMethod("Setup");

            methodInfo?.Invoke(instance, new[] { this });
        });
    }
}