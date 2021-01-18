using System;
using Xamarin.Forms;
using System.Collections.Generic;



namespace YPS.Views.Menu
{
    public class MenuListView : ListView
    {
        public MenuListView()
        {
            try
            {
                List<MenuItem> data = new MenuListData();
                ItemsSource = data;
                VerticalOptions = LayoutOptions.FillAndExpand;
                BackgroundColor = Color.FromHex("#000000");

                //Opacity = 0.8;
                HasUnevenRows = true;
                SeparatorVisibility = SeparatorVisibility.None;
                SeparatorColor = Color.LightGray;
                RowHeight = 60;


                ItemTemplate = new DataTemplate(typeof(MenuCell));
            }
            catch(Exception ex)
            {

            }
        }
    }
}
