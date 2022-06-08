using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Codebase.Models.Enums;

namespace Assets.Codebase.Models.Entities
{
    public class MindPoint
    {
        private Vector3 _pointCoordinate;
        private MindPointType _poointType;
        private long _addTime;
        public MindPoint(Vector3 pointCoordinate, MindPointType poointType)
        {
            _pointCoordinate = pointCoordinate;
            _poointType = poointType;
            _addTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        public Vector3 GetPointCoordinate()
        {
            return _pointCoordinate;
        }
        public MindPointType GetPointType()
        {
            return _poointType;
        }
        public long GetAddTime()
        {
            return _addTime;
        }
    }
}
