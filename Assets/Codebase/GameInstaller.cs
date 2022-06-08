using Assets.Codebase.Services;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    /// <summary>
    /// Register your scene services here
    /// </summary>
    public override void InstallBindings()
    {
        //Debug.ClearDeveloperConsole();
        
        Container.Bind<EventBridge>().AsSingle().NonLazy();
        
        Container.Bind<AliveService>().AsSingle().NonLazy();
        Container.Bind<InteractiveService>().AsSingle().NonLazy();
        Container.Bind<LifelessService>().AsSingle().NonLazy();
        Container.Bind<GoodWorldService>().AsSingle().NonLazy();
        Container.Bind<LabirinthsService>().AsSingle().NonLazy();
    }

}