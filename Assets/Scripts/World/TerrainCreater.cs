using Assets.Codebase.Interfaces;
using Assets.Codebase.Models.DataModels;
using Assets.Codebase.Services;
using Assets.Scripts.World;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(MeshFilter))]
public class TerrainCreater : MonoBehaviour
{

    #region Settings
    public GameObject[] objects;
    public Text TextCanvas;
    
    [Space(5)]

    #region Map
    [Label("Зерно")]
    [BoxGroup("Map")]
    public int seed;

    [Range(2, 2048)]
    [Label("Ширина")]
    [BoxGroup("Map")]
    public int width = 250;

    [BoxGroup("Map")]
    [Label("Цвет. мин.карта")]
    public bool subMapColorized = false;

    [BoxGroup("Map")]
    [Label("Бомы мин.карт.")]
    public Gradient gradient;

    [BoxGroup("Map")]
    [Label("RawImg мини карты")]
    public RawImage rawImage;

    [BoxGroup("Map")]
    public NoiseData mapNoise;

    [BoxGroup("Map")]
    public List<BiomeData> biomesData;

    [BoxGroup("Map")]
    public GameObject aliveObj;

    [BoxGroup("Map")]
    public GameObject notAliveObj;



    #endregion

    #region River
    //[Range(0.1f, 30f)]
    //[Label("Глубина")]
    //[BoxGroup("Rivers")]
    //public int amplitudeHieghtRiver = 10;

    //[Range(0.1f, 30f)]
    //[Label("Ширина")]
    //[BoxGroup("Rivers")]
    //public int amplitudeWidthRiver = 1;

    //[Range(1, 10)]
    //[Label("Кол-во")]
    //[BoxGroup("Rivers")]
    //public int countRiver = 1;

    //[MinMaxSlider(0.0001f, 1.0f)]
    //[Label("Дельа высоты старта")]
    //[BoxGroup("Rivers")]
    //public Vector2 firstPointHeightDelta;


    //[Label("Кривая глубины")]
    //[BoxGroup("Rivers")] //"??? ???? ????????, 0.8 t - ??? ??????? ????"
    //public AnimationCurve curveHeightByRiver;


    //[Label("Кривая подъема сторон")]
    //[BoxGroup("Rivers")] //"??? ???? ????????, 0.8 t - ??? ??????? ????"
    //public AnimationCurve curveSideHeightByRiver;

    //[Label("Кривая ширины")]
    //[BoxGroup("Rivers")] //"??? ???? ????????, 0.8 t - ??? ??????? ????"
    //public AnimationCurve curveWidthByRiver;

    //[Range(1, 10)]
    //[Label("Радиус поиска")]
    //[BoxGroup("Rivers")]
    //public int scaleStepAnalises = 1;

    //[Range(-100, 0)]
    //[Label("Ур-н песка")]
    //[BoxGroup("Rivers")]
    //public int sandStartLvlHeight = -15;

    #endregion

    #region Water
    [BoxGroup("Вода")]
    public int waterOffset;
    [BoxGroup("Вода")]
    public int waterLine;
    #endregion

    #region TestNoise
    [Label("Тест шум")]
    [BoxGroup("TestNoise")]
    public NoiseData mapNoiseTest;

    [Label("Img")]
    [BoxGroup("TestNoise")]
    public RawImage imgNoiseTest;

    [Label("Лимит высоты")]
    [BoxGroup("TestNoise")]
    [Range(-1f, 1f)]
    public float heightNoiseLimit;
    #endregion



    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void GenWorldButtonSetting() => _eventBridge.OnWorldStartCreating(this);

    [Button(enabledMode: EButtonEnableMode.Always)]
    private void GenNoisePreviewBtn() => GenNoisePreview(mapNoiseTest.GetGenerator(seed), imgNoiseTest, heightNoiseLimit).Forget();

    [Button(enabledMode: EButtonEnableMode.Always)]
    private void GenNoiseMainBtn() => GenNoisePreview(mapNoise.GetGenerator(seed), rawImage).Forget();

    #endregion

    Mesh _mesh;
    
    TerrainMap _meshMap;
    MeshFilter _meshFilter;
    MeshCollider _meshCollaider;
    MeshRenderer _meshRenderer;
    Transform _transform;

    EventBridge _eventBridge { get; set; }


    [Inject]
    public void InjectHandler(EventBridge eventBridge)
    {
        _eventBridge = eventBridge;
    }


    private void Awake()
    {
        if (seed == 0)
            seed = Random.Range(0, 100000);

        aliveObj.SetActive(false);

        _mesh = new Mesh();
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollaider = GetComponent<MeshCollider>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _transform = GetComponent<Transform>();

       

        _eventBridge.WorldCreated_Event += _eventBridge_WorldCreated_Event;
        _eventBridge.WorldCreatIngStepMsg_Event += _eventBridge_WorldCreatIngStepMsg_Event;
    }

    private void Start()
    {
        _eventBridge.OnWorldStartCreating(this);
    }

    private void OnDisable()
    {
        _eventBridge.WorldCreated_Event -= _eventBridge_WorldCreated_Event;
        _eventBridge.WorldCreatIngStepMsg_Event -= _eventBridge_WorldCreatIngStepMsg_Event;
    }


    private void _eventBridge_WorldCreated_Event(TerrainMap meshMap)
    {

        _meshMap = meshMap;

        UpdateMesh();
        SpawnObjects();
        
        
        //_eventBridge.OnWorldFinilised();
    }

    private void _eventBridge_WorldCreatIngStepMsg_Event(int step, string description)
    {
        //TextCanvas.text = "Шаг: " + step + " " + description;
        Debug.Log(description);
    }


    public Vector3 GetWorldPosition(Vector3 point)
    {
        return _transform.TransformPoint(point);
    }

    private async UniTaskVoid GenNoisePreview(INoise noise, RawImage img, float heightLimit = -1 )
    {
        Texture2D tex = new Texture2D(width + 1, width + 1);
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {

                noise.GetHeight(x, z, out float clearH, out bool isBound);
                if (clearH > heightLimit)
                {
                    var colorH = Color.Lerp(Color.black, Color.white, clearH + 1);
                    tex.SetPixel(x, z, colorH);
                }
                else
                {
                    tex.SetPixel(x, z, Color.black);
                }
            }
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        img.texture = tex;

        await UniTask.Yield();
    }

    private void GenRawImage()
    {
        if (rawImage == null)
            return;

        
        Color[] pixels = new Color[_meshMap.Verticies.Length];
        if (subMapColorized)
            for (int i = 0; i < _meshMap.Verticies.Length; i++)
            {
                float height = Mathf.InverseLerp(_meshMap.DeltaMinHeight, _meshMap.DeltaMaxHeight, _meshMap.Verticies[i].y);
                pixels[i] = gradient.Evaluate(height);
                i++;
            }
        else
            for (int i = 0; i < _meshMap.Verticies.Length; i++)
            {
                var height = _meshMap.Verticies[i].y + Mathf.Abs(_meshMap.DeltaMinHeight);
                pixels[i] = Color.Lerp(Color.white, Color.black, 1 / (float)height);
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


        _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
         
        _mesh.vertices = _meshMap.Verticies;
 

        List<int> triss = new();
        foreach (var tris in _meshMap.Triangles)
            triss.AddRange(tris.ToList());

        _mesh.triangles = triss.ToArray();

        //Задаем параметры высоты для шейдера текстур карты
        int c = 0;
        foreach (var biome in biomesData)
        {
            
            float bHeight = 0f;
            if (c != 0)
            {
                if (biome.Min < 0)
                    bHeight = -(biome.Min * _meshMap.DeltaMinHeight);
                else
                    bHeight = biome.Min * _meshMap.DeltaMaxHeight;
            }
            else
                bHeight = _meshMap.DeltaMinHeight;

            c++;


            int biomeHeight = Shader.PropertyToID(biome.ShaderHeightName);

            _meshRenderer.material.SetFloat(biomeHeight, bHeight);
        }
 


        _mesh.RecalculateNormals();
        _mesh.uv = UvCalculator.CalculateUVs(_meshMap.Verticies, 1);

        _mesh.RecalculateTangents();
        _mesh.RecalculateBounds();

        _mesh.Optimize();
        _mesh.OptimizeIndexBuffers();
        _mesh.OptimizeReorderVertexBuffer();

        GenRawImage();
        _mesh.name = "meshTerrain";
        _meshFilter.mesh = _mesh;
        _meshCollaider.sharedMesh = _mesh;

        _eventBridge.OnWorldPreFinilised();
    }

    private void SpawnObjects()
    {
        Debug.Log("Start spawn objects");

        int countAlive = 0;
        int countNotAlive = 0;
        bool isAlive = false;
        foreach (var objectInst in _meshMap.GameObjects)
        {
            isAlive = objectInst.Item1.CompareTag("Alive");

            if (isAlive && countAlive < 500)
            {
                Instantiate(objectInst.Item1, objectInst.Item2, Quaternion.identity, aliveObj.transform);
                countAlive++;
            }
            else if (!isAlive && countNotAlive < 40000)
            {
                Instantiate(objectInst.Item1, objectInst.Item2, Quaternion.identity, notAliveObj.transform);
                countNotAlive++;
            }
            else
                break;
        }
        _meshMap.GameObjects.Clear();

        Debug.Log("Спаун объектов завершен");
        Debug.Log("countAlive = " + countAlive);
        Debug.Log("countNotAlive = " + countNotAlive);
        _eventBridge.OnWorldFinilised();

        if (aliveObj != default)
            aliveObj.SetActive(true);

    }


}
