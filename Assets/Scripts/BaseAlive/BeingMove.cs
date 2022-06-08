using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.BaseAlive
{
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    public class BeingMove : MonoBehaviour
    {
        private UnityEngine.AI.NavMeshAgent _navAgent;
        private BeingAI _beingAI;
        private BeingEventBridge _BEB;
        private bool _onTheWay;

        private void Awake()
        {
            _navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            _BEB = GetComponent<BeingEventBridge>();
            _beingAI = GetComponent<BeingAI>();
            _onTheWay = false;
        }

        private void FixedUpdate()
        {
            if (_onTheWay && !_navAgent.hasPath)
            {
                _onTheWay = false;
                _BEB.OnCameToPoint();
            }
        }

        public void MoveToPoint(Vector3 point)
        {
            _navAgent.destination = point;
            _navAgent.isStopped = false;
            _navAgent.speed = _beingAI.GetSpeed();
            _onTheWay = true;
        }
        public void StopMove()
        {
            if (_onTheWay)
            {
                _onTheWay = false;
                _navAgent.isStopped = true;
            }
        }
    }
}