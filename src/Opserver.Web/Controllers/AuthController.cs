using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Opserver.Data.SQL;
using Opserver.Helpers;
using Opserver.Models;
using Opserver.Security;
using Opserver.Views.Login;

namespace Opserver.Controllers
{
    public partial class AuthController : StatusController<SQLModule>
    {
        //private readonly IConfiguration _configuration;
        public AuthController(IOptions<OpserverSettings> settings, IHostEnvironment env,SQLModule sqlModule) : base(sqlModule, settings)
        {
            //_configuration = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            //.AddJsonFile("Config/UserSettings.json", false)
            //.Build();
        }

        [AllowAnonymous]
        [Route("login"), HttpGet]
        public IActionResult Login(string returnUrl)
        {
            if (!Current.Security.IsConfigured || Current.Security.FlowType == SecurityProviderFlowType.None)
            {
                return View("NoConfiguration");
            }

            if (returnUrl == "/")
            {
                return RedirectToAction(nameof(Login));
            }

            return View(new LoginModel());
        }

        [AllowAnonymous]
        [Route("login"), HttpPost]
        public async Task<IActionResult> Login(string user, string pass, string url)
        {
            var returnUrl = url.HasValue() ? url : "~/";
            if (Current.Security.FlowType == SecurityProviderFlowType.OIDC)
            {
                // OpenID Connect needs to go through an authorization flow
                // before we can login successfully...
                return RedirectToProvider(returnUrl);
            }

            if (!Current.Security.TryValidateToken(new UserNamePasswordToken(user, pass), out var claimsPrincipal))
            {
                return View(
                    "Login",
                    new LoginModel
                    {
                        ErrorMessage = "Login failed, Username or password is incorrect"
                    }
                );
            }

            if (!ValidateToken(user))
            {
                return View("Login",
                    new LoginModel
                    {
                        ErrorMessage = "Access denied, You don't have permission to access on this server"
                    });
            }


            await HttpContext.SignInAsync(claimsPrincipal);
            return Redirect(returnUrl);
        }

        public bool ValidateToken(string userName)
        {

            //var query = ReadFromUserSetting().Any(x => x.UserName == userName);
            var query = Module.Settings.Users.Any(x => x.UserName == userName);

            if (!query)
            {
                return false;
            }

            return true;
        }

        //private List<UserModel> ReadFromUserSetting()
        //{
        //    var users = _configuration.GetSection("users").Get<List<UserModel>>();
        //    return users;
        //}

        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
