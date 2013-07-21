using System;
using System.Collections.Generic;
using System.Threading;

namespace Thermometer.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            Action<string, Temperature> callback = async (thresholdName, temperature) =>
            {
                System.Console.WriteLine("Threshold: {0}, Temperature: {1} {2}",
                    thresholdName, temperature.ToString("C"), temperature.ToString("F"));
                Thread.Sleep(1000);
                //System.Console.ReadLine();
            };
           
            var thermometer = new Thermometer();
            var timer = new System.Timers.Timer();

            timer.Elapsed += (sender, eventArgs) =>
            {
                var guid = Guid.NewGuid().ToString().Substring(0, 6);

                var thresholds = new List<Threshold>
                {
                    new Threshold("Boiling FireOnce " + guid, Temperature.FromKind("100", TemperatureKind.Celcius), ThresholdType.FireOnce, callback),
                    new Threshold("Freezing Fire Always " + guid, Temperature.FromKind("0", TemperatureKind.Celcius), ThresholdType.FireAlways, callback),
                    new Threshold("Freezing Trending Upwards " + guid, Temperature.FromKind("0", TemperatureKind.Celcius), ThresholdType.TrendingUp, callback),     
                    new Threshold("Boling Fire Always " + guid, Temperature.FromKind("100", TemperatureKind.Celcius), ThresholdType.TrendingUp, callback)      
                };

                var subscriber = new Subscriber(thresholds);
                thermometer.Subscribe(subscriber);

                timer.Interval = 10000;
            };

            timer.Start();

            var samples = new List<double>();
            for (double i = -200; i < 200; i += 0.5)
            {
                samples.Add(i);
            }

            var random = new Random();
            for (; ;)
            {
                var indexTemp = random.Next(samples.Count);
                var temp = samples[indexTemp];

                var temperature = Temperature.FromKind(temp.ToString(), TemperatureKind.Celcius);
                System.Console.WriteLine(temperature.ToString());

                thermometer.ProvideTemperature(temperature);
            }
        }
    }
}
