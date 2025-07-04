using Rsk.Enforcer;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEnforcer("AcmeCorp.Global", options =>
    {
        options.Licensee = "DEMO";
        options.LicenseKey = "Obtain a free demo license key from https://www.identityserver.com/products/enforcer";
    })
    .AddFileSystemPolicyStore("Policies")
    ;

var app = builder.Build();

app.Run();
