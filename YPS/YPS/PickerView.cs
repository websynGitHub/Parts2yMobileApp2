using System;
using System.Collections.Generic;
using System.Text;
using System;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.CommonClasses;
using YPS.Parts2y.Parts2y_Views;
using YPS.CustomToastMsg;
using YPS.Service;
using YPS.Helpers;

namespace YPS
{
    public class PickerView : View
    {
        public event EventHandler StartScanningRequested;
        public event EventHandler PauseScanningRequested;
        public event EventHandler StopScanningRequested;

        public IScannerDelegate Delegate { get; set; }
        public ScanerSettings Settings { get; set; }

        public static string GetAppKey()
        {
            try
            {
                return HostingURL.scandItLicencekey;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetAppKey method -> in PickerView.cs " + CommonClasses.Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
                return "";
            }
        }

        public void StartScanning()
        {
            try
            {
                StartScanningRequested?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "StartScanning method -> in PickerView.cs " + CommonClasses.Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        public void PauseScanning()
        {
            try
            {
                PauseScanningRequested?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PauseScanning method -> in PickerView.cs " + CommonClasses.Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        public void StopScanning()
        {
            try
            {
                StopScanningRequested?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "StopScanning method -> in PickerView.cs " + CommonClasses.Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        public void DidScan(string symbology, string code)
        {
            try
            {
                if (Delegate != null)
                {
                    Delegate.DidScan(symbology, code);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DidScan method -> in PickerView.cs " + CommonClasses.Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }
    }
}
