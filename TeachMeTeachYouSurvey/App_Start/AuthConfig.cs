﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Web.WebPages.OAuth;
using Newtonsoft.Json;

namespace TeachMeTeachYouSurvey
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            var appSettings = ConfigurationManager.AppSettings;
            Func<string,Dictionary<string,string>> getOAuthSetting = key => 
                JsonConvert.DeserializeObject<Dictionary<string, string>>(appSettings[key]);

            var oauthMicrosoftSetting = getOAuthSetting("OAuth.Microsoft");
            OAuthWebSecurity.RegisterMicrosoftClient(
                oauthMicrosoftSetting["clientId"],
                oauthMicrosoftSetting["clientSecret"]);

            var oauthTwitterSetting = getOAuthSetting("OAuth.Twitter");
            OAuthWebSecurity.RegisterTwitterClient(
                oauthTwitterSetting["consumerKey"],
                oauthTwitterSetting["consumerSecret"]);

            var oauthFacebookSetting = getOAuthSetting("OAuth.facebook");
            OAuthWebSecurity.RegisterFacebookClient(
                oauthFacebookSetting["appId"],
                oauthFacebookSetting["appSecret"]);

            OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
