using System;
using System.Xml.Linq;

namespace IIProjectClient.Models
{
    public class RequestInformation
    {
        public string Receiver { get; set; }
        public string Location { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }

        public RequestInformation(XElement xml)
        {
            Receiver = xml.Element("reciever").Value;
            Location = xml.Element("RequestArguments").Element("location").Value;
            FromTime = DateTime.Parse(xml.Element("RequestArguments").Element("fromTime").Value);
            ToTime = DateTime.Parse(xml.Element("RequestArguments").Element("toTime").Value);
        }
    }
}