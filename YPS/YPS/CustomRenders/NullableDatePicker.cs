using System;
using Xamarin.Forms;
using YPS.CommonClasses;

namespace YPS.CustomRenders
{
    public class NullableDatePicker : DatePicker
    {
        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public NullableDatePicker()
        {
            Format = "MM/dd/yyyy";
        }

        public string _originalFormat = null;

        public static readonly BindableProperty PlaceHolderProperty =
            BindableProperty.Create(nameof(PlaceHolder), typeof(string), typeof(NullableDatePicker), Settings.DateFormatforAll,BindingMode.TwoWay);

        public string PlaceHolder
        {
            get { return (string)GetValue(PlaceHolderProperty); }
            set
            {
                SetValue(PlaceHolderProperty, value);
            }
        }
        public static readonly BindableProperty NullableDateProperty =
        BindableProperty.Create(nameof(NullableDate), typeof(DateTime?), typeof(NullableDatePicker), null, defaultBindingMode: BindingMode.TwoWay);

        public DateTime? NullableDate
        {
            get { return (DateTime?)GetValue(NullableDateProperty); }
            set
            {
                SetValue(NullableDateProperty, value);
                UpdateDate();
            }
        }

        /// <summary>
        /// UpdateDate
        /// </summary>
        private void UpdateDate()
        {
            if (NullableDate != null)
            {
                if (_originalFormat != null)
                {
                    Format = _originalFormat;
                }
            }
            else
            {
                Format = PlaceHolder;
            }
        }

        /// <summary>
        /// OnBindingContextChanged
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
            {
                _originalFormat = Format;
                UpdateDate();
            }
        }

        /// <summary>
        /// OnPropertyChanged
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == DateProperty.PropertyName || (propertyName == IsFocusedProperty.PropertyName && !IsFocused && (Date.ToString("d") == DateTime.Now.ToString("d"))))
            {
                AssignValue();
            }

            if (propertyName == NullableDateProperty.PropertyName && NullableDate.HasValue)
            {
                Date = NullableDate.Value;
                if (Date.ToString(_originalFormat) == DateTime.Now.ToString(_originalFormat))
                {
                    //this code was done because when date selected is the actual date the"DateProperty" does not raise  
                    UpdateDate();
                }
            }
        }

        /// <summary>
        /// Clean date from nullable date.
        /// </summary>
        public void CleanDate()
        {
            NullableDate = null;
            UpdateDate();
        }

        /// <summary>
        /// Assign value to nullable date.
        /// </summary>
        public void AssignValue()
        {
            NullableDate = Date;
            UpdateDate();
        }
    }
}
