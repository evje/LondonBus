using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LondonBus.Filters
{
    public class BusStopInfoDownloader
    {
        private JArray _basstopdatabase;
        public JArray AllBusStopInfo { get { return _basstopdatabase; } }
        public BusStopInfoDownloader()
        {
            if (_basstopdatabase != null) return;
            using (WebClient busStopWebClient = new WebClient())
            {
                try
                {
                    MemoryStream _busstopinfoMemoryStream =
                        new MemoryStream(busStopWebClient.DownloadData("https://api.tfl.gov.uk/line/24/stoppoints"));
                    StreamReader _allbusstopsReader = new StreamReader(_busstopinfoMemoryStream);
                    _basstopdatabase = (JArray) JToken.ReadFrom(new JsonTextReader(_allbusstopsReader));
                }
                catch (Exception ex)
                {

                    throw new InvalidOperationException(
                        "Can't connect to TFL Server. Please, check Your internet connection and restart this aplication",
                        ex);
                }

            }
        }
    }
}