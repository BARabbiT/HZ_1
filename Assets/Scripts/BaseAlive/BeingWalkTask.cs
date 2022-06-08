using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.BaseAlive
{
    class BeingWalkTask : MonoBehaviour, ITask
    {
        private int _priority;
        private int _specId;
        private BeingMove _beingMove;
        private Vector3 _targetPoint;
        private BeingEventBridge _BEB;
        private bool _mayIDo;
        int cycleRange = 10;
        int currentCycle = 1;
        int rangeMultiplier = 1;

        private void Awake()
        {
            _beingMove = gameObject.GetComponent<BeingMove>();
            _BEB = gameObject.GetComponent<BeingEventBridge>();
            _priority = 1;
        }

        private void MoveToNextPoint()
        {
            if (_mayIDo)
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
                _targetPoint = transform.position + Quaternion.Euler(0, Random.Range(0f, 360f), 0) * Vector3.forward * Random.Range(1f, 15f * rangeMultiplier);
                _beingMove.MoveToPoint(_targetPoint);
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
            _mayIDo = true;
            _targetPoint = transform.position + Quaternion.Euler(0, Random.Range(0f, 360f), 0) * Vector3.forward * Random.Range(1f, 15f);
            _BEB.CameToPoint += MoveToNextPoint;
            _beingMove.MoveToPoint(_targetPoint);
        }

        public void TaskDone()
        {
            _BEB.OnTaskDone(this);
        }

        public void StopTask()
        {
            _mayIDo = false;
            _BEB.CameToPoint -= MoveToNextPoint;
            _beingMove.StopMove();
            currentCycle = 1;
            rangeMultiplier = 1;
        }

        public int GetSpecId()
        {
            return _specId;
        }
    }
}
