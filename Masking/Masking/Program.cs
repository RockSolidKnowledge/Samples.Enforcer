using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rsk.Enforcer;
using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PEP.OutcomeActionHandlers;
using Rsk.Enforcer.PolicyModels;

namespace Masking
{
    class Program
    {
         static async Task Main(string[] args)
        {
            // See identityserver.com/products/enforcer for a new license key
            string licenseKey = "See identityserver.com/products/enforcer for a new license key";
            string rootPolicy = "Policies.TestPolicy";
            ServiceCollection containerBuilder = new ServiceCollection();

            containerBuilder.AddEnforcer(rootPolicy, options =>
                {
                    options.Licensee = "Demo";
                    options.LicenseKey = licenseKey;
                })
                .AddEmbeddedPolicyStore("Masking")
                .AddPolicyEnforcementPoint(options =>
                {
                    options.Bias = PepBias.Deny;
                });

            var container = containerBuilder.BuildServiceProvider();

            var pep = container.GetService<IPolicyEnforcementPoint>();

            var context = new EmptyContext();

            var masking = new MaskingOutcomeActionHandler();
            PolicyEvaluationOutcome canSend = await pep.Evaluate(context, new OutcomeActionHandler[] {masking});
            if (canSend.Outcome == PolicyOutcome.Permit)
            {
                var resultDataToMask = new Response()
                {
                    Message = "Top secret",
                    From = "andy@microsoft.com",
                    To = "Sally@acme.com"
                };

                masking.ApplyMask(resultDataToMask);

                Console.WriteLine(resultDataToMask);
            }
        }
    }
}