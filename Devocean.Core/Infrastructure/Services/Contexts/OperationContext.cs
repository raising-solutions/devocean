using System.Security.Claims;

namespace Devocean.Core.Infrastructure.Services.Contexts;

public class OperationContext
{
    public ClaimsPrincipal? Principal { get; }
    public string? UserName { get; }
    public Dictionary<string, string>? Claims { get; }
    public Dictionary<string, object> Bag { get; } = new();

    public OperationContext(Factory<ClaimsPrincipal> claimsFactory)
    {
        Principal = claimsFactory.Get();
        UserName = Principal?.Identity?.Name;
        if (Principal?.Claims != null)
        {
            Claims = Principal.Claims.ToDictionary(claim => claim.Type.ToLower(), claim => claim.Value);
        }   
    }

    public void SetItem<T>(string key, T value)
    {
        if (value != null) Bag.Add(key, value);
    }

    public T? GetItem<T>(string key) => Bag.GetValueOrDefault(key) is T? 
        ? (T?)Bag.GetValueOrDefault(key) 
        : default;
}