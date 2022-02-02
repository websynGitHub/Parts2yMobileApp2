﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Service;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PolyBox : ContentPage
    {

        YPSService trackService;
        PolyBoxViewModel Vm;
        public PolyBox()
        {
            try
            {
                InitializeComponent();
                Settings.scanredirectpage = "Polybox";
                BindingContext = Vm = new PolyBoxViewModel(Navigation, this, false);
                Vm.loadindicator = true;
                trackService = new YPSService();
            }
            catch (Exception ex)
            {

            }
            Vm.loadindicator = false;
        }

        public async void ClearHistory(object sender, EventArgs e)
        {
            try
            {
                Vm.loadindicator = true;
                BindingContext = Vm = new PolyBoxViewModel(Navigation, this, true);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ClearHistory method -> in PlayBox.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            Vm.loadindicator = false;
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
                Navigation.PopToRootAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped method -> in PlayBox.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        private async void ScanRadioChecked(object sender, Syncfusion.XForms.Buttons.StateChangedEventArgs e)
        {
            try
            {
                var rd = sender as Syncfusion.XForms.Buttons.SfRadioButton;
                if(e.IsChecked.Value)
                if (rd.ClassId == Vm.FalseId.ToString())
                {
                    Vm.ScanFalse = true;
                    Vm.ScanTrue = false;
                }
                else if(rd.ClassId == Vm.TrueId.ToString())
                {
                    Vm.ScanFalse = false;
                    Vm.ScanTrue = true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ScanRadioChecked method -> in PlayBox.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        private async void ConfigRadioChecked(object sender, Syncfusion.XForms.Buttons.StateChangedEventArgs e)
        {
            try
            {
                var rd = sender as Syncfusion.XForms.Buttons.SfRadioButton;
                if(e.IsChecked.Value)
                if (rd.ClassId == Vm.FalseId.ToString())
                {
                    Vm.ConfigFalse = true;
                    Vm.ConfigTrue = false;
                }
                else if (rd.ClassId == Vm.TrueId.ToString())
                {
                    Vm.ConfigFalse = false;
                    Vm.ConfigTrue = true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ConfigRadioChecked method -> in PlayBox.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }
    }
}