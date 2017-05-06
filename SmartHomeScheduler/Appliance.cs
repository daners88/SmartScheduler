using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeScheduler
{
    public class Appliance
    {
        public Appliance()
        {
            preferredDays = new List<Day>();
            needsToRun = false;
            ranToday = false;
            daysSinceLastUse = 0;
        }

        public string name { get; set; }
        public int wattsToUse { get; set; }
        public int timeUsedMinutes { get; set; }
        public List<Day> preferredDays { get; set; }
        public bool needsToRun { get; set; }
        public bool ranToday { get; set; }
        public int daysSinceLastUse { get; set; }
    }
}
