using System.Xml.Linq;

namespace IIProjectClient.Models
{
    public class ServiceMessage
    {
        public RequestInformation requestInformation;
        public ResponseInformation responseInformation;

        public ServiceMessage(XElement xml)
        {
            this.requestInformation = new RequestInformation(xml.Element("RequestInformation"));
            this.responseInformation = new ResponseInformation(xml.Element("ResponseInformation"));
        }
    }
}