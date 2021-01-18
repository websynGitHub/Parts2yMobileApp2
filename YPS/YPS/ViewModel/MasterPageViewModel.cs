using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using YPS.CommonClasses;
using YPS.Model;

namespace YPS.ViewModel
{
    public class MasterPageViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Parameter less constructor
        /// </summary>
        public MasterPageViewModel()
        {
            List<MenuList> listOfMenuItems = new List<MenuList>();
            
            #region Preparing menu items 
            listOfMenuItems.Add(new MenuList { Title = "Home", IconSource = Icons.HomeIc, IconType = false, opacity = 1, roule = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8,9,10,11,12,13,14,15 } });
            listOfMenuItems.Add(new MenuList { Title = "User", IconSource = "menuusers.png", IconType = true, opacity = 0.5, roule = new List<int> { 1, 2, 3, 4, 6, 8, 10, 12, 14} });
           // listOfMenuItems.Add(new MenuList { Title = "Owner", IconSource = Icons.owners, IconType = false, opacity = 0.5, roule = new List<int> { 1 } });
            listOfMenuItems.Add(new MenuList { Title = "Owner", IconSource = "Owner.png", IconType = true, opacity = 0.5, roule = new List<int> { 1 } });
            listOfMenuItems.Add(new MenuList { Title = "Project", IconSource = "projectsMenu.png", IconType = true, opacity = 0.5, roule = new List<int> { 1, 2, 3, 4 } });
            listOfMenuItems.Add(new MenuList { Title = "Job", IconSource = "jobMenu.png", IconType = true, opacity = 0.5, roule = new List<int> { 1, 2, 3, 4 } });
            listOfMenuItems.Add(new MenuList { Title = "Entity", IconSource = "EntityMenu.png", IconType = true, opacity = 0.5, roule = new List<int> { 1, 4 } });
            listOfMenuItems.Add(new MenuList { Title = "Supplier", IconSource = Icons.Entities, IconType = false, opacity = 0.5, roule = new List<int> { 2, 3 } });
            listOfMenuItems.Add(new MenuList { Title = "Report", IconSource = Icons.ShipCtrlReport, IconType = false, opacity = 0.5, roule = new List<int> { 1, 2, 3, 4, 5 } });
            listOfMenuItems.Add(new MenuList { Title = "Alert", IconSource = Icons.DiscrepancyReport, IconType = false, opacity = 0.5, roule = new List<int> { 1, 2, 3, 4, 5 } });
            listOfMenuItems.Add(new MenuList { Title = "Source Data", IconSource = Icons.SourceDataUpload, IconType = false, opacity = 0.5, roule = new List<int> { 1, 4 } });
          //  listOfMenuItems.Add(new MenuList { Title = "SPMat Data Upload", IconSource = Icons.SourceDataUpload, IconType = false, opacity = 0.5, roule = new List<int> { 2, 3 } });
            listOfMenuItems.Add(new MenuList { Title = "Location", IconSource = Icons.LocationUpload, IconType = false, opacity = 0.5, roule = new List<int> { 1 } });
            listOfMenuItems.Add(new MenuList { Title = "Location Master", IconSource = Icons.LocationUpload, IconType = false, opacity = 0.5, roule = new List<int> { 2, 3 } });
            listOfMenuItems.Add(new MenuList { Title = "yShip", IconSource = Icons.yShip, IconType = false, opacity = 1, roule = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 } });
            listOfMenuItems.Add(new MenuList { Title = "Configurations", IconSource = Icons.Configurations, IconType = false, opacity = 0.5, roule = new List<int> { 1 } });
            //listOfMenuItems.Add(new MenuList { Title = "Mobile Apps", IconSource = Icons.MobileAppsIc, IconType = false, opacity = 1, roule = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 } });
            //listOfMenuItems.Add(new MenuList { Title = "Guided Tour", IconSource = "GTour.png", IconType = false, opacity = 1, roule = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 } });
            listOfMenuItems.Add(new MenuList { Title = "About", IconSource = Icons.InfoIc, IconType = false, opacity = 1, roule = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 } });
            listOfMenuItems.Add(new MenuList { Title = "Logout", IconSource = Icons.Logout, IconType = false, opacity = 1, roule = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 } });
            #endregion


            var data = listOfMenuItems.Where(x => x.roule.Contains(Settings.userRoleID)).ToList();

            foreach(var items in data)
            {
                if(items.IconType == true)
                {
                    items.Image = true;
                    items.Label = false;
                }
                else
                {
                    items.Image = false;
                    items.Label = true;
                }
            }
            MenuItems = data;
        }


        #region Properties
        private List<MenuList> _MenuItems;
        public List<MenuList> MenuItems
        {
            get { return _MenuItems; }
            set
            {
                _MenuItems = value;
                NotifyPropertyChanged();
            }
        }

        private string _UserName = Settings.SGivenName;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                NotifyPropertyChanged();
            }
        }

        private string _UserEmail = Settings.UserMail;
        public string UserEmail
        {
            get { return _UserEmail; }
            set
            {
                _UserEmail = value;
                NotifyPropertyChanged();
            }
        }

        private string _LoginID = EncryptManager.Decrypt(Settings.LoginID);
        public string LoginID
        {
            get { return _LoginID; }
            set
            {
                _LoginID = value;
                NotifyPropertyChanged();
            }
        }

        private string _UserEntityName = Settings.EntityName;
        public string UserEntityName
        {
            get { return _UserEntityName; }
            set
            {
                _UserEntityName = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region INotifyPropertyChanged Implimentation
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public void RaisePropertyChanged(String name)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
