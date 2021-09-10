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
            string licenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjEtMDYtMDNUMDE6MDA6MDUuNzI4NzUyMSswMTowMCIsImlhdCI6IjIwMjEtMDUtMDRUMDA6MDA6MDUiLCJvcmciOiJERU1PIiwiYXVkIjo3fQ==.jsy5lPG6IuG26a8lyEfAK9gQkl2XuNxm0Kz+fEqyiMk/A9jFrMEcCbqDBq5FlhLPAvKHNChcnfRGMT+lKZ+/1jNNal/uLw741unbt/Qhqk8A9ncoIQnNWxXZ1PGK/Ng2Ko60CrV+BhDJqigiY8V85hkfKNReuK5H8ZDSTZ2yBFCpvtPRsegMvFlGkh8Yq7euUURL8VGG3pgrNd+exnMa6dWYib4BNt4dgP9DEfPrTEB11ycN1RB9+AznWqCMGM3DYUxKJwEpqPOwaCj5SnDh1ZhkswJBKGhkvrx27eRMW9SrtHME/69576GNkHYteOj6LKA/P91NV8JKcF3SOQO4pAJFHNq/3Kx1Me/Q4zUYB7losjYCF66TMTvEYYzTssNiZF2PqOlmACsm719YmZR6BG9pkquiSycrgc9RF9ROaM+tiD/4RMxRknshetxNTE3ejek4177/uNIyXy2JMlk3+X4q0DI9t4xpXk+uL9IalL3azsUoNQvAUhfmsm/BnR95t+W5C9OALinEunpYPrcj4xJL6kpLf6Bg31aJLa/AM7SWacoZwt0Sb2R6eNeAzgwJYbIW10Ye5Mqco/l6OzVaNwjM0Vt2FjhbSyPJc/xmMV44R1Q9/QXXm72etRLwQAqvsPAbCXE+DR462lMu4TC1xOrpuZ2c23J8Z8anL9DGQGQ=";
            
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
                app.UseDatabaseErrorPage();
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
