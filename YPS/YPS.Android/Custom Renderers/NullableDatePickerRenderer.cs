using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Widget;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YPS.CustomRenders;
using YPS.Droid.Custom_Renderers;

[assembly: ExportRenderer(typeof(NullableDatePicker), typeof(NullableDatePickerRenderer))]
namespace YPS.Droid.Custom_Renderers
{
    public class NullableDatePickerRenderer : ViewRenderer<NullableDatePicker, EditText>
    {
        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="context"></param>
        public NullableDatePickerRenderer(Context context) : base(context) { }

        DatePickerDialog _dialog; //Data member

        /// <summary>
        /// Gets called when any changes occur to the NullableDatePicker.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<NullableDatePicker> e)
        {
            base.OnElementChanged(e);

            this.SetNativeControl(new EditText(Forms.Context));
            if (Control == null || e.NewElement == null)
                return;
            this.Control.Click += OnPickerClick;
            this.Control.Text = (Element.Format == "MM/dd/yyyy") ? "mm/dd/yyyy" : Element.Date.ToString(Element.Format);
            this.Control.KeyListener = null;
            this.Control.FocusChange += OnPickerFocusChange;
            this.Control.Enabled = Element.IsEnabled;
            Control.BackgroundTintList = ColorStateList.ValueOf(global::Android.Graphics.Color.Transparent);
        }

        /// <summary>
        /// Gets called when property change occur to NullableDatePicker.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Xamarin.Forms.DatePicker.DateProperty.PropertyName || e.PropertyName == Xamarin.Forms.DatePicker.FormatProperty.PropertyName)
                SetDate(Element.Date);
        }

        /// <summary>
        /// OnPickerFocusChange
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPickerFocusChange(object sender, FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {
                ShowDatePicker();
            }
        }

        /// <summary>
        /// This is to dispose NullableDatePicker.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (Control != null)
            {
                this.Control.Click -= OnPickerClick;
                this.Control.FocusChange -= OnPickerFocusChange;

                if (_dialog != null)
                {
                    _dialog.Hide();
                    _dialog.Dispose();
                    _dialog = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// OnPickerClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPickerClick(object sender, EventArgs e)
        {
            ShowDatePicker();
        }

        /// <summary>
        /// This is to set the date.
        /// </summary>
        /// <param name="date"></param>
        void SetDate(DateTime date)
        {
            this.Control.Text = String.Format("{0:00}/{1:00}/{2:0000}", date.Month, date.Day, date.Year);
            Element.Date = date;
        }

        /// <summary>
        /// ShowDatePicker
        /// </summary>
        private void ShowDatePicker()
        {
            CreateDatePickerDialog(this.Element.Date.Year, this.Element.Date.Month - 1, this.Element.Date.Day);
            
            _dialog.Show();
        }

        /// <summary>
        /// CreateDatePickerDialog
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        void CreateDatePickerDialog(int year, int month, int day)
        {
            NullableDatePicker view = Element;
            _dialog = new DatePickerDialog(Context, (o, e) =>
            {
                view.Date = e.Date;
                ((IElementController)view).SetValueFromRenderer(VisualElement.IsFocusedProperty, false);
                Control.ClearFocus();

                _dialog = null;
            }, year, month, day);

            _dialog.SetButton("Done", (sender, e) =>
            {
                SetDate(_dialog.DatePicker.DateTime);
                this.Element.Format = this.Element._originalFormat;
                this.Element.AssignValue();
            });
        }
    }
}