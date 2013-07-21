using System;

namespace Thermometer.Console
{
    public class InvalidTemperatureException : Exception
    {
        public InvalidTemperatureException() : base()
        {
            
        }

        public InvalidTemperatureException(string message) : base(message)
        {
            
        }
    }
}