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
        public const string LicenseKey = "your license key";
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