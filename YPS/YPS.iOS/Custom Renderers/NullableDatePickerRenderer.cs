using System;
using System.Collections.Generic;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.CustomRenders;
using YPS.iOS.Custom_Renderers;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Service;

[assembly: ExportRenderer(typeof(NullableDatePicker), typeof(NullableDatePickerRenderer))]
namespace YPS.iOS.Custom_Renderers
{
    class NullableDatePickerRenderer : DatePickerRenderer
    {
        /// <summary>
        /// OnElementChanged
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            try
            {
                base.OnElementChanged(e);

                if (e.NewElement != null && this.Control != null)
                {
                    this.AddClearButton();

                    this.Control.BorderStyle = UITextBorderStyle.None;

                    this.Control.Text = (Element.Format == "MM/dd/yyyy") ? "mm/dd/yyyy" : Element.Date.ToString(Element.Format);
                    this.Control.TextColor = UIColor.LightGray;
                    Element.DateSelected += Element_DateSelected;
                    Control.Font = UIFont.SystemFontOfSize(15f);
                }
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "OnElementChanged method -> in NullableDatePickerRenderer.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Element_DateSelected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Element_DateSelected(object sender, DateChangedEventArgs e)
        {
            try
            {
                this.Control.TextColor = UIColor.Black;

                NullableDatePicker baseDatePicker = this.Element as NullableDatePicker;
                baseDatePicker.CleanDate();
                this.Control.TextColor = UIColor.Black;
                this.Control.Text = String.Format("{0:00}/{1:00}/{2:0000}", e.NewDate.Month, e.NewDate.Day, e.NewDate.Year);
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "Element_DateSelected method -> in NullableDatePickerRenderer.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// AddClearButton
        /// </summary>
        private void AddClearButton()
        {
            try
            {
                var originalToolbar = this.Control.InputAccessoryView as UIToolbar;

                if (originalToolbar != null && originalToolbar.Items.Length <= 2)
                {
                    var newItems = new List<UIBarButtonItem>();
                    foreach (var item in originalToolbar.Items)
                    {
                        newItems.Add(item);
                    }
                    originalToolbar.Items = newItems.ToArray();
                    originalToolbar.SetNeedsDisplay();
                }
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "AddClearButton method -> in NullableDatePickerRenderer.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }
    }
}