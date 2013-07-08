using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CodeQuizQuestionsEntryForm.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace CodeQuizQuestionsEntryForm.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController() : this(IdentityConfig.Secrets, IdentityConfig.Logins, IdentityConfig.Users, IdentityConfig.Roles) { }

        public AccountController(IUserSecretStore secrets, IUserLoginStore logins, IUserStore users, IRoleStore roles)
        {
            Secrets = secrets;
            Logins = logins;
            Users = users;
            Roles = roles;
        }

        public IUserSecretStore Secrets { get; private set; }
        public IUserLoginStore Logins { get; private set; }
        public IUserStore Users { get; private set; }
        public IRoleStore Roles { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { loginProvider = provider, ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string loginProvider, string returnUrl)
        {
            // Get the information about the user from the external login provider
            var id = await HttpContext.GetExternalIdentity();
            if (id == null)
            {
                return View("ExternalLoginFailure");
            }

            // Make sure the external identity is from the loginProvider we expect
            var providerKeyClaim = id.FindFirst(ClaimTypes.NameIdentifier);
            if (providerKeyClaim == null || providerKeyClaim.Issuer != loginProvider)
            {
                return View("ExternalLoginFailure");
            }

            // Succeeded so we should be able to lookup the local user name and sign them in
            var providerKey = providerKeyClaim.Value;
            var userId = await Logins.GetUserId(loginProvider, providerKey);
            if (!String.IsNullOrEmpty(userId))
            {
                await SignIn(userId, id.Claims, isPersistent: false);
            }
            else
            {
                // No local user for this account
                if (User.Identity.IsAuthenticated)
                {
                    // If the current user is logged in, just add the new account
                    await Logins.Add(new UserLogin(User.Identity.GetUserId(), loginProvider, providerKey));
                }
                else
                {
                    // Create a local user and sign in
                    var user = new User(id.Name);
                    if (await Users.Create(user) &&
                        await Logins.Add(new UserLogin(user.Id, loginProvider, id.FindFirstValue(ClaimTypes.NameIdentifier))))
                    {
                        await SignIn(user.Id, id.Claims, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        return View("ExternalLoginFailure");
                    }
                }
            }

            return RedirectToLocal(returnUrl);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpContext.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return (ActionResult)PartialView("_ExternalLoginsListPartial", new List<AuthenticationDescription>(HttpContext.GetExternalAuthenticationTypes()));
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private Task SignIn(string userId, bool isPersistent)
        {
            return SignIn(userId, new Claim[0], isPersistent);
        }

        private async Task SignIn(string userId, IEnumerable<Claim> claims, bool isPersistent)
        {
            User user = await Users.Find(userId) as User;
            if (user != null)
            {
                // Replace UserIdentity claims with the application specific claims
                IList<Claim> userClaims = IdentityConfig.RemoveUserIdentityClaims(claims);
                IdentityConfig.AddUserIdentityClaims(userId, user.UserName, userClaims);
                IdentityConfig.AddRoleClaims(await Roles.GetRolesForUser(userId), userClaims);
                IdentityConfig.SignIn(HttpContext, userClaims, isPersistent);
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUrl)
            {
                LoginProvider = provider;
                RedirectUrl = redirectUrl;
            }

            public string LoginProvider { get; set; }
            public string RedirectUrl { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                context.HttpContext.Challenge(LoginProvider, new AuthenticationExtra() { RedirectUrl = RedirectUrl });
            }
        }

        #endregion
    }
}
