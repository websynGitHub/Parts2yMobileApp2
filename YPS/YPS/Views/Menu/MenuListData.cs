using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YPS.CommonClasses;

namespace YPS.Views.Menu
{
    public class MenuListData : List<MenuItem>
    {
        public MenuListData()
        {
            try
            {
                this.Add(new MenuItem()
                {
                    Title = "Home",
                    IconSource = "home.png",
                    TargetType = typeof(MainPage)
                });
                //this.Add(new MenuItem()
                //{
                //    Title = "Update Profile",
                //    IconSource = "uPSetting.png",
                //    TargetType = typeof(UpdateProfilePage)
                //});

                if (Settings.userRoleID == 1 || Settings.userRoleID == 2 || Settings.userRoleID == 3)
                {
                    this.Add(new MenuItem()
                    {
                        Title = "Users",
                        IconSource = "users.png",
                    });

                    this.Add(new MenuItem()
                    {
                        Title = "Project",
                        IconSource = "projects.png",
                    });

                    this.Add(new MenuItem()
                    {
                        Title = "Job",
                        IconSource = "job.png",
                    });

                    this.Add(new MenuItem()
                    {
                        Title = "Supplier",
                        IconSource = "supplierM.png",
                    });

                    this.Add(new MenuItem()
                    {
                        Title = "Ship Ctrl Report",
                        IconSource = "shipreport.png",
                    });
                    this.Add(new MenuItem()
                    {
                        Title = "Discrepancy Report",
                        IconSource = "reportM.png",
                    });

                    this.Add(new MenuItem()
                    {
                        Title = "SPMat Data Upload",
                        IconSource = "SPMatM.png",
                    });
                    this.Add(new MenuItem()
                    {
                        Title = "Location Master",
                        IconSource = "locationM.png",
                    });
                    this.Add(new MenuItem()
                    {
                        Title = "yShip",
                        IconSource = "yshipM.png",
                    });
                }
               else if (Settings.userRoleID == 6)
                {
                    this.Add(new MenuItem()
                    {
                        Title = "Users",
                        IconSource = "users.png",

                    });
                }
              else if (Settings.userRoleID == 4)
                {
                    this.Add(new MenuItem()
                    {
                        Title = "Users",
                        IconSource = "users.png",
                    });

                    this.Add(new MenuItem()
                    {
                        Title = "Project",
                        IconSource = "projects.png",
                    });

                    this.Add(new MenuItem()
                    {
                        Title = "Job",
                        IconSource = "job.png",
                    });

                    this.Add(new MenuItem()
                    {
                        Title = "Supplier",
                        IconSource = "supplierM.png",
                    });
                    this.Add(new MenuItem()
                    {
                        Title = "Ship Ctrl Report",
                        IconSource = "shipreport.png",
                    });
                    this.Add(new MenuItem()
                    {
                        Title = " Discrepancy Report",
                        IconSource = "reportM.png",
                    });
                }

               else if (Settings.userRoleID == 5)
                {                    
                    this.Add(new MenuItem()
                    {
                        Title = "Ship Ctrl Report",
                        IconSource = "shipreport.png",
                    });
                    this.Add(new MenuItem()
                    {
                        Title = " Discrepancy Report",
                        IconSource = "reportM.png",
                    });
                }

                this.Add(new MenuItem()
                {
                    Title = "Logout",
                    IconSource = "logouM.png",
                    TargetType = typeof(LoginPage)
                });
            }
            catch (Exception ex)
            {

            }
        }
    }
}