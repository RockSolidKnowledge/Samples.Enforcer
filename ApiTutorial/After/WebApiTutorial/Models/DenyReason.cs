using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

namespace WebApiTutorial.Models
{
    [EnforcerAdvice("accessDeniedAdvice")]
    public class DenyReason
    {
        private const string QuotesCategory = "urn:acmecorp-quotes";

        [PolicyAttribute(QuotesCategory, "deniedReason")]
        public string Reason { get; set; }
    }
}