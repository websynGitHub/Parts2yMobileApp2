using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;
using YPS.CustomControls;

namespace YPS.Views
{
    public class FileView:ContentPage
    {
        public string extension;
        WebView browser;
        ZoomImage image;
        public FileView( string url)
        {
            Title = "Preview";
            BackgroundColor = Color.White;
            extension = Path.GetExtension(url);
            browser = new WebView
            {
                Source = "http://drive.google.com/viewerng/viewer?embedded=true&url=" + url,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                
            };
           //image = new ZoomImage
           // {
           //     Source = url,
           //     HorizontalOptions = LayoutOptions.FillAndExpand,
           //     VerticalOptions = LayoutOptions.FillAndExpand,

           // };
            //if(extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
            //{
            //    Content = new StackLayout
            //    {

            //        Children ={
                          
            //        image,
            //          }
            //    };
            //}
            //else 
            //{
                Content = new StackLayout
                {

                    Children ={
                           
                          
                    browser,
                      }
                };
            //}
  
        }
    }
}
