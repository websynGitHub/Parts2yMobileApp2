using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;

namespace YPS.Droid.Implementation
{
	public class XiaomiHomeBadger: MyBadge
	{
		public static String INTENT_ACTION = "android.intent.action.APPLICATION_MESSAGE_UPDATE";
		public static String EXTRA_UPDATE_APP_COMPONENT_NAME = "android.intent.extra.update_application_component_name";
		public static String EXTRA_UPDATE_APP_MSG_TEXT = "android.intent.extra.update_application_message_text";

		public XiaomiHomeBadger (Context context) : base(context)
		{
		}

		public override void executeBadge(int badgeCount)
		{
			try {
				Java.Lang.Class miuiNotificationClass = Java.Lang.Class.ForName("android.app.MiuiNotification");
				Java.Lang.Object miuiNotification = miuiNotificationClass.NewInstance();
				Java.Lang.Reflect.Field field = miuiNotification.Class.GetDeclaredField("messageCount");
				field.Accessible = true;
				field.Set(miuiNotification, badgeCount == 0 ? "" : badgeCount.ToString());
			} catch (Exception e) {
				Intent localIntent = new Intent(
					INTENT_ACTION);
				localIntent.PutExtra(EXTRA_UPDATE_APP_COMPONENT_NAME, getContextPackageName() + "/" + getEntryActivityName());
				localIntent.PutExtra(EXTRA_UPDATE_APP_MSG_TEXT, badgeCount == 0 ? "" : badgeCount.ToString());
				mContext.SendBroadcast(localIntent);
			}
		}

		public override List<String>  getSupportLaunchers() {
			List<string> supportedLaunchers = new List<string> ();
			supportedLaunchers.Add ("com.miui.miuilite");
			supportedLaunchers.Add ("com.miui.home");
			supportedLaunchers.Add ("com.miui.miuihome");
			supportedLaunchers.Add ("com.miui.miuihome2");
			supportedLaunchers.Add ("com.miui.mihome");
			supportedLaunchers.Add ("com.miui.mihome2");
			return supportedLaunchers;
		}
	}
}
