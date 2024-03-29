using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rsk.Enforcer;
using Rsk.Enforcer.AspNetCore;
using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PEP.OutcomeActionHandlers;
using Rsk.Enforcer.Remote;
using Rsk.Enforcer.Services.DataMasking;
using SecureMVCApp.Data;
using SecureMVCApp.PolicyInformationPoint;
using SecureMVCApp.Services;

namespace PolicyServer.Client
{
    
    internal class NullMaskingService : IMaskingOutcomeActionHandlerCreator
    {
        public MaskingOutcomeActionHandler Create()
        {
            return new MaskingOutcomeActionHandler();
        }
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


            services.AddSingleton<IMaskingOutcomeActionHandlerCreator, NullMaskingService>();
            services
                .AddRemoteEnforcer(o =>
                {
                }, r =>
                {
                    r.EndpointAddress = "http://localhost:5132/pdp";
                })
                .AddPolicyEnforcementPoint(o => o.Bias = PepBias.Deny)
                .AddClaimsAttributeValueProvider(o => { })
                .AddDefaultAdviceHandling()
                .AddEnforcerAuthorizationRazorViewDenyHandler<GeneralViewDenyHandler>();

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
