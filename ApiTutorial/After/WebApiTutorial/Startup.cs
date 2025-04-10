using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rsk.Enforcer;
using Rsk.Enforcer.AspNetCore;
using Rsk.Enforcer.PEP;
using WebApiTutorial.ApiKeyAuthentication;
using WebApiTutorial.Models;
using WebApiTutorial.PIP;
using WebApiTutorial.Services;

namespace WebApiTutorial
{
    public class Constants
    {
        public const string LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjQtMDYtMDFUMDA6MDA6MDAiLCJpYXQiOiIyMDI0LTAxLTAxVDE5OjIzOjM1Iiwib3JnIjoiREVNTyIsImF1ZCI6N30=.Vj2lPONmaK3YIl9PtAwFac0759PxA81hdFBzzxW2OE0KcpIZ/9nJY8gM5Pry+7MQ9X+VhwItkATDDTObDSJeEcdXfiHG3WXQABV/uRleGfpxpEDntJyxK632OIgdHr3f2g20CyRtJea7st+ZTry3qJdHx9Z++m5atGHq662AnTwKmG/8rXS7GitYulgaENHIpHl7jRi/eFJgKY4R0h2lXTfoHfmkg1FTDZJR8yjhHv/6kQi3QmMVEOiep3tJk+4O+TLYh8vpW5c+lKVQQDC65fU9ZgvQMqtFUsQcWn8+hfNbwmR28A7B5J89yeioYa5J6yWFWAK9hCogt6X7axPM9OdSYqRG+yjLvCsez4mhNDa913cm9hOzrzyOxu6HMKC9l7s6g6D2gpUu5PuHPsLhQvgw+umVtbYZnTewBDvkXKkRSS3ZZaRbtW4ecw4bd9JyzrMrGFSD32uIFdBI6ZkAkfgiqT42xoleom+vdpX11QMv5C7q3cttlh74fZ0RtuFMafHUBedIAiJyV04MTCDZVCMDFV/UxEdMd/Xm5VBTjhue8uVvMDyps8JI6wPyqQ1wKHQZbpDOLDjRI8f+2TD3EKksr1TIufqNkbCEJtIYLwslPVot+6kf4zr0a8qN1LOWeJSbrpKVShrOU+mv8Po67He+nRpJVIrA8O17XUvo2JU=";
        public const string Licensee = "DEMO";
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<IQuoteService, QuoteService>();

            services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
                .AddApiKeyInHeaderOrQueryParams<ApiKeyProvider>(o =>
                {
                    o.Realm = "WebApiTutorial";
                    o.KeyName = "apiKey";
                });
            
            ConfigureAuthorization(services);
        }

        private void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddEnforcer("AcmeQuotes.ReadQuotes", o =>
                {
                    o.Licensee = Constants.Licensee;
                    o.LicenseKey = Constants.LicenseKey;
                })
                .AddEmbeddedPolicyStore($"{Assembly.GetExecutingAssembly().GetName().Name}.Policy")
                .AddPolicyEnforcementPoint(o => o.Bias = PepBias.Deny)
                .AddDefaultAdviceHandling()
                .AddClaimsAttributeValueProvider(o =>
                {
                    o.NonSensitiveClaims = new[] {"subscriptionLevel"};
                })
                .AddHttpRequestAttributeValueProvider()
                .AddPolicyAttributeProvider<RateLimitingAttributeValueProvider>()
                .AddParameterizedJsonApiDenyHandler<DenyReason>(o => { });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}