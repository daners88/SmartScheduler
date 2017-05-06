using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeScheduler
{
    
    class Program
    {
        static void Main(string[] args)
        {
            string xs = "", ys = "", zs = "";
            string waterx = "", watery = "";

            for(int index = 0; index < 100; index++)
            {

                SchedularBll bll = new SchedularBll();
                Random rnd = new Random();
                House myHouse = new House();
                List<Day> week = new List<Day>();
                Day prevDay = null;
                for (int i = 0; i < 7; i++)
                {
                    int sky = rnd.Next(12);
                    if (sky < 3)
                    {
                        sky = 1;
                    }
                    else if (sky < 7)
                    {
                        sky = 2;
                    }
                    else
                    {
                        sky = 3;
                    }
                    Day day = new Day();
                    day.skyStatus = sky;
                    int precipitation = rnd.Next(11);
                    if (precipitation < 3 && sky == 1)
                    {
                        day.millimetersOfPrecipitation = rnd.Next(3, 12);
                    }
                    else if (precipitation < 6 && sky != 3)
                    {
                        day.millimetersOfPrecipitation = rnd.Next(1, 3);
                    }
                    else
                    {
                        day.millimetersOfPrecipitation = 0;
                    }

                    int watts;

                    if (sky == 1)
                    {
                        watts = rnd.Next(2750, 6875);
                    }
                    else if (sky == 2)
                    {
                        watts = rnd.Next(13750, 22500);
                    }
                    else
                    {
                        watts = rnd.Next(22500, 27500);
                    }

                    day.projectedWatts = watts;
                    day.name = bll.getNextDayName(prevDay);
                    prevDay = day;
                    week.Add(day);
                }
                Sprinklers mySprinklers = new Sprinklers();
                mySprinklers.millimetersOfWaterNeeded = 51;
                mySprinklers.daysRun = bll.getRandomDays(3);
                myHouse.sprinklerSystem = mySprinklers;

                Battery myBattery = new Battery();
                myBattery.maxStorage = rnd.Next(7000, 10000);
                myBattery.energy = myBattery.maxStorage;
                myHouse.homeBattery = myBattery;

                Appliance washer = new Appliance();
                washer.preferredDays = bll.getRandomDays(3);
                washer.name = "Washer";
                washer.timeUsedMinutes = 60;
                washer.wattsToUse = 1000;
                myHouse.homeAppliances.Add(washer);

                Appliance dryer = new Appliance();
                dryer.preferredDays = washer.preferredDays;
                dryer.name = "Dryer";
                dryer.timeUsedMinutes = 90;
                dryer.wattsToUse = 800;
                myHouse.homeAppliances.Add(dryer);

                Appliance dishWasher = new Appliance();
                dishWasher.preferredDays = bll.getRandomDays(4);
                dishWasher.name = "Dish Washer";
                dishWasher.timeUsedMinutes = 60;
                dishWasher.wattsToUse = 1200;
                myHouse.homeAppliances.Add(dishWasher);

                Appliance oven = new Appliance();
                oven.preferredDays = bll.getRandomDays(3);
                oven.name = "Oven";
                oven.timeUsedMinutes = 45;
                oven.wattsToUse = 1100;
                myHouse.homeAppliances.Add(oven);

                // microwave + fridge + freezer + 42" flatscreen tv 4 hrs a day + 8 60w lightbulbs on 4 hours a day + water heater + 24000 btu AC UNit
                myHouse.miscDailyUsage = 700 + 3000 + 2250 + 900 + 2400 + 10000 + 6700;
                House badHouse = (House)myHouse.Clone();
                List<Day> badWeek = week.ToList();

                House okHouse = (House)myHouse.Clone();
                List<Day> okWeek = week.ToList();
                double x, y, z;

                Console.WriteLine("A home with no renewables or smart scheduler:");
                Console.WriteLine();
                x = bll.determineNonCleanSchedule(badHouse, badWeek);
                xs += (x.ToString() + " ");

                foreach (var app in okHouse.homeAppliances)
                {
                    app.needsToRun = false;
                    app.daysSinceLastUse = 0;
                    app.ranToday = false;
                }
                foreach (var day in okWeek)
                {
                    day.appliancesToRun.Clear();
                    day.wattsPurchased = 0;
                    day.wattsSold = 0;
                    day.wattsStored = 0;
                    day.wattsUsed = 0;
                }
                okHouse.homeBattery.energy = okHouse.homeBattery.maxStorage;
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("A home with renewables but no smart scheduler:");
                Console.WriteLine();
                y = bll.determineCleanNonSmartSchedule(okHouse, okWeek);
                waterx += (y.ToString() + " ");

                foreach (var app in myHouse.homeAppliances)
                {
                    app.needsToRun = false;
                    app.daysSinceLastUse = 0;
                    app.ranToday = false;
                }
                foreach (var day in week)
                {
                    day.appliancesToRun.Clear();
                    day.wattsPurchased = 0;
                    day.wattsSold = 0;
                    day.wattsStored = 0;
                    day.wattsUsed = 0;
                }

                myHouse.homeBattery.energy = myHouse.homeBattery.maxStorage;
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("A home with renewables and a smart scheduler:");
                Console.WriteLine();
                z = bll.determineSchedule(myHouse, week);
                watery += (z.ToString() + " ");
            }



            string[] lines = { waterx, watery };
            string[] badwaterarr = waterx.Split(' ');
            string[] goodwaterarr = watery.Split(' ');
            string[] badarr = xs.Split(' ');
            string[] okarr = ys.Split(' ');
            string[] goodarr = zs.Split(' ');
            System.IO.File.WriteAllLines(@"C:\Users\Dane\Desktop\water.txt", lines);

            double badTotal = 0, goodTotal = 0;
            for(int i = 0; i < 100; i++)
            {
                double b, g;
                double.TryParse(badwaterarr[i], out b);
                double.TryParse(goodwaterarr[i], out g);
                badTotal += b;
                goodTotal += g;
            }

            Console.WriteLine(goodTotal);
            Console.WriteLine(badTotal);

            Console.ReadKey();
        }
    }
}
