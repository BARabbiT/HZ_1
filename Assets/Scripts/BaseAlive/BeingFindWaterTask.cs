using Assets.Codebase.Models.Enums;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.BaseAlive
{
    class BeingFindWaterTask : MonoBehaviour, ITask
    {
        private int _priority;
        private int _specId;
        private BeingMove _beingMove;
        private BeingEventBridge _beingEventBringe;
        private BeingAI _beingAI;

        private int cycleRange = 10;
        private int currentCycle = 1;
        private int rangeMultiplier = 1;
        private bool _mayIDo;

        private void Awake()
        {
            _beingMove = gameObject.GetComponent<BeingMove>();
            _beingEventBringe = gameObject.GetComponent<BeingEventBridge>();
            _beingAI = gameObject.GetComponent<BeingAI>();
            _priority = 1;
        }

        private void CameToPoint()
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
                _beingMove.MoveToPoint(transform.position + Quaternion.Euler(0, Random.Range(0f, 360f), 0) * Vector3.forward * Random.Range(1f, 15f * rangeMultiplier));
            }
        }

        private void GoDrink(Vector3 point)
        {
            _beingMove.StopMove();
            _beingMove.MoveToPoint(point);
            _beingEventBringe.CameToPoint -= CameToPoint;
            _beingEventBringe.CameToPoint += CheckDrink;
        }

        private async void CheckDrink()
        {
            await UniTask.Delay(1000);
            if (_beingAI.GetHungry() < 90)
            {
                _beingMove.MoveToPoint(transform.position + Quaternion.Euler(0, Random.Range(0f, 360f), 0) * Vector3.forward * Random.Range(1f, 15f * rangeMultiplier));
                _beingEventBringe.CameToPoint += CameToPoint;
                _beingEventBringe.CameToPoint -= CheckDrink;
            }
            else
            {
                TaskDone();
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
            _beingEventBringe.CameToPoint += CameToPoint;
            var mindPoint = _beingAI.GetPointFromMind(MindPointType.Drink);
            if (mindPoint != null)
            {
                _beingMove.MoveToPoint(mindPoint.GetPointCoordinate());
            }
            else
            {
                _beingMove.MoveToPoint(transform.position + Quaternion.Euler(0, Random.Range(0f, 360f), 0) * Vector3.forward * Random.Range(1f, 15f * rangeMultiplier));
            }
            _beingEventBringe.SeeEat += GoDrink;
        }

        public void TaskDone()
        {
            _mayIDo = false;
            _beingEventBringe.CameToPoint -= CameToPoint;
            _beingEventBringe.CameToPoint -= CheckDrink;
            _beingEventBringe.SeeEat -= GoDrink;
            currentCycle = 1;
            rangeMultiplier = 1;
            _beingEventBringe.OnTaskDone(this);
        }

        public void StopTask()
        {
            _mayIDo = false;
            _beingEventBringe.CameToPoint -= CameToPoint;
            _beingEventBringe.CameToPoint -= CheckDrink;
            _beingEventBringe.SeeEat -= GoDrink;
            currentCycle = 1;
            rangeMultiplier = 1;
            _beingMove.StopMove();
        }

        public int GetSpecId()
        {
            return _specId;
        }
    }
}
