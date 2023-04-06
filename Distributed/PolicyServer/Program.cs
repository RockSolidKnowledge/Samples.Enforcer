using PolicyServer.PolicyInformationPoints;
using Rsk.Enforcer;
using Rsk.Enforcer.Remote.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEnforcer("AcmeCorp.Global", options =>
    {
        options.Licensee = "DEMO";
        options.LicenseKey ="Get license key from https://identityserver.com/products/Enforcer"
    })
    .AddFileSystemPolicyStore("policies")
    .AddPolicyAttributeProvider<FinanceDepartmentAttributeProvider>()
    .WithRemoteHosting(options =>
    {
        options.BaseUrl = "pdp";
    });

var app = builder.Build();

app.UseEnforcerRemoteHosting();

app.MapGet("/", () => "Enforcer Policy Server");

app.Run();

