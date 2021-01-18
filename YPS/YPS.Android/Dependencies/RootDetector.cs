using Android.Gms.Common;
using Android.Gms.SafetyNet;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using YPS.Droid.Dependencies;
using YPS.Helpers;
using YPS.Model;

[assembly: Dependency(typeof(RootDetector))]
namespace YPS.Droid.Dependencies
{
    public class RootDetector : IRootDetection
    {
        #region Data members
        bool response = false;
        SafetyNetApiAttestationResponse attestationResponse;
        #endregion

        /// <summary>
        /// Checks if device is rooted or not.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckIfRooted()
        {
            try
            {
                var api = SafetyNetClass.GetClient(MainActivity.mainActivity);
                var nonce = Nonce.Generate(); // Should be at least 16 bytes in length.
                var jwsResponse = await api.AttestAsync(nonce, MainActivity.SafetyNetApiKey);

                if (jwsResponse != null && !string.IsNullOrEmpty(jwsResponse.JwsResult))
                {

                    // Store for future verification with google servers
                    attestationResponse = jwsResponse;
                    var decodedResult = jwsResponse.DecodeJwsResult(nonce);
                    if(decodedResult!=null)
                    {
                        var result = JsonConvert.DeserializeObject<SafetyNetModel>(decodedResult);
                       
                        if(result!=null)
                        {
                            if (result.ctsProfileMatch== "true" && result.basicIntegrity== "true")
                            {
                                response = true;
                            }
                            else
                            {
                                response = false;

                            }
                        }
                    }
                }
                else
                {
                    response = false;
                }
            }
            catch (Exception ex)
            {
                response = false;

            }
            return response;
        }
       
        /// <summary>
        /// Gets called when connection Google Play failes.
        /// </summary>
        /// <param name="result"></param>
        public void OnConnectionFailed(ConnectionResult result)
        {
            App.Current.MainPage.DisplayAlert("Alert", "Failed to connect to Google Play Services", "OK");
        }

    }
}