using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeScheduler
{
    public class SchedularBll
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }

        public List<Day> getRandomDays(int numDays)
        {
            List<Day> myDays = new List<Day>();
            while (myDays.Count < numDays)
            {
                int rand = RandomNumber(1, 8);
                Day day = new Day();
                switch (rand)
                {
                    case 1:
                        day.name = "Sunday";
                        break;
                    case 2:
                        day.name = "Monday";
                        break;
                    case 3:
                        day.name = "Tuesday";
                        break;
                    case 4:
                        day.name = "Wednesday";
                        break;
                    case 5:
                        day.name = "Thursday";
                        break;
                    case 6:
                        day.name = "Friday";
                        break;
                    case 7:
                        day.name = "Saturday";
                        break;
                    default:
                        day.name = "Sunday";
                        break;
                }

                if (myDays.Where(d => d.name == day.name).FirstOrDefault() == null)
                {
                    myDays.Add(day);
                }
            }

            return myDays;
        }

        public string getNextDayName(Day previousDay)
        {
            string name;
            if (previousDay != null)
            {
                switch (previousDay.name)
                {
                    case "Sunday":
                        name = "Monday";
                        break;
                    case "Monday":
                        name = "Tuesday";
                        break;
                    case "Tuesday":
                        name = "Wednesday";
                        break;
                    case "Wednesday":
                        name = "Thursday";
                        break;
                    case "Thursday":
                        name = "Friday";
                        break;
                    case "Friday":
                        name = "Saturday";
                        break;
                    case "Saturday":
                        name = "Sunday";
                        break;
                    default:
                        name = "Sunday";
                        break;
                }
            }
            else
            {
                name = "Sunday";
            }
            return name;
        }

        public int determineNonCleanSchedule(House myHouse, List<Day> thisWeek)
        {
            int totalWattsSold = 0;
            int totalWattsPurchased = 0;
            int totalWattsStored = 0;
            int totalWattsUsed = 0;
            int millimetersOfWaterSaved = 0;
            foreach (var day in thisWeek)
            {
                day.wattsUsed += myHouse.miscDailyUsage;
                foreach (var app in myHouse.homeAppliances)
                {
                    if (app.preferredDays.Where(d => d.name == day.name).FirstOrDefault() != null)
                    {
                        day.appliancesToRun.Add(app);
                        day.wattsUsed += app.wattsToUse;
                    }
                }
                day.wattsSold = 0;
                day.wattsStored = 0;

                totalWattsSold += day.wattsSold;
                totalWattsPurchased += day.wattsUsed;
                totalWattsStored += day.wattsStored;
                totalWattsUsed += day.wattsUsed;
            }
            Console.WriteLine("Total Energy Sold: " + totalWattsSold);
            Console.WriteLine("Total Energy Bought: " + totalWattsPurchased + ", $" + (double)totalWattsPurchased / 1000 * 0.0817);
            Console.WriteLine("Total Energy Stored in Battery: " + totalWattsStored);
            Console.WriteLine("Total Energy Use: " + totalWattsUsed + " watts");
            //average american yard is 2,500 sq ft, or 762,000 sq mm
            Console.WriteLine("Millimeters of Water Saved: " + millimetersOfWaterSaved + " per square mm, " + millimetersOfWaterSaved * 762000 + " mm of water saved total");
            Console.WriteLine("Liters of Water Saved: " + (((double)millimetersOfWaterSaved * 762000) / 1000) / 1000);
            foreach (var day in thisWeek)
            {
                Console.WriteLine();
                Console.WriteLine(day.name);
                string skyStatus;
                if (day.skyStatus == 1)
                {
                    skyStatus = "Cloudy";
                }
                else if (day.skyStatus == 2)
                {
                    skyStatus = "Partly Cloudy";
                }
                else
                {
                    skyStatus = "Clear Skies";
                }
                Console.WriteLine("Weather Conditions: " + skyStatus);

                Console.WriteLine("Suggested Schedule: ");
                foreach (var app in day.appliancesToRun)
                {
                    Console.WriteLine("     Run the " + app.name);
                }
                Console.WriteLine();
            }
            return totalWattsPurchased;
        }

        public double determineCleanNonSmartSchedule(House myHouse, List<Day> thisWeek)
        {
            int totalWattsSold = 0;
            int totalWattsPurchased = 0;
            int totalWattsStored = 0;
            int totalWattsUsed = 0;
            int millimetersOfWaterSaved = 0;
            foreach (var day in thisWeek)
            {
                int wattsRemaining = day.projectedWatts + myHouse.homeBattery.energy;
                wattsRemaining -= myHouse.miscDailyUsage;
                day.wattsUsed += myHouse.miscDailyUsage;
                foreach (var app in myHouse.homeAppliances)
                {
                    if (app.preferredDays.Where(d => d.name == day.name).FirstOrDefault() != null)
                    {
                        day.appliancesToRun.Add(app);
                        wattsRemaining -= app.wattsToUse;
                        day.wattsUsed += app.wattsToUse;
                    }
                }
                if(wattsRemaining < 0)
                {
                    int boughtAmount = Math.Abs(wattsRemaining) + 1;
                    day.wattsPurchased += boughtAmount;
                }
                int temp = wattsRemaining - myHouse.homeBattery.maxStorage;
                if (temp > 0)
                {
                    myHouse.homeBattery.energy = myHouse.homeBattery.maxStorage;
                    day.wattsStored = myHouse.homeBattery.maxStorage;
                }
                else if (wattsRemaining >= 0)
                {
                    myHouse.homeBattery.energy = wattsRemaining;
                    day.wattsStored = wattsRemaining;
                }
                else
                {
                    myHouse.homeBattery.energy = 0;
                    day.wattsStored = 0;
                }
                day.wattsSold = 0;
                totalWattsSold += day.wattsSold;
                totalWattsPurchased += day.wattsPurchased;
                totalWattsStored += day.wattsStored;
                totalWattsUsed += day.wattsUsed;
            }
            Console.WriteLine("Total Energy Sold: " + totalWattsSold);
            Console.WriteLine("Total Energy Bought: " + totalWattsPurchased + ", $" + (double)totalWattsPurchased / 1000 * 0.08177);
            Console.WriteLine("Total Energy Stored in Battery: " + totalWattsStored);
            Console.WriteLine("Total Energy Use: " + totalWattsUsed + " watts");
            //average american yard is 2,500 sq ft, or 762,000 sq mm
            Console.WriteLine("Millimeters of Water Saved: " + millimetersOfWaterSaved + " per square mm, " + millimetersOfWaterSaved * 762000 + " mm of water saved total");
            Console.WriteLine("Liters of Water Saved: " + (((double)millimetersOfWaterSaved * 762000) / 1000) / 1000);
            foreach (var day in thisWeek)
            {
                Console.WriteLine();
                Console.WriteLine(day.name);
                string skyStatus;
                if (day.skyStatus == 1)
                {
                    skyStatus = "Cloudy";
                }
                else if (day.skyStatus == 2)
                {
                    skyStatus = "Partly Cloudy";
                }
                else
                {
                    skyStatus = "Clear Skies";
                }
                Console.WriteLine("Weather Conditions: " + skyStatus);

                Console.WriteLine("Suggested Schedule: ");
                foreach (var app in day.appliancesToRun)
                {
                    Console.WriteLine("     Run the " + app.name);
                }
                Console.WriteLine();
            }
            return (((double)millimetersOfWaterSaved * 762000) / 1000) / 1000;
        }


        public double determineSchedule(House myHouse, List<Day> thisWeek)
        {
            int totalWattsSold = 0;
            int totalWattsPurchased = 0;
            int totalWattsStored = 0;
            int totalWattsUsed = 0;
            int millimetersOfWaterSaved = 0;
            int millimetersOfWaterThisWeek = 0;
            int sprinklingDay = 1;
            foreach (var day in thisWeek)
            {
                myHouse.washerRan = false;
                int wattsRemaining = day.projectedWatts + myHouse.homeBattery.energy;
                wattsRemaining -= myHouse.miscDailyUsage;
                day.wattsUsed += myHouse.miscDailyUsage;
                foreach (var app in myHouse.homeAppliances)
                {
                    if(app.preferredDays.Where(d => d.name == day.name).FirstOrDefault() != null || app.needsToRun)
                    {
                        if(app.name == "Dryer" && !myHouse.washerRan)
                        {
                            //dryer is only needed if the washer ran
                        }
                        else if(wattsRemaining - app.wattsToUse > 0)
                        {
                            day.appliancesToRun.Add(app);
                            wattsRemaining -= app.wattsToUse;
                            day.wattsUsed += app.wattsToUse;
                            if(app.name == "Washer")
                            {
                                myHouse.washerRan = true;
                            }
                            app.daysSinceLastUse = -1;
                        }
                        else if (app.needsToRun || (app.name == "Dryer" && myHouse.washerRan))
                        {
                            day.appliancesToRun.Add(app);
                            int toPurchase = app.wattsToUse - wattsRemaining;
                            wattsRemaining = 0;
                            day.wattsPurchased += toPurchase;
                            day.wattsUsed += app.wattsToUse;
                            if (app.name == "Washer")
                            {
                                myHouse.washerRan = true;
                            }
                            else if(app.name == "Dryer")
                            {
                                myHouse.washerRan = false;
                            }
                            
                            if (app.needsToRun)
                            {
                                app.needsToRun = false;
                            }
                            app.daysSinceLastUse = -1;
                        }
                        else
                        {
                            string nextDay = getNextDayName(day);
                            if(app.preferredDays.Where(d=>d.name == nextDay).FirstOrDefault() == null)
                            {
                                Day tempDay = new Day();
                                tempDay.name = nextDay;
                                app.preferredDays.Add(tempDay);
                            }
                        }
                    }
                    app.daysSinceLastUse++;
                    if (app.daysSinceLastUse > 2)
                    {
                        app.needsToRun = true;
                    }
                }
                day.wattsSold = 0;
                int temp = wattsRemaining - myHouse.homeBattery.maxStorage;
                if(temp > 0)
                {
                    day.wattsSold = temp;
                    myHouse.homeBattery.energy = myHouse.homeBattery.maxStorage;
                    day.wattsStored = myHouse.homeBattery.maxStorage;
                }
                else if(wattsRemaining >= 0)
                {
                    myHouse.homeBattery.energy = wattsRemaining;
                    day.wattsStored = wattsRemaining;
                }
                else
                {
                    myHouse.homeBattery.energy = 0;
                    day.wattsStored = 0;
                }
                totalWattsSold += day.wattsSold;
                totalWattsPurchased += day.wattsPurchased;
                totalWattsStored += day.wattsStored;
                totalWattsUsed += day.wattsUsed;
                millimetersOfWaterThisWeek += day.millimetersOfPrecipitation;

                if (myHouse.sprinklerSystem.daysRun.Where(d => d.name == day.name).FirstOrDefault() != null)
                {
                    if(sprinklingDay == 1)
                    {
                        if (millimetersOfWaterThisWeek <= (myHouse.sprinklerSystem.millimetersOfWaterNeeded / 3))
                        {
                            int holder = millimetersOfWaterSaved;
                            millimetersOfWaterSaved += millimetersOfWaterThisWeek;
                            millimetersOfWaterThisWeek += ((myHouse.sprinklerSystem.millimetersOfWaterNeeded / 3) - holder);
                        }
                        else
                        {
                            millimetersOfWaterSaved += (myHouse.sprinklerSystem.millimetersOfWaterNeeded / 3);
                        }
                        sprinklingDay++;
                    }
                    else if(sprinklingDay == 2)
                    {
                        if (millimetersOfWaterThisWeek <= (myHouse.sprinklerSystem.millimetersOfWaterNeeded / 3) * 2)
                        {
                            int holder = (millimetersOfWaterThisWeek - (myHouse.sprinklerSystem.millimetersOfWaterNeeded / 3));
                            millimetersOfWaterSaved += (millimetersOfWaterThisWeek - (myHouse.sprinklerSystem.millimetersOfWaterNeeded / 3));
                            millimetersOfWaterThisWeek += ((myHouse.sprinklerSystem.millimetersOfWaterNeeded / 3) - holder);
                        }
                        else
                        {
                            millimetersOfWaterSaved += (myHouse.sprinklerSystem.millimetersOfWaterNeeded / 3);
                        }
                        sprinklingDay++;
                    }
                    else if(sprinklingDay == 3)
                    {
                        if (millimetersOfWaterThisWeek <= myHouse.sprinklerSystem.millimetersOfWaterNeeded)
                        {
                            int holder = (millimetersOfWaterThisWeek - ((myHouse.sprinklerSystem.millimetersOfWaterNeeded / 3) * 2));
                            millimetersOfWaterSaved += (millimetersOfWaterThisWeek - ((myHouse.sprinklerSystem.millimetersOfWaterNeeded / 3) * 2));
                            millimetersOfWaterThisWeek += ((myHouse.sprinklerSystem.millimetersOfWaterNeeded / 3) - holder);
                        }
                        else
                        {
                            millimetersOfWaterSaved += (myHouse.sprinklerSystem.millimetersOfWaterNeeded / 3);
                        }
                        sprinklingDay++;
                    }

                }
            }
            Console.WriteLine("Total Energy Sold: " + totalWattsSold + ", $" + (double)totalWattsSold / 1000 * 0.0817);
            Console.WriteLine("Total Energy Bought: " + totalWattsPurchased +", $" + (double)totalWattsPurchased / 1000 * 0.0817);
            Console.WriteLine("Total Energy Stored in Battery: " + totalWattsStored);
            Console.WriteLine("Total Energy Use: " + totalWattsUsed + " watts");
            //average american yard is 2,500 sq ft, or 762,000 sq mm
            Console.WriteLine("Millimeters of Water Saved: " + millimetersOfWaterSaved + " per square mm, " + millimetersOfWaterSaved * 762000 + " mm of water saved total");
            Console.WriteLine("Liters of Water Saved: " + (((double)millimetersOfWaterSaved * 762000) / 1000) / 1000);
            foreach (var day in thisWeek)
            {
                Console.WriteLine();
                Console.WriteLine(day.name);
                string skyStatus;
                if(day.skyStatus == 1)
                {
                    skyStatus = "Cloudy";
                }
                else if(day.skyStatus == 2)
                {
                    skyStatus = "Partly Cloudy";
                }
                else
                {
                    skyStatus = "Clear Skies";
                }
                Console.WriteLine("Weather Conditions: " + skyStatus);

                Console.WriteLine("Suggested Schedule: ");
                foreach (var app in day.appliancesToRun)
                {
                    Console.WriteLine("     Run the " + app.name);
                }
            }
            return (((double)millimetersOfWaterSaved * 762000) / 1000) / 1000;
        }
    }
}
