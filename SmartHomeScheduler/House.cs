using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeScheduler
{
    public class House : ICloneable
    {
        public House()
        {
            homeAppliances = new List<Appliance>();
            sprinklerSystem = new Sprinklers();
            homeBattery = new Battery();
            washerRan = false;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public List<Appliance> homeAppliances { get; set; }
        public bool washerRan { get; set; }
        public Sprinklers sprinklerSystem { get; set; }
        public int maxEnergyGeneration { get; set; }
        public Battery homeBattery { get; set; }
        public int miscDailyUsage { get; set; }
    }
}
