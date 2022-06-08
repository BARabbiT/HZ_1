using System;
using UnityEngine;
using Assets.Codebase.Services;
using Zenject;
using Assets.Codebase.Interfaces.BaseAlive;

namespace Assets.Codebase.Models.Entities.Addons
{
    class FootLimb : ILimb
    {
        private EventBridge _eventBridge;
        public FootLimb(EventBridge eventBridge)
        {
            _eventBridge = eventBridge;
        }

        public int Calculate(int parametr)
        {
            return parametr;
        }

        public void Execute()
        {

        }

        public void Subscribe(BeingAI obj)
        {

        }

        public void UnSubscribe(BeingAI obj)
        {

        }
    }
}
