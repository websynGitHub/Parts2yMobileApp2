using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.Maps;

namespace YPS.Parts2y.Parts2y_Models
{
    public class mapModel
    {
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string SlotColor { get; set; }


        //public string alert { get; set; }

        //public int? CentreId { get; set; }
        //public int? RoomId { get; set; }
        //public string RoomName { get; set; }
        //public int? Duration { get; set; }
        //public string Address1 { get; set; }
        //public string Address2 { get; set; }
        //public string Address3 { get; set; }
        //public string PostalCode { get; set; }
        //public string Price { get; set; }
        //public int? RadiologyScanTestId { get; set; } // this is specific to each radiology
        //public string LogoName { get; set; }
        //public string Currency { get; set; }
        //public string Country { get; set; }
        //public int? TestTypeId { get; set; }
    }
    public class LocationDetails
    {
        public string Address { get; set; }
        public string Description { get; set; }
        public Position Position { get; set; }

        public ObservableCollection<BusDetails> busDetailsList { get; set; }


    }
    public class BusDetails
    {
        public string Bus_No { get; set; }
        public string BusArriving_Time { get; set; }
        public string BusDeparture_Time { get; set; }
    }

}
