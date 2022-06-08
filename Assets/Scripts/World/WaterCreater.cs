using Assets.Codebase.Models.WorldModels;
using Assets.Codebase.Services;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.World
{
    public class WaterCreater : MonoBehaviour
    {

 
        EventBridge _eventBridge;

        Mesh _mesh;
        WaterMap _waterMap;
        MeshFilter _meshFilter;
        MeshCollider _meshCollaider;
        Transform _transform;
        float _mapWidth;


        [Inject]
        public void InjectHandler(EventBridge eventBridge)
        {
            _eventBridge = eventBridge;
        }


        private void Awake()
        {
          
            _mesh = new Mesh();
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollaider = GetComponent<MeshCollider>();
            _transform = GetComponent<Transform>();


            _eventBridge.WaterCreated_Event += _eventBridge_WaterCreated_Event;
        }

        private void _eventBridge_WaterCreated_Event(WaterMap waterMap, float mapWidth)
        {
            _mapWidth = mapWidth;
            _waterMap = waterMap;
            UpdateMesh();
        }

        private void UpdateMesh()
        {
            _transform.position = new Vector3(-_mapWidth/2, _waterMap.WaterLine, -_mapWidth/2);

            _mesh.Clear();

            _meshFilter.mesh = _mesh;

            _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            _mesh.vertices = _waterMap.Verticies;

            List<int> triss = new();
            foreach (var tris in _waterMap.Triangles)
            {
                triss.AddRange(tris.ToList());
               
            }
               
            _mesh.triangles = triss.ToArray();
            

            _mesh.RecalculateNormals();
            _mesh.uv = UvCalculator.CalculateUVs(_waterMap.Verticies, 1);


            _mesh.RecalculateTangents();
            _mesh.RecalculateBounds();

 
            _meshCollaider.sharedMesh = _mesh;


        }
    }
}
