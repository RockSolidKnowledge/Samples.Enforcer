using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using SecureMVCApp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rsk.Enforcer;
using Rsk.Enforcer.AspNetCore;
using Rsk.Enforcer.PEP;
using SecureMVCApp.Models;
using SecureMVCApp.PolicyInformationPoint;
using SecureMVCApp.Services;

namespace SecureMVCApp
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
            string licensee = "DEMO";
            string licenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjMtMDctMTlUMDA6MDA6MDAiLCJpYXQiOiIyMDIzLTA2LTE5VDIyOjM0OjE3Iiwib3JnIjoiREVNTyIsImF1ZCI6N30=.IydhalJkfx7EflmJz4eXblCuyrvzF/aYJ0tcq2feAlhbMkTxGuWRpZ7wP3bGxEWc82K87xjPF51s8bnYZI7IAPtH4534lFkD+iadbUUOJ3IfwuBvYROJz4pygGQDEiD74glMafOoMQ7FB8vI9qBtCHcclwVmIFaQLddJjThQ5yxUQtADBgJIVz0t47CxAOWmFZdFtRVntz1ENb3sr46ln36od689jEGwYIRas6O1EwgiSbnnGgULorDqDQESmXlXGTOpXdDK2+Qvy7KmAo5hDbyJiBRlyeCvBGUKufXfc4fNm0APdH9IrY6ddrMaoMOObZGmtHRBXDTby1fO9ZgyHuepHFAOUaULysG1z4krZ7bo22u2P95JRZUICxsTWW3JcMU9YHLyxmaA9j5A0JoSrhep0O8RvqKrAt4fexvW+Q1U5oYGfjuF6LzM5P3GHBbMlAjGU7XXPVbpuQiHVFLllyoD0lTsCSGasX7HSyVY8xgceV5Kubx+iX84LmcThguNHRJgaYCuci9Cdc49ohOjojHd+jdxdcSkitukuAYl/3MGvNpvhZ6f0CaaBmE9yNnXmD0PwcWYFy1/nG+lW3DkApg9Z9kN4fbqlJTaeK+aY7991JUTcr24YgtOyJaMpPaeIrVWfA/VjxNljQRO3lUtm++zjmBJ1Rx3KKL61Tga2KM=";
            services.AddLogging(lb =>
            {
                lb.SetMinimumLevel(LogLevel.Trace);
                lb.AddConsole();
            });

            services.AddSingleton<IManagePurchaseOrders, PurchaseOrderService>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("Identity"));
            services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 1;

                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services
                .AddEnforcer("AcmeCorp.Global", options =>
                {
                    options.Licensee = licensee;
                    options.LicenseKey = licenseKey;
                })
                .AddFileSystemPolicyStore("policies")
                .AddPolicyEnforcementPoint(o => o.Bias = PepBias.Deny)
                .AddClaimsAttributeValueProvider(o => { })
                .AddDefaultAdviceHandling()
                .AddPolicyAttributeProvider<FinanceDepartmentAttributeValueProvider>()
                .AddEnforcerAuthorizationRazorViewDenyHandler<AuthorizationFailureAdvice>("~/Views/Shared/NotAuthorized.cshtml");
            
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            await AddUsers(app.ApplicationServices);
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
        
        private async Task AddUsers(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            
            var userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
            
            await AddUser( userManager,"alice@acme.com", "alice",
                new Claim("role", "manager"),
                new Claim("role", "employee"),
                new Claim("department","engineering"));
            
            await AddUser(userManager, "bob@acme.com", "bob",
                new Claim("role", "employee"),
                new Claim("department","engineering"));
            
            await AddUser(userManager, "sally@acme.com", "sally",
                new Claim("role", "employee"),
                new Claim("role", "manager"),
                new Claim("department","finance"));

            await AddUser(userManager, "charlie@acme.com", "charlie",
                new Claim("role", "contractor"),
                new Claim("department", "engineering"));
        }

        private async Task AddUser( UserManager<IdentityUser> userManager,string email, string password, params Claim[] claims)
        {
            var result = await userManager.CreateAsync(new IdentityUser(email)
            {
                Email =  email
            }, password);
            
            var user = await userManager.FindByNameAsync(email);

            await userManager.AddClaimAsync(user, new Claim("email", email));
            
            foreach(Claim claim in claims )
            {
                await userManager.AddClaimAsync(user, claim);
            }
        }
    }
}
