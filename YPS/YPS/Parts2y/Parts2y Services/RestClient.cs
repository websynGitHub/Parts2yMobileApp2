using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;

namespace YPS.Parts2y.Parts2y_Services
{
    public class RestClient
    {
        HttpClient httpClient;
        //private string WebServiceUrl = HostingURL.WebServiceUrl;
        //private string WebServiceUrl = "https://ypsepod.azurewebsites.net/api/";
        private string WebServiceUrl = Settings.WebServiceUrl;

        public RestClient()
        {
            httpClient = new HttpClient();
            // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.access_token);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailid"></param>
        /// <returns></returns>
        public async Task<string> GetAllePODdata(string mailid)
        {
            try
            {
                string ePODresult = string.Empty;
                HttpContent httpContent = new StringContent("");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                string url = WebServiceUrl + "GetSupLoadList?Emailid=" + mailid;
                var response = httpClient.PostAsync(url, null).Result;
                var result = await response.Content.ReadAsStringAsync();
                ePODresult = result;
                return ePODresult;
            }
            catch (Exception ex)
            {
                // await service.Handleexception(ex);
                // YPSLogger.ReportException(ex, "SendChatMail method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Getdrivervindata
        /// </summary>
        /// <param name="mailid"></param>
        /// <returns></returns>
        public async Task<string> Getdrivervindata(string mailid)
        {
            try
            {
                string ePODresult = string.Empty;
                HttpContent httpContent = new StringContent("");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                string url = WebServiceUrl + "GetDriverVinList?Emailid=" + mailid;
                var response = httpClient.PostAsync(url, null).Result;
                var result = await response.Content.ReadAsStringAsync();
                ePODresult = result;
                return ePODresult;
            }
            catch (Exception ex)
            {
                // await service.Handleexception(ex);
                // YPSLogger.ReportException(ex, "SendChatMail method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }
        /// <summary>
        /// Loadnumber Detauls
        /// </summary>
        /// <param name="Loadnumber"></param>
        /// <returns></returns>
        public async Task<string> GetLoaddetails(string Loadnumber)
        {
            try
            {
                string Loadresult = string.Empty;
                HttpContent httpContent = new StringContent("");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                string url = WebServiceUrl + "GetLoadDetails?Loadnumber=" + Loadnumber;
                var response = httpClient.PostAsync(url, null).Result;
                var result = await response.Content.ReadAsStringAsync();
                Loadresult = result;
                return Loadresult;
            }
            catch (Exception ex)
            {
                // await service.Handleexception(ex);
                // YPSLogger.ReportException(ex, "SendChatMail method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }
        /// <summary>
        /// Vin Deatils
        /// </summary>
        /// <param name="Vinnumber"></param>
        /// <returns></returns>

        public async Task<string> GetSupvindetails(string Vinnumber)
        {
            try
            {
                string Vinresult = string.Empty;
                HttpContent httpContent = new StringContent("");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                string url = WebServiceUrl + "GetSupVinDetails?Vinid=" + Vinnumber ;
                var response = httpClient.PostAsync(url, null).Result;
                var result = await response.Content.ReadAsStringAsync();
                Vinresult = result;
                return Vinresult;
            }
            catch (Exception ex)
            {
                // await service.Handleexception(ex);
                // YPSLogger.ReportException(ex, "SendChatMail method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        public async Task<string> GetDrivervindetails(string Vinnumber,string HRLText)
        {
            try
            {
                string Vinresult = string.Empty;
                HttpContent httpContent = new StringContent("");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                string url = WebServiceUrl + "GetDriverVinDetails?Vinid=" + Vinnumber + "&HRLText=" + HRLText;
                var response = httpClient.PostAsync(url, null).Result;
                var result = await response.Content.ReadAsStringAsync();
                Vinresult = result;
                return Vinresult;
            }
            catch (Exception ex)
            {
                // await service.Handleexception(ex);
                // YPSLogger.ReportException(ex, "SendChatMail method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        public async Task<string> GetScandetails(string Scanresult,string Vinnumber, string HRLText)
        {
            try
            {
                string getScanresult = string.Empty;
                HttpContent httpContent = new StringContent("");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                string url = WebServiceUrl + "ScanResult?result=" + Scanresult + "&Vinid=" + Vinnumber + "&HRLText=" + HRLText;
                var response = httpClient.PostAsync(url, null).Result;
                var result = await response.Content.ReadAsStringAsync();
                getScanresult = result;
                return getScanresult;
            }
            catch (Exception ex)
            {
                // await service.Handleexception(ex);
                // YPSLogger.ReportException(ex, "SendChatMail method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        public async Task<string> GetQuestionarieeDetails()
        {
            try
            {
                string QuestionResult = string.Empty;
                HttpContent httpContent = new StringContent("");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                string url = WebServiceUrl + "CPQuestions";
                var response = httpClient.PostAsync(url, null).Result;
                var result = await response.Content.ReadAsStringAsync();
                QuestionResult = result;
                return QuestionResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// GetCompletedVinList
        /// </summary>
        /// <param name="Vinnumber"></param>
        /// <param name="HRLText"></param>
        /// <returns></returns>
        public async Task<string> GetCompletedVinList(string Vinnumber, string HRLText)
        {
            try
            {
                string Completedresult = string.Empty;
                HttpContent httpContent = new StringContent("");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                string url = WebServiceUrl + "GetCompletedVinList?Vinnum=" + Vinnumber + "&HRLText=" + HRLText + "&Emailid=" +Settings.UserMail;
                var response = httpClient.PostAsync(url, null).Result;
                var result = await response.Content.ReadAsStringAsync();
                Completedresult = result;
                return Completedresult;
            }
            catch (Exception ex)
            {
                // await service.Handleexception(ex);
                // YPSLogger.ReportException(ex, "SendChatMail method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        public async Task<string> GetDealerData(string mailid)
        {
            try
            {
                string dealerData = string.Empty;
                HttpContent httpContent = new StringContent("");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                string url = WebServiceUrl + "GetDealerData?Emailid=" + mailid;
                var response = httpClient.PostAsync(url, null).Result;
                var result = await response.Content.ReadAsStringAsync();
                dealerData = result;
                return dealerData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<string> GetDealerCarrierListDetails(string CarrierNo)
        {
            try
            {
                string Carrierresult = string.Empty;
                HttpContent httpContent = new StringContent("");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                string url = WebServiceUrl + "GetDealerCarrierlist?Carrierno=" + CarrierNo;
                var response = httpClient.PostAsync(url, null).Result;
                var result = await response.Content.ReadAsStringAsync();
                Carrierresult = result;
                return Carrierresult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> GetLoginDetails(string EmailID)
        {
            try
            {
                string Loginresult = string.Empty;
                HttpContent httpContent = new StringContent("");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                string url = WebServiceUrl + "GetUserDetails?Emailid=" + EmailID;
                var response = httpClient.PostAsync(url, null).Result;
                var result = await response.Content.ReadAsStringAsync();
                Loginresult = result;
                return Loginresult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> GetDealervindetails(string Vinnumber)
        {
            try
            {
                string DealerVinresult = string.Empty;
                HttpContent httpContent = new StringContent("");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                string url = WebServiceUrl + "GetDealerVinDetails?Vinnum=" + Vinnumber;
                var response = httpClient.PostAsync(url, null).Result;
                var result = await response.Content.ReadAsStringAsync();
                DealerVinresult = result;
                return DealerVinresult;
            }
            catch (Exception ex)
            {
                // await service.Handleexception(ex);
                // YPSLogger.ReportException(ex, "SendChatMail method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

    }
}
