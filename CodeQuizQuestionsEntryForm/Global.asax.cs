using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CodeQuizQuestionsEntryForm
{
    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/fwlink/?LinkId=301868
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            IdentityConfig.ConfigureIdentity();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var request = HttpContext.Current.Request;
            var logText = new StringBuilder();

            logText.AppendLine("#exception");
            logText.AppendLine(Server.GetLastError().ToString());

            try
            {
                logText
                    .AppendLine("#request")
                    .AppendLine(request.HttpMethod + " " + request.RawUrl)
                    .AppendLine(request.ServerVariables["ALL_RAW"]);
            }
            catch { }

            try
            {
                logText.AppendLine("#form");
                foreach (var key in request.Form.AllKeys)
                {
                    logText.AppendLine(key + "=" + request.Form[key]);
                }
            }
            catch { }

            try
            {
                logText.AppendLine("#server-variables");
                foreach (var key in request.ServerVariables.AllKeys.Where(k => new[] { "ALL_HTTP", "ALL_RAW" }.Contains(k) == false))
                {
                    logText.AppendLine(key + "=" + request.ServerVariables[key]);
                }
            }
            catch { }

            try { Trace.TraceError(logText.ToString()); }
            catch { }
        }
    }
}
