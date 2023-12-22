using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rsk.Enforcer;
using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PEP.OutcomeActionHandlers;
using Rsk.Enforcer.PolicyModels;

namespace EvaluateObligationHandler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // See identityserver.com/products/enforcer for a new license key
            //
            string licenseKey = "See identityserver.com/products/enforcer for a new license key";            string rootPolicy = "Policies.TestPolicy";
            ServiceCollection containerBuilder = new ServiceCollection();

            containerBuilder.AddEnforcer(rootPolicy, options =>
                {
                    options.Licensee = "Demo";
                    options.LicenseKey = licenseKey;
                })
                .AddEmbeddedPolicyStore("EvaluateObligationHandler")
                .AddPolicyEnforcementPoint(options =>
                {
                    options.Bias = PepBias.Deny;
                });

            var container = containerBuilder.BuildServiceProvider();

            var pep = container.GetService<IPolicyEnforcementPoint>();

            var context = new EmptyContext();
           
            AttributeSpyOutcomeActionHandler[] obligations = new[]
            {
                new AttributeSpyOutcomeActionHandler("outcomeSpy"),
                new AttributeSpyOutcomeActionHandler("anotherSpy")
            };
            
            await pep.Evaluate(context, obligations);

            foreach (var obligation in obligations)
            {
                Console.WriteLine(obligation.Name);
                foreach (PolicyAttributeValue attribute in obligation.Attributes)
                {
                    Console.WriteLine($"{attribute.Name} : {String.Join(",", attribute.GetValue<object>())}");
                }

                Console.WriteLine();
            }

        }
    }
}