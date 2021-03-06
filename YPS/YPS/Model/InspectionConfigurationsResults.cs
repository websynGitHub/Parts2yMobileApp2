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

        private int _VersionID;
        public int VersionID
        {
            get
            {
                return _VersionID;
            }
            set
            {
                this._VersionID = value;
                RaisePropertyChanged("VersionID");
            }
        }

        private string _VersionName;
        public string VersionName
        {
            get
            {
                return _VersionName;
            }
            set
            {
                this._VersionName = value;
                RaisePropertyChanged("VersionName");
            }
        }

        private int _SerialNo;
        public int SerialNo
        {
            get
            {
                return _SerialNo;
            }
            set
            {
                this._SerialNo = value++;
                RaisePropertyChanged("SerialNo");
            }
        }

        private Color _AreBgColor = Color.Yellow;
        public Color AreBgColor
        {
            get => _AreBgColor;
            set
            {
                _AreBgColor = value;
                RaisePropertyChanged("AreBgColor");
            }
        }

        private Color _SignQuesBgColor = Color.Gray;
        public Color SignQuesBgColor
        {
            get => _SignQuesBgColor;
            set
            {
                _SignQuesBgColor = value;
                RaisePropertyChanged("SignQuesBgColor");
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

                if (string.IsNullOrEmpty(this.Area))
                {
                    AreBgColor = Color.Transparent;
                }
                else
                {
                    AreBgColor = Color.Yellow;
                }
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

        private int _Direct = 0;
        public int Direct
        {
            get
            {
                return _Direct;
            }
            set
            {
                this._Direct = value;
                RaisePropertyChanged("Direct");
            }
        }
        
        private int _additional_entry = 0;
        public int additional_entry
        {
            get
            {
                return _additional_entry;
            }
            set
            {
                this._additional_entry = value;
                RaisePropertyChanged("additional_entry");
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
                    SignQuesBgColor = Color.Black;
                }
                else
                {
                    ItemBackground = Color.Transparent;
                    SignQuesBgColor = Color.Gray;
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

        private InspectionResultsList inspectionResult;
        public InspectionResultsList InspectionResult
        {
            get => inspectionResult;
            set
            {
                inspectionResult = value;
                RaisePropertyChanged("InspectionResult");
            }
        }
    }
}
