using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;

namespace YPS.GridConverters
{
    public class GridThemeColor : DataGridStyle
    {
        /// <summary>
        /// Parameter less constructor.
        /// </summary>
        public GridThemeColor(){}

        /// <summary>
        /// GetGroupCollapseIcon
        /// </summary>
        /// <returns></returns>
        public override ImageSource GetGroupCollapseIcon()
        {
            return ImageSource.FromFile("right.png");
        }

        /// <summary>
        /// GetGroupExpanderIcon
        /// </summary>
        /// <returns></returns>
        public override ImageSource GetGroupExpanderIcon()
        {
            return ImageSource.FromFile("down.png");
        }

        /// <summary>
        /// Customize border width for header cells.
        /// </summary>
        /// <returns></returns>
        public override float GetHeaderBorderWidth()
        {
            return 5;
        }

        /// <summary>
        /// GetHeaderBackgroundColor
        /// </summary>
        /// <returns></returns>
        public override Color GetHeaderBackgroundColor()
        {
            return Color.FromHex("#999999");
        }

        /// <summary>
        /// GetHeaderForegroundColor
        /// </summary>
        /// <returns></returns>
        public override Color GetHeaderForegroundColor()
        {
            return Color.White;
        }

        /// <summary>
        /// GetRecordBackgroundColor
        /// </summary>
        /// <returns></returns>
        public override Color GetRecordBackgroundColor()
        {
            return Color.White;
        }

        /// <summary>
        /// GetRecordForegroundColor
        /// </summary>
        /// <returns></returns>
        public override Color GetRecordForegroundColor()
        {
            return Color.FromHex("#000");
        }

        /// <summary>
        /// GetCaptionSummaryRowBackgroundColor
        /// </summary>
        /// <returns></returns>
        public override Color GetCaptionSummaryRowBackgroundColor()
        {
            return Color.FromHex("#f7f7f7");
        }

        /// <summary>
        /// GetCaptionSummaryRowForeGroundColor
        /// </summary>
        /// <returns></returns>
        public override Color GetCaptionSummaryRowForeGroundColor()
        {
            return Color.FromHex("#000");
        }

        /// <summary>
        /// GetSelectionBackgroundColor
        /// </summary>
        /// <returns></returns>
        public override Color GetSelectionBackgroundColor()
        {
            return Color.LightBlue;
        }

        /// <summary>
        /// GetSelectionForegroundColor
        /// </summary>
        /// <returns></returns>
        public override Color GetSelectionForegroundColor()
        {
            return Color.Black;
        }

        /// <summary>
        /// Setting border color.
        /// </summary>
        /// <returns></returns>
        public override Color GetBorderColor()
        {
            return Color.White;
        }

        /// <summary>
        /// GetAlternatingRowBackgroundColor
        /// </summary>
        /// <returns></returns>
        public override Color GetAlternatingRowBackgroundColor()
        {
            return Color.FromHex("#dedddd");
        }
    }
}
