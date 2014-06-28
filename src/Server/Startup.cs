using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Server.Startup))]

namespace Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use((context, next) =>
            {
                var userAgent = context.Request.Headers["User-Agent"];

                if (userAgent == null)
                {
                    return next();
                }

                var userName = "Browser";
                if (userAgent.IndexOf("NativeHost", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    userName = "Phone";
                }

                var ci = new ClaimsIdentity("UserAgent");
                ci.AddClaim(new Claim(ClaimTypes.Name, userName));
                var cp = new ClaimsPrincipal(ci);
                context.Request.User = cp;

                return next();
            });

            app.MapSignalR();
        }
    }
}
