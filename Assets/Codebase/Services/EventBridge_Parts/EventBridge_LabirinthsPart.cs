using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Codebase.Services
{
    public partial class EventBridge
    {
        //Запрос на создание мира
        public delegate void LabirinthsCreatingRequest(LabirinthsCreater terrain);
        public event LabirinthsCreatingRequest LabirinthsCreatingRequest_Event;
        public void OnLabirinthsStartCreating(LabirinthsCreater terrain)
        {
            LabirinthsCreatingRequest_Event?.Invoke(terrain);
        }


        //Мир создан
        public delegate void LabirinthsCreated(LabirinthsMap meshMap);
        public event LabirinthsCreated LabirinthsCreated_Event;
        public void OnLabirinthsCreated(LabirinthsMap meshMap)
        {
            LabirinthsCreated_Event?.Invoke(meshMap);
        }


    }
}
