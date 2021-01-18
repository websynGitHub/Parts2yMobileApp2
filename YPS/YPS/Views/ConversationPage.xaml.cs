using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Service;

namespace YPS.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConversationPage : ContentPage
	{
		YPSService service;

		/// <summary>
		/// Parameterless constructor.
		/// </summary>
		public ConversationPage ()
		{
			try
            {
				InitializeComponent();
				YPSLogger.TrackEvent("ConversationPage", "Page constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
				Settings.currentPage = "ConversationPage";
			}
			catch(Exception ex)
            {
				service.Handleexception(ex);
				YPSLogger.ReportException(ex, "FileUpload constructor -> in FileUpload Page.cs " + Settings.userLoginID);
			}
        }
	}
}