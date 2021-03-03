﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPS.Parts2y.Parts2y_Views;
using YPS.Parts2y.Parts2y_View_Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Service;
using Syncfusion.XForms.Buttons;
using YPS.Model;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadPage : ContentPage
    {
        LoadPageViewModel Vm;
        YPSService service;

        public LoadPage(AllPoData selectedtagdata)
        {
            try
            {
                InitializeComponent();
                Settings.IsRefreshPartsPage = true;
                BindingContext = Vm = new LoadPageViewModel(Navigation, selectedtagdata, this);
                service = new YPSService();
            }
            catch (Exception ex)
            {

            }

        }


        private async void DoneClicked(object sender, EventArgs e)
        {
            try
            {
                string text = (sender as SfButton).Text;

                //if (text.Trim().ToLower() == vm.labelobjComplete.Trim().ToLower() && accessPhoto == false)
                //{
                //    //await vm.ClosePic();
                //}


            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DoneClicked method -> in LoadPage.xaml.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when back icon is clicked, to redirect  to the previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Back_Tapped(object sender, EventArgs e)
        {
            try
            {
                //if (Settings.POID > 0)
                //{
                //    Navigation.RemovePage(Navigation.NavigationStack[1]);
                //    Navigation.InsertPageBefore(new POChildListPage(await GetUpdatedAllPOData(), sendPodata), Navigation.NavigationStack[1]);
                //    Navigation.InsertPageBefore(new ParentListPage(), Navigation.NavigationStack[1]);
                //    Settings.POID = 0;
                //}

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped constructor -> in PhotoUpload.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
    }
}