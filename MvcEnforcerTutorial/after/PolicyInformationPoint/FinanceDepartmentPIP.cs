using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

namespace SecureMVCApp.PolicyInformationPoint
{
    public class FinanceDepartmentAttributeValueProvider: RecordAttributeValueProvider<FinanceDepartmentLimits>
    {
        private static readonly PolicyAttribute Department =
            new PolicyAttribute("department",PolicyValueType.String,PolicyAttributeCategories.Subject);
            
        protected  override async Task<FinanceDepartmentLimits> GetRecordValue(IAttributeResolver attributeResolver)
        {
            // Retrieve the department from the evaluation context
            IReadOnlyCollection<string> departments= await attributeResolver.Resolve<string>(Department);

            double purchaseOrderLimit = 0;
            switch (departments.Single())
            {
                case "engineering": purchaseOrderLimit = 500;
                    break;
                case "finance": purchaseOrderLimit = 2000;
                    break;
            }

            return new FinanceDepartmentLimits()
            {
                MaxPurchaseOrder = purchaseOrderLimit
            };
        }
    }
}