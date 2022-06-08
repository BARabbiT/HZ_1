using UnityEngine;

public class BeingEventBridge : MonoBehaviour
{
    public delegate void SeeEatDelegate(Vector3 point);
    public event SeeEatDelegate SeeEat;
    public void OnSeeEat(Vector3 point)
    {
        if (SeeEat != null) SeeEat.Invoke(point);
    }

    public delegate void SeeDerinkDelegate(Vector3 point);
    public event SeeDerinkDelegate SeeDrink;
    public void OnSeeDrink(Vector3 point)
    {
        if (SeeDrink != null) SeeDrink.Invoke(point);
    }

    public delegate void TaskDoneDelegate(ITask task);
    public event TaskDoneDelegate TaskDone;
    public void OnTaskDone(ITask task)
    {
        if (TaskDone != null) TaskDone.Invoke(task);
    }

    public delegate void CameToPointDelegate();
    public event CameToPointDelegate CameToPoint;
    public void OnCameToPoint()
    {
        if (CameToPoint != null) CameToPoint.Invoke();
    }
}