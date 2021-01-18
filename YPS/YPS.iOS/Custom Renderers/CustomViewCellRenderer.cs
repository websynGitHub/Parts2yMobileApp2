using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.CustomRenders;
using YPS.iOS.Custom_Renderers;

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
            var cell = base.GetCell(item, reusableCell, tv);
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            cell.BackgroundColor = UIColor.Clear;
            return cell;
        }
    }
}