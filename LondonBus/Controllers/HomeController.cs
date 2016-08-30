using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using LondonBus.Filters;
using LondonBus.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LondonBus.Controllers
{
    public class HomeController : Controller
    {
        private readonly BusStopInfoDownloader _initializingBusStopInfoDownloader = new BusStopInfoDownloader();
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AutocompleteSearch(string term)
        {
            IList<string> stopNames = _initializingBusStopInfoDownloader.AllBusStopInfo.Select(n => (string)n["commonName"]).Distinct().ToList();
            
            var stops = stopNames.Select(a => a.ToUpperInvariant()).Where(a => a.StartsWith(term.ToUpperInvariant())).Select(a => new { value = a });

            return Json(stops, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FindArrivalsAtBusStop(string stopName)
        {
            IList<string> stopNames = _initializingBusStopInfoDownloader.AllBusStopInfo.Select(n => (string)n["commonName"]).Distinct().ToList();

            if (stopNames.Select(z => z.ToUpperInvariant()).Contains(stopName))
            {
                string naptanId = _initializingBusStopInfoDownloader.AllBusStopInfo.Where(n => (((string)n["commonName"]).ToUpperInvariant() == stopName)).Select(n => (string)n["naptanId"]).FirstOrDefault();

                string urlFirstPart = "https://api.tfl.gov.uk/StopPoint/";
                string urlLastPart = "/arrivals";
                string urlFull = urlFirstPart + naptanId + urlLastPart;

                JArray oneStopArivings;
                using (var oneStopWebClient = new WebClient())
                {
                    var onestopinfoMemoryStream = new MemoryStream(oneStopWebClient.DownloadData(urlFull));
                    var onestopreader = new StreamReader(onestopinfoMemoryStream);
                    var onestopinfo = (JArray)JToken.ReadFrom(new JsonTextReader(onestopreader));
                    oneStopArivings = onestopinfo;
                }
                IList<string> lineName = oneStopArivings.Select(a => (string)a["lineName"]).ToList();
                IList<string> direction = oneStopArivings.Select(a => (string)a["direction"]).ToList();
                IList<string> timeToStation = oneStopArivings.Select(a => (string)a["timeToStation"]).ToList();
                IList<string> modeName = oneStopArivings.Select(a => (string)a["modeName"]).ToList();
                int capForShownInfo = lineName.Count();
                List<string> shownInfo = new List<string>(capForShownInfo);

                for (int i = 0; i < capForShownInfo; i++)
                {
                    shownInfo.Add(direction[i] + " " + modeName[i] + " " + lineName[i] + " " + " arriving at "
                    + String.Format("{0:00} min. {1} sec.", (Int32.Parse(timeToStation[i]) / 60),
                    (Int32.Parse(timeToStation[i]) - (60 * (Int32.Parse(timeToStation[i]) / 60)))));
                }

                shownInfo.Sort();
                ViewData["StopName"] = stopName;

                return View(shownInfo);
            }
            else
            {
                TempData["warning"] = "Be careful, when tiping StopName. Better choose it from list below.";
                return View("Index");
            }
            
        }
    }
}
