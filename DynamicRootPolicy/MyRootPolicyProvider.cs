using Rsk.Enforcer.PDP;

namespace DynamicRootPolicy;

internal class MyRootPolicyProvider : IProvideRootPolicy
{
    private readonly HttpContext context;

    public MyRootPolicyProvider(IHttpContextAccessor accessor)
    {
        context = accessor.HttpContext ?? throw new ArgumentException("No HttpContext available", nameof(accessor));
    }
    public Task<string> GetRootPolicyName()
    {
        string rootPolicy =
            context.Request.Path.StartsWithSegments("/A") ? "Policies.TenantA" :
            context.Request.Path.StartsWithSegments("/B") ? "Policies.TenantB" :
            "Policies.TenantOther";
        
        return Task.FromResult(rootPolicy);
    }
}