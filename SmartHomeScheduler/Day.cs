using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeScheduler
{
    public class Day
    {
        public Day()
        {
            thirtyMinuteIntervals = new string[48];
            skyStatus = 3;
            appliancesToRun = new List<Appliance>();
            wattsSold = 0;
            wattsPurchased = 0;
            wattsUsed = 0;
            wattsStored = 0;
        }

        public string[] thirtyMinuteIntervals { get; set; }
        public string name { get; set; }
        public int millimetersOfPrecipitation { get; set; }
        // 1 = Cloudy, 2 = Partly Cloudy, 3 = Clear
        public int skyStatus { get; set; }
        public int projectedWatts { get; set; }
        public int wattsUsed { get; set; }
        public int wattsPurchased { get; set; }
        public int wattsSold { get; set; }
        public int wattsStored { get; set; }
        public List<Appliance> appliancesToRun { get; set; }

    }
}
