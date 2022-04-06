using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

namespace Filtering;

/// <summary>
/// Represents the authorization context when making a sales report authorization request
/// </summary>
public class SalesReportAuthorizationContext: AuthorizationContext<SalesReportAuthorizationContext>
{
    public SalesReportAuthorizationContext() : base("reports","view","salesReport")
    {
        Roles = Array.Empty<string>();
    }
    
    [PolicyAttributeValue(PolicyAttributeCategories.Subject,"role")]
    public string[] Roles { get; set; }
    
    [PolicyAttributeValue(PolicyAttributeCategories.Subject,"region")]
    public string? Region { get; set; }
}