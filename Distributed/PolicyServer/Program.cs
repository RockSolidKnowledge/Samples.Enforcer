using PolicyServer.PolicyInformationPoints;
using Rsk.Enforcer;
using Rsk.Enforcer.Remote.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEnforcer("AcmeCorp.Global", options =>
    {
        options.Licensee = "DEMO";
        options.LicenseKey =
            "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjMtMDQtMDlUMDA6MDA6MDAiLCJpYXQiOiIyMDIzLTAzLTA5VDE0OjE1OjIxIiwib3JnIjoiREVNTyIsImF1ZCI6N30=.Uf9ZduQt5j3qSjBKw6o2oj/qyKgAUj4Z4FVXwHHek2FizP63I9MBjzer2OeymZFpDbJpYHsZf9/h6q5x5xTrE2qiWkTMOzUJtP8EDF+r6RC34SCSfxLgM8Dmr+cQm4AQxTnqMPIftBQrSXMvdbjSLGVHwyVmFaCsCA8hUWWegJPj/KrhAWEk4WIYiGpf8RGSjVzcmQDEu8LFapVxgCTqhwjOA5MWaQhT8nwhHpHcG2hwgJ48ZXyGuNMHubANCylbpbIQCpq92zM7K3w3qqtqqwIFgd4frroyiBQpiwRearbkmY3QoXRojaqdaBgoyyqojFG35ykSBK6pRnA4I+70GP4rgTIlE0hTm2Whta722Zrx/EN0VCNpvrCxNMjKsyUZ5MURqeFmPy22+tj21x1qaLMhrbIMmn8/CaDsQlBwiym9BsVp9hhnuy2sSSADHiGfj9quOVsvuaIOZN0OQ1SaA95RG/1+BLWRMPfXXhbxUBkd5BBLw26PjA89oPdFUKeYk3jvXBAfGW4PF2wJ41ChKc1ElHleJZQ/7DjeqNe/gueze9Q8oUfdIGmUgXVZZRZ+bbt2muOyDWc70s3zI5YNv/b1mYMS7BA75IKKpLctPV2REMK5pIqjetuPMnJm/2YWrjygmbI1UnW2gTPi2eS9nK/FiMgZCgW8E/Oq5L688jU=";
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

