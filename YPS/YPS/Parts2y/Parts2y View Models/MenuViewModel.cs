﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_Services;
using YPS.Parts2y.Parts2y_SQLITE;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Parts2y.Parts2y_Views;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class MenuViewModel : BaseViewModel
    {
        public MenuViewModel()
        {
            UserName = Settings.UserName;
            usermail = Settings.UserMail;
            EntityName = Settings.Entity_Name;
            BgColor = Settings.Bar_Background;
            List<MenuList> listOfMenuItems = new List<MenuList>();
            listOfMenuItems.Add(new MenuList { Title = "Dashboard", IconSource = Icons.Dashboard});
            listOfMenuItems.Add(new MenuList { Title = "Home", IconSource = Icons.Home });
            listOfMenuItems.Add(new MenuList { Title = "About", IconSource = Icons.info });
            //listOfMenuItems.Add(new MenuList { Title = "TransportReportDetails", IconSource = "home.png" });
            listOfMenuItems.Add(new MenuList { Title = "Logout", IconSource = Icons.Logout });
            MenuItems = listOfMenuItems;
        }
        #region Properties
        private List<MenuList> _MenuItems;
        public List<MenuList> MenuItems
        {
            get { return _MenuItems; }
            set
            {
                _MenuItems = value;
                OnPropertyChanged("MenuItems");
            }
        }
        private Color _BgColor;
        public Color BgColor
        {
            get { return _BgColor; }
            set
            {
                _BgColor = value;
                OnPropertyChanged("BgColor");
            }
        }

        private string _UserName = Settings.UserName;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                OnPropertyChanged("UserName");
            }
        }

        private string _usermail = Settings.UserMail;
        public string usermail
        {
            get { return _usermail; }
            set
            {
                _usermail = value;
                OnPropertyChanged("usermail");
            }
        }
        private string _EntityName = Settings.Entity_Name;
        public string EntityName
        {
            get { return _EntityName; }
            set
            {
                _EntityName = value;
                OnPropertyChanged("EntityName");
            }
        }
        #endregion

    }
}
