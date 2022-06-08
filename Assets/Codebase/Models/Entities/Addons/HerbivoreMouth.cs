using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Codebase.Services;
using Zenject;

public class HerbivoreMouth : IMouth
{
    public int Herbivore { get => 0;}
    private EventBridge _eventBridge;

    public HerbivoreMouth(EventBridge eventBridge)
    {
        _eventBridge = eventBridge;
    }

    [Inject]
    public void InjectHandler(Assets.Codebase.Services.EventBridge eventBridge)
    {
        _eventBridge = eventBridge;
    }

    public int Calculate(int parametr)
    {
        return parametr*1;
    }

    public void Execute()
    {

    }

    public void Subscribe(BeingAI obj)
    {
        _eventBridge.LessHungry_Event += obj.LessHungry;
        _eventBridge.LessThirsty_Event += obj.LessThirsty;
    }

    public void UnSubscribe(BeingAI obj)
    {
        _eventBridge.LessHungry_Event -= obj.LessHungry;
        _eventBridge.LessThirsty_Event -= obj.LessThirsty;
    }
}
