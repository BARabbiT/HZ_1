using System;
using UnityEngine;
using Assets.Codebase.Services;
using Zenject;
using Assets.Codebase.Interfaces.BaseAlive;

namespace Assets.Codebase.Models.Entities.Addons
{
    class StandartBody : IBody
    {
        private EventBridge _eventBridge;
        public StandartBody(EventBridge eventBridge)
        {
            _eventBridge = eventBridge;
        }

        public int CalculateAge(int parametr)
        {
            throw new NotImplementedException();
        }

        public int CalculateHealth(int parametr)
        {
            throw new NotImplementedException();
        }

        public int CalculateMetaboly(int parametr)
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            
        }

        public void Subscribe(BeingAI obj)
        {
            _eventBridge.DayEnd_Event+=obj.DayEnd;
            _eventBridge.DayStart_Event += obj.DayStart;
        }

        public void UnSubscribe(BeingAI obj)
        {
            _eventBridge.DayEnd_Event -= obj.DayEnd;
            _eventBridge.DayStart_Event -= obj.DayStart;
        }
    }
}
