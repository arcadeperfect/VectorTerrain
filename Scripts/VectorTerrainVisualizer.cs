
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Clipper2Lib;
using ProceduralToolkit;
using Sirenix.OdinInspector;
using Terrain;
using UnityEngine;
using VectorTerrain;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Utils;
using VectorTerrain.Scripts.Visualizer;
using VectorTools;
using static VectorTools.ClipperOffsetTools;
using CavalierContours;
using static CavalierContours.Interop;

public class VectorTerrainVisualizer : MonoBehaviour
{
    public bool SavitzkyGolay;
    public int savitzkyGolayWindowSize;
    public int polyNomialOrder;
    [Space]
    
    public bool BilateralFilter;
    public int BilateralWindowSize;
    public float BilateralSpatialSigma;
    public float BilateralIntensitySigma;
    [Space]
    public bool RamerDouglasPeucker;
    public float ramerDouglasPeuckerTolerance = 0.1f;
    [Space]
    public bool ChaikinsCorner;
    public int chaikinsCornerIterations = 3;
    [Space]
    public bool removeIntersections;
    [Space]
    public bool removeCloseVertices;
    public float closeVerticesTolerance = 0.1f;
    [Space] 
    public bool resample;
    public float resampleDistance;
    public int resampleCount;
    [Space]
    public bool removeColinearPoints;
    public float colinearTolerance = 0.1f;
    [Space]
    public bool smooth;
    public int blurr;
    public float sigma;
    [Space]
    public bool offset;
    public double offsetDelta;
    [Space]
    
    public int clipperScale = 100000;
    public float proximityTolerance = 1f;
    public bool closed;
    
    
    
    
    
    [Space]
    
    // [Min(0)]
    
    public double arcTollerance = 0.1;
    public double miterLimit = 0.1;
    public JoinType joinType = JoinType.Miter;
    public EndType endType = EndType.Butt;

    [Space] 
    public double posEqualEps;
    public double sliceJoinEps;
    public double offsetDistEps;
    public bool handleSelfIntersects;
    public bool isClosed;
    
    private VectorTerrainManager _vectorTerrainManager;
    private TerrainGraph graph;
    private Transform _vizContainer;
    private GameObject _vizObject;
    private TerrainVisualizerController _vizController;
    private Dictionary<int, TerrainShapesRenderer> renderDict = new();

    private PathOffsetter pathOffsetter;
    
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
        pathOffsetter = new PathOffsetter(arcTollerance, miterLimit);
        
        DestroyAllVisualizers();
        
        _vectorTerrainManager = GetComponent<VectorTerrainManager>();
        // graph = _vectorTerrainManager.graph;
        _vizContainer = CreateContainter();
        _vizController = _vizContainer.GetComponent<TerrainVisualizerController>();
        _vizObject = _vizContainer.gameObject;
        _vectorTerrainManager.OnRefresh += Refresh;
    }

    [Button]
    void Do()
    {
        Init();
        renderDict.Clear();
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
        foreach (var shapesRenderer in renderDict)
        {
            var sectorController = _vectorTerrainManager._sectorDict[shapesRenderer.Key];
            var points = sectorController.sectorData.Verts;
            var settings = new PolyLineRenderSettings(2, Color.red, closed, 2);
            var vectors = VertexProcessing.Verts2Vectors(points).ToArray();
            
            // if(SavitzkyGolay) vectors = VectorPathFiltering.ApplySavitzkyGolayFilter(vectors, savitzkyGolayWindowSize, polyNomialOrder);
            if(BilateralFilter) vectors = VectorPathFiltering.BilateralFilter(vectors, BilateralWindowSize, BilateralSpatialSigma, BilateralIntensitySigma);
            if(RamerDouglasPeucker) vectors = VectorPathFiltering.ApplyRamerDouglasPeucker(vectors, ramerDouglasPeuckerTolerance);
            if(ChaikinsCorner) vectors = VectorPathFiltering.ChaikinsCornerCutting(vectors, chaikinsCornerIterations);
            if(removeIntersections) vectors = VectorPathCleanup.RemoveSelfIntersections(vectors);
            if(removeCloseVertices) vectors = VectorPathCleanup.RemoveCloseVertices(vectors, closeVerticesTolerance);
            if(resample) vectors = VectorPathFiltering.ResamplePathByPointCount(vectors, resampleCount);
            if (removeColinearPoints) vectors = VectorPathCleanup.RemoveCollinearPoints(vectors, colinearTolerance);
            if(smooth) vectors = VectorTools.VectorPathFiltering.SmoothLineFaster(vectors, blurr, sigma);
            if(offset) vectors = Expand(vectors);

            shapesRenderer.Value.Init(vectors, settings);
        }
    }

    public Vector2[] Expand(Vector2[] vectors)
    {
        Vector2[] v = vectors;
        
        var options = new OffsetOptions();
        options.PosEqualEps = posEqualEps;
        options.SliceJoinEps = sliceJoinEps;
        options.OffsetDistEps = offsetDistEps;
        options.HandleSelfIntersects = handleSelfIntersects;

        v = CavcUnity.ParralelOffset(vectors, (float) offsetDelta, isClosed);

        ClipperOffsetTools.ClipperScale = clipperScale;

        // ClipperOffset offset = new ClipperOffset(arcTollerance, miterLimit);
        // offset.AddPath(ToPath64(vectors), joinType, endType);
        // Paths64 solution = new Paths64();
        // offset.Execute(offsetDelta * clipperScale, solution);
        // vectors = ToVector2Path(solution[0]).ToArray();
        // vectors = VectorTools.VectorTools.RemoveClosePoints(vectors, proximityTolerance);

        // Paths64 solution = new Paths64();
        // var v = ToPath64(vectors);
        // var w = new Paths64();
        // w.Add(v);
        // var result =Clipper.InflatePaths(w,offsetDelta*clipperScale,joinType,endType, miterLimit);
        // vectors = ToVector2Path(result[0]).ToArray();
        // vectors = VectorTools.VectorTools.RemoveClosePoints(vectors, proximityTolerance);

        // var v = PolylineOffset.OffsetPolyline(vectors.ToList(), (float)offsetDelta).ToArray();
        // vectors = v;

        // using (CCPath path = new CCPath())
        // {
        //     path.PushVertex(0, 0);
        //     path.PushVertex(1, 0);
        //     path.PushVertex(1, 1);
        //
        //     using (CCPath offsetPath = path.ComputeOffset(0.1f, true, true))
        //     {
        //         // Do something with the offsetPath
        //     }
        // }
                
        // ClipperOffset offset = new ClipperOffset(arcTollerance, miterLimit);
        // var v = ToIntPath(vectors.ToList());
        // offset.AddPath(vectors, joinType, endType);
        
        
        
        return v;
    }

    // IntPtr GetOptions(IntPtr pline, double posEqualEps, double sliceJoinEps, double offsetDistEps, bool HandleSelfIntersects)
    // {
    //     IntPtr aabbindex;
    //
    //     int result = cavc_pline_create_approx_aabbindex(pline, out aabbindex);
    //     
    //     if (result != 0) Debug.Log("Error: The pline is empty");
    //     
    //     CavcPlineParallelOffsetO cavcPlineParallelOffsetO = new CavcPlineParallelOffsetO
    //     {
    //         AabbIndex = aabbindex,
    //         PosEqualEps = posEqualEps,
    //         SliceJoinEps = sliceJoinEps,
    //         OffsetDistEps = offsetDistEps,
    //         HandleSelfIntersects = (byte)(HandleSelfIntersects ? 1 : 0)
    //     };
    //
    //     IntPtr cavcPlineParallelOffsetOPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CavcPlineParallelOffsetO)));
    //     Marshal.StructureToPtr(cavcPlineParallelOffsetO, cavcPlineParallelOffsetOPtr, false);
    //     
    //     return cavcPlineParallelOffsetOPtr;
    // }

    
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
        renderDict[id] = renderer;
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
    
    // public List<List<Vector2>> OffsetPathOneDirection(Vector2[] inputPath, float offsetDelta)
    // {
    //     // Convert the inputPath array to a List<Vector2>
    //     List<Vector2> inputList = new List<Vector2>(inputPath);
    //
    //     // Create a new PathOffsetter object
    //     PathOffsetter pathOffsetter = new PathOffsetter();
    //
    //     // Add the inputPath to the pathOffsetter object
    //     pathOffsetter.AddPath(inputList, joinType, endType);
    //
    //     // Prepare lists to store the result of the offset operations
    //     List<List<Vector2>> offsetResultPositive = new List<List<Vector2>>();
    //     List<List<Vector2>> offsetResultNegative = new List<List<Vector2>>();
    //
    //     // Perform the offset operation with the positive offsetDelta
    //     pathOffsetter.Offset(ref offsetResultPositive, offsetDelta);
    //
    //     // Perform the offset operation with the negative offsetDelta
    //     pathOffsetter.Offset(ref offsetResultNegative, -offsetDelta);
    //
    //     // Perform the union operation on the two offset results
    //     Clipper clipper = new Clipper();
    //     clipper.AddPaths(ClipperUtility.ToIntPaths(offsetResultPositive), PolyType.ptSubject, true);
    //     clipper.AddPaths(ClipperUtility.ToIntPaths(offsetResultNegative), PolyType.ptClip, true);
    //
    //     List<List<IntPoint>> unionResult = new List<List<IntPoint>>();
    //     clipper.Execute(ClipType.ctUnion, unionResult);
    //
    //     // Convert the union result to List<List<Vector2>>
    //     List<List<Vector2>> finalResult = new List<List<Vector2>>();
    //     ClipperUtility.ToVector2Paths(unionResult, ref finalResult);
    //
    //     return finalResult;
    // }
    
    
}
