using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

namespace SecureMVCApp.PolicyInformationPoint
{
    public class FinanceDepartmentLimits
    {
        [PolicyAttributeValue(PolicyAttributeCategories.Resource,"MaxPurchaseOrderValue")]
        public double? MaxPurchaseOrder { get; set; }
    }
}