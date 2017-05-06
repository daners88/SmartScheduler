using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeScheduler
{
    public class Sprinklers
    {
        public Sprinklers()
        {
            daysRun = new List<Day>();
        }
        public List<Day> daysRun { get; set; }
        public int millimetersOfWaterNeeded { get; set; }

    }
}
