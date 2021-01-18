using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;

namespace YPS.CustomRenders
{
    public class Dark : DataGridStyle
    {
        public Dark()
        {
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
            return Color.Black;
        }

        public override Color GetCaptionSummaryRowBackgroundColor()
        {
            return Color.FromHex("#f7f7f7");
        }

        public override Color GetCaptionSummaryRowForeGroundColor()
        {
            return Color.Black;
        }

        public override Color GetSelectionBackgroundColor()
        {
            return Color.Red;
        }

        public override Color GetSelectionForegroundColor()
        {
            return Color.LemonChiffon;
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
