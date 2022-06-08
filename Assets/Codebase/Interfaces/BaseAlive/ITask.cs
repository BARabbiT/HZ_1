using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface ITask
{
    void SetParameters(object[] parameters = null);
    void SetPriority(int priority);
    int GetPriority();
    int GetSpecId();
    void StartTask();
    void TaskDone();
    void StopTask();
}
