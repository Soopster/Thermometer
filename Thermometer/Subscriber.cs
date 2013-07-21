using System;
using System.Collections.Generic;

namespace Thermometer.Console
{
    public class Subscriber : ISubsciber
    {
        public List<Threshold> Thresholds { get; private set; }
        public Guid ID { get; private set; }

        public Subscriber() : this(new List<Threshold>())
        {
        }

        public Subscriber(List<Threshold> thresholds)
        {
            this.Thresholds = thresholds;
            this.ID = Guid.NewGuid();
        }
    }
}