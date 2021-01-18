using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;

namespace YPS.GridConverters
{
    public class CustomSelectionController : GridSelectionController
    {
        public CustomSelectionController()
        {
            this.SelectedRows = new GridSelectedRowsCollection();
        }
        protected override async void SetSelectionAnimation(VirtualizingCellsControl rowElement)
        {
            rowElement.Opacity = 0.50;
            await rowElement.FadeTo(1, 400, Easing.CubicInOut);
        }
    }
}
