using System;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.iOS.PageRender;
using YPS.Service;
using YPS.Views;

[assembly: ExportRenderer(typeof(ProviderLoginPage), typeof(LoginRenderer))]
namespace YPS.iOS.PageRender
{
    public class LoginRenderer : PageRenderer
    {
        #region Data member
        bool showLogin = true;
        #endregion

        /// <summary>
        /// Gets called view appears.
        /// </summary>
        /// <param name="animated"></param>
        public override void ViewDidAppear(bool animated)
        {
            try
            {
                base.ViewDidAppear(animated);
                /// Get and Assign ProviderName from ProviderLoginPage
                var loginPage = Element as ProviderLoginPage;
                string providername = loginPage.ProviderName;

                if (showLogin && OAuthConfig.User == null)
                {
                    showLogin = false;
                    /// Create OauthProviderSetting class with Oauth Implementation .Refer Step 6
                    OAuthProviderSetting oauth = new OAuthProviderSetting();

                    var auth = oauth.LoginWithProvider(providername);

                    auth.Completed += async (sender, eventArgs) =>
                    {
                        if (eventArgs.IsAuthenticated)
                        {
                            OAuthConfig.User = new UserDetails();

                            OAuthConfig.User.Token = eventArgs.Account.Properties["access_token"];
                            OAuthConfig.User.Expires = eventArgs.Account.Properties["expires_in"];
                            OAuthConfig.User.id_token = eventArgs.Account.Properties["id_token"];
                            OAuthConfig.User.Token_type = eventArgs.Account.Properties["token_type"];

                            Settings.IIJToken = OAuthConfig.User.Token;
                            Settings.expires_in = OAuthConfig.User.Expires;
                            Settings.id_token = OAuthConfig.User.id_token;
                            Settings.token_type = OAuthConfig.User.Token_type;

                            OAuthConfig.SuccessfulLoginAction.Invoke();
                        }
                        else
                        {
                        /// The user cancelled
                        try
                            {
                                App.Current.MainPage = new YPSMasterPage(typeof(LoginPage));
                            }
                            catch (Exception ex)
                            {
                                YPSService service = new YPSService();
                                await service.Handleexception(ex);
                                YPSLogger.ReportException(ex, "LoginRenderer-> in ViewDidAppear " + Settings.userLoginID);
                            }
                        }
                    };

                    auth.Error += Auth_Error;

                    try
                    {
                        PresentViewController(auth.GetUI(), true, null);
                        auth.AllowCancel = false;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Gets called when error occurs while authentication.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Auth_Error(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;

            authenticator.ShowErrors = false;
        }
    }
}