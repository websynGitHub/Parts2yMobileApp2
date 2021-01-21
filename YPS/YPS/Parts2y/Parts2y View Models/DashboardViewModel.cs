using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Parts2y.Parts2y_Views;

namespace YPS.Parts2y.Parts2y_View_Models
{
    class DashboardViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public Command TaskClickCmd { get; set; }
        public Command CompareClickCmd { get; set; }

        public DashboardViewModel(INavigation _Navigation)
        {
            try
            {
                Navigation = _Navigation;
                BgColor = Settings.Bar_Background;
                TaskClickCmd = new Command(async () => await RedirectToPage("task"));
                CompareClickCmd = new Command(async () => await RedirectToPage("compare"));
            }
            catch (Exception ex)
            {

            }
        }

        public async Task RedirectToPage(string page)
        {
            loadindicator = true;
            try
            {
                if (page == "task")
                {
                    if (Settings.roleid == 1)
                    {
                        await Navigation.PushAsync(new Driverpage());
                    }
                    else if (Settings.roleid == 2)
                    {
                        await Navigation.PushAsync(new HomePage());

                    }
                    else
                    {
                        await Navigation.PushAsync(new DealerPage());
                    }
                }
                else
                {
                    await Navigation.PushAsync(new Compare());
                }
            }
            catch (Exception ex)
            {

            }
            loadindicator = false;
        }
        #region Properties
        private Color _BgColor;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                OnPropertyChanged("BgColor");
            }
        }

        private bool _isCompareVisible = Settings.roleid == 1 ? true : false;
        public bool isCompareVisible
        {
            get => _isCompareVisible;
            set
            {
                _isCompareVisible = value;
                NotifyPropertyChanged("isCompareVisible");
            }
        }

        private bool _loadindicator = false;
        public bool loadindicator
        {
            get => _loadindicator;
            set
            {
                _loadindicator = value;
                NotifyPropertyChanged("loadindicator");
            }
        }
        #endregion Properties

    }
}
