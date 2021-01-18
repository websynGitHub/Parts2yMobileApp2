using System;
using System.Numerics;
using Xamarin.Forms;

namespace YPS.Behaviours
{
    public class NumberValidation : Behavior<Entry>
    {
        /// <summary>
        /// Attached method.
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += Bindable_TextChanged;
            base.OnAttachedTo(bindable);
        }

        /// <summary>
        /// Detaching method.
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= Bindable_TextChanged;
            base.OnDetachingFrom(bindable);
        }

        /// <summary>
        /// Text bindable method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Bindable_TextChanged(object sender, TextChangedEventArgs args)
        {
            BigInteger result;

            if (!String.IsNullOrWhiteSpace(args.NewTextValue))
            {
                bool isValid = BigInteger.TryParse(args.NewTextValue, out result);
                if(!isValid)
                {
                    ((Entry)sender).Text = args.OldTextValue;
                }
            }
        }
    }
}
