using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

namespace KongTutorialRemoteEnforcer.AttributeValueProviders;

public class UserRole
{
    public UserRole(string role)
    {
        Role = role;
    }

    [PolicyAttributeValue(PolicyAttributeCategories.Subject, "role")]
    public string Role { get; set; }
}

public class RoleAttributeValueProvider : RecordAttributeValueProvider<UserRole>
{
    private readonly Dictionary<string, UserRole> _roles = new()
    {
        { "Alice", new UserRole("Admin") },
        { "Bob", new UserRole("Worker") },
        { "Carol", new UserRole("Guest") }
    };
    
    protected override async Task<UserRole> GetRecordValue(IAttributeResolver attributeResolver, CancellationToken ct)
    {
        var subAttribute = await attributeResolver.Resolve<string>(
            new PolicyAttribute("id", PolicyValueType.String, PolicyAttributeCategories.Subject), ct);

        var userName = subAttribute.SingleOrDefault();

        if (userName == null) return null;

        if (!_roles.TryGetValue(userName, out var role))
            return null;

        return role;
    }
}