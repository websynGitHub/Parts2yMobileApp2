using System;
using System.Collections.Generic;
using System.Text;
using System;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.CommonClasses;
using YPS.Parts2y.Parts2y_Views;
using YPS.CustomToastMsg;

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
            return HostingURL.scandItLicencekey;
        }

        public void StartScanning()
        {
            StartScanningRequested?.Invoke(this, EventArgs.Empty);
        }

        public void PauseScanning()
        {
            PauseScanningRequested?.Invoke(this, EventArgs.Empty);
        }

        public void StopScanning()
        {
            StopScanningRequested?.Invoke(this, EventArgs.Empty);
        }

        public void DidScan(string symbology, string code)
        {
            if (Delegate != null)
            {
                Delegate.DidScan(symbology, code);
            }
        }
    }
}
