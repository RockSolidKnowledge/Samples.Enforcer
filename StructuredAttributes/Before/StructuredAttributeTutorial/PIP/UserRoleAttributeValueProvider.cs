using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

namespace StructuredAttributeTutorial.PIP;

public class UserRoleAttributeValueProvider : RecordAttributeValueProvider<UserRoleRecord>
{
    private readonly Dictionary<string, string> roles = new()
    {
        { "Alan", "editor" },
        { "Bob", "writer" }
    };

    protected override async Task<UserRoleRecord> GetRecordValue(IAttributeResolver attributeResolver, CancellationToken ct)
    {
        var userAttribute = await attributeResolver.Resolve<string>(
            new PolicyAttribute("request.user", PolicyValueType.String, PolicyAttributeCategories.Subject),
            ct);

        var user = userAttribute.SingleOrDefault();

        if (user == null || !roles.TryGetValue(user, out var role)) return null;

        return new UserRoleRecord
        {
            Role = role
        };
    }
}