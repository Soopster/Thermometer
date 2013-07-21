using System;
using System.Collections.Generic;

namespace Thermometer.Console
{
    public interface ISubsciber
    {
        List<Threshold> Thresholds { get; }
        Guid ID { get; }
    }
}