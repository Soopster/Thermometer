using System;

namespace Thermometer.Console
{
    public class Threshold
    {
        private readonly string _name;
        private readonly Temperature _temperature;
        private readonly ThresholdType _thresholdType;
        private readonly Action<string, Temperature> _callback;

        public string Name
        {
            get { return _name; }
        }

        public Action<string, Temperature> Callback
        {
            get { return _callback; }
        }

        public Temperature Temperature
        {
            get { return _temperature; }
        }

        public ThresholdType ThresholdType
        {
            get { return _thresholdType; }
        }

        public Threshold(string name, Temperature temperature, ThresholdType thresholdType, Action<string, Temperature> callback)
        {
            _name = name;
            _temperature = temperature;
            _thresholdType = thresholdType;
            _callback = callback;
        }
    }
}