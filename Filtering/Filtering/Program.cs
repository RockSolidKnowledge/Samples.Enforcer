using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rsk.Enforcer;
using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PEP.OutcomeActionHandlers;
using Rsk.Enforcer.PolicyModels;

namespace Filtering
{
    
    class Program
    {
        private static SaleEntry[] sales = new[]
        {
            new SaleEntry() { Item = "Red Car", Location = "USA", Price = 20.5m },
            new SaleEntry() { Item = "Yellow Car", Location = "EUR", Price = 25m },
            new SaleEntry() { Item = "Pink Car", Location = "EUR", Price = 23m },
            new SaleEntry() { Item = "Grey Car", Location = "USA", Price = 19m },
            new SaleEntry() { Item = "Purple Car", Location = "USA", Price = 27m },
        };
        
         static async Task Main(string[] args)
        {
            // See identityserver.com/products/enforcer for a new license key
            string licenseKey = "";
            string rootPolicy = "Policies.salesReport";
            ServiceCollection containerBuilder = new ServiceCollection();

            containerBuilder.AddEnforcer(rootPolicy, options =>
                {
                    options.Licensee = "Demo";
                    options.LicenseKey = licenseKey;
                })
                .AddEmbeddedPolicyStore("Filtering")
                .AddPolicyEnforcementPoint(options =>
                {
                    options.Bias = PepBias.Deny;
                });

            var container = containerBuilder.BuildServiceProvider();

            var pep = container.GetService<IPolicyEnforcementPoint>();

            var usaSalesRep = new SalesReportAuthorizationContext()
            {
                Roles = new string[] { "sales", "employee" },
                Region = "USA",
            };
            
            var europeSalesRep = new SalesReportAuthorizationContext()
            {
                Roles = new string[] { "sales", "employee" },
                Region = "EUR",
            };

            var director = new SalesReportAuthorizationContext()
            {
                Roles = new string[] { "director", "employee" },
            };

            var justAnEmployee = new SalesReportAuthorizationContext()
            {
                Roles = new string[] { "employee" }
            };

            var context =
                usaSalesRep
                //europeSalesRep
                //director
                //justAnEmployee
                ;
            
            var salesFilter = new FilterSalesByRegion();
           
            PolicyEvaluationOutcome reportAccess =
                await pep!.Evaluate(context, new OutcomeActionHandler[] {  salesFilter });

            if (reportAccess.Outcome == PolicyOutcome.Permit)
            {
                // Request that the obligation handler updates the query
                IEnumerable<SaleEntry> salesReport = salesFilter.ApplyFilter(sales);

                foreach (var sale in salesReport)
                {
                    Console.WriteLine(sale);
                }
            }
            else
            {
                Console.WriteLine("Denied access to sales report");
            }
        }
    }
}