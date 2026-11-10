using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System.Collections.Generic;
//using System.IdentityModel.Tokens;
using Owin;
//using Microsoft.Owin.Builder;

[assembly: OwinStartup(typeof(POMS.Startup))]

namespace POMS
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "cookies"
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "tempstate",
                AuthenticationMode = AuthenticationMode.Passive
            });
        }


    }
}