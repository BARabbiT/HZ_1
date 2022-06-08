using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Codebase.Services
{
    public class AliveService : IService
    {
        private EventBridge EventBridge { get; set; }

        public AliveService(EventBridge eventBridge)
        {
            EventBridge = eventBridge;
            Debug.Log(this.GetType().Name + " service started");
            Run();
        }

        public async void Run()
        {

        }
        public void GoLessHunger()
        {
            EventBridge.OnLessHungry();
        }

        public void Stop()
        {
           
        }
    }
}
