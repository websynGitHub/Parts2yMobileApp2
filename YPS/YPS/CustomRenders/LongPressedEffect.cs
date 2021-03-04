using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace YPS.CustomRenders
{
    public class LongPressedEffect : RoutingEffect
    {
        public LongPressedEffect() : base("YPS.LongPressedEffect")
        {
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.CreateAttached("Command", typeof(ICommand), typeof(LongPressedEffect), (object)null);
        public static ICommand GetCommand(BindableObject view)
        {
            return (ICommand)view.GetValue(CommandProperty);
        }

        public static void SetCommand(BindableObject view, ICommand value)
        {
            view.SetValue(CommandProperty, value);
        }


        public static readonly BindableProperty CommandParameterProperty = BindableProperty.CreateAttached("CommandParameter", typeof(object), typeof(LongPressedEffect), (object)null);
        public static object GetCommandParameter(BindableObject view)
        {
            return view.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(BindableObject view, object value)
        {
            view.SetValue(CommandParameterProperty, value);
        }

        public static readonly BindableProperty SlCommandProperty = BindableProperty.CreateAttached("SlCommand", typeof(ICommand), typeof(LongPressedEffect), (object)null);
        public static ICommand GetSlCommand(BindableObject view)
        {
            return (ICommand)view.GetValue(SlCommandProperty);
        }

        public static void SetSlCommand(BindableObject view, ICommand value)
        {
            view.SetValue(SlCommandProperty, value);
        }


        public static readonly BindableProperty SlCommandParameterProperty = BindableProperty.CreateAttached("SlCommandParameter", typeof(object), typeof(LongPressedEffect), (object)null);
        public static object GetSlCommandParameter(BindableObject view)
        {
            return view.GetValue(SlCommandParameterProperty);
        }

        public static void SetSlCommandParameter(BindableObject view, object value)
        {
            view.SetValue(SlCommandParameterProperty, value);
        }
    }
}
