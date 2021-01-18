using Syncfusion.SfDataGrid.XForms;

namespace YPS.CustomRenders
{
    public class GridSwitchRendererExt : GridCellSwitchRenderer
    {
        public override void OnInitializeDisplayView(DataColumnBase dataColumn, SfSwitchControl view)
        {
            base.OnInitializeDisplayView(dataColumn, view);
            view.Toggled += (sender, e) =>
            {
                if (e.Value)
                {
                    dataColumn.Renderer.DataGrid.SelectedItems.Add(dataColumn.RowData);
                }
                else
                {
                    dataColumn.Renderer.DataGrid.SelectedItems.Remove(dataColumn.RowData);
                }
                dataColumn.Renderer.DataGrid.UpdateDataRow(dataColumn.RowIndex);
            };
        }
    }
}
