using System;
using Xamarin.Forms;
using System.Collections.Generic;
using YPS.CommonClasses;
using YPS.Service;
using YPS.CustomToastMsg;

namespace YPS.Views.Menu
{
    public class MenuPage : ContentPage
    {
        public ListView Menu { get; set; }

        public MenuPage()
        {
            try
            {
                Icon = "menuW48.png";
                Title = "Menu";
                BackgroundColor = Color.Transparent;
                //WidthRequest = 30;
                //BackgroundColor = Color.FromHex("#09AD8A");
               
                Menu = new MenuListView();
                var logo = new Image
                {
                    Source = "ypslogo12.png",
                    Aspect = Aspect.AspectFit,
                    VerticalOptions = LayoutOptions.Center,
                    //BackgroundColor = Color.White,
                    WidthRequest = 100,HeightRequest=80

                };
                var userName = new Label
                {
                    Text = Settings.Username,
                    TextColor = Color.Black,
                    //BackgroundColor=Color.White,
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    LineBreakMode = LineBreakMode.TailTruncation
                };

                var userMail = new Label
                {
                    Text = Settings.UserMail,
                    TextColor = Color.Black,
                    //BackgroundColor=Color.White,
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    LineBreakMode= LineBreakMode.TailTruncation
                };

                var companyName = new Label
                {
                    Text = Settings.EntityName,
                    TextColor = Color.Black,
                    //BackgroundColor=Color.White,
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    LineBreakMode = LineBreakMode.TailTruncation
                };

                var roleName = new Label
                {
                    Text = Settings.RoleName,
                    TextColor = Color.Black,
                    //BackgroundColor=Color.White,
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    LineBreakMode = LineBreakMode.TailTruncation
                };

                //var UpdateSetting = new Image
                //{
                //    Source = "SettingsIc.png",
                //    WidthRequest = 20,
                //    HeightRequest = 20,
                //    //BackgroundColor=Color.White,
                //    HorizontalOptions = LayoutOptions.EndAndExpand,
                //    Margin = new Thickness(0,0,10,0),

                //};


                //var tapGestureRecognizer = new TapGestureRecognizer();
                //tapGestureRecognizer.Tapped  += async (sender, e) => 
                //{
                //    try
                //    {
                //        var checkInternet = await App.CheckInterNetConnection();
                //        if(checkInternet)
                //        {
                //            Image theImage = (Image)sender;
                //            await Navigation.PushModalAsync(new UpdateProfilePage());
                //        }
                //        else
                //        {
                //            DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection");
                //        }
                //    }
                //    catch(Exception ex)
                //    {

                //    }

                //    //YPSService pSService = new YPSService();
                //    //int loginID = Settings.userLoginID;
                //    //var getProfileData = await pSService.GetProfile(loginID);

                //    //if(getProfileData.status != 0)
                //    //{
                //    //    string Email, GivenName, FamilyName,UserCulture;
                //    //    Email  = getProfileData.data.Email;
                //    //    GivenName = getProfileData.data.GivenName;
                //    //    FamilyName = getProfileData.data.FamilyName;
                //    //    UserCulture = getProfileData.data.UserCulture;


                //    //    await Navigation.PushModalAsync(new UpdateProfilePage(Email,GivenName,FamilyName, UserCulture));
                //    //}

                //    //App.Current.MainPage.Navigation.PushModalAsync()
                //};

                //UpdateSetting.GestureRecognizers.Add(tapGestureRecognizer);

                var logo_innerStak_stackLayout = new StackLayout
                {
                    //BackgroundColor = Color.FromHex("#ff6666"),
                    Orientation = StackOrientation.Vertical,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };

                logo_innerStak_stackLayout.Children.Add(userName);
                logo_innerStak_stackLayout.Children.Add(userMail);
                logo_innerStak_stackLayout.Children.Add(companyName);
                logo_innerStak_stackLayout.Children.Add(roleName);

                //logo_innerStak_stackLayout.Children.Add(UpdateSetting);

                var logo_stackLayout = new StackLayout
                {
                    //BackgroundColor = Color.FromHex("#827f7f"),
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    Padding = new Thickness(0, 12, 0, 12),
                };

                logo_stackLayout.Children.Add(logo);
                logo_stackLayout.Children.Add(logo_innerStak_stackLayout);
                
                //logo_stackLayout.HeightRequest = 100;

                var menuLabel = new ContentView
                {
                    Padding = new Thickness(8, 0, 0, 0),

                    Content = logo_stackLayout
                };

                var layout = new StackLayout
                {
                    Spacing = 0,
                    BackgroundColor = Color.White,
                    //Opacity =0.8,
                   //WidthRequest=300,
                    VerticalOptions = LayoutOptions.Center,
                   HorizontalOptions= LayoutOptions.FillAndExpand
                };
                layout.Children.Add(menuLabel);
                layout.Children.Add(Menu);
                Content = layout;
                //Content.WidthRequest = 300;
                Content.VerticalOptions = LayoutOptions.FillAndExpand;
            }
            catch (Exception ex)
            {

            } 
        }

        public Color BackColor()
        {          
          return Color.FromHex("#161340");
        }
    }
}
