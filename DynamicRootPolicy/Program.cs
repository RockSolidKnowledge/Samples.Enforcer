using System.Reflection;
using DynamicRootPolicy;
using Rsk.Enforcer;
using Rsk.Enforcer.AspNetCore;
using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PolicyModels;


// Demonstrates the use of a Root Policy Provider, to allow dynamic selection of a root policy
// In this case based on the path of the request. Could be used to implement multi-tenancy
// Hit /A/ will select TenantA policy and produce Hello World
// Hit /B/ will select TenantB policy and produce Hello World
// Hit /AnythihngElse/ will select Other policy resulting in access denied


var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEnforcer(options =>  {
        options.Licensee = "DEMO";
        options.LicenseKey = "Goto identityserver.com/products/enforcer for a license key"
    })
    .AddRootPolicyProvider<MyRootPolicyProvider>()
    .AddEmbeddedPolicyStore(Assembly.GetCallingAssembly(), "DynamicRootPolicy.Policies")
    .AddPolicyEnforcementPoint(o => o.Bias = PepBias.Deny)
    .AddClaimsAttributeValueProvider(o => { })
    .AddDefaultAdviceHandling();


var app = builder.Build();

var items = Assembly.GetCallingAssembly().GetManifestResourceNames().ToList();

app.MapGet("/{tenant}/", async (IPolicyEnforcementPoint pep) =>
{
    PolicyEvaluationOutcome result = await pep.Evaluate();

    if (result.Outcome == PolicyOutcome.Permit)
    {
        return "Hello World!";
    }
    else
    {
        return "Access denied";
    }
});

app.Run();