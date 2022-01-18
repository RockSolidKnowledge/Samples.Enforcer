using System;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.DependencyInjection;
using Rsk.Enforcer;
using Rsk.Enforcer.PAP;
using Rsk.Enforcer.PDP;
using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PolicyModels;

namespace JustPDP
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sc = new ServiceCollection();

            ConfigureServices(sc);

            var sp = sc.BuildServiceProvider();

            IPolicyDecisionPoint pdp = sp.GetService<IPolicyDecisionPoint>();

            var ctx = new OfficeAuthorizationContext("Enter", new string[] {"employee"});

            try
            {
                var authorizationResult = await pdp.EvaluatePolicy(ctx, new ConsoleEnforcerLogger());

                Console.WriteLine(authorizationResult.Outcome);

                PrintPolicyActions("Obligation", authorizationResult.Obligations);
                PrintPolicyActions("Advice", authorizationResult.Advice);
            }
            catch (PolicyCompilationException compilerErrors)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                foreach (var error in compilerErrors.Errors)
                {
                    Console.WriteLine($"{error.Line}:{error.Column} {error.Message} ");
                }
                
                Console.ResetColor();
            }
        }

        private static void PrintPolicyActions(string type, IEnumerable<PolicyEvaluationAction> policyOutcomeActions)
        {
            foreach (var policyOutcomeAction in policyOutcomeActions)
            {
                Console.WriteLine($"{type} {policyOutcomeAction.Name}");
                foreach (var argument in policyOutcomeAction.Arguments)
                {
                    string values = string.Join( ",",argument.GetValue<object>());
                    Console.WriteLine($"\t{argument.Name} : {values}" );
                }
            }
        }

        public const string LicenseKey = "Get a key from identityserver.com/products/enforcer";
        public const string Licensee = "DEMO";

        private static void ConfigureServices(IServiceCollection services)
        {
           
                var licensee = Licensee;
                var licenseKey = LicenseKey;
                var policyRootDirectory = @"policies";
              
                services.AddEnforcer("RSK.Samples.Global", o =>
                {
                    o.PolicyInformationPointFailureBehavior = PolicyInformationPointFailureBehavior.FailFast;
                    o.Licensee = licensee;
                    o.LicenseKey = licenseKey;
                })
                .AddFileSystemPolicyStore(policyRootDirectory)
                .AddPolicyEnforcementPoint(o => o.Bias = PepBias.Deny);

        }
       
    }
}
