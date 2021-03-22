﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Model;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionsPage : ContentPage
    {
        QuestionsViewModel Vm;

        public QuestionsPage(int tagId, string tagNumber, string indentCode, string bagNumber, QuestiionsPageHeaderData questiionsPageHeaderData)
        {
            try
            {
                InitializeComponent();

                if (Device.RuntimePlatform == Device.iOS)// for adjusting the display as per the notch
                {
                    var safeAreaInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                    safeAreaInset.Bottom = 0;
                    safeAreaInset.Top = 30;
                    headerpart.Padding = safeAreaInset;
                }

                BindingContext = Vm = new QuestionsViewModel(Navigation, this, tagId, tagNumber, indentCode, bagNumber, questiionsPageHeaderData);
            }
            catch(Exception ex)
            {

            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Vm.GetConfigurationResults();
        }
    }
}