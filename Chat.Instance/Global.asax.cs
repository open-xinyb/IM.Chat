using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Chat.Instance
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            System.Data.SqlClient.SqlDependency.Start(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            System.Data.SqlClient.SqlDependency.Stop(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }
    }
}