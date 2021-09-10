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
            string licenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjEtMTAtMTBUMDE6MDA6MDUuNDg2ODg4NiswMTowMCIsImlhdCI6IjIwMjEtMDktMTBUMDA6MDA6MDUiLCJvcmciOiJERU1PIiwiYXVkIjo3fQ==.WoEHRPpbt60/zrlZfPONrXfd193994TCjN94Y0s00vlKqsr5lXKhy3TfuxfXNHAR0bzalI9fGPMLlVCFr/nstvoc4hg9NZo0AG15lJxiYM/MDuWrysJk6Hes0kkysGmIZDeg6BZXWuEU0qd8ePKypX8FNU9WiuZ58B4zn6FxcKi4xVPIs5cUAxdS/4b4nNHVrTDApC0dGJMo7/1Fba/Anfl4r2aB8SoYKOdO/H3iaEWIMG2xpRmgQC+DxVkjgW+rzYVgIbCkF2VW/96T40mjeS8IcQTr0pEFxCPIxM8kdqF8/K/8fosGgbeIYiIa+nBjWNYaBUIaD3JXShgsfZawi124bSIUoSV8hMRrd0pHFrktRGM+ClcNUsCeQHhFVW63tsNN7yQJ+BOUQN7+7JwsWtXdwh0UOJ+XBz6pHXQ1sMTpMKgn9qRGNd+/AeuG86aS3kUxl/Qee849cv54kveurvyoHVs88Z+1Dpo7ssZuWRRca8+W0GDq2VpI4ZVRZcrxXumzE+lZzZWg3sundMDPhNUUD7SIFYAgWkg0YKmMQvzPdmZrWA1JJicvkvSifaKX/2dGIvSf4EDhJxkejmLl5a2hKLnveuK+A8rw/BKC5EkpBLGcLwzRa3OXXdWHZNqBKU86XPpj1+lm/EuUgS/2fJ/Me9+wNYd7ejYnw+39zYM=";
            string rootPolicy = "Policies.TestPolicy";
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