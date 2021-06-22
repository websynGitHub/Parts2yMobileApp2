using YPS.Helpers;
using YPS.Models;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Threading.Tasks;
using YPS.Service;
using YPS.CommonClasses;
using static YPS.Models.ChatMessage;
using Xamarin.Forms;
using YPS.CustomToastMsg;

namespace YPS.ChatServices
{
    public class ChatServices : ModelBase
    {
        #region Data member declaration
        public event EventHandler<ChatMessage> OnChatMessageReceived;
        YPSService service = new YPSService();
        #endregion

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public ChatServices()
        {
            GetChatGroupName().Wait();
        }

        /// <summary>
        /// commented on present scenario
        /// </summary>
        /// <returns></returns>
        public async Task GetChatGroupName()
        {
            try
            {
                //if (App.CheckSignalRConnection())
                //{
                //    await App.Proxy.Invoke("groupconnect", Settings.QaId, Settings.UserMail);
                //}
                //else
                //{
                //    Device.BeginInvokeOnMainThread(() =>
                //    {
                //        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                //    });
                //}
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetChatGroupName method -> in ChatServices.cs " + Settings.userLoginID);

                await service.Handleexception(ex);
            }
        }

        #region IChatServices implementation
        public async Task Connect()
        {
            try
            {

                App.Proxy.On("receiveMessage", (string name, string message, string messagedate, string messagetype, DateTime messagetime, int userid, int QAID) =>
                {
                    string Namefromserver = name;
                    string MessageFromServer = message;

                    if (Settings.QaId == QAID)
                    {
                        OnChatMessageReceived(this, new ChatMessage
                        {
                            Name = name,
                            Message = message,
                            UserID = userid,
                            MessagDateTime = messagetime.ToLocalTime().ToString(Settings.DateFormat),
                            MessageType = messagetype,
                            MessagUtcDateTime = messagetime
                        });
                    }
                }

               );
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Connect method -> in ChatServices.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// commented on present scenario
        /// </summary>
        /// <returns></returns>
        public async Task Send(CMesssage message)
        {
            try
            {
                //if (App.CheckSignalRConnection())
                //{
                //    await App.Proxy.Invoke("sendmessage", message);
                //}
                //else
                //{
                //    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                //}
            }
            catch (Exception ex)
            {
                DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                YPSLogger.ReportException(ex, "Send method -> in ChatServices.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
        #endregion
    }
}
