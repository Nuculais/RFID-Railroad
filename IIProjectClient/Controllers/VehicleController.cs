using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using IIProjectClient.IIService;
using IIProjectClient.IIVehicleService;
using IIProjectClient.Models;
using PagedList.Mvc;
using PagedList;

namespace IIProjectClient.Controllers
{
    public class VehicleController : Controller
    {
        static List<VehiclePassage> vehiclePassageList = new List<VehiclePassage>();
        static XElement fullXmlAnswer;

        // GET: Vehicle

        public ActionResult Index()
        {

            NamingServiceClient namingService = new NamingServiceClient();
            //  VehicleServiceClient vehicleService = new VehicleServiceClient();
            

            XElement xmlResponse = namingService.GetAllLocations();

            List<string> locationList = (from location in xmlResponse.Descendants("Location")
                             select location.Element("Name").Value).ToList();



            List<SelectListItem> listOfLocations = new List<SelectListItem>();

            foreach (var a in xmlResponse.Descendants("Location"))
            {
                listOfLocations.Add(new SelectListItem
                {
                    Text = a.Element("Name").Value.ToString(),
                    Value = a.Element("Name").Value.ToString() + "*" + a.Element("Epc").Value.ToString()
                });
            }

           
            ViewBag.ListOfLocations = listOfLocations;
            

            //  (DateTime start, DateTime end, String locationEPC, string location, string user)
            DateTime start = new DateTime(2011, 03, 25);
            DateTime end = new DateTime(2011, 03, 28);
            String locationEPC = "urn:epc:id:sgln:735999271.000.12";
            string locationn = "Göteborg";
            string user = "Felix";

            

          //  XElement xmlTest = vehicleService.XmlAnswer(start, end, locationEPC, locationn, user);
            



            return View();
        }
        [HttpPost]
        public ActionResult Index(string startDate, string endDate, string locationAndEpc, string user, string itemsPerPage)
        {
            int format = 0;
            bool inputError = false;
            DateTime start = new DateTime();
            DateTime end = new DateTime();
            VehicleServiceClient vehicleService = new VehicleServiceClient();
            string[] locationEpcArray = locationAndEpc.Split('*');
            string location = locationEpcArray[0];
            String locationEPC = locationEpcArray[1];

            bool authenticated = vehicleService.AuthenticateUser(user);

            //ViewBags
            ViewBag.start = startDate;
            ViewBag.end = endDate;
            ViewBag.locc = locationAndEpc;
            ViewBag.epc = locationEPC;
            ViewBag.usr = user;

            vehiclePassageList.Clear();

            //Checks if itemsPerPage, startDate or endDate are in the wrong format
            if (Int32.TryParse(itemsPerPage, out format) == false || DateTime.TryParse(startDate, out start) == false || DateTime.TryParse(endDate, out end) == false)
            {
                //if any of them are, sets format to -1 which will cause a responseCode 2, resulting in an error
                inputError = true;
                format = 1;
            }
            else
            {
                format = Convert.ToInt32(itemsPerPage);
            }


            // fullXmlAnswer = vehicleService.XmlAnswer(start, end, locationEPC, location, user, authenticated);
            fullXmlAnswer = vehicleService.XmlAnswer(start, end, locationEPC, location, user, authenticated, inputError);
           

            foreach (XElement vehiclePassage in fullXmlAnswer.Descendants("FordonPassage"))
            {
                vehiclePassageList.Add(new VehiclePassage(vehiclePassage));
            }




            return RedirectToAction("SearchResults", new { size = format });
        }

        public ActionResult SearchResults(int? page, int size)
        {

            var pageNumber = page ?? 1;

            var onePageOfPassages = vehiclePassageList.ToPagedList(pageNumber, size);

            ViewBag.OnePageOfPassages = onePageOfPassages;
            ServiceMessage serviceMessage = new ServiceMessage(fullXmlAnswer.Descendants("TjänsteMeddelande").First());
            ViewBag.serviceMessage = serviceMessage;
            ViewBag.perPage = size;
           // vehiclePassageList.Clear();

            return View();
        }
       
    }
}