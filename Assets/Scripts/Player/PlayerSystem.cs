using Assets.Codebase.Services;
using Assets.Codebase.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class PlayerSystem : MonoBehaviour
{
    public int SecondsToRequest = 3;
    public GameObject terrainObject;
    public GameObject biomeInfo;
    public GameObject weatherInfo;
    public GameObject dayPeriodInfo;



    TerrainCreater terrainCreater;
    TextMeshPro biomeText;
    TextMeshPro weatherText;
    TextMeshPro dayPeriodText;
    EventBridge _eventBridge { get; set; }
    

    [Inject]
    public void InjectHandler(EventBridge eventBridge)
    {
        _eventBridge = eventBridge;
    }

    // Start is called before the first frame update
    void Awake()
    {

        terrainCreater = terrainObject.GetComponent<TerrainCreater>();
        Observable.Interval(new(0, 0, SecondsToRequest)).Subscribe(x => WhereAmI()).AddTo(this);

        biomeText = biomeInfo.GetComponent<TextMeshPro>();
        weatherText = weatherInfo.GetComponent<TextMeshPro>();
        dayPeriodText = dayPeriodInfo.GetComponent<TextMeshPro>();

        _eventBridge.WorldFinilised_Event += _eventBridge_WorldFinilised_Event;
       
    }

    private void _eventBridge_WorldFinilised_Event()
    {
        var pos = this.transform.position;
        this.transform.SetPositionAndRotation(new Vector3(pos.x, 100, pos.z), this.transform.rotation); //ћожно написать метод определени€ позиции

        _eventBridge.WeatherChange_Event += _eventBridge_WeatherChange_Event;
        _eventBridge.TypeOfDayChange_Event += _eventBridge_TypeOfDayChange_Event; ;

    }

    private void _eventBridge_TypeOfDayChange_Event(UniStorm.UniStormSystem.CurrentTimeOfDayEnum timeOfDayEnum)
    {
        dayPeriodText.SetText(timeOfDayEnum.ToString());
    }

    private void _eventBridge_WeatherChange_Event(UniStorm.WeatherType typeWeather)
    {
        weatherText.SetText(typeWeather.name);
    }

    private void WhereAmI()
    {
        float x = this.transform.position.x;
        float z = this.transform.position.z;
        float y = terrainCreater.mapNoise.GetGenerator(terrainCreater.seed).GetHeight(x,z);

        Assets.Codebase.Models.DataModels.BiomeData biom = BiomeUtils.GetBiome(x,z,y, terrainCreater.biomesData, terrainCreater.seed);
        if (biom.name != biomeText.text)
        {
            biomeText.SetText(biom.name);
            Debug.Log("Player in biom " + biom.name);
        }
        
    }


    private void OnDisable()
    {
        _eventBridge.WeatherChange_Event -= _eventBridge_WeatherChange_Event;
        _eventBridge.TypeOfDayChange_Event -= _eventBridge_TypeOfDayChange_Event;
        _eventBridge.WorldFinilised_Event -= _eventBridge_WorldFinilised_Event;
    }

    private void OnDestroy()
    {
        _eventBridge.WeatherChange_Event -= _eventBridge_WeatherChange_Event;
        _eventBridge.TypeOfDayChange_Event -= _eventBridge_TypeOfDayChange_Event;
        _eventBridge.WorldFinilised_Event -= _eventBridge_WorldFinilised_Event;
    }
}
