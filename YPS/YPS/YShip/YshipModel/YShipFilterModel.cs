using System.Collections.Generic;

namespace YPS.YShip.YshipModel
{
    /// <summary>
    /// Update yShip Details Model
    /// </summary>

    public class UpdateyShipDetails
    {
        public int yShipId { get; set; }
        public string yShipIdEncrypted { get; set; }
        public string yBkgNo { get; set; }
        public string yShipBkgNumber { get; set; }
        public int CompanyID { get; set; }
        public int PLFinalId { get; set; }
        public string BkgRefNo { get; set; }
        public int? BkgConfirmed { get; set; }
        public int Cancel { get; set; }
        public int Complete { get; set; }
        public int ShmtType { get; set; }
        public int EqmtType1 { get; set; }
        public int EqmtType1Qty { get; set; }
        public int EqmtType2 { get; set; }
        public int EqmtType2Qty { get; set; }
        public int EqmtType3 { get; set; }
        public int EqmtType3Qty { get; set; }
        public int TotalEqmtQty { get; set; }
        public string PickupPIC { get; set; }
        public string DestinationPIC { get; set; }
        public string Shipper { get; set; }
        public string OrgSearch { get; set; }
        public int OrgLocationID { get; set; }
        public string OrgCntryCode { get; set; }
        public string OrgCntryName { get; set; }
        public string OrgLocCode { get; set; }
        public string OrgLocName { get; set; }
        public string PickupLocation { get; set; }
        public string ETD { get; set; }
        public string PickUpTime { get; set; }
        public string ShipToParty { get; set; }
        public string DestSearch { get; set; }
        public int DestLocationID { get; set; }
        public string DestCntryCode { get; set; }
        public string DestCntryName { get; set; }
        public string DestLocCode { get; set; }
        public string DestLocName { get; set; }
        public string DeliveryLocation { get; set; }
        public string ETA { get; set; }
        public string DeliveryTime { get; set; }
        public string InvNo { get; set; }
        public string CustRefNo { get; set; }
        public int ShmtTerm { get; set; }
        public string SpecialCargoHandlingInstructions { get; set; }
        public string GeneralDescription { get; set; }
        public object Currency { get; set; }
        public decimal TotalAmount { get; set; }
        public int TtlPkg { get; set; }
        public string PkgUnit { get; set; }
        public decimal TtlM3 { get; set; }
        public decimal TtlGrossWt { get; set; }
        public decimal TtlNetWt { get; set; }
        public int ShipmentLoadedBy { get; set; }
        public string ShipmentOpenedby { get; set; }
        public int CreatedBy { get; set; }
        public object CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public object UpdatedDate { get; set; }
        public int IsACT { get; set; }
        public int IsBkgConfirmed { get; set; }
        public int IsOTLBkg { get; set; }
        public int IsInv_P_L { get; set; }
        public int IsESTPickUpTime { get; set; }
        public int IsATD { get; set; }
        public int IsETA { get; set; }
        public int IsATA { get; set; }
        public int IsETD { get; set; }
        public object ETD1 { get; set; }
        public int IsPickUpInfo { get; set; }
        public int IsDeliveryInfo { get; set; }
        public int IsExportBkg { get; set; }
        public int IsImportBkg { get; set; }
        public int yShipQACount { get; set; }
        public int yShipQAClosedCount { get; set; }
        public int InvoicePLCount { get; set; }
        public int PhotoCount { get; set; }
        public int PermitCount { get; set; }
    }

    public class ResponseUpdateyShipDetails
    {
        public string message { get; set; }
        public int status { get; set; }
        public List<string> data { get; set; }
    }

    public class GetyShipDetailsResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public UpdateyShipDetails data { get; set; }
    }
    public class YShipClose
    {
        public string message { get; set; }
        public int status { get; set; }
        public int data { get; set; }
    }

    public class Company
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
        public string DisplayText { get; set; }
        public int Status { get; set; }
    }

    public class LoadType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
        public string DisplayText { get; set; }
        public int Status { get; set; }
    }

    public class ShipmentType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
        public string DisplayText { get; set; }
        public int Status { get; set; }
    }

    public class ShipmentLoadedType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
        public string DisplayText { get; set; }
        public int Status { get; set; }
    }

    public class ShipmentTerm
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
        public string DisplayText { get; set; }
        public int Status { get; set; }
    }

    public class BkgConfirmed
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
        public string DisplayText { get; set; }
        public int Status { get; set; }
    }

    public class Data
    {
        public List<Company> Company { get; set; }
        public List<LoadType> LoadType { get; set; }
        public List<ShipmentType> ShipmentType { get; set; }
        public List<ShipmentLoadedType> ShipmentLoadedType { get; set; }
        public List<ShipmentTerm> ShipmentTerm { get; set; }
        public List<BkgConfirmed> BkgConfirmed { get; set; }
    }

    public class YShipPickerDataModel
    {
        public string message { get; set; }
        public int status { get; set; }
        public Data data { get; set; }
    }
}
