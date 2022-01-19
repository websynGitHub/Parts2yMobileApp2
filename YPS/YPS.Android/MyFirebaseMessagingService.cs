using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.App;
using Firebase.Messaging;
using Java.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Xamarin.Essentials;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.Droid.Dependencies;
using YPS.Helpers;
using YPS.Service;
using Color = Android.Graphics.Color;
using System.Linq;

namespace YPS.Droid
{
    [Service(Name = "com.synergies.ypsapp.firebase.MyFirebaseMessagingService")]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseIIDService";
        string refreshedToken;
        public async override void OnNewToken(string p0)
        {
            base.OnNewToken(p0);
            try
            {
                // refreshedToken = p0;// FirebaseInstanceId.Instance.Token;
                var hh = refreshedToken = p0;
                await SecureStorage.SetAsync("Token", p0);
                // Log.Debug(TAG, "Refreshed token: " + refreshedToken);
            }
            catch (System.Exception ex)
            {
                ex.ToString();
            }
        }
        //public override async void OnNewToken(string Token)
        //{
        //    base.OnNewToken(Token);
        //    await SecureStorage.SetAsync("Token", Token);
        //}

        /// <summary>
        /// Gets called when msg received.
        /// </summary>
        /// <param name="message"></param>
        public async override void OnMessageReceived(RemoteMessage message)
        {
            try
            {
                ICollection<string> msg = message.Data.Values;
                List<string> val = new List<string>();

                if (msg != null)
                {
                    foreach (string s in msg)
                    {
                        val.Add(s);
                    }
                    try
                    {
                        var showNotify = val[1].Split(';');
                        if (Settings.currentChatPage != Convert.ToInt32(showNotify[4]))
                            SendNotification(val[0], val[1]);
                    }
                    catch (Exception ex)
                    {
                        YPSService trackService = new YPSService();
                        await trackService.Handleexception(ex);
                        YPSLogger.ReportException(ex, "OnMessageReceived method inner catch -> in MyFirebaseMessagingService.cs " + Settings.userLoginID);
                    }
                }
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "OnMessageReceived method outer catch -> in MyFirebaseMessagingService.cs " + Settings.userLoginID);
            }
        }

        Bitmap bitmap;
        async void SendNotification(string messageBody, string param)
        {
            try
            {
                var intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("getParam", param);
                intent.AddFlags(ActivityFlags.ClearTop);
                var showNotify = param.Split(';');
                int PushId = Convert.ToInt32(showNotify[4]);
                string showMessages = "";
                Uri uriResult;

                if (showNotify[0].Trim().ToLower() == "JobAssigned".Trim().ToLower())
                {
                    showMessages = messageBody;

                    if (showNotify[11] == Convert.ToString(Settings.CompanyID) &&
                        showNotify[12] == Convert.ToString(Settings.ProjectID) &&
                        showNotify[13] == Convert.ToString(Settings.JobID))
                    {
                        Settings.notifyJobCount = Settings.notifyJobCount + 1;
                        MessagingCenter.Send<string, string>("PushNotificationCame", "IncreaseJobCount", Convert.ToString(Settings.notifyJobCount));
                    }
                }
                else
                {
                    #region count
                    NotifyCountDB countDB = new NotifyCountDB();
                    NotifyMessagesCountDB messages = new NotifyMessagesCountDB();

                    var SpecificId = countDB.SpecificNotification(PushId);
                    NotifyMessagesCount msg = new NotifyMessagesCount();

                    if (SpecificId.Count() == 0)
                    {
                        NotifyCount saveData = new NotifyCount();
                        saveData.QaId = PushId;
                        saveData.AllPramText = param;
                        countDB.SaveNotifyCount(saveData);
                        msg.QaId = PushId;
                        msg.Msg = messageBody;
                        msg.TypeChat = showNotify[0];
                        msg.AllParams = param;
                        messages.SaveNotifyMsg(msg);
                    }
                    else
                    {
                        msg.QaId = PushId;
                        msg.Msg = messageBody;
                        msg.TypeChat = showNotify[0];
                        msg.AllParams = param;
                        messages.SaveNotifyMsg(msg);
                    }

                    var showData = messages.SpecificNotification(PushId);


                    foreach (var m in showData)
                    {
                        if (showData.Count == 1)
                        {
                            showMessages = m.Msg;
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(showMessages))
                            {
                                showMessages = m.Msg;
                            }
                            else
                            {
                                showMessages = showMessages + "\n" + m.Msg;
                            }
                        }
                    }

                    #endregion

                    Settings.notifyCount = Settings.notifyCount + 1;
                    MessagingCenter.Send<string, string>("PushNotificationCame", "IncreaseCount", Convert.ToString(Settings.notifyCount));
                    MessagingCenter.Send<string, int>("firebaseservice", "ChatCount", PushId);
                }

                Badge badge = new Badge(this);
                badge.count(Convert.ToInt32(showNotify[10]));

                var pendingIntent = PendingIntent.GetActivity(this, PushId, intent, PendingIntentFlags.OneShot);

                Bitmap icon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.Parts2y9696);

                if (!Uri.TryCreate(messageBody, UriKind.Absolute, out uriResult))
                {
                    var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                                          .SetSmallIcon(Resource.Drawable.Parts2y3636)
                                          .SetContentTitle(showNotify[7])
                                          .SetContentText(messageBody)
                                          .SetColor(Color.Rgb(37, 74, 126))
                                          .SetStyle(new NotificationCompat.BigPictureStyle()
                                          .BigPicture(bitmap))
                                          .SetAutoCancel(true)
                                          .SetStyle(new NotificationCompat.BigTextStyle().BigText(showMessages))
                                          .SetContentIntent(pendingIntent)
                                          .SetLargeIcon(icon);

                    var notificationManager = NotificationManagerCompat.From(this);
                    notificationManager.Notify(PushId, notificationBuilder.Build());
                }
                else
                {
                    var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                                             .SetSmallIcon(Resource.Drawable.Parts2y3636)
                                              .SetContentTitle(showNotify[7])
                                              .SetAutoCancel(true)
                                              .SetColor(Color.Rgb(37, 74, 126))
                                              .SetStyle(new NotificationCompat.BigTextStyle().BigText(showMessages))
                                              .SetContentIntent(pendingIntent)
                                              .SetLargeIcon(icon);

                    var urlString = messageBody;
                    var url = new URL(urlString);
                    var connection = (HttpURLConnection)url.OpenConnection();
                    connection.DoInput = true;
                    connection.Connect();
                    var input = connection.InputStream;
                    var bitmap = BitmapFactory.DecodeStream(input);
                    var style = new NotificationCompat.BigPictureStyle()
                            .BigPicture(bitmap);
                    connection.Dispose();
                    notificationBuilder.SetStyle(style);

                    var notificationManager = NotificationManagerCompat.From(this);
                    notificationManager.Notify(PushId, notificationBuilder.Build());
                }


                if (!MyFirebaseMessagingPNService.pnIDs.Contains(PushId))
                {
                    MyFirebaseMessagingPNService.pnIDs.Add(PushId);
                }
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "SendNotification method -> in MyFirebaseMessagingService.cs " + Settings.userLoginID);
            }
        }
    }

    public static class MyFirebaseMessagingPNService
    {
        public static ArrayList pnIDs = new ArrayList();
    }
}