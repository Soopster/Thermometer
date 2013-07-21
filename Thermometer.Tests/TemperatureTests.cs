using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thermometer.Console;

namespace Thermometer.Tests
{
    [TestClass]
    public class TemperatureTests
    {
        protected Temperature _sut;

        [TestInitialize]
        public void Init()
        {
            _sut = Temperature.FromKind("10", TemperatureKind.Celcius);
        }

        [TestClass]
        public class TheToStringMethod : TemperatureTests
        {
            [TestMethod]
            public void CanRequestAFormat()
            {
                var sut = Temperature.FromKind("10", TemperatureKind.Celcius);

                var cTemp = sut.ToString("C");
                Assert.IsTrue(cTemp == "10.00 °C");

                var fTemp = sut.ToString("F");
                Assert.IsTrue(fTemp == "50.00 °F");

                var defaultTemp = sut.ToString();
                Assert.IsTrue(defaultTemp == "10.00 °C");
            }
    
        }

        [TestClass]
        public class TheTryParseMethod : TemperatureTests
        {
            [TestMethod]
            public void CanAcceptCelciusValues()
            {
                string input = "10 C";
                var sut = Temperature.TryParse(input);

                Assert.IsTrue(sut.Kind == TemperatureKind.Celcius);
                Assert.IsTrue(sut.Value == 10.00);
            }

            [TestMethod]
            public void CanAcceptFahrenheitValue()
            {
                string input = "10 F";
                var sut = Temperature.TryParse(input);

                Assert.IsTrue(sut.Kind == TemperatureKind.Fahrenheit);
                Assert.IsTrue(sut.Value == 10.00);
            }

            [TestMethod]
            [ExpectedException(typeof(InvalidTemperatureException))]
            public void WillThrowExceptionIfAnInvalidTemperatureIsEntered()
            {
                var errorThrown = false;
                try
                {
                    string input = "10 K";
                    var sut = Temperature.TryParse(input);
                }
                catch (Exception InvalidTemperatureException)
                {
                    errorThrown = true;
                    throw;
                }

                Assert.IsTrue(errorThrown, "Expected invalid Temp Exception");
            }
        }

        [TestClass]
        public class TheFromKindMethod : TemperatureTests
        {
            [TestMethod]
            public void CanCreateACelciusTemperature()
            {
                string input = "10";
                var sut = Temperature.FromKind(input, TemperatureKind.Celcius);

                Assert.IsTrue(sut.Kind == TemperatureKind.Celcius);
                Assert.IsTrue(sut.Value == 10.00);
            }

            [TestMethod]
            public void CanCreateAFahrenheitTemperature()
            {
                string input = "10";
                var sut = Temperature.FromKind(input, TemperatureKind.Fahrenheit);

                Assert.IsTrue(sut.Kind == TemperatureKind.Fahrenheit);
                Assert.IsTrue(sut.Value == 10.00);
            }
        }
    }
}
