using Assets.Codebase.Models.WorldModels;
using UnityEngine;

namespace Assets.Codebase.Services
{
    //EventBridge_WorldPart  
    public partial class EventBridge
    {
        //Запрос на создание мира
        public delegate void WorldCreatingRequest(TerrainCreater terrain);
        public event WorldCreatingRequest WorldCreatingRequest_Event;
        public void OnWorldStartCreating(TerrainCreater terrain)
        {
            WorldCreatingRequest_Event?.Invoke(terrain);
        }


        //Мир создан
        public delegate void WorldCreated(TerrainMap meshMap);
        public event WorldCreated WorldCreated_Event;
        public void OnWorldCreated(TerrainMap meshMap)
        {
            WorldCreated_Event?.Invoke(meshMap);
        }


        public delegate void WorldPreFinilised();
        public event WorldPreFinilised WorldPreFinilised_Event;
        public void OnWorldPreFinilised()
        {
            WorldPreFinilised_Event?.Invoke();
        }

        //Мир завершен
        public delegate void WorldFinilised();
        public event WorldFinilised WorldFinilised_Event;
        public void OnWorldFinilised()
        {
            WorldFinilised_Event?.Invoke();
        }

        //Инфа по этапам создания мира
        public delegate void WorldCreatIngStepMsg(int step, string description);
        public event WorldCreatIngStepMsg WorldCreatIngStepMsg_Event;
        public void OnWorldCreatIngStepMsg(int step, string description)
        {
            WorldCreatIngStepMsg_Event?.Invoke(step, description);
        }

       
        public delegate void WaterCreated(WaterMap waterMap, float mapSize);
        public event WaterCreated WaterCreated_Event;
        public void OnWaterCreated(WaterMap waterMap, float mapSize)
        {
            WaterCreated_Event?.Invoke(waterMap, mapSize);
        }

        //Изменение ландшафта мира
        public delegate void WorldChanged(Vector3 pointOfChange);
        public event WorldChanged WorldChanged_Event;
        public void OnWorldChanged(Vector3 pointOfChange)
        {
            WorldChanged_Event?.Invoke(pointOfChange);
        }
    }
}
