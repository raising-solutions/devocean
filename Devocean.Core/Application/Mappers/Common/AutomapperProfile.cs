using System.Reflection;
using AutoMapper;

namespace Devocean.Core.Application.Mappers.Common;

public class AutomapperProfile : Profile
{
    public static List<Assembly> IncludedAssemblies = new();
    public AutomapperProfile()
    {
        IncludedAssemblies.Add(Assembly.GetExecutingAssembly());
        IncludedAssemblies.Add(Assembly.GetCallingAssembly());

        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly != null) IncludedAssemblies.Add(entryAssembly);
        
        var types = IncludedAssemblies.SelectMany(assembly => assembly.GetExportedTypes().Where(type => type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAutomapFrom<>)))
            .ToList());

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);
            var methodInfo = type.GetMethod("Mapping")
                             ?? type.GetInterface("IAutomapFrom`1")!.GetMethod("Mapping");
            methodInfo?.Invoke(instance, new[] { this });
        }
    }
    
}