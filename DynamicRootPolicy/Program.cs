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
        options.LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjMtMDctMTRUMDA6MDA6MDAiLCJpYXQiOiIyMDIzLTA2LTE0VDIwOjM1OjQ4Iiwib3JnIjoiREVNTyIsImF1ZCI6N30=.N7Zzc8N7lzJqjfI3xONnIcS701F/b3HCyASFIwoybbP+RfG5VNLc8CYOnVQ7fOVmr/MTH9vmLLPdeje/XzpVqsqNqb/rorS9MOABBByieRhKa8eACaAyusvZL4lYufGyYvEnL0wuwuJj6JAhcGuoQPmcMnyRGUC45gzzGJnKPAT5UMN5qJi3XWBPbt84XjeYIvoc0E/XajdxSul5+KHmk5aRC0QvOer4yox/3F1ly604BV3ACre7OEZ7CzYvD6uW6T6xFR1lGStEgkZJx1cyumuJFX+Ev5V4wEhOoTgD6y2EQGA9elkTHSzlRELOHOmSwNVnt3CRmExLLgImezxk+RIfGWwFIAfYs0CRN8CVzFXbttiii8Ai3WqYn2cQOKYw5dM6DMvEXAHVmFoL78sC54dSJYFr7f3a7SJ5zGQ0pigDvrLNof7aSEFHFwHv7DrCGYwnidQxH8T1cfL2jDpC0ljVKXdKd4Jul+bVgm6s70XVgkPR4MNzPivxNqvV/jwFDjX/dQEt5jlmElCyJUZaA8cCviVDZM83zRL+2IVxG+b3NPO5RadRakZ4UoRnoysctgZV0C7roTVR5QWeJimNZeKdqotA5nw4ibgw5P+5cTsrj7XA6JJ7oo0TGc8xKdjzyfZfTkwPxAjdLrFDD6+4E8KgVFPyC8PNaCneVAHKeb0=";
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