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
            string licenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjItMTItMTRUMDA6MDA6MDAiLCJpYXQiOiIyMDIyLTExLTE0VDE5OjQzOjIxIiwib3JnIjoiREVNTyIsImF1ZCI6N30=.gtxAnW1zdxJeSgNCCbnh28gp4YjnQqiqvcOglXkRlDu9Ewa8YtrJmjRkxwzkDAUU9JBxk5OVO9npuDfh3++ToVvGwCvLm2/CmD2kvcLxx12ZzBCmVniQC2C9pyI9dMfpnJdGFfJSo4VUYjFQ6l8+7pjec4c6mq2LEPJUOQM2sly8ijrN21uZSlZ1QLA8sosCNNAjEkxVTybvc59SEx/9YlyqhWSSusxE299Fhtex0Lf6gAnnx2F7TVkaAXG9+C6U8qzhy8oj4KEZlCucDdwuaPg595OXISM5HDmrAcIHF5snfY20+qH/e3xX1cFMzB2sQK6yZ0D7CIyis1ny/+9FMYgh7HcJjfc0AuLVN+AV8gJr9/AsL9aHBe587si/a/qe6ws4C6+xafjV+vhLDXlc5JPErj1oFx1ugG/l6N3VeMSZqVkI29Qd9v1DJslDfl4Wtjit6U6N4m0FURbZ0Stw4TfY6+3a6aUJ21ttqSFc2y+suSBV7+DSABBM0nSeDKutnMGnBwWBhN3SBTCTawRrIPiVlvWvresqArHd3UYPdYh4pgS9p5wlLNDaMKLJRHUGQPtvWUXuS1zoIPxxMC5nVAcO/upuq8hFpG07ZSuS6B6HNgSdjWRG2MjOCX2yDzLNGTNL0tjSVhYD88vndSL4q0phgPUOOWDe+5brIzh9LMw=";
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