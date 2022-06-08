using Assets.Codebase.Generators;
using Assets.Codebase.Interfaces;
using Assets.Codebase.Models.DataModels;
using Assets.Codebase.Models.Enums;
using Assets.Codebase.Models.WorldModels;
using Assets.Codebase.Services;
using Assets.Scripts.World;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using RcherNZ.AccidentalNoise;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class LabirinthsCreater : MonoBehaviour
{
 
    [Space(5)]

    #region Map
    [Range(1, 2048)]
    [Label("Ширина")]
    [BoxGroup("Map")]
    public int width = 250;

    [BoxGroup("Map")]
    public float terrainSurface = 0.5f;

    [BoxGroup("Map")]
    public bool flatShaded = true;

    [BoxGroup("Map")]
    public bool smoothTerrain = true;
 
    [Range(1, 2048)]
    [Label("Высота")]
    [BoxGroup("Map")]
    public int height = 250;

    [BoxGroup("Map")]
    [Label("RawImg мини карты")]
    public RawImage rawImage;

    [BoxGroup("Map")]
    public NoiseData mapNoise;

    [BoxGroup("Map")]
    public LabirinthData LabirinthsData;

    [Label("Зерно")]
    [BoxGroup("Map")]
    public int seed;

    [BoxGroup("Chanks")]
    public Dictionary<Vector2Int, int> Chanks;

    [BoxGroup("Chanks")]
    public int ChankWidth;
    #endregion


    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void GenWorldButtonSetting() => _eventBridge.OnLabirinthsStartCreating(this);
 

    Mesh _mesh;
    LabirinthsMap _meshMap;
    MeshFilter _meshFilter;
    MeshCollider _meshCollaider;

    EventBridge _eventBridge { get; set; }


    [Inject]
    public void InjectHandler(EventBridge eventBridge)
    {
        _eventBridge = eventBridge;
    }


    private void _eventBridge_LabirinthsCreated_Event(LabirinthsMap meshMap)
    {
        _meshMap = meshMap;
        UpdateMesh();

    }


    private void Awake()
    {
        if (seed == 0)
            seed = Random.Range(0, 100000);

        _mesh = new Mesh();
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollaider = GetComponent<MeshCollider>();

        _eventBridge.LabirinthsCreated_Event += _eventBridge_LabirinthsCreated_Event;
    }

    private void Start()
    {
        _eventBridge.OnLabirinthsStartCreating(this);
    }

    private void OnDisable()
    {
        _eventBridge.LabirinthsCreated_Event -= _eventBridge_LabirinthsCreated_Event;
    }

    private void OnDestroy()
    {
        _meshMap = null;
        _mesh.Clear();
        _eventBridge.LabirinthsCreated_Event -= _eventBridge_LabirinthsCreated_Event;
    }

 
    private void CreateChunks()
    {
        int chankI = 0;
        for (int x = 0; x < width; x += ChankWidth)
        {
            for (int z = 0; z < width; z+=ChankWidth)
            {
 
                //Dictionary<Vector2Int,int> keyValues = new Dictionary<Vector2Int, int>(,)
                //Chanks.Add(new Vector2Int(), chankI)
                ////chankI++;
            }
        }
    }

    private void GenRawImage()
    {
        if (rawImage == null)
            return;

        
        Color[] pixels = new Color[_meshMap.Verticies.Length];

        int i = 0;
        for(int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                float heightF = 0;
                i++;
                var heights = _meshMap.Verticies.Where(v => v.x == x && v.z == z);
                foreach (var h in heights)
                {
                    heightF += h.y;
                }
                pixels[i] = Color.Lerp(Color.black, Color.white, 1 / heightF);
            }
        }
            

 

        Texture2D tex = new Texture2D(width+1, width+1);
        tex.SetPixels(pixels);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        rawImage.texture = tex;
    }

    private void UpdateMesh()
    {
        _mesh.Clear();
        
        _meshFilter.mesh = _mesh;

        _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
         
        _mesh.vertices = _meshMap.Verticies;
        _mesh.triangles = _meshMap.Triangles.ToArray();

        //List<int> triss = new();
        //foreach (var tris in _meshMap.Triangles)
        //    triss.AddRange(tris.ToList());




        //Material[] materials = new Material[_meshMap.Labirinths.Count + 1];

        //foreach (var biom in _meshMap.Biomes)
        //{
        //    var typeBiom = (int)biom.Type - 1;
        //    materials[typeBiom] = biom.Material;
        //}
        //_meshRenderer.materials = materials;

        //_mesh.subMeshCount = _meshMap.Biomes.Count + 1;
        //foreach (var biom in _meshMap.Biomes)
        //{
        //    int submeshInd = (int)biom.Type;
        //    var listsIndices = _meshMap.Triangles.Where(t => t.BiomeType == submeshInd).Select(b => new List<int>() { b.I1, b.I2, b.I3 }).ToList();
        //    List<int> resultSubMesh = new();
        //    foreach (var indice in listsIndices)
        //        resultSubMesh.AddRange(indice);

        //    _mesh.SetTriangles(resultSubMesh, submeshInd - 1);
        //}

        _mesh.uv = UvCalculator.CalculateUVs(_meshMap.Verticies, 1);


        _mesh.RecalculateNormals();
        _mesh.RecalculateTangents();
        _mesh.RecalculateBounds();

        _mesh.Optimize();
        _mesh.OptimizeIndexBuffers();
        _mesh.OptimizeReorderVertexBuffer();
        _mesh.MarkDynamic();
        GenRawImage();

        _meshCollaider.sharedMesh = _mesh;

    }
 
}
