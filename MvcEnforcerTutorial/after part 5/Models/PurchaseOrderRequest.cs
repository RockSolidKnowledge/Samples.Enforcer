
using Rsk.Enforcer.PDP;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

namespace SecureMVCApp.Models
{
    public class PurchaseOrderRequest
    {
        [PolicyAttributeValue(PolicyAttributeCategories.Action,"PurchaseOrderTotal", Sensitivity = PolicyAttributeSensitivity.NonSensitive)]
        public double? Amount { get; set; }
        public string Description { get; set; }
    }
}