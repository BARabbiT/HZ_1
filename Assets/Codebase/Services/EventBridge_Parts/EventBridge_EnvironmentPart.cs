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
        public delegate void WeatherChange(UniStorm.WeatherType typeWeather);
        /// <summary>
        /// Происходит при смене погоды
        /// </summary>
        public event WeatherChange WeatherChange_Event;
        public void OnWeatherChange(UniStorm.WeatherType typeWeather)
        {
            WeatherChange_Event?.Invoke(typeWeather);
        }

       
        public delegate void DayChange();
        /// <summary>
        /// Происходит при смене дня
        /// </summary>
        /// <param name="timeOfDayEnum"></param>
        public event DayChange DayChange_Event;
        public void OnDayChange()
        {
            DayChange_Event?.Invoke();
        }

       
        public delegate void TypeOfDayChange(UniStorm.UniStormSystem.CurrentTimeOfDayEnum timeOfDayEnum);
        /// <summary>
        /// Происходит при смене типа в течение дня
        /// </summary>
        /// <param name="timeOfDayEnum"></param>
        public event TypeOfDayChange TypeOfDayChange_Event;
        public void OnTypeOfDayChangeChange(UniStorm.UniStormSystem.CurrentTimeOfDayEnum timeOfDayEnum)
        {
            TypeOfDayChange_Event?.Invoke(timeOfDayEnum);
        }
    }
}
