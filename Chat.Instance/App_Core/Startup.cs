using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup("Configuration", typeof(Startup))]

//namespace Chat.Instance
//{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
//}