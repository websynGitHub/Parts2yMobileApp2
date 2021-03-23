using System;
using System.Collections.Generic;
using System.Text;
using YPS.Service;
using Xamarin.Forms;

namespace YPS.Model
{
    public class InspectionConfigurationsResults
    {
        public string message { get; set; }
        public int status { get; set; }
        public List<InspectionConfiguration> data { get; set; }
    }

    public class InspectionConfiguration : IBase
    {
        private int _MInspectionConfigID;
        public int MInspectionConfigID
        {
            get
            {
                return _MInspectionConfigID;
            }
            set
            {
                this._MInspectionConfigID = value;
                RaisePropertyChanged("MInspectionConfigID");
            }
        }

        private int _SerialCount;
        public int SerialCount
        {
            get
            {
                return _SerialCount;
            }
            set
            {
                this._SerialCount = value++;
                RaisePropertyChanged("SerialCount");
            }
        }



        private int _CategoryID;
        public int CategoryID
        {
            get
            {
                return _CategoryID;
            }
            set
            {
                this._CategoryID = value;
                RaisePropertyChanged("CategoryID");
            }
        }

        private string _CategoryName;
        public string CategoryName
        {
            get
            {
                return _CategoryName;
            }
            set
            {
                this._CategoryName = value;
                RaisePropertyChanged("CategoryName");
            }
        }

        private string _Area;
        public string Area
        {
            get
            {
                return _Area;
            }
            set
            {
                this._Area = value;
                RaisePropertyChanged("Area");
            }
        }

        private string _Question;
        public string Question
        {
            get
            {
                return _Question;
            }
            set
            {
                this._Question = value;
                RaisePropertyChanged("Question");
            }
        }

        private int _IsFront;
        public int IsFront
        {
            get
            {
                return _IsFront;
            }
            set
            {
                this._IsFront = value;
                RaisePropertyChanged("IsFront");
            }
        }
        private int _FrontLeft;
        public int FrontLeft
        {
            get
            {
                return _FrontLeft;
            }
            set
            {
                this._FrontLeft = value;
                RaisePropertyChanged("FrontLeft");
            }
        }
        private int _FrontRight;
        public int FrontRight
        {
            get
            {
                return _FrontRight;
            }
            set
            {
                this._FrontRight = value;
                RaisePropertyChanged("FrontRight");
            }
        }
        private int _IsBack;
        public int IsBack
        {
            get
            {
                return _IsBack;
            }
            set
            {
                this._IsBack = value;
                RaisePropertyChanged("IsBack");
            }
        }
        private int _BackLeft;
        public int BackLeft
        {
            get
            {
                return _BackLeft;
            }
            set
            {
                this._BackLeft = value;
                RaisePropertyChanged("BackLeft");
            }
        }
        private int _BackRight;
        public int BackRight
        {
            get
            {
                return _BackRight;
            }
            set
            {
                this._BackRight = value;
                RaisePropertyChanged("BackRight");
            }
        }
        private int _Status;
        public int Status
        {
            get
            {
                return _Status;
            }
            set
            {
                this._Status = value;
                if (value == 1)
                {
                    ItemBackground = YPS.CommonClasses.Settings.Bar_Background;
                }
                else
                {
                    ItemBackground = Color.Transparent;
                }
                RaisePropertyChanged("Status");
            }
        }

        private Color _ItemBackground = Color.Transparent;
        public Color ItemBackground
        {
            get
            {
                return _ItemBackground;
            }
            set
            {
                this._ItemBackground = value;
                RaisePropertyChanged("ItemBackground");
            }
        }

        private Color _SelectedTagBorderColor = Color.Transparent;
        public Color SelectedTagBorderColor
        {
            get
            {
                return _SelectedTagBorderColor;
            }
            set
            {
                this._SelectedTagBorderColor = value;
                RaisePropertyChanged("SelectedTagBorderColor");
            }
        }
    }
}
