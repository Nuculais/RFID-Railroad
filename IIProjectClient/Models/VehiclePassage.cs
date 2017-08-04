using System;
using System.Xml.Linq;
using System.Linq;

namespace IIProjectClient.Models
{
    public class VehiclePassage
    {
        public string VehicleEPC { get; set; }
        public string LocationEPC { get; set; }
        public DateTime Time { get; set; }
        public string LocationName { get; set; }
        public string EVN { get; set; }
        public string VehicleOwner { get; set; }
        public string MaintenanceManager { get; set; }
        public string VehicleType { get; set; }
        public string VehicleSubCategory { get; set; }
        public string ApprovedForTraffic { get; set; }
        public string ApprovedFrom { get; set; }
        public string ApprovedTo { get; set; }

        public VehiclePassage(XElement xml)
        {
           
            this.VehicleEPC = xml.Element("FordonsEPC").Value;
            this.LocationEPC = xml.Element("PlatsEPC").Value;
            this.Time = DateTime.Parse(xml.Element("Tid").Value);
            this.LocationName = xml.Element("Plats").Value;
            this.EVN = xml.Descendants("EVN").Any() ? (xml.Element("EVN").Value) :  "Not available";
            // xml.Descendants("EVN").Any() ? (this.EVN = xml.Element("EVN").Value) : (this.EVN = "");
            this.VehicleOwner = xml.Descendants("Fordonsinnehavare").Any() ? (xml.Element("Fordonsinnehavare").Value) : "Not available";
            // xml.Descendants("Fordonsinnehavare").Any() ? (this.VehicleOwner = xml.Element("Fordonsinnehavare").Value) : (this.VehicleOwner = "");
            this.MaintenanceManager = xml.Descendants("Underhållsansvarig").Any() ? (xml.Element("Underhållsansvarig").Value) : "Not available";
            // xml.Descendants("Underhållsansvarig").Any() ? (this.MaintenanceManager = xml.Element("Underhållsansvarig").Value) : (this.MaintenanceManager = "");
            this.VehicleType = xml.Descendants("Fordonstyp").Any() ? (xml.Element("Fordonstyp").Value) : "Not available";
            // this.VehicleType = xml.Element("Fordonstyp").Value;
            this.VehicleSubCategory = xml.Descendants("Fordonsunderkategori").Any() ? (xml.Element("Fordonsunderkategori").Value) : "Not available";
            //this.VehicleSubCategory = xml.Element("Fordonsunderkategori").Value;
            this.ApprovedForTraffic = xml.Descendants("Godkänd").Any() ? (xml.Element("Godkänd").Value) : "Not available";
            //this.ApprovedForTraffic = xml.Element("Godkänd").Value;
            this.ApprovedFrom = xml.Descendants("GiltigFrån").Any() ? (xml.Element("GiltigFrån").Value).Split('T')[0] :  "";
            this.ApprovedTo = xml.Descendants("GiltigTill").Any() ? (xml.Element("GiltigTill").Value).Split('T')[0] : "" ;
        }
    }
}