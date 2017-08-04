using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Linq;
using IIProjectService.IIService;

namespace IIProjectService
{
    public class VehicleService : IVehicleService
    {
        EpcisEventServiceClient epcisEventClient = new EpcisEventServiceClient();
        NamingServiceClient namingClient = new NamingServiceClient();   

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        /// <summary>
        /// Main method to return the full XML answer
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="locationEPC"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public XElement XmlAnswer(DateTime start, DateTime end, String locationEPC, string location, string user, bool authenticated, bool inputError)
        {
            // Fetches all the events and the specified location and between the specified timespan
            IEnumerable<XElement> allEvents = epcisEventClient.GetEvents(start, end, locationEPC);

            XElement objectEventList = new XElement("Events");

            foreach(var a in allEvents)
            {
                foreach (var b in a.Descendants("ObjectEvent"))
                {
                    objectEventList.Add(b);
                }
            }
            // The variable that will hold the final Xml to be returned to the client
            XElement xmlAnswer;

            // Handles the three cases specified in the assignment
            if (allEvents == null || allEvents.Descendants("ObjectEvent").Any() == false || inputError == true)
            {
                xmlAnswer = new XElement("XMLSvar",
                                new XElement("TjänsteMeddelande",
                                    new XElement("ResponseInformation",
                                        new XElement("responseCode", 2),
                                        new XElement("Message", "Kunde inte genomföra operationen"),
                                        new XElement("Reciever", "Grupp 14"),
                                        new XElement("ApplicationInformation", "Fordonservice 1.0"),
                                        new XElement("TimeOfRequest", DateTime.Now)),
                                    new XElement("RequestInformation",
                                        new XElement("reciever", user),
                                        new XElement("RequestArguments",
                                            new XElement("fromTime", start),
                                            new XElement("toTime", end),
                                            new XElement("location", location)))));
            }
            // User not authenticated
            else if (authenticated == false )
            {
                xmlAnswer = new XElement("XMLSvar",
                            new XElement("TjänsteMeddelande",
                                new XElement("ResponseInformation",
                                    new XElement("responseCode", 3),
                                    new XElement("Message", "Autentisering misslyckades"),
                                    new XElement("Reciever", "Grupp 14"),
                                    new XElement("ApplicationInformation", "Fordonservice 1.0"),
                                    new XElement("TimeOfRequest", DateTime.Now)),
                                new XElement("RequestInformation",
                                    new XElement("reciever", user),
                                    new XElement("RequestArguments",
                                        new XElement("fromTime", start),
                                        new XElement("toTime", end),
                                        new XElement("location", location)))));
 
            }
            else
            {
                xmlAnswer = new XElement("XmlSvar");

               // IEnumerable<XElement> vehicleInformation;
                
                // For each event identified, build a FordonPassage and add it to the xmlSvar 
                foreach (var epcisEvent in objectEventList.Descendants("ObjectEvent"))
                {

                    XElement svar = FordonPassageConstruct(epcisEvent, location);
                    xmlAnswer.Add(svar);                    
                }

                XElement xmlServiceMessage = new XElement("TjänsteMeddelande",
                                        new XElement("ResponseInformation",
                                            new XElement("responseCode", 1),
                                            new XElement("Message", "Anropet gick bra"),
                                            new XElement("Reciever", "Grupp 14"),
                                            new XElement("ApplicationInformation", "Fordonservice 1.0"),
                                            new XElement("TimeOfRequest", DateTime.Now)),
                                        new XElement("RequestInformation",
                                            new XElement("reciever", user),
                                            new XElement("RequestArguments",
                                                new XElement("fromTime", start),
                                                new XElement("toTime", end),
                                                new XElement("location", location))));
                
                // Adds the service message to the end of the xmlSvar
                xmlAnswer.Add(xmlServiceMessage);          
            }     
            return xmlAnswer;
        }

        /// <summary>
        /// Constructs a FordonPassage node and returns it to the caller method
        /// </summary>
        /// <param name="epcisEvent"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public XElement FordonPassageConstruct(XElement epcisEvent, string location)
        {
            // Gets information about a specific vehicle
            XElement vehicle = namingClient.GetVehicle(epcisEvent.Element("epcList").Element("epc").Value.ToString());


            XElement vehiclePassage;

            // If there isn't any master data about the vehicle
            if (vehicle.Descendants("Fordonsindivider").Any() == false)
            {
                vehiclePassage = new XElement("FordonPassage",
                                            new XElement("FordonsEPC", GetElementValue(epcisEvent.Element("epcList").Element("epc"))),
                                            new XElement("PlatsEPC", GetElementValue(epcisEvent.Element("readPoint").Element("id"))),
                                            new XElement("Tid", GetElementValue(epcisEvent.Element("eventTime"))),
                                            new XElement("Plats", location));

            }
            else
            {       
            // Builds the FordonPassage node 
            vehiclePassage = new XElement("FordonPassage",

                                            new XElement("FordonsEPC", GetElementValue(epcisEvent.Element("epcList").Element("epc"))),
                                            new XElement("PlatsEPC", GetElementValue(epcisEvent.Element("readPoint").Element("id"))),
                                            new XElement("Tid", GetElementValue(epcisEvent.Element("eventTime"))),
                                            new XElement("Plats", location),



                                            new XElement("EVN", GetElementValue(vehicle.Element("Fordonsindivider").Element("FordonsIndivid").Element("Fordonsnummer"))),
                                            new XElement("Fordonsinnehavare", GetElementValue(vehicle.Element("Fordonsindivider").Element("FordonsIndivid").Element("Fordonsinnehavare").Element("Foretag"))),
                                        
                                            new XElement("Godkänd", GetElementValue(vehicle.Element("Fordonsindivider").Element("FordonsIndivid").Element("Godkannande").Element("FordonsgodkannandeFullVardeSE"))),
                                            new XElement("GiltigFrån", GetElementValue(vehicle.Element("Fordonsindivider").Element("FordonsIndivid").Element("Godkannande").Element("GiltigtFrom"))),
                                            new XElement("GiltigTill", GetElementValue(vehicle.Element("Fordonsindivider").Element("FordonsIndivid").Element("Godkannande").Element("GiltigtTom"))),
                                            new XElement("Underhållsansvarig", GetElementValue(vehicle.Element("Fordonsindivider").Element("FordonsIndivid").Element("UnderhallsansvarigtForetag").Element("Foretag"))),

                                      
                                            new XElement("Fordonstyp", GetElementValue(vehicle.Element("FordonsTyp").Element("FordonskategoriKodFullVardeSE"))),
                                            new XElement("Fordonsunderkategori", GetElementValue(vehicle.Element("FordonsTyp").Element("FordonsunderkategoriKodFullVardeSE")))
                                        );            
        }

            // Returns the FordonPassage node
            return vehiclePassage;
        }

        public string GetElementValue(XElement element)
        {
            return element != null ? element.Value : string.Empty;
        }

        public bool AuthenticateUser(string user)
        {
            List<string> authenticatedUsers = new List<string>();

            string user1 = "Felix";
            string user2 = "Fredrik";
            string user3 = "Erik";
            string user4 = "Malin";
            authenticatedUsers.Add(user1);
            authenticatedUsers.Add(user2);
            authenticatedUsers.Add(user3);
            authenticatedUsers.Add(user4);

            if (authenticatedUsers.Contains(user) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
