using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thermometer.Console;

namespace Thermometer.Tests
{
    [TestClass]
    public class ThermometerTests
    {
        protected Console.Thermometer _sut;

        [TestInitialize]
        public void Init()
        {
            _sut = new Console.Thermometer();
        }

        [TestClass]
        public class TheSubscribeMethod : ThermometerTests
        {
            [TestMethod]
            public void CanAcceptASubscriber()
            {
                ISubsciber subsciber = new Subscriber();
                _sut.Subscribe(subsciber);

                Assert.IsTrue(_sut.Subscribers.Count == 1);
            }

            [TestMethod]
            public void WillOnlyAcceptTheSameSubcriberOnce()
            {
                ISubsciber subscriber = new Subscriber();
                _sut.Subscribe(subscriber);

                Assert.IsTrue(_sut.Subscribers.Count == 1);

                _sut.Subscribe(subscriber);
                Assert.IsFalse(_sut.Subscribers.Count == 2);
            }
        }

        [TestClass]
        public class TheProvideTemperatureMethod : ThermometerTests
        {
            [TestMethod]
            public void WillSetTheCurrentTemperature()
            {
                var newTemp = Temperature.FromKind("10", TemperatureKind.Celcius);
                _sut.ProvideTemperature(newTemp);

                Assert.IsTrue(_sut.CurrentTemperature.Equals(newTemp));
            }

            [TestMethod]
            public void WillAlwaysNotifiyInterestedSubscribersWhenAThresholdIsMet()
            {
                ISubsciber subscriber = new Subscriber();
                var thresholdFired = false;
                var timesFired = 0;

                subscriber.Thresholds.Add(new Threshold("Test", Temperature.FromKind("10", TemperatureKind.Celcius), ThresholdType.FireAlways,
                    (thresholdName, temperature) =>
                    {
                        Assert.IsTrue(thresholdName == "Test");
                        Assert.IsTrue(temperature.Value == 10.00);
                        Assert.IsTrue(temperature.Kind == TemperatureKind.Celcius);

                        thresholdFired = true;
                        ++timesFired;
                    }));

                _sut.Subscribe(subscriber); 
                _sut.ProvideTemperature(Temperature.FromKind("10", TemperatureKind.Celcius));
                _sut.ProvideTemperature(Temperature.FromKind("10", TemperatureKind.Celcius));

                Assert.IsTrue(thresholdFired);
                Assert.IsTrue(timesFired == 2);
            }

            [TestMethod]
            public void WillNotifiyInterestedSubscribersWhenAFireOnceThresholdIsMet()
            {
                ISubsciber subscriber = new Subscriber();
                var thresholdFired = false;
                var timesFired = 0;

                subscriber.Thresholds.Add(new Threshold("Test", Temperature.FromKind("10", TemperatureKind.Celcius), ThresholdType.FireOnce,
                    (thresholdName, temperature) =>
                    {
                        Assert.IsTrue(thresholdName == "Test");
                        Assert.IsTrue(temperature.Value == 10.00);
                        Assert.IsTrue(temperature.Kind == TemperatureKind.Celcius);

                        thresholdFired = true;
                        ++timesFired;
                    }));

                _sut.Subscribe(subscriber);
                _sut.ProvideTemperature(Temperature.FromKind("10", TemperatureKind.Celcius));
                _sut.ProvideTemperature(Temperature.FromKind("10", TemperatureKind.Celcius));

                Assert.IsTrue(thresholdFired);
                Assert.IsTrue(timesFired == 1);
            }

            [TestMethod]
            public void WillNotifiyInterestedSubscribersWhenWhenATrendingDownThresholdIsMet()
            {
                ISubsciber subscriber = new Subscriber();
                var thresholdFired = false;
                var timesFired = 0;
                _sut.ProvideTemperature(Temperature.FromKind("10", TemperatureKind.Celcius));

                subscriber.Thresholds.Add(new Threshold("Freezing", Temperature.FromKind("0", TemperatureKind.Celcius), ThresholdType.TrendingDown,
                    (thresholdName, temperature) =>
                    {
                        Assert.IsTrue(thresholdName == "Freezing");
                        Assert.IsTrue(temperature.Value == 0.0);
                        Assert.IsTrue(temperature.Kind == TemperatureKind.Celcius);

                        thresholdFired = true;
                        ++timesFired;
                    }));

                _sut.Subscribe(subscriber);
                _sut.ProvideTemperature(Temperature.FromKind("0", TemperatureKind.Celcius));
                
                _sut.ProvideTemperature(Temperature.FromKind("-1", TemperatureKind.Celcius));
                _sut.ProvideTemperature(Temperature.FromKind("0", TemperatureKind.Celcius));

                _sut.ProvideTemperature(Temperature.FromKind("30", TemperatureKind.Celcius));
                _sut.ProvideTemperature(Temperature.FromKind("0", TemperatureKind.Celcius));

                Assert.IsTrue(thresholdFired);
                Assert.IsTrue(timesFired == 2);
            }

            [TestMethod]
            public void WillNotifiyInterestedSubscribersWhenWhenATrendingUpThresholdIsMet()
            {
                ISubsciber subscriber = new Subscriber();
                var thresholdFired = false;
                var timesFired = 0;
                _sut.ProvideTemperature(Temperature.FromKind("10", TemperatureKind.Celcius));

                subscriber.Thresholds.Add(new Threshold("Boiling", Temperature.FromKind("100", TemperatureKind.Celcius), ThresholdType.TrendingUp,
                    (thresholdName, temperature) =>
                    {
                        Assert.IsTrue(thresholdName == "Boiling");
                        Assert.IsTrue(temperature.Value == 100.00);
                        Assert.IsTrue(temperature.Kind == TemperatureKind.Celcius);

                        thresholdFired = true;
                        ++timesFired;
                    }));

                _sut.Subscribe(subscriber);
                _sut.ProvideTemperature(Temperature.FromKind("100", TemperatureKind.Celcius));

                _sut.ProvideTemperature(Temperature.FromKind("-1", TemperatureKind.Celcius));
                _sut.ProvideTemperature(Temperature.FromKind("100", TemperatureKind.Celcius));

                _sut.ProvideTemperature(Temperature.FromKind("110", TemperatureKind.Celcius));
                _sut.ProvideTemperature(Temperature.FromKind("100", TemperatureKind.Celcius));

                Assert.IsTrue(thresholdFired);
                Assert.IsTrue(timesFired == 2);
            }
        }
    }
}
