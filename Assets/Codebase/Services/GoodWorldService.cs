///////////////////////////////////
/// author: Vohmyakov Andrey
///////////////////////////////////

using Assets.Codebase.Generators;
using Assets.Codebase.Models.Enums;
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;

namespace Assets.Codebase.Services
{
    /// <summary>
    /// Top world business-logical service
    /// Singleton
    /// </summary>
    public class GoodWorldService : IService
    {
        private EventBridge _eventBridge { get; set; }

        public GoodWorldService(EventBridge eventBridge)
        {
            _eventBridge = eventBridge;
            Debug.Log(this.GetType().Name + " service started");

            _eventBridge.WorldCreatingRequest_Event += CreateGoodWorld;
        }


        /// <summary>
        /// Создание поверхности мира
        /// </summary>
        /// <param name="terrain"></param>
        private async void CreateGoodWorld(TerrainCreater terrain)
        {
            await UniTask.SwitchToThreadPool();

            TerrainMap _meshMap;

            _eventBridge.OnWorldCreatIngStepMsg(1, "Генерация меша");
            _meshMap = MeshTerrainGenerator.CreateMeshShapeByNoiseType_Terrain(
                terrain.width,
                terrain.mapNoise.GetGenerator(terrain.seed),
                terrain.biomesData,
                terrain.seed
            );

            await UniTask.SwitchToMainThread();

       
            //Определяем высоту воды. 
            float sandMax = terrain.biomesData.Single(b => b.Type == BiomType.Sand).Max-0.03f;
            float sandMaxHeight = sandMax * _meshMap.DeltaMinHeight;
            var minVector = terrain.GetWorldPosition(new Vector3(0, -sandMaxHeight, 0));

            await UniTask.SwitchToThreadPool();

            _eventBridge.OnWorldCreatIngStepMsg(2, "Подготовка воды");
            WaterGenerator waterGenerator = new WaterGenerator(_meshMap.Verticies, terrain.width, terrain.waterOffset, (int)minVector.y);

            //_eventBridge.OnWorldCreatIngStepMsg(3, "Формирование рек");
            //RiverGenerator riverGenerator = new(_meshMap, terrain.firstPointHeightDelta, terrain.curveHeightByRiver, terrain.curveSideHeightByRiver, terrain.curveWidthByRiver, terrain.cellSize, terrain.width, terrain.scaleStepAnalises,
            //                                    terrain.countRiver, terrain.amplitudeHieghtRiver, terrain.amplitudeWidthRiver, terrain.sandStartLvlHeight, terrain.mapNoise.amplitudeHieght);
            //_meshMap = riverGenerator.Generate();


            

            _eventBridge.OnWorldCreatIngStepMsg(3, "Создание воды");
            var waterMap = waterGenerator.Generate(new Vector3[0]);

            await UniTask.SwitchToMainThread();

            _eventBridge.OnWaterCreated(waterMap, terrain.width);

            _eventBridge.OnWorldCreatIngStepMsg(4, "Создание мира");
            _eventBridge.OnWorldCreated(_meshMap);
            Debug.Log("MinHeight: " + _meshMap.DeltaMinHeight + "\n\r" +
                      "MaxHeight: " + _meshMap.DeltaMaxHeight);
 

        }
 

        public void Run()
        {
           
        }

        public void Stop()
        {
           
        }

    }
}
