using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thermometer.Console
{
    public class Thermometer
    {
        private readonly ConcurrentBag<Temperature> _temperatures;
        private readonly ConcurrentBag<ISubsciber> _subscribers;
        private ConcurrentBag<Guid> _fireOnceSubscibers;

        public Temperature CurrentTemperature { get; private set; }

        public ReadOnlyCollection<ISubsciber> Subscribers
        {
            get
            {
                return _subscribers.ToList().AsReadOnly();
            }
        }

        public Thermometer()
        {
            this._temperatures = new ConcurrentBag<Temperature>();
            this._subscribers = new ConcurrentBag<ISubsciber>();
            this._fireOnceSubscibers = new ConcurrentBag<Guid>();
        }

        public void ProvideTemperature(Temperature temperature)
        {
            this.CurrentTemperature = temperature;

            foreach (var subscriber in _subscribers)
            {
                foreach (var threshold in subscriber.Thresholds)
                {
                    if (CurrentTemperature.Equals(threshold.Temperature))
                    {
                        bool fireThreshold = false;
                        Temperature lastTemperature;

                        switch (threshold.ThresholdType)
                        {
                            case ThresholdType.FireAlways:
                                fireThreshold = true;
                                break;
                            case ThresholdType.FireOnce:
                                if (!this._fireOnceSubscibers.Contains(subscriber.ID))
                                {
                                    _fireOnceSubscibers.Add(subscriber.ID);
                                    fireThreshold = true;
                                }
                                break;
                            case ThresholdType.TrendingDown:
                                if (_temperatures.TryPeek(out lastTemperature))
                                {
                                    if (CurrentTemperature.Value < lastTemperature.Value)
                                    {
                                        fireThreshold = true;
                                    }
                                }
                                else
                                {
                                    fireThreshold = true;
                                }
                               break;
                            case ThresholdType.TrendingUp:
                                if (_temperatures.TryPeek(out lastTemperature))
                                {
                                    if (CurrentTemperature.Value > lastTemperature.Value)
                                    {
                                        fireThreshold = true;
                                    }
                                }
                                else
                                {
                                    fireThreshold = true;
                                }
                               break;
                            default:
                                break;
                        }

                        if (fireThreshold)
                        {
                            threshold.Callback(threshold.Name, CurrentTemperature);
                        }
                    }
                }
            }

            this._temperatures.Add(temperature);
        }

        public void Subscribe(ISubsciber subscriber)
        {
            if(!this._subscribers.Contains(subscriber))
            {
                this._subscribers.Add(subscriber);
            }
        }
    }
}
