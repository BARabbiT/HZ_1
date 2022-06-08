using Assets.Codebase.Generators;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Codebase.Services
{
    public class LabirinthsService : IService
    {
        private EventBridge _eventBridge { get; set; }

        public LabirinthsService(EventBridge eventBridge)
        {
            _eventBridge = eventBridge;
            Debug.Log(this.GetType().Name + " service started");

 
            _eventBridge.LabirinthsCreatingRequest_Event += _eventBridge_LabirinthsCreatingRequest_Event;
        }



        /// <summary>
        /// Создание лабиринтов
        /// </summary>
        /// <param name="labirinths"></param>
        private async void _eventBridge_LabirinthsCreatingRequest_Event(LabirinthsCreater labirinths)
        {
            await UniTask.SwitchToThreadPool();

            LabirinthsMap _meshMap;

            DualContourGenerator dualContourGenerator = new DualContourGenerator(labirinths.width, labirinths.height, 
                                                                                 labirinths.terrainSurface, labirinths.flatShaded, 
                                                                                 labirinths.smoothTerrain, labirinths.mapNoise.GetGenerator(labirinths.seed));

            _meshMap = dualContourGenerator.Generate();


            await UniTask.SwitchToMainThread();
            _eventBridge.OnLabirinthsCreated(_meshMap);
 
        }
 

        public void Run()
        {
           
        }

        public void Stop()
        {
           
        }


    }
}
