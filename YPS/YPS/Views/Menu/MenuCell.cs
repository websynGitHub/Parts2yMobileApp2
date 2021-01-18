using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YPS.CommonClasses;

namespace YPS.Views.Menu
{
    public class MenuCell : ViewCell
    {

        public MenuCell()
        {
            try
            {

                var grd = new Grid();

                grd.ColumnDefinitions.Add(new ColumnDefinition { Width = 40 });
                grd.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grd.RowDefinitions.Add(new RowDefinition { Height = 60 });
                grd.RowDefinitions.Add(new RowDefinition { Height = 0.5 });
                var stkLyt = new StackLayout();
                Label lblTitle = new Label();
                //Label lblIcon = new Label();
                Image lblIcon = new Image();
                //Image SeperatorIcon = new Image();
                lblIcon.Margin = Device.OnPlatform(
                      iOS: new Thickness(0, 20, 0, 20),
                      Android: new Thickness(0, 0, 0, 0),
                      WinPhone: new Thickness(0, 0, 0, 0)
                      );
                lblTitle.Margin = Device.OnPlatform(
                    iOS: new Thickness(0, 20, 0, 20),
                    Android: new Thickness(0, 0, 0, 0),
                    WinPhone: new Thickness(0, 0, 0, 0)
                    );
                //SeperatorIcon.Margin = Device.OnPlatform(
                //      iOS: new Thickness(0, 20, 0, 20),
                //      Android: new Thickness(0, 0, 0, 0),
                //      WinPhone: new Thickness(0, 0, 0, 0)
                //      );
                //lblTitle.Style = (Style)Application.Current.Resources["LableWithFont"];
                lblIcon.HeightRequest = 25;
                lblIcon.WidthRequest = 25;
                // SeperatorIcon.HeightRequest = 10;


                lblTitle.SetBinding(Label.TextProperty, new Binding("Title"));
                lblTitle.FontSize = 16;
                if (Settings.userRoleID == 1 || Settings.userRoleID == 2 || Settings.userRoleID == 3)
                {
                    if (Settings.countmenu >= 2 && Settings.countmenu <= 10)
                    {
                        lblTitle.TextColor = Color.Gray;
                        lblIcon.Opacity = 0.7;
                        Settings.countmenu = Settings.countmenu + 1;
                    }
                    else
                    {
                        lblTitle.TextColor = Color.White;
                        Settings.countmenu = Settings.countmenu + 1;
                    }
                }
                else if (Settings.userRoleID == 6)
                {
                    if (Settings.countmenu == 2 )
                    {
                        lblTitle.TextColor = Color.Gray;
                        lblIcon.Opacity = 0.7;
                        Settings.countmenu = Settings.countmenu + 1;
                    }
                    else
                    {
                        lblTitle.TextColor = Color.White;
                        Settings.countmenu = Settings.countmenu + 1;
                    }
                }
                else if (Settings.userRoleID == 4 )
                {
                    if (Settings.countmenu >= 2 && Settings.countmenu <= 7)
                    {
                        lblTitle.TextColor = Color.Gray;
                        lblIcon.Opacity = 0.7;
                        Settings.countmenu = Settings.countmenu + 1;
                    }
                    else
                    {
                        lblTitle.TextColor = Color.White;
                        Settings.countmenu = Settings.countmenu + 1;
                    }
                }
                else if (Settings.userRoleID == 5)
                {
                    if (Settings.countmenu >= 2 && Settings.countmenu <= 3)
                    {
                        lblTitle.TextColor = Color.Gray;
                        lblIcon.Opacity = 0.7;
                        Settings.countmenu = Settings.countmenu + 1;
                    }
                    else
                    {
                        lblTitle.TextColor = Color.White;
                        Settings.countmenu = Settings.countmenu + 1;
                    }
                }
                else
                {
                    lblTitle.TextColor = Color.White;
                    Settings.countmenu = 1;
                }
                lblTitle.VerticalOptions = LayoutOptions.Center;
                lblTitle.FontAttributes = FontAttributes.Bold;

                lblIcon.SetBinding(Image.SourceProperty, new Binding("IconSource"));
                lblIcon.VerticalOptions = LayoutOptions.Center;

                //SeperatorIcon.SetBinding(Image.SourceProperty, new Binding("SeperatorIconSource"));
                //SeperatorIcon.VerticalOptions = LayoutOptions.Center;
                //SeperatorIcon.HorizontalOptions = LayoutOptions.FillAndExpand;

                stkLyt.Children.Add(lblIcon);
                grd.Children.Add(lblIcon, 0, 0);
                grd.Children.Add(new BoxView
                {
                    Color = Color.White,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                    // Margin = new Thickness(10,0,0,10)
                }, 0, 2, 1, 2);
                stkLyt.Children.Add(lblTitle);
                stkLyt.HorizontalOptions = LayoutOptions.FillAndExpand;
                stkLyt.Orientation = StackOrientation.Horizontal;

                grd.Children.Add(lblTitle, 1, 0);
                //  grd.Children.Add(SeperatorIcon, 0, 1);
                grd.RowSpacing = 0;
                grd.ColumnSpacing = 5;
                grd.Padding = new Thickness(10, 0, 20, 0);

                //grd.BackgroundColor = Color.LightYellow;

                grd.HorizontalOptions = LayoutOptions.FillAndExpand;
                StackLayout objLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    //BackgroundColor = Color.LightBlue,
                    Children = { grd }
                };

                View = objLayout;
            }
            catch (Exception ex)
            {

            }

        }
    }
}
