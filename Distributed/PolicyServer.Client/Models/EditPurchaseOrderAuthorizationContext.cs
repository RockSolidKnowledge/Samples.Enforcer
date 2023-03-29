using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

namespace SecureMVCApp.Models
{
    public class EditPurchaseOrderAuthorizationContext : AuthorizationContext<EditPurchaseOrderAuthorizationContext>
    {
        public EditPurchaseOrderAuthorizationContext(string action) :base("PurchaseOrder",action)
        {
            
        }
        [PolicyAttributeValue(PolicyAttributeCategories.Resource,"PurchaseOrderDepartment")]
        public string PurchaseOrderDepartment { get; set; }
        
        [PolicyAttributeValue(PolicyAttributeCategories.Action,"PurchaseOrderTotal")]
        public double? PurchaseOrderAmount { get; set; }
    }
}