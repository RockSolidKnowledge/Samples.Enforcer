using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PolicyModels;
using Rsk.Enforcer.Services.Logging;

namespace EvaluateObligationHandler
{
    public class AttributeSpyOutcomeActionHandler : OutcomeActionHandler
    {
        public override Task Execute(IEnumerable<PolicyAttributeValue> parameters, IEnforcerLogger evaluationLogger)
        {
            Attributes = parameters.ToArray();
            return Task.CompletedTask;
        }

        public PolicyAttributeValue[] Attributes { get; private set; }
        public override string Name => "outcomeSpy";
    }
}