using Assets.Scripts.BaseAlive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assets.Codebase.Models.Enums;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System.Threading;

public class BeingSleepTask : MonoBehaviour, ITask
{
    private int _priority;
    private int _specId;
    private BeingMove _beingMove;
    private BeingEventBridge _beingEventBridge;
    private BeingAI _beingAI;

    private int cycleRange = 10;
    private int currentCycle = 1;
    private int rangeMultiplier = 1;
    private bool canISleep = true;

    private void Awake()
    {
        _beingMove = gameObject.GetComponent<BeingMove>();
        _beingEventBridge = gameObject.GetComponent<BeingEventBridge>();
        _beingAI = gameObject.GetComponent<BeingAI>();
        _priority = 1;
    }

    private bool CheckPoint()
    {
        return true;
    }

    private async void Sleep()
    {
        while (canISleep)
        {
            if (_beingAI.GetSleepy() < 100)
            {
                await UniTask.Delay(1000);
                _beingAI.AddSleepy(5);
            }
            else
            {
                TaskDone();
                canISleep = false;
            }
        }
    }

    private void CameToPoint()
    {
        if (CheckPoint())
        {
            _beingAI.AddPointToMind(MindPointType.Sleep, gameObject.transform.position);
            Sleep();
        }
        else
        {
            if (currentCycle < cycleRange)
            {
                currentCycle++;
            }
            else
            {
                currentCycle = 1;
                rangeMultiplier++;
            }
            _beingMove.MoveToPoint(transform.position + Quaternion.Euler(0, Random.Range(0f, 360f), 0) * Vector3.forward * Random.Range(1f, 15f * rangeMultiplier));
        }
    }

    //Реализация ITask
    public void SetParameters(object[] parametrs = null)
    {
        if (parametrs != null)
        {
            _specId = (int)parametrs[0];
        }
    }
    public void SetPriority(int priority)
    {
        _priority = priority;
    }
    public int GetPriority()
    {
        return _priority;
    }
    public void StartTask()
    {
        _beingEventBridge.CameToPoint += CameToPoint;
        var mindPoint = _beingAI.GetPointFromMind(MindPointType.Sleep);
        if (mindPoint != null)
        {
            _beingMove.MoveToPoint(mindPoint.GetPointCoordinate());
        }
        else
        {
            _beingMove.MoveToPoint(transform.position + Quaternion.Euler(0, Random.Range(0f, 360f), 0) * Vector3.forward * Random.Range(1f, 15f * rangeMultiplier));
        }
    }
    public void TaskDone()
    {
        _beingEventBridge.CameToPoint -= CameToPoint;
        currentCycle = 1;
        rangeMultiplier = 1;
        _beingEventBridge.OnTaskDone(this);
    }
    public void StopTask()
    {
        _beingEventBridge.CameToPoint -= CameToPoint;
        _beingMove.StopMove();
        canISleep = false;
        currentCycle = 1;
        rangeMultiplier = 1;
    }
    public int GetSpecId()
    {
        return _specId;
    }
}
