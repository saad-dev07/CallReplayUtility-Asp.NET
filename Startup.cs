﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CallBackUtility.Startup))]
namespace CallBackUtility
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
           // app.MapSignalR();
        }
    }
}
