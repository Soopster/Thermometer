using System;
using System.Globalization;
using System.IO;


namespace Thermometer.Console
{
    public struct Temperature : IFormattable, IEquatable<Temperature>
    {
        public double Value { get; private set; }

        public TemperatureKind Kind { get; private set;}
 
        public static Temperature TryParse(string input)
        {
            try
            {
                var returnValue = new Temperature();
                input = input.Trim(); // Remove any trailing spaces

                if (input.EndsWith("C"))
                {
                    returnValue.Value = Convert.ToDouble(input.TrimEnd('C'));
                    returnValue.Kind = TemperatureKind.Celcius;
                }
                else if (input.EndsWith("F"))
                {
                    returnValue.Value = Convert.ToDouble(input.TrimEnd('F'));
                    returnValue.Kind = TemperatureKind.Fahrenheit;
                }
                else
                {
                    throw new InvalidTemperatureException("Unknown temperature requested");
                }

                return returnValue;
            }
            catch (FormatException ex)
            {
                throw new InvalidTemperatureException("Unknown temperature requested");
            }
        }

        public static Temperature FromKind(string input, TemperatureKind temperatureKind)
        {
            try
            {
                var returnValue = new Temperature
                {
                    Value = Convert.ToDouble(input),
                    Kind = temperatureKind
                };

                return returnValue;
            }
            catch (FormatException ex)
            {
                throw new InvalidTemperatureException("Unknown temperature requested");
            }
        }

        public override string ToString()
        {
            // Default to General
            // A class that implements IFormattable must support the "G" (general) format specifier.
            return this.ToString("G", CultureInfo.CurrentCulture);
        }

        public string ToString(string format)
        {
            return this.ToString(format, CultureInfo.CurrentCulture);
        }   

        private double GetConvertedTemperature(TemperatureKind tempKind)
        {
            var returnValue = this.Value;

            switch (tempKind)
            {
                case TemperatureKind.Celcius:
                    if (this.Kind == TemperatureKind.Fahrenheit)
                    {
                        returnValue = this.Value - 32 *5 / 9;
                    }
                    break;
                case TemperatureKind.Fahrenheit:
                    if (this.Kind == TemperatureKind.Celcius)
                    {
                        returnValue = this.Value * 9 / 5 + 32;
                    }
                    break;
                default: 
                    throw new InvalidTemperatureException();
                    break;
            }

            return returnValue;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (String.IsNullOrEmpty(format))
            {
                format = "G";
            }
            if (formatProvider == null)
            {
                formatProvider = CultureInfo.CurrentCulture;
            }

            var returnValue = string.Empty;
            switch (format.ToUpperInvariant())
            {
                case "G":
                case "C":
                    returnValue = this.GetConvertedTemperature(TemperatureKind.Celcius).ToString("F2", formatProvider) + " °C";
                    break;
                case "F":
                    returnValue = this.GetConvertedTemperature(TemperatureKind.Fahrenheit).ToString("F2", formatProvider) +" °F";
                     break;
                default:
                    throw new FormatException(String.Format("The {0} format string is not supported.", format));
            }

            return returnValue;
        }

        public bool Equals(Temperature other)
        {
            return Equals(other, this);
        }

        public override bool Equals(object obj)
        {
            if(!(obj is Temperature))
            {
                return false;
            }

            var objectToCompareWith = (Temperature)obj;

            return objectToCompareWith.Kind == this.Kind && objectToCompareWith.Value == this.Value;
        }

        public static bool operator == (Temperature t1, Temperature t2)
        {
            return t1.Equals(t2);
        }

        public static bool operator !=(Temperature t1, Temperature t2)
        {
            return !(t1 == t2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}