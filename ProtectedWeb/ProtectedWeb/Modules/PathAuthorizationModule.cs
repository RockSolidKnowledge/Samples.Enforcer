using Microsoft.Extensions.DependencyInjection;
using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;
using System;
using System.Collections.Generic;
using System.Web;

namespace ProtectedWeb.Modules
{
    public class HttpAuthorizationContext : AuthorizationContext<HttpAuthorizationContext>
    {
        public HttpAuthorizationContext(string action , string path) : base("http",action,path)
        {
            
        }

        [PolicyAttributeValue(PolicyAttributeCategories.Subject,"role")]
        public List<string> Roles { get; set; }
    }
    public class PathAuthorizationModule : IHttpModule
    {
        public void Dispose()
        {
            return;
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += AuthorizePageAccess;
        }

        private  void AuthorizePageAccess(object sender, EventArgs e)
        {
            HttpContext ctx = ((HttpApplication)sender).Context;

            string path = ctx.Request.Path;

            var authorizationCtx = new HttpAuthorizationContext(ctx.Request.HttpMethod, path);

      
            authorizationCtx.Roles = new List<string> {
                "admin" ,
                "employee"
            };

            using (var services = Global.Services.CreateScope())
            {
                var pep = services.ServiceProvider
                    .GetRequiredService<IPolicyEnforcementPoint>();

                PolicyEvaluationOutcome result = pep.Evaluate(authorizationCtx).Result;
                if (result.Outcome != Rsk.Enforcer.PolicyModels.PolicyOutcome.Permit)
                {
                    ctx.Response.StatusCode = 403;
                    ctx.Response.End();
                }
            }
            return;
        }
    }
}