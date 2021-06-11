using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rsk.Enforcer;
using Rsk.Enforcer.AspNetCore;
using Rsk.Enforcer.PEP;

namespace MaskingWebApp
{
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
            services.AddControllersWithViews();
            
            // See identityserver.com/products/enforcer for a new license key
            string licenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjEtMDctMDlUMDE6MDA6MDUuOTg1MDM5KzAxOjAwIiwiaWF0IjoiMjAyMS0wNi0wOVQwMDowMDowNSIsIm9yZyI6IkRFTU8iLCJhdWQiOjd9.K6ojh1tCXt3AdROXQUWD3RRqrxfHdVd26YzNbWB6QOyvWJhOjmz4unzUmUUkei3IzEpcCcPN8k9nGMM1l0SW0+lz0gK/Y+EQMT5FPRFCyGsU5Gs4TFbS2umlAYWh2CDaqtUVS6iMiK5SzKbFPvd/GAbzmdYliaZtExbLveaEWLt23exxZn9nKppETR6UhHtqM6nBm/k5Ekc83i//MiA3XnMcu11hHb3XyOquVlX2rbr/cyLeajJBmCvUUqJYognHJ3gQYPZygQdw4ECCdpuSmTVAtyB+4W/GuICWFjq5AbbgbZOznNQMJ8ODiasXc1+OyjzzPkiX9oOFxhjAClaVVZU4fc3Yi7C1TfugJ+PzwLMm9Omzr0qRfs2lkNlQERIpEogfh26QhOlB6y4V98C0ADKiJHzW+heSnTKZEZzMvaMBiamgbxOwSuzBy+UOzEdnzCor+kzFXa3FMADuWX4u/zdpYWikq6UpzRP+QqapLFKI1IMUOuNAkLEhGcHMbvcp6yLfPfkts4iOucePSrhz/6SXWO+RGuhTXzp+6QCyHxB00+gDqm4HO/BVk8mvBTs2tmJAQDByEC5oXT+AIg7xBlLWPhob25zd8ucPmFemO11KrkvV6viWeHW0TfskH7910pT77BrpRFEg3a5XIDEiCXnm9rsnKnaPe82//ZaJOHU=";
            string rootPolicy = "Policies.TestPolicy";

            services.AddEnforcer(rootPolicy, options =>
                {
                    options.Licensee = "Demo";
                    options.LicenseKey = licenseKey;
                })
                .AddEmbeddedPolicyStore("MaskingWebApp")
                .AddPolicyEnforcementPoint(options =>
                {
                    options.Bias = PepBias.Deny;
                })
                .AddDefaultAdviceHandling();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}