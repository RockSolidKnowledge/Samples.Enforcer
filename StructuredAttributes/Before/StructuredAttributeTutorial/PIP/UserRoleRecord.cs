using Rsk.Enforcer.PDP;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

namespace StructuredAttributeTutorial.PIP;

public class UserRoleRecord
{
    [PolicyAttributeValue(PolicyAttributeCategories.Subject, "role", Sensitivity = PolicyAttributeSensitivity.NonSensitive)]
    public string Role { get; set; }
}