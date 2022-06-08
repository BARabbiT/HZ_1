using SensorToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Codebase.Models.Entities
{
    public class RaySensorWrapper
    {
        private RaySensor _raySensor;
        private BeingEventBridge _beingEventBridge;
        public RaySensorWrapper(RaySensor raySensor, BeingEventBridge eventBridge)
        {
            _raySensor = raySensor;
            _beingEventBridge = eventBridge;

            _raySensor.OnSensorUpdate += OnSenseSome;
        }

        private void OnSenseSome()
        {
            var appleList = _raySensor.GetDetected().FindAll(go => go.name == "Apple");
            if (appleList != null)
            {
                foreach(var apple in appleList)
                {
                    _beingEventBridge.OnSeeEat(apple.gameObject.transform.position);
                }
            }
        }
    }
}
