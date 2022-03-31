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
            string licenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjItMDQtMzBUMDA6MDA6MDAiLCJpYXQiOiIyMDIyLTAzLTMwVDA4OjA1OjI0Iiwib3JnIjoiREVNTyIsImF1ZCI6N30=.SOctrKY4TL5ekqu1GSXUKvKBnwqmgwjtWSE9J6bBkZstqao6HrmrqRgzyfMtS5XT+NhT4eCrfpc8vn7wrU9uEW0Ff5x4XXUIY/w2LHLLD+7ZxSGI1zAJc2TRPgFSasCMrj6YwlvGODGw6f+mL8c7R2m6d4WtbraF9n1A6sr5rYKZicYGUrcMIF5fU4HLHGwr/Nh0iiVsaB7PV2vk1cEnFGytIXddMvzqvgc0qH+fH0eZKQFlw0j9HTpy/YShbLey3imefjB8IjhCnJdm/zfsBhWn1wTiyxZcyD2hQzvkd4QtamzCxTn2Uc5xl/5rjQqElpRkZ9vew15ciJw3VnL8BD9vwLBJ8cuvgNVvx3Fmqvo8VyxnIKpsTvcyax2Sf68m9bzlwJhXnyc1hrbP3qwFPO+6/oMyvJ5EPdFs5ron8KgZ1yc8JALIYtiLv6BFAfqMpj4hkaO+HrG2m7rLuQVxyK9NnYJyzhAa2sK1yHbPOSiOwbCcFHiCYhK2g6kYFNh/HIybnuE5ntAS/v09rDTj2cXuOol5ScUO1lnesXfX0qycVkeh6juC0Snz4pRbdtmfw7FLn/UC2q3BsxVH4n82q2kzfIEuKAE2Igmma/UAIUpwCvpiFpYY7QMJDzmDnMciyGfdu1DLQO06RVaOI2Grva7XteNqiKZf0NeISjbXzQg=";
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
                await pep.Evaluate(context, new OutcomeActionHandler[] {  salesFilter });

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