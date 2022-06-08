using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Codebase.Services
{
    public partial class EventBridge
    {
        //Понижение насыщения
        public delegate void LessHungry();
        public event LessHungry LessHungry_Event;
        public void OnLessHungry()
        {
            LessHungry_Event?.Invoke();
        }

        //Повышение жажды
        public delegate void LessThirsty();
        public event LessThirsty LessThirsty_Event;
        public void OnLessThirsty()
        {
            LessThirsty_Event?.Invoke();
        }
    }
}
