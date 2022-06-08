using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Codebase.Services
{
    public partial class EventBridge
    {
        //День закончился
        public delegate void DayEnd();
        public event DayEnd DayEnd_Event;
        public void OnDayEnd()
        {
            DayEnd_Event?.Invoke();
        }

        //День начался
        public delegate void DayStart();
        public event DayStart DayStart_Event;
        public void OnDayStart()
        {
            DayStart_Event?.Invoke();
        }
    }
}
