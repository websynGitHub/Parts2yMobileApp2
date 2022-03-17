using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;

namespace YPS.RestClientAPI
{
    public class RequestProvider : IRequestProvider
    {
        #region Data members declaration
        YPSService service;
        HttpClient httpClient;
        bool stopresponceconversion;
        #endregion

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public RequestProvider()
        {
            try
            {
                service = new YPSService();
                httpClient = new HttpClient();

                #region Public Key
                httpClient = new HttpClient(new NativeMessageHandler(throwOnCaptiveNetwork: false,
                                                                      customSSLVerification: true
                                                                       ));
                #endregion

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.access_token);
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "RequestProvider constructor -> in RequestProvider.cs" + Settings.userLoginID);
            }
        }

        /// <summary>
        /// This method calling API by using PostAsync() and passing url to method.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<TResult> PostAsync<TResult>(string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(Settings.LoginID))
                {
                     var response = httpClient.PostAsync(url, null).Result;
                    await HandleResponse(response);

                    if (stopresponceconversion == false)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<TResult>(jsonResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                #region Public Key
                if (Settings.isExpectedPublicKey == false || ex.Message.ToLower().Replace(" ", null) == EndpointConfiguration.message)
                {
                    Settings.isExpectedPublicKey = true;
                    YPSLogger.ReportException(ex, "PostAsync method with one parameters and type string because of SSL public key mismatch-> in RequestProvider.cs" + Settings.userLoginID);
                    CloudFolderKeyVal.Appredirectloginwithoutlogout(true);
                }
                #endregion
                else
                {
                    if (ex.Message.StartsWith("Unable to resolve host") && ex.Message.EndsWith("No address associated with hostname"))
                    {
                        YPSLogger.ReportException(ex, "PostAsync method with one parameters and type string -> in RequestProvider.cs -> this exception gets logged, when there is no internet connection and you try to hit api. We can ignore this..." + Settings.userLoginID + " from Api=" + url);
                    }
                    else
                    {
                        YPSLogger.ReportException(ex, "PostAsync method with one parameters and type string -> in RequestProvider.cs UserID: " + Settings.userLoginID + " from Api=" + url);
                    }
                    await service.Handleexception(ex);
                    //throw new Exception("Poor Internet Connection.");
                }
            }
            returnnulldata nullData = new returnnulldata();
            nullData.message = "No data found";
            nullData.status = 0;
            string json = JsonConvert.SerializeObject(nullData);
            return JsonConvert.DeserializeObject<TResult>(json);
        }

        /// <summary>
        /// This method calling API by using PostAsync() and passing url & model to method.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<TResult> PostAsync<TResult>(string url, TResult data)
        {
            return PostAsync<TResult, TResult>(url, data);
        }

        /// <summary>
        /// This method calling API by using PostAsync() and passing url and model to method.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<TResult> PostAsync<TRequest, TResult>(string url, TRequest data)
        {
            try
            {
                if (!string.IsNullOrEmpty(Settings.LoginID))
                {
                    string json = JsonConvert.SerializeObject(data);
                    HttpContent httpContent = new StringContent(json);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage response = null;
                    response = await httpClient.PostAsync(url, httpContent);
                    await HandleResponse(response);
                    if (stopresponceconversion == false)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<TResult>(jsonResponse);
                    }
                }

            }
            catch (Exception ex)
            {
                #region Public Key
                if (Settings.isExpectedPublicKey == false || ex.Message.ToLower().Replace(" ", null) == EndpointConfiguration.message)
                {
                    Settings.isExpectedPublicKey = true;
                    YPSLogger.ReportException(ex, "PostAsync method with two parameters, string(url) and model because of SSL public key mismatch -> in RequestProvider.cs" + Settings.userLoginID);
                    CloudFolderKeyVal.Appredirectloginwithoutlogout(true);
                }
                #endregion
                else
                {
                    if (ex.Message.StartsWith("Unable to resolve host") && ex.Message.EndsWith("No address associated with hostname"))
                    {
                        YPSLogger.ReportException(ex, "PostAsync method with two parameters, string(url) and model -> in RequestProvider.cs -> this exception gets logged, when there is no internet connection and you try to hit api. We can ignore this..." + Settings.userLoginID + " from Api=" + url);
                    }
                    else
                    {
                        YPSLogger.ReportException(ex, "PostAsync method with two parameters, string(url) and model -> in RequestProvider.cs UserID: " + Settings.userLoginID + " from Api=" + url);
                    }
                    await service.Handleexception(ex);
                    //throw new Exception("Poor internet connection.");
                }
            }
            returnnulldata empty = new returnnulldata();
            empty.message = "No data found";
            empty.status = 0;
            string json1 = JsonConvert.SerializeObject(empty);
            return JsonConvert.DeserializeObject<TResult>(json1);
        }

        /// <summary>
        /// This method calling API by using PostAsync() and passing url,model & bool value to method.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="data1"></param>
        /// <param name="logout"></param>
        /// <returns></returns>
        public async Task<TResult> PostAsync<TRequest, TResult>(string url, TRequest data1, bool logout)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data1);
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = null;
                response = await httpClient.PostAsync(url, httpContent);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResult>(jsonResponse);

            }
            catch (Exception ex)
            {
                #region Public Key
                if (Settings.isExpectedPublicKey == false || ex.Message.ToLower().Replace(" ", null) == EndpointConfiguration.message)
                {
                    Settings.isExpectedPublicKey = true;
                    YPSLogger.ReportException(ex, "PostAsync method with three parameters and type string because of SSL public key mismatch -> in RequestProvider.cs" + Settings.userLoginID);
                    CloudFolderKeyVal.Appredirectloginwithoutlogout(true);
                }
                #endregion
                else
                {
                    if (ex.Message.StartsWith("Unable to resolve host") && ex.Message.EndsWith("No address associated with hostname"))
                    {
                        YPSLogger.ReportException(ex, "PostAsync method with three parameters and type string -> in RequestProvider.cs -> this exception gets logged, when there is no internet connection and you try to hit api. We can ignore this..." + Settings.userLoginID + " from Api=" + url);
                    }
                    else
                    {
                        YPSLogger.ReportException(ex, "PostAsync method with three parameters and type string -> in RequestProvider.cs UserID: " + Settings.userLoginID + " from Api=" + url);
                    }
                    await service.Handleexception(ex);
                    //throw new Exception("Poor Internet Connection.");
                }
            }
            returnnulldata empty = new returnnulldata();
            empty.message = "No data found";
            empty.status = 0;
            string json1 = JsonConvert.SerializeObject(empty);
            return JsonConvert.DeserializeObject<TResult>(json1);
        }

        /// <summary>
        /// This method calling API by using PostSettings() and passing url to method.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<TResult> PostSettings<TResult>(string url)
        {
            try
            {
                var response = httpClient.PostAsync(url, null).Result;
                await HandleResponse(response);

                if (stopresponceconversion == false)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TResult>(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                #region Public Key
                if (Settings.isExpectedPublicKey == false || ex.Message.ToLower().Replace(" ", null) == EndpointConfiguration.message)
                {
                    Settings.isExpectedPublicKey = true;
                    YPSLogger.ReportException(ex, "PostAsync method with one parameters and type string because of SSL public key mismatch-> in RequestProvider.cs UserID: " + Settings.userLoginID + " from Api=" + url);
                    CloudFolderKeyVal.Appredirectloginwithoutlogout(true);
                }
                #endregion
                else
                {
                    if (ex.Message.StartsWith("Unable to resolve host") && ex.Message.EndsWith("No address associated with hostname"))
                    {
                        YPSLogger.ReportException(ex, "PostAsync method with one parameters and type string -> in RequestProvider.cs -> this exception gets logged, when there is no internet connection and you try to hit api. We can ignore this..." + Settings.userLoginID + " from Api=" + url);
                    }
                    else
                    {
                        YPSLogger.ReportException(ex, "PostAsync method with one parameters and type string -> in RequestProvider.cs UserID: " + Settings.userLoginID + " from Api=" + url);
                    }
                    await service.Handleexception(ex);
                    //throw new Exception("Poor Internet Connection.");
                }
            }
            returnnulldata nullData = new returnnulldata();
            nullData.message = "No data found";
            nullData.status = 0;
            string json = JsonConvert.SerializeObject(nullData);
            return JsonConvert.DeserializeObject<TResult>(json);
        }

        /// <summary>
        /// This method calling API by using PostSettingsAsyncwithModel() and passing url and model to method.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<TResult> PostSettingsAsyncwithModel<TRequest, TResult>(string url, TRequest data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data);
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = null;
                response = await httpClient.PostAsync(url, httpContent);
                await HandleResponse(response);

                if (stopresponceconversion == false)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TResult>(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                #region Public Key
                if (Settings.isExpectedPublicKey == false || ex.Message.ToLower().Replace(" ", null) == EndpointConfiguration.message)
                {
                    Settings.isExpectedPublicKey = true;
                    YPSLogger.ReportException(ex, "PostAsync method with two parameters, string(url) and model because of SSL public key mismatch -> in RequestProvider.cs UserID: " + Settings.userLoginID + " from Api=" + url);
                    CloudFolderKeyVal.Appredirectloginwithoutlogout(true);
                }
                #endregion
                else
                {
                    if (ex.Message.StartsWith("Unable to resolve host") && ex.Message.EndsWith("No address associated with hostname"))
                    {
                        YPSLogger.ReportException(ex, "PostAsync method with two parameters, string(url) and model -> in RequestProvider.cs -> this exception gets logged, when there is no internet connection and you try to hit api. We can ignore this..." + Settings.userLoginID + " from Api=" + url);
                    }
                    else
                    {
                        YPSLogger.ReportException(ex, "PostAsync method with two parameters, string(url) and model -> in RequestProvider.cs UserID: " + Settings.userLoginID + " from Api=" + url);
                    }
                    await service.Handleexception(ex);
                    //throw new Exception("Poor internet connection.");
                }
            }
            returnnulldata empty = new returnnulldata();
            empty.message = "No data found";
            empty.status = 0;
            string json1 = JsonConvert.SerializeObject(empty);
            return JsonConvert.DeserializeObject<TResult>(json1);
        }

        /// <summary>
        /// Based on the respond return value.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task HandleResponse(HttpResponseMessage response)
        {
            try
            {
                stopresponceconversion = false;
                string strConnection = response.Headers.Connection.ToString();
                returnnulldata data = new returnnulldata();

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (Convert.ToString(content) == "User Session Expired." || Convert.ToString(content) == "Invalid JWT Token.")
                    {
                        data.message = Convert.ToString(content);
                        data.status = 0;
                    }
                    else
                    {
                        data = JsonConvert.DeserializeObject<returnnulldata>(content);
                    }

                    if (data.message == "User Session Expired.")
                    {
                        stopresponceconversion = true;
                        CloudFolderKeyVal.Appredirectlogin("Your yID token expired, please login.");
                    }
                    else if (data.message == "Invalid JWT Token.")
                    {
                        CloudFolderKeyVal.Appredirectlogin("Your yID token expired, please login.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "HandleResponse method -> in RequestProvider.cs UserID: " + Settings.userLoginID);
                await service.Handleexception(ex);
                //throw new Exception("Poor internet connection.");
            }
        }
    }
}
