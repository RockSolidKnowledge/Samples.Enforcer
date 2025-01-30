using Rsk.Enforcer.PDP;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

namespace StructuredAttributeTutorial.Models;

public class CmsRequest
{
    [PolicyAttributeValue(PolicyAttributeCategories.Subject, "request.user", Sensitivity = PolicyAttributeSensitivity.NonSensitive)]
    public string User { get; set; }
    
    [PolicyAttributeValue(PolicyAttributeCategories.Subject, "request.authenticationType", Sensitivity = PolicyAttributeSensitivity.NonSensitive)]
    public string AuthenticationType { get; set; }
}