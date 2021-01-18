using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;

namespace YPS.YShip
{
    public class YshipGridThemeColor : DataGridStyle
    {
        public YshipGridThemeColor()
        {

        }
        public override ImageSource GetGroupCollapseIcon()
        {
            return ImageSource.FromFile("right.png");
        }

        public override ImageSource GetGroupExpanderIcon()
        {
            return ImageSource.FromFile("down.png");
        }

        // Customize border width for header cells
        public override float GetHeaderBorderWidth()
        {
            return 5;
        }

        public override Color GetHeaderBackgroundColor()
        {
            return Color.FromHex("#999999");
        }

        public override Color GetHeaderForegroundColor()
        {
            return Color.White;
        }

        public override Color GetRecordBackgroundColor()
        {
            return Color.White;
        }

        public override Color GetRecordForegroundColor()
        {
            return Color.FromHex("#000");
        }

        public override Color GetCaptionSummaryRowBackgroundColor()
        {
            return Color.FromHex("#f7f7f7");
        }

        public override Color GetCaptionSummaryRowForeGroundColor()
        {
            return Color.FromHex("#000");
        }

        public override Color GetSelectionBackgroundColor()
        {
            return Color.LightBlue;
        }

        public override Color GetSelectionForegroundColor()
        {
            return Color.Black;
        }

        public override Color GetBorderColor()
        {
            return Color.White;
        }
        public override Color GetAlternatingRowBackgroundColor()
        {
            return Color.FromHex("#dedddd");
        }
    }
}
