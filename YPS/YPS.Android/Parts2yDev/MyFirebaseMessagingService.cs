using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Firebase.Messaging;

using Xamarin.Forms;
using Android.Graphics;
using YPS.Droid;
using Android.Support.V4.App;
using YPS.Model;
using YPS.Service;
using Android.Media;
using System.Linq;
//using Plugin.SecureStorage;
using YPS.Droid.Dependencies;
using YPS.CommonClasses;
using Java.Net;
using System.Text;
using System.IO;
using System.Net;
using Xamarin.Essentials;
using YPS.Helpers;

namespace YPS.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]

    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        //it will fire when msg received
        public async override void OnMessageReceived(RemoteMessage message)
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
                    var trackResult = await trackService.Handleexception(ex);
                    YPSLogger.ReportException(ex, "MyFirebaseMessagingService-> in OnMessageReceived " + Settings.userLoginID);
                }
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
                Uri uriResult;
                if (Uri.TryCreate(messageBody, UriKind.Absolute, out uriResult))
                {
                    WebClient wc = new WebClient();
                    byte[] originalData = wc.DownloadData(messageBody);
                    MemoryStream stream = new MemoryStream(originalData);
                    bitmap = BitmapFactory.DecodeByteArray(originalData, 0, originalData.Length);
                }
                string IsMianPage = await SecureStorage.GetAsync("mainPageisOn");// CrossSecureStorage.Current.GetValue("mainPageisOn").ToString();
                if (IsMianPage == "1")
                {
                    Settings.notifyCount = Settings.notifyCount + 1;
                    MessagingCenter.Send<string, string>("PushNotificationCame", "IncreaseCount", Settings.notifyCount.ToString());
                }


                var showData = messages.SpecificNotification(PushId);

                string showMessages = "";// String.Join("\n", showData);
                foreach (var m in showData)
                {
                    if (showData.Count == 1)
                        showMessages = m.Msg;
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

                //Random random = new Random();
                //int pushCounts = random.Next(9999 - 1000) + 1000; //for multiplepushnotifications

                var pendingIntent = PendingIntent.GetActivity(this, PushId, intent, PendingIntentFlags.OneShot);
                if (bitmap != null)
                {
                    var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                                          .SetSmallIcon(Resource.Drawable.ypslogo)
                                          .SetContentTitle(showNotify[7])
                                          .SetContentText(messageBody)
                                          //.SetLargeIcon(bitmap)
                                          .SetStyle(new NotificationCompat.BigPictureStyle()
                                          .BigPicture(bitmap))
                                         // .BigLargeIcon(null))
                                          .SetAutoCancel(true)
                                          .SetStyle(new NotificationCompat.BigTextStyle().BigText(showMessages))
                                          .SetContentIntent(pendingIntent);
                    var notificationManager = NotificationManagerCompat.From(this);
                    notificationManager.Notify(PushId, notificationBuilder.Build());
                }
                else
                {
                    var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                                              .SetSmallIcon(Resource.Drawable.ypslogo)
                                              .SetContentTitle(showNotify[7])
                                              .SetContentText(messageBody)
                                              .SetAutoCancel(true)
                                              .SetStyle(new NotificationCompat.BigTextStyle().BigText(showMessages))
                                              .SetContentIntent(pendingIntent);
                    var notificationManager = NotificationManagerCompat.From(this);
                    notificationManager.Notify(PushId, notificationBuilder.Build());
                }


            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                var trackResult = await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "MyFirebaseMessagingService-> in SendNotification " + Settings.userLoginID);

            }
        }

    }
}
