using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class MenuViewModel : IBase
    {
        private YPSService trackService;
        public MenuViewModel()
        {
            try
            {
                bool isarchive = true;
                trackService = new YPSService();
                UserName = Settings.Username;
                usermail = Settings.LoginIDDisplay;
                EntityName = Settings.EntityName;
                BgColor = YPS.CommonClasses.Settings.Bar_Background;

                List<MenuList> listOfMenuItems = new List<MenuList>();
                listOfMenuItems.Add(new MenuList { Title = "Home", IconSource = Icons.Home, ISVisible = true }); ;
                listOfMenuItems.Add(new MenuList { Title = "Settings", IconSource = Icons.SettingsIc, ISVisible = true });
                listOfMenuItems.Add(new MenuList { Title = "About", IconSource = Icons.info, ISVisible = true });
                listOfMenuItems.Add(new MenuList { Title = "Logout", IconSource = Icons.Logout, ISVisible = true });

                MenuItems = listOfMenuItems;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "MenuViewModel constructor -> in MenuViewModel.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        #region Properties
        private List<MenuList> _MenuItems;
        public List<MenuList> MenuItems
        {
            get { return _MenuItems; }
            set
            {
                _MenuItems = value;
                NotifyPropertyChanged("MenuItems");
            }
        }
        private Color _BgColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color BgColor
        {
            get { return _BgColor; }
            set
            {
                _BgColor = value;
                NotifyPropertyChanged("BgColor");
            }
        }

        private string _UserName = Settings.Username;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                NotifyPropertyChanged("UserName");
            }
        }

        private string _usermail = Settings.LoginIDDisplay;
        public string usermail
        {
            get { return _usermail; }
            set
            {
                _usermail = value;
                NotifyPropertyChanged("usermail");
            }
        }
        private string _EntityName = Settings.EntityName;
        public string EntityName
        {
            get { return _EntityName; }
            set
            {
                _EntityName = value;
                NotifyPropertyChanged("EntityName");
            }
        }
        #endregion
    }
}
