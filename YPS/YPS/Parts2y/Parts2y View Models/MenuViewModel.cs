﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using YPS.CommonClasses;
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
            try
            {
                bool isarchive = true; ;
                UserName = Settings.Username;
                usermail = Settings.UserMail;
                EntityName = Settings.EntityName;
                //BgColor = Settings.Bar_Background;
                BgColor = YPS.CommonClasses.Settings.Bar_Background;

                //if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                //{
                //    isarchive = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "ArchivedChatsView".Trim()).FirstOrDefault()) != null ? true : false;
                //}

                List<MenuList> listOfMenuItems = new List<MenuList>();
                listOfMenuItems.Add(new MenuList { Title = "Home", IconSource = Icons.Home, ISVisible = true }); ;
                //listOfMenuItems.Add(new MenuList { Title = "Home", IconSource = Icons.Home });
                listOfMenuItems.Add(new MenuList { Title = "Settings", IconSource = Icons.SettingsIc, ISVisible = true });
                //listOfMenuItems.Add(new MenuList { Title = "Archive", IconSource = Icons.Archived, ISVisible = isarchive });
                listOfMenuItems.Add(new MenuList { Title = "About", IconSource = Icons.info, ISVisible = true });
                //listOfMenuItems.Add(new MenuList { Title = "TransportReportDetails", IconSource = "home.png" });
                listOfMenuItems.Add(new MenuList { Title = "Logout", IconSource = Icons.Logout, ISVisible = true });

                MenuItems = listOfMenuItems;
            }
            catch (Exception ex)
            {

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
                OnPropertyChanged("MenuItems");
            }
        }
        private Color _BgColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color BgColor
        {
            get { return _BgColor; }
            set
            {
                _BgColor = value;
                OnPropertyChanged("BgColor");
            }
        }

        private string _UserName = Settings.Username;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                OnPropertyChanged("UserName");
            }
        }

        private string _usermail = Settings.LoginID;
        public string usermail
        {
            get { return _usermail; }
            set
            {
                _usermail = value;
                OnPropertyChanged("usermail");
            }
        }
        private string _EntityName = Settings.EntityName;
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
