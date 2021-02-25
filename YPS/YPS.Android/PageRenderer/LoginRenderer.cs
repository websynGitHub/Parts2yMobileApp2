using Android.App;
using Android.Content;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YPS.CommonClasses;
using YPS.Droid.PageRender;
using YPS.Parts2y.Parts2y_Views;
using YPS.Views;

[assembly: ExportRenderer(typeof(ProviderLoginPage), typeof(LoginRenderer))]
namespace YPS.Droid.PageRender
{
    public class LoginRenderer : PageRenderer
    {
        #region Data member
        bool showLogin = true;
        #endregion

        public LoginRenderer(Context context) : base(context) { }

        /// <summary>
        /// Gets called view appears.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            /// Get and Assign ProviderName from ProviderLoginPage
            var loginPage = Element as ProviderLoginPage;
            string providername = loginPage.ProviderName;

            var activity = this.Context as Activity;
            if (showLogin && OAuthConfig.User == null)
            {
                showLogin = false;

                /// Create OauthProviderSetting class with Oauth Implementation .Refer Step 6
                OAuthProviderSetting oauth = new OAuthProviderSetting();

                var auth = oauth.LoginWithProvider(providername);

                /// After facebook,google and all identity provider login completed 
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
                        App.Current.MainPage = new MenuPage(typeof(YPS.Views.LoginPage));
                    }
                };
                auth.Error += Auth_Error;
                activity.StartActivity(auth.GetUI(activity));
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