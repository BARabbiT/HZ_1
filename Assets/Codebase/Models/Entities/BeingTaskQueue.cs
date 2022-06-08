using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class BeingTaskQueue : MonoBehaviour
{
    public int taskCount;

    private List<ITask> _tasks = new List<ITask>();
    public bool _isBusy = false;
    private ITask _currentTask;
    private int _currenttaskPriority;
    private BeingEventBridge _BEB;
    private BeingAI _beingAI;

    private int _firstTaskId;
    private int _secondTaskId;
    private int _paritetResult;
    private bool _haveParitet = false;

    private void Awake()
    {
        _BEB = GetComponent<BeingEventBridge>();
        _beingAI = GetComponent<BeingAI>();
        _BEB.TaskDone += DoneTask;
    }

    private void Update()
    {
        if (!_isBusy)
        {
            StartTaskOnQueue();
        }

        taskCount = _tasks.Count();
    }



    private void StartTaskOnQueue()
    {
        if (_tasks.Count() > 0)
        {
            switch (_tasks.Count)
            {
                case 1:
                    {
                        _currentTask = _tasks[0];
                        break;
                    }
                case > 1:
                    {
                        var firstTask = _tasks.OrderBy(t => t.GetPriority()).ToList()[0];
                        var secondTask = _tasks.OrderBy(t => t.GetPriority()).ToList()[1];
                        if (Math.Abs(firstTask.GetPriority() - secondTask.GetPriority()) <= 5)
                        {
                            _haveParitet = true;
                            _firstTaskId = firstTask.GetSpecId();
                            _secondTaskId = secondTask.GetSpecId();
                            _paritetResult = 0;

                            if (_beingAI.GetParitetResult(firstTask.GetSpecId(), secondTask.GetSpecId()) == 0)
                            {
                                _currentTask = firstTask;
                                _paritetResult = 0;
                            }
                            else
                            {
                                _currentTask = secondTask;
                                _paritetResult = 1;
                            }
                        }
                        else
                        {
                            _currentTask = firstTask;
                        }
                        break;
                    }
            }

            if (_currentTask != null)
            {
                _isBusy = true;
                _currentTask.StartTask();
                _currenttaskPriority = _currentTask.GetPriority();
            }
        }
    }

    private void DoneTask(ITask task)
    {
        if (_currentTask != null)
        {
            _isBusy = false;
            DeleteTaskFromQueue(_currentTask);
            _currenttaskPriority = 0;

            if (_haveParitet)
            {
                _beingAI.SetParitetResult(_firstTaskId,_secondTaskId,_paritetResult);
                _haveParitet = false;
                _firstTaskId = 0;
                _secondTaskId = 0;
                _paritetResult = 0;
            }
            _currentTask = null;
        }
    }

    public void AddTaskInQueue(ITask task)
    {
        var taskInQ = _tasks.FirstOrDefault(t => t.GetType() == task.GetType());
        if (taskInQ != default && taskInQ.GetPriority() < task.GetPriority())
        {
            taskInQ.SetPriority(task.GetPriority());
        }
        else
        {
            _tasks.Add(task);
        }

        if (_currentTask!=null && Math.Abs(_currentTask.GetPriority() - task.GetPriority()) <= 5)
        {
            _haveParitet = true;
            _firstTaskId = _currentTask.GetSpecId();
            _secondTaskId = task.GetSpecId();
            _paritetResult = 0;

            if (_beingAI.GetParitetResult(_currentTask.GetSpecId(), task.GetSpecId()) == 0)
            {
                AbortCurrentTask();
                _paritetResult = 0;
            }
            else
            {
                _paritetResult = 1;
            }
        }
    }

    private void DeleteTaskFromQueue(ITask task)
    {
        _tasks.Remove(task);
    }

    private void AbortCurrentTask()
    {
        if (_currentTask != null)
        {
            _currentTask.StopTask();
            DeleteTaskFromQueue(_currentTask);
            _isBusy = false;
            _currentTask = null;
            _currenttaskPriority = 0;

            if (_haveParitet)
            {
                if (_paritetResult == 1)
                {
                    _paritetResult = 0;
                }
                else
                {
                    _paritetResult = 1;
                }
                _beingAI.SetParitetResult(_firstTaskId, _secondTaskId, _paritetResult);
                _haveParitet = false;
                _firstTaskId = 0;
                _secondTaskId = 0;
                _paritetResult = 0;
            }
        }
    }

    public void Destroy()
    {
        _tasks = new List<ITask>();
        if(_currentTask != null)
            _currentTask.StopTask();
    }

    public int GetTaskCount()
    {
        return _tasks.Count();
    }

    public int GetCurrentTaskPriority()
    {
        return _currenttaskPriority;
    }
}
