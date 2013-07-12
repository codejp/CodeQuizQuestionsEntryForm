using System;
using System.Collections.Generic;
using System.Configuration;
using Newtonsoft.Json;
using Owin;

namespace CodeQuizQuestionsEntryForm
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseApplicationSignInCookie();
            
            // Enable the application to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie();

            var appSettings = ConfigurationManager.AppSettings;
            Func<string, Dictionary<string, string>> getOAuthSetting = key =>
                JsonConvert.DeserializeObject<Dictionary<string, string>>(appSettings[key]);

            var oauthMicrosoftSetting = getOAuthSetting("OAuth.Microsoft");
            app.UseMicrosoftAccountAuthentication(
                oauthMicrosoftSetting["clientId"],
                oauthMicrosoftSetting["clientSecret"]);

            var oauthTwitterSetting = getOAuthSetting("OAuth.Twitter");
            app.UseTwitterAuthentication(
                oauthTwitterSetting["consumerKey"],
                oauthTwitterSetting["consumerSecret"]);

            var oauthFacebookSetting = getOAuthSetting("OAuth.facebook");
            app.UseFacebookAuthentication(
                oauthFacebookSetting["appId"],
                oauthFacebookSetting["appSecret"]);

            app.UseGoogleAuthentication();
        }
    }
}