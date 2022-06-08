using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Codebase.Services
{
    /// <summary>
    /// Base event router between components and services.
    /// Singleton
    /// </summary>
    public partial class EventBridge : IService
    {
        public EventBridge()
        {
            Debug.Log(this.GetType().Name + " service started");
        }

        public void Run()
        {
        }

        public void Stop()
        {
        }
    }
}
