using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Codebase.Services
{
    /// <summary>
    /// System responsible for building, touching and other ineractive staff
    /// </summary>
    /// <seealso cref="IService" />
    public class InteractiveService : IService
    {
        private EventBridge EventBridge { get; set; }
        public InteractiveService(EventBridge eventBridge)
        {
            EventBridge = eventBridge;
            
            Debug.Log(this.GetType().Name + " service started");
            Run();
        }

        public void Run()
        {
        }

        public void Stop()
        {
           
        }
    }
}
