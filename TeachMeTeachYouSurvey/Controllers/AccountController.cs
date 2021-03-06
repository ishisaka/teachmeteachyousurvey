﻿using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Web.WebPages.OAuth;
using TeachMeTeachYouSurvey.Code;

namespace TeachMeTeachYouSurvey.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult SignIn()
        {
            return View(OAuthWebSecurity.RegisteredClientData);
        }

        [HttpPost]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/");
        }

        [HttpPost]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new LamdaResult(_ =>
            {
                OAuthWebSecurity.RequestAuthentication(
                    provider,
                    Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            });
        }

        [HttpGet]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            var result = OAuthWebSecurity.VerifyAuthentication(
                Url.Action("ExternalLoginCallback",
                new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return Redirect("~/");
            }

            var salt = ConfigurationManager.AppSettings["SaltOfUserID"];
            Func<string, string> hash = (s) => FormsAuthentication.HashPasswordForStoringInConfigFile(s, "MD5");
            var user = new TeachMeTeachYouSurvey.Models.User {
                UserId = hash(string.Join("@", salt, result.ProviderUserId, result.Provider)),
                IdProviderName = result.Provider,
                Name = result.UserName
            };

            using (var db = new TeachMeTeachYouSurvey.Models.TeachMeTeachYouDB())
            {
                if (db.Users.Find(user.UserId) == null)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                }
            }

            var cookie = FormsAuthentication.GetAuthCookie(user.Name, false);
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            ticket.GetType().InvokeMember("_UserData",
                BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Instance,
                null, ticket, new object[] { user.UserId });
            cookie.Value = FormsAuthentication.Encrypt(ticket);
            Response.Cookies.Add(cookie);

            return Redirect("~/");
        }
    }
}
