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
                .GetExportedTypes()
                .Where(type => type
                    .GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAutomapFrom<>)))
                .DistinctBy(type => type.AssemblyQualifiedName)
                .ToList());

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);
            var methodInfo = type.GetMethod("Mapping")
                             ?? type.GetInterface("IAutomapFrom`1")!.GetMethod("Mapping");

            methodInfo.Invoke(instance, new[] { this });
        }
    }
}