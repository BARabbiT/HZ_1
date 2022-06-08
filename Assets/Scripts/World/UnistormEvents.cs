using Assets.Codebase.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UnistormEvents : MonoBehaviour
{
    UniStorm.UniStormSystem.CurrentTimeOfDayEnum prevTimeOfDay = UniStorm.UniStormSystem.CurrentTimeOfDayEnum.Day;
    bool daySubscribed = false;
    bool weatherSubscribed = false;
    bool hourSubscribed = false;
    EventBridge _eventBridge { get; set; }

    [Inject]
    public void InjectHandler(EventBridge eventBridge)
    {
        _eventBridge = eventBridge;
    }

    private void Start()
    {
        if (UniStorm.UniStormSystem.Instance != null)
        {
            if (UniStorm.UniStormSystem.Instance.OnWeatherChangeEvent != null && !weatherSubscribed)
            {
                UniStorm.UniStormSystem.Instance.OnWeatherChangeEvent.AddListener(() => WeatherChanged());
                weatherSubscribed = true;
            }
                

            if (UniStorm.UniStormSystem.Instance.OnDayChangeEvent != null && !daySubscribed)
            {
                UniStorm.UniStormSystem.Instance.OnDayChangeEvent.AddListener(() => DayChanged());
                daySubscribed = true;
            }
               
            if (UniStorm.UniStormSystem.Instance.OnHourChangeEvent != null && !hourSubscribed)
            {
                UniStorm.UniStormSystem.Instance.OnHourChangeEvent.AddListener(() => HourChanged());
                hourSubscribed = true;
            }
            
        }
    }
 
    private void OnDisable()
    {
        UniStorm.UniStormSystem.Instance.OnWeatherChangeEvent?.RemoveListener(() => WeatherChanged());
        UniStorm.UniStormSystem.Instance.OnDayChangeEvent?.RemoveListener(() => DayChanged());
        UniStorm.UniStormSystem.Instance.OnHourChangeEvent?.RemoveListener(() => HourChanged());
    }


    private void WeatherChanged()
    {
        var weather = UniStorm.UniStormSystem.Instance.CurrentWeatherType;
        _eventBridge.OnWeatherChange(weather);
        Debug.Log("Weather changed " + weather.name);
    }

    private void DayChanged()
    {
        _eventBridge.OnDayChange();
        Debug.Log("New day in this world");
    }

    private void HourChanged()
    {
        if (prevTimeOfDay != UniStorm.UniStormSystem.Instance.CurrentTimeOfDay)
        {
            prevTimeOfDay = UniStorm.UniStormSystem.Instance.CurrentTimeOfDay;
            _eventBridge.OnTypeOfDayChangeChange(UniStorm.UniStormSystem.Instance.CurrentTimeOfDay);
            Debug.Log("Time of Day Changed to " + UniStorm.UniStormSystem.Instance.CurrentTimeOfDay.ToString());
        }
        
    }


}
