

using System;
using System.Collections.Generic;
using _scripts.Utils;
using Shapes;
using Sirenix.OdinInspector;
using Terrain;
using Unity.VisualScripting;
using UnityEngine;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Utils;
using VectorTerrain.Scripts.Visualizer;

public class VectorTerrainVisualizer : MonoBehaviour
{
    public int blurr;
    public float sigma;
    
    private VectorTerrainManager _vectorTerrainManager;
    private TerrainGraph graph;
    private Transform _vizContainer;
    private GameObject _vizObject;
    private TerrainVisualizerController _vizController;
    private Dictionary<int, TerrainShapesRenderer> renderers = new();

    private void OnValidate()
    {
        // Do();
        Refresh();
    }

    private void OnEnable()
    {
        Init();
    }

    private void Awake()
    {
        Init();
    }

    [Button]
    void Init()
    {
        DestroyAllVisualizers();
        _vectorTerrainManager = GetComponent<VectorTerrainManager>();
        graph = _vectorTerrainManager.graph;
        _vizContainer = CreateContainter();
        _vizController = _vizContainer.GetComponent<TerrainVisualizerController>();
        _vizObject = _vizContainer.gameObject;
        _vectorTerrainManager.OnRefresh += Refresh;
    }

    [Button]
    void Do()
    {
        Init();
        var v = _vectorTerrainManager._sectorDict;
        foreach (var sectorController in v)
        {
            var points = sectorController.Value.sectorData.Verts;
            // Visualize(points, sectorController.Key);
            CreateRenderer(sectorController.Key);
            Refresh();
        }
    }

    void Refresh()
    {
        foreach (var shapesRenderer in renderers)
        {
            var sectorController = _vectorTerrainManager._sectorDict[shapesRenderer.Key];
            var points = sectorController.sectorData.Verts;
            var settings = new PolyLineRenderSettings(Color.red);
            var Vectors = VertexProcessing.Verts2Vectors(points).ToArray();
            var smoothed = VectorTools.SmoothLineFaster(Vectors, blurr, sigma);
            shapesRenderer.Value.Init(smoothed, settings);
        }
    }

    void Visualize(List<Vertex2> points, int id)
    {
        var renderer = CreateRenderer(id);
        // var settings = new PolyLineRenderSettings(Color.red);

        // var Vectors = VertexProcessing.Verts2Vectors(points).ToArray();

        // var smoothed = VectorTools.SmoothLineGaussian(Vectors, blurr, sigma);

        // renderer.Init(smoothed, settings);
    }

    TerrainShapesRenderer CreateRenderer(int id)
    {
        var obj = new GameObject();
        obj.transform.SetParent(_vizContainer);
        var renderer = obj.AddComponent<TerrainShapesRenderer>();
        renderers[id] = renderer;
        return renderer;
    }
    
    
    public Transform CreateContainter()
    {
        var w = FindObjectsOfType<TerrainVisualizerController>();

        foreach (var containerController in w)
        {
            if (Application.isPlaying)
                Destroy(containerController.gameObject);
            else if(Application.isEditor)
                DestroyImmediate(containerController.gameObject);
        }
            
        GameObject terrainViz = new GameObject("TerrainViz");
        terrainViz.AddComponent<TerrainVisualizerController>();

        return terrainViz.transform;
    }
    
    
    void DestroyAllVisualizers()
    {
        foreach (var viz in FindObjectsOfType<TerrainVisualizerController>())
        {
            if (Application.isPlaying)
                Destroy(viz.gameObject);
            else if(Application.isEditor)
                DestroyImmediate(viz.gameObject);
        }
    }
}
