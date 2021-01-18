using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace YPS.CustomRenders
{
    public class MyWebView : WebView
    {
        public static readonly BindableProperty UrlProperty = BindableProperty.Create(
            propertyName: "Url",
            returnType: typeof(string),
            declaringType: typeof(MyWebView),
            defaultValue: default(string));

        public static readonly BindableProperty HtmlProperty = BindableProperty.Create(
            propertyName: "Html",
            returnType: typeof(string),
            declaringType: typeof(MyWebView),
            defaultValue: default(string));

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(UrlProperty, value); }
        }
    }
}
