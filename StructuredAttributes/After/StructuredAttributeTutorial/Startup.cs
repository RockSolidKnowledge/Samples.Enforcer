using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rsk.Enforcer;
using Rsk.Enforcer.AspNetCore;
using Rsk.Enforcer.PEP;
using StructuredAttributeTutorial.PIP;
using StructuredAttributeTutorial.Services;

namespace StructuredAttributeTutorial
{
    public class Constants
    {
        public const string LicenseKey = "<Your demo license Key>";
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

            services.AddSingleton<IDocumentStore, DocumentStore>();
            
            ConfigureAuthorization(services);
        }

        private void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddEnforcer("AcmeCorp.CMS", o =>
                {
                    o.Licensee = Constants.Licensee;
                    o.LicenseKey = Constants.LicenseKey;
                })
                .AddEmbeddedPolicyStore("StructuredAttributeTutorial.Policy")
                .AddPolicyEnforcementPoint(o => o.Bias = PepBias.Deny)
                .AddDefaultAdviceHandling()
                .AddClaimsAttributeValueProvider(o => { })
                .AddHttpRequestAttributeValueProvider()
                .AddPolicyAttributeProvider<UserPermissionAttributeValueProvider>()
                ;
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}