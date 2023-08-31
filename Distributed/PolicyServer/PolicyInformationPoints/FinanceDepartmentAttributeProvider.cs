using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

namespace PolicyServer.PolicyInformationPoints
{
    public class FinanceDepartmentAttributeProvider: RecordAttributeValueProvider<FinanceDepartmentLimits>
    {
        private static readonly PolicyAttribute Department =
            new PolicyAttribute("department",PolicyValueType.String,PolicyAttributeCategories.Subject);
            
        protected  override async Task<FinanceDepartmentLimits> GetRecordValue(IAttributeResolver attributeResolver, CancellationToken cts)
        {
            // Retrieve the department from the evaluation context
            IReadOnlyCollection<string> departments= await attributeResolver.Resolve<string>(Department,cts);

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