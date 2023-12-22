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
        public const string LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjQtMDEtMjBUMDA6MDA6MDAiLCJpYXQiOiIyMDIzLTEyLTIwVDE4OjI0OjU5Iiwib3JnIjoiREVNTyIsImF1ZCI6N30=.WkpW2oOSwgzgN//yTAbqFHrCW4sdepY36e4GtvVCNSwQ7ae11HmvpOFLcS7HRjIdXHKI+QjF3xsEPKrKkHKfH+Kz8luLJpALr46GvG/fvU/JQW+FPG73OLdzohO7MjsbS85qiKFBqaFrkI1aKADGRCaxgr/+0+soFnwBb/evzYltGfg3h6s9jjS1buHLY+b7wOfYHoKTVNRZJKfhKzAo0dZk07OC5FYv02x49rNIlCy3KHOi8dRnSB9BTsH+GUh5Oc9lby6SbgIAmV1aLAZy86HKlGCWWlmIKIy07PKrSAfxL/CmNs/SZ3RVU+BvtEsK3IDhkzuj0O2gY0F9JEebFazs0I4j4i76HyyxjMvGOgFQ29PNcraGhHVgYnoGOHX0zWhL6fBAYsuQlfjJVCt4tPx9BGvouItuTGjzP2frDkjnqDpWoDXOp3FSOwWmA4kIiqBSSwLqILo/yM+o1yXt9/tktT8glIYFTLk4ARatdbjDzx8vDcSQOVrdph+a3/CK2Ulnq+f/U8ulq1OROStm0yGQqTKbt1Mw5JZDUpDL/3SwUg1++2tHSxPIlHyWnzi0wiJ8cDSiwcTlbRHi8W2ECA3I8tJ2w1y+vbk215WFbi6JsmGqs32p1T0sDvL0PzjfAvudvalaXJVZ5IwF5JhH6RfY3W3T+8N1B4R6qpqm4lk=";
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