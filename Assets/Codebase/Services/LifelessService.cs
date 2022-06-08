using UnityEngine;

namespace Assets.Codebase.Services
{
    /// <summary>
    /// System responsible for lifeless object life-cycle 
    /// </summary>
    /// <seealso cref="IService" />
    public class LifelessService : IService
    {
        private EventBridge EventBridge { get; set; }

        public LifelessService(EventBridge eventBridge)
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
