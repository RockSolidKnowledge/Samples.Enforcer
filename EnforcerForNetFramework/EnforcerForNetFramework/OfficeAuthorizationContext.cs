using Rsk.Enforcer.PDP;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

namespace EnforcerFramework
{
    public class OfficeAuthorizationContext : AuthorizationContext<OfficeAuthorizationContext>
    {
        [PolicyAttributeValue(PolicyAttributeCategories.Subject,"role",Sensitivity = PolicyAttributeSensitivity.NonSensitive)]
        public string[] Roles { get; }

        public OfficeAuthorizationContext(string officeAction , string[] roles) : base("Office", officeAction)
        {
            Roles = roles;
        }
    }
}