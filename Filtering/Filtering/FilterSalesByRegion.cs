using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PolicyModels;
using Rsk.Enforcer.Services.Logging;

namespace Filtering
{
    public class FilterArguments
    {
        [PolicyAttribute(PolicyAttributeCategories.Resource, "region")]
        public string? Region { get; set; }
    }

    public class FilterSalesByRegion : OutcomeActionHandler<FilterArguments>
    {
        private Func<SaleEntry, bool> filter = _ => true;

        protected override Task Execute(FilterArguments parameters, IEnforcerLogger evaluationLogger)
        {
            if (parameters.Region != null)
            {
                filter = se => se.Location == parameters.Region;
            }

            return Task.CompletedTask;
        }

        public override string Name => "filterSalesByRegion";

        public IEnumerable<SaleEntry> ApplyFilter(IEnumerable<SaleEntry> query)
        {
            return query.Where(filter);
        }
    }

}