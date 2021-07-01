using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.CommonClasses;
using YPS.CustomRenders;
using YPS.Helpers;
using YPS.iOS.Custom_Renderers;
using YPS.Service;

[assembly: ExportRenderer(typeof(MyViewCell), typeof(CustomViewCellRenderer))]
namespace YPS.iOS.Custom_Renderers
{
    public class CustomViewCellRenderer : ViewCellRenderer
    {
        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="reusableCell"></param>
        /// <param name="tv"></param>
        /// <returns></returns>
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            try
            {
                var cell = base.GetCell(item, reusableCell, tv);
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.BackgroundColor = UIColor.Clear;
                return cell;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetCell method -> in CustomViewCellRenderer.cs " + Settings.userLoginID);
                YPSService trackService = new YPSService();
                trackService.Handleexception(ex);
                return null;
            }
        }
    }
}