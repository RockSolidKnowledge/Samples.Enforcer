using Microsoft.Extensions.DependencyInjection;
using Rsk.Enforcer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ProtectedWeb
{
    public static class Global
    {
        public static IServiceProvider Services { get; set; }
    }
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
           var ass = Assembly.Load("Newtonsoft.Json");

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddEnforcer("Global.ApiPolicy", o =>
                {
                    o.Licensee = "DEMO";
                    o.LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjMtMTAtMjZUMDA6MDA6MDAiLCJpYXQiOiIyMDIzLTA5LTI2VDE2OjIzOjExIiwib3JnIjoiREVNTyIsImF1ZCI6N30=.T6jRK0GTRIhEtIyt70D0Y3mWX07AUB08rDIcyM2hkaHKsva2pAwMHd4aFo4c1otXWFZogyxXg/e57fsbOPStKpl4wccPrqL72qTWaUTddY4h+5ogEGfb0ZB+oqernwgqMu03k2lyqul6sK0DBnyF/So0E7aqNgz5eQewZ3TF2s+SU35mtAfJ1mH0phT2Vn0L1B89YjPkcTNHZHH9lxMoSlD0Mj31lzCgEXXLGBTRT5rAh2YYU7HBk2qk42yissHVkvgdo7s63p9c+Elpa8GetBPs5FZhJbFjqnUqdVW97TnCLRaUhyutosa74DtAGIfWSJ4+q0WK5HcNrmlmSu/sFjX5y1TQhY/ftaQozW5rcQfaS/aVZ5aeyB/DvPzDK2nS+NURI9Cs+RPQ9N2oIZ4R9H+PUb+/1kz/fYuQIU8SyxCw7cRMj2VehtLT9Yf2JZNf+79KPfWtLURxNirrxGIvGl7JksD8PSqPINnYytTTP/gJRdlDwfFrRaUGrW895rSJxB/BMyl3L+M7+bDNPSdYzacbH7OACEcKA/wB0LkehBip+dDXmtEB/31259JJsXcF5S91nEFPpbt+lGu9BWq4TXAUlha93rmJzwhhIYSsK3/lziyNOr5rX/LaIrLdEy63b3AcX65JCoy6lyLaXMUleQmN1tMra4HXlQFyYTIjbXg=";
                })
                .AddPolicyEnforcementPoint(o =>
                {
                    o.Bias = Rsk.Enforcer.PEP.PepBias.Deny;
                })
                .AddEmbeddedPolicyStore("ProtectedWeb.Policies")
                ;

            Global.Services = serviceCollection.BuildServiceProvider();
        }
    }
}
