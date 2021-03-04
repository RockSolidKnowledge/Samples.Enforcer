using System;
using Microsoft.Extensions.DependencyInjection;
using Rsk.Enforcer;
using Rsk.Enforcer.PDP;
using Rsk.Enforcer.PEP;

namespace EnforcerFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            var sc = new ServiceCollection();

            ConfigureServices(sc);

            var sp = sc.BuildServiceProvider();

            IPolicyEnforcementPoint pep = sp.GetService<IPolicyEnforcementPoint>();

            var ctx = new OfficeAuthorizationContext("Enter", new string[] {"employee"});

            var authorizationResult = pep.Evaluate(ctx).Result;

            Console.WriteLine(authorizationResult.Outcome);
        }

        public const string LicenseKey = "Obtain a demo license key from https://identityserver.com/products/enforcer";
        public const string Licensee = "DEMO";

        private static void ConfigureServices(IServiceCollection services)
        {
           
                var licensee = Licensee;
                var licenseKey = LicenseKey;
                var policyRootDirectory = @"..\..\policies";
              
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
