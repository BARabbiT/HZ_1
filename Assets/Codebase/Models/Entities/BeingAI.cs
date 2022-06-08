using Assets.Codebase.Interfaces.BaseAlive;
using Assets.Scripts.BaseAlive;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Assets.Codebase.Models.Enums;
using Assets.Codebase.Models.Entities;
using System.Linq;
using Assets.Codebase.Models.Entities.Addons;
using Cysharp.Threading.Tasks;
using System.Threading;
using Assets.Codebase.Models.Entities.Specifications;
using SensorToolkit;

public class BeingAI : MonoBehaviour
{
    private Assets.Codebase.Services.EventBridge _eventBridge;
    private BeingTaskQueue _beingTaskQueue;
    private BeingEventBridge _beingEventBridge;
    private System.Random _randomiser;
    private RaySensorWrapper _raySensorWrapper;

    #region Массивы абилок
    //рот
    private List<IMouth> Mouths;

    //тело
    private List<IBody> Bodys;

    //разум
    private List<IMind> Minds;

    //конечности
    private List<ILimb> Limbs;
    #endregion

    #region Изменяемые характеристики (100 - минимальная потребость, 0 - максимальная необходимость)
    //насыщение (голод) - Mouth
    private Hungry _hungry = new(80);

    //жажда - Mouth
    private Thirsty _thirsty = new(100);

    //сонливость - Body - Metaboly
    private Sleepy _sleepy = new(80);

    //здоровье - Body - Health
    private Healthy _healthy = new(100);
    
    //память (количество мест для запоминания) - Mind
    private int _memory = 5;
    List<MindPoint> mindPoints;

    //возраст - Body - Age
    private int _age = 0;

    //таблица решения паритетов
    private int[,] _paritetMatrix = new int[5,5];
    #endregion

    #region Неизменяемые характеристики
    //скорость
    private int _speed = 10;

    //максимальный возраст
    private int _deadAge = 360;
    #endregion

    private int SetTaskPriority(int characteristic)
    {
        return 90 - characteristic + (10 - Math.DivRem(characteristic,10,out _)) ^ 2;
    }

    #region Методы, наследуемые от MonoBehaviour
    //инъекция сервисов
    [Inject]
    public void InjectHandler(Assets.Codebase.Services.EventBridge eventBridge)
    {
        _eventBridge = eventBridge;
    }

    //Фактически конструктор
    private void Awake()
    {
        _beingTaskQueue = GetComponent<BeingTaskQueue>();
        _beingEventBridge = GetComponent<BeingEventBridge>();

        _raySensorWrapper = new RaySensorWrapper(GetComponent<RaySensor>(), _beingEventBridge);

        mindPoints = new();

        Mouths = new List<IMouth>
        {
            new HerbivoreMouth(_eventBridge)
        };
        Bodys = new();
        Minds = new();
        Limbs = new()
        {
            new FootLimb(_eventBridge)
        };

        foreach (var mouth in Mouths)
        {
            mouth.Subscribe(this);
        }
        foreach (var body in Bodys)
        {
            body.Subscribe(this);
        }
        foreach (var mind in Minds)
        {
            mind.Subscribe(this);
        }
        foreach (var limb in Limbs)
        {
            limb.Subscribe(this);
        }

        ThinkAboutSleep();
        ThinkAboutHungry();
    }

    //Метод, вызываемый при удалении объекта
    private void OnDestroy()
    {
        foreach (var mouth in Mouths)
        {
            mouth.UnSubscribe(this);
        }
        foreach (var body in Bodys)
        {
            body.UnSubscribe(this);
        }
        foreach (var mind in Minds)
        {
            mind.UnSubscribe(this);
        }
        foreach (var limb in Limbs)
        {
            limb.UnSubscribe(this);
        }
        _beingTaskQueue.Destroy();
    }
    #endregion

    #region Методы, подменяющие get\set над полями
    public int GetHungry()
    {
        return _hungry.value;
    }
    public void AddHungry(int value)
    {
        foreach (var mouth in Mouths)
        {
            var subscribeMethod = typeof(IMouth).GetMethod("Calculate");
            value = (int)(subscribeMethod?.Invoke(mouth, new object[] { value }));
        }
        _hungry.value += value;
    }
    public void Loosehungry(int value)
    {
        _hungry.value -= value;
        ThinkAboutHungry();
    }

    public int GetThirsty()
    {
        return _thirsty.value;
    }
    public void AddThirsty(int value)
    {
        foreach (var mouth in Mouths)
        {
            var subscribeMethod = typeof(IMouth).GetMethod("Calculate");
            value = (int)(subscribeMethod?.Invoke(mouth, new object[] { value }));
        }
        _thirsty.value += value;
    }
    public void LooseThirsty(int value)
    {
        _thirsty.value -= value;
        ThinkAboutThirsty();
    }

    public int GetSleepy()
    {
        return _sleepy.value;
    }
    public void AddSleepy(int value)
    {
        foreach (var body in Bodys)
        {
            var subscribeMethod = typeof(IBody).GetMethod("CalculateMetaboly");
            value = (int)(subscribeMethod?.Invoke(body, new object[] { value }));
        }
        _sleepy.value += value;
    }
    public void LooseSleepy(int value)
    {
        _sleepy.value -= value;
        ThinkAboutSleep();
    }

    public int GetHealthy()
    {
        return _healthy.value;
    }
    public void AddHealthy(int value)
    {
        foreach (var body in Bodys)
        {
            var subscribeMethod = typeof(IBody).GetMethod("CalculateHealth");
            value = (int)(subscribeMethod?.Invoke(body, new object[] { value }));
        }
        _healthy.value += value;
    }
    public void LooseHealthy(int value)
    {
        _healthy.value -= value;
        ThinkAboutHealthy();
    }

    public int GetSpeed()
    {
        int value = _speed;
        foreach (var limb in Limbs)
        {
            var subscribeMethod = typeof(ILimb).GetMethod("Calculate");
            value = (int)(subscribeMethod?.Invoke(limb, new object[] { value }));
        }
        return value;
    }

    public void IncreaseAge(int value)
    {
        foreach (var body in Bodys)
        {
            var subscribeMethod = typeof(IBody).GetMethod("CalculateAge");
            value = (int)(subscribeMethod?.Invoke(body, new object[] { value }));
        }
        _age += value;
        ThinkAboutAge();
    }
    #endregion

    #region Память особи
    public MindPoint GetPointFromMind(MindPointType pointType)
    {
        MindPoint findedPOints = mindPoints.Find(mp => mp.GetPointType() == pointType);
        if (findedPOints != null)
        {
            return findedPOints;
        }
        else
        {
            return null;
        }
        
    }
    public void AddPointToMind(MindPointType pointType, Vector3 coordinate)
    {
        if (_memory == 0)
        {
            return;
        }
        if (mindPoints.Count < _memory)
        {
            mindPoints.Add(new MindPoint(coordinate, pointType));
        }
        else
        {
            MindPoint findedPoint = mindPoints.FirstOrDefault(mp => mp.GetPointType() == pointType && mp.GetPointCoordinate() == coordinate);
            if (findedPoint != default)
            {
                mindPoints.Remove(findedPoint);
                mindPoints.Add(new MindPoint(coordinate, pointType));
            }
            else
            {
                mindPoints.Remove(mindPoints.First(mp => mp.GetAddTime() == mindPoints.Max(mp => mp.GetAddTime())));
                mindPoints.Add(new MindPoint(coordinate, pointType));
            }
        }
    }
    public int GetParitetResult(int spec1, int spec2)
    {
        return _paritetMatrix[spec1, spec2];
    }
    public void SetParitetResult(int spec1, int spec2, int result)
    {
        _paritetMatrix[spec1, spec2] = result;
        _paritetMatrix[spec2, spec1] = result == 0 ? 1 : 0;
    }
    #endregion

    #region Методы иницирующие действия особи (вызываются по изменению характеристики\реакцией на событие и являются "мыслями" особи)
    //Проверка голода и постановка задачи на поиск еды
    private void ThinkAboutHungry()
    {
        int priority = SetTaskPriority(GetHungry());
        if (priority > 0)
        {
            GetComponent<BeingFindEatTask>().SetPriority(priority);
            GetComponent<BeingFindEatTask>().SetParameters(new object[]{ _hungry.Id });
            _beingTaskQueue.AddTaskInQueue(GetComponent<BeingFindEatTask>());
        }
    }

    //Постановка задачи на поиск воды
    private void ThinkAboutThirsty()
    {
        int priority = SetTaskPriority(GetSleepy());
        if (priority > 0)
        {
            GetComponent<BeingFindWaterTask>().SetPriority(priority);
            GetComponent<BeingFindWaterTask>().SetParameters(new object[] { _thirsty.Id });
            _beingTaskQueue.AddTaskInQueue(GetComponent<BeingFindWaterTask>());
        }
    }

    //Проверка сонливости и постановка задачи на сон
    private void ThinkAboutSleep()
    {
        int priority = SetTaskPriority(GetSleepy());
        if (priority > 0)
        {
            GetComponent<BeingSleepTask>().SetPriority(priority);
            GetComponent<BeingSleepTask>().SetParameters(new object[] { _sleepy.Id });
            _beingTaskQueue.AddTaskInQueue(GetComponent<BeingSleepTask>());
        }
    }

    //Проверка здоровья
    private void ThinkAboutHealthy()
    {

    }

    //Постановка задачи на прогулку
    private void ThinkAboutWalk()
    {
        GetComponent<BeingWalkTask>().SetPriority(1);
        _beingTaskQueue.AddTaskInQueue(GetComponent<BeingWalkTask>());
    }

    //Проверка возраста
    private void ThinkAboutAge()
    {
        if (_age >= 100 && _age <= 200)
        {
            //размножение активно
            IsReproduction();
        }
        else if (_age > 200 && _age < _deadAge)
        {
            //размножение не активно
            IsNoReproduction();
        }
        else
        {
            //смерть
            Death();
        }
    }
    #endregion

    #region Прочие методы особи
    private void Death()
    {
        
    }
    #endregion

    #region Подписки на события из интерфейсов способностей
    //Реакция на событие понижения насыщения
    public async void LessHungry()
    {
        await UniTask.Delay(_randomiser.Next(0, 2000));
        Loosehungry(5);
    }

    //Реакция на повышение жажды
    public async void LessThirsty()
    {
        await UniTask.Delay(_randomiser.Next(0, 2000));
        LooseThirsty(5);
    }

    //Реакция на наступление ночи
    public async void DayEnd()
    {
        await UniTask.Delay(_randomiser.Next(0, 2000));
        ThinkAboutSleep();
    }

    //реакция на наступление утра
    public async void DayStart()
    {
        await UniTask.Delay(_randomiser.Next(0, 2000));
        IncreaseAge(1);
    }
    #endregion

    #region Полуавтоматические подписки
    //Подписка на события размножения
    public async void IsReproduction()
    {
        
    }

    //Отписка от событий размножения
    public async void IsNoReproduction()
    {

    }
    #endregion
}