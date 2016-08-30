using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LondonBus.Models
{
    public class Ariving
    {
        string _timeToStation;
        public string LineName { get; set; }

        public string TimeToStation
        {
            get { return _timeToStation; }
            set
            {
                _timeToStation = String.Format("{0} min. {1} sec.", Int32.Parse(value)/60, (Int32.Parse(value) - (60*(Int32.Parse(value)/60))));
            } 
                    
        }
    }
}