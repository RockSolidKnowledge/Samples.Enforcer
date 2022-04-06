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
            string licenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjItMDQtMzBUMDA6MDA6MDAiLCJpYXQiOiIyMDIyLTAzLTMwVDA4OjA1OjI0Iiwib3JnIjoiREVNTyIsImF1ZCI6N30=.SOctrKY4TL5ekqu1GSXUKvKBnwqmgwjtWSE9J6bBkZstqao6HrmrqRgzyfMtS5XT+NhT4eCrfpc8vn7wrU9uEW0Ff5x4XXUIY/w2LHLLD+7ZxSGI1zAJc2TRPgFSasCMrj6YwlvGODGw6f+mL8c7R2m6d4WtbraF9n1A6sr5rYKZicYGUrcMIF5fU4HLHGwr/Nh0iiVsaB7PV2vk1cEnFGytIXddMvzqvgc0qH+fH0eZKQFlw0j9HTpy/YShbLey3imefjB8IjhCnJdm/zfsBhWn1wTiyxZcyD2hQzvkd4QtamzCxTn2Uc5xl/5rjQqElpRkZ9vew15ciJw3VnL8BD9vwLBJ8cuvgNVvx3Fmqvo8VyxnIKpsTvcyax2Sf68m9bzlwJhXnyc1hrbP3qwFPO+6/oMyvJ5EPdFs5ron8KgZ1yc8JALIYtiLv6BFAfqMpj4hkaO+HrG2m7rLuQVxyK9NnYJyzhAa2sK1yHbPOSiOwbCcFHiCYhK2g6kYFNh/HIybnuE5ntAS/v09rDTj2cXuOol5ScUO1lnesXfX0qycVkeh6juC0Snz4pRbdtmfw7FLn/UC2q3BsxVH4n82q2kzfIEuKAE2Igmma/UAIUpwCvpiFpYY7QMJDzmDnMciyGfdu1DLQO06RVaOI2Grva7XteNqiKZf0NeISjbXzQg=";
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