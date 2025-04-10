using System.Collections.Generic;
using Rsk.Enforcer.PDP;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;
using Rsk.Enforcer.PolicyModels.StructuredAttributes;

namespace StructuredAttributeTutorial.PIP;

public class UserPermissionRecord
{
    [PolicyAttributeValue(PolicyAttributeCategories.Subject, "permissions", Sensitivity = PolicyAttributeSensitivity.NonSensitive)]
    public IEnumerable<Permission> Permissions { get; set; }
}

public class Permission : StructuredAttribute<Permission>
{
    public string Action { get; set; }
    public string Resource { get; set; }
}