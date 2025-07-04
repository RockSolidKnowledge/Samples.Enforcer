using KongTutorialRemoteEnforcer;
using KongTutorialRemoteEnforcer.AttributeValueProviders;
using Rsk.Enforcer;
using Rsk.Enforcer.AspNetCore;
using Rsk.Enforcer.AuthZen;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEnforcer("AcmeCorp.Global", options =>
    {
        options.Licensee = "DEMO";
        options.LicenseKey = "Obtain a free demo license key from https://www.identityserver.com/products/enforcer";
    })
    .AddFileSystemPolicyStore("Policies")
    .AddPolicyAttributeProvider<RoleAttributeValueProvider>()
    ;

var app = builder.Build();

app.UseEnforcerAuthZen();

app.Run();
