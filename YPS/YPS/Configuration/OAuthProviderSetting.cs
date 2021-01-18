using System;
using Xamarin.Auth;
using YPS.CommonClasses;
using YPS.CustomToastMsg;

namespace YPS
{
    public class OAuthProviderSetting
    {

        public string ClientId { get; private set; }
        public string ConsumerKey { get; private set; }
        public string ConsumerSecret { get; private set; }
        public string RequestTokenUrl { get; private set; }
        public string AccessTokenUrl { get; private set; }
        public string AuthorizeUrl { get; private set; }
        public string CallbackUrl { get; private set; }

        public OAuth2Authenticator LoginWithProvider(string Provider)
        {
            OAuth2Authenticator auth = null;
            switch (Provider)
            {
                case "IIJ":
                    {
                        auth = new OAuth2Authenticator(           

              clientId: Settings.IIJConsumerKey,
          clientSecret: Settings.IIJConsumerSecret,
          //clientId: "2fb7243b1052945db821e9b2f2d207ccb358d0732ed934b5c9dd73d2f49bc92a",
          //clientSecret: "c442a212e3eea5f9b7615bfb370978487e134119f2d320fdb50cf48a773c91b7",
          scope: "openid profile",

          authorizeUrl: new Uri("https://www.auth.iij.jp/op/authorization"),
          redirectUrl: new Uri(HostingURL.HubConnectionUrl +"Home/About"),          
          accessTokenUrl: new Uri("https://www.auth.iij.jp/op/token"));
                        break;
                    }
            }
            return auth;
        }
    }
    ///// For LinkedIN login, for configure refer http://www.c-sharpcorner.com/article/register-identity-provider-for-new-oauth-application/
}

