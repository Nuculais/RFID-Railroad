using System;
using System.Xml.Linq;

namespace IIProjectClient.Models
{
    public class ResponseInformation
    {
        public string ResponseCode { get; set; }
        public string Message { get; set; }
        public string Receiver { get; set; }
        public string ApplicationInformation { get; set; }

        public DateTime TimeOfRequest { get; set; }

        public ResponseInformation(XElement xml)
        {
            ResponseCode = xml.Element("responseCode").Value;
            Message = xml.Element("Message").Value;
            Receiver = xml.Element("Reciever").Value;
            ApplicationInformation = xml.Element("ApplicationInformation").Value;
            TimeOfRequest = DateTime.Now;
        }
    }
}