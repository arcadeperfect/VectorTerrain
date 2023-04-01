using System;
using System.Collections.Generic;
using Nodez.Nodes;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using VectorTerrain.Scripts;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Sector;
using VectorTerrain.Scripts.Terrain;
using VectorTerrain.Scripts.Types.Exceptions;

[ExecuteAlways]
public class TerrainTester : MonoBehaviour
{
    [Required] public TerrainGraph terrainGraph;

    public bool active;
    public VisualiserConfig visualiserConfig;
    public int seed;
    public int sectors;
    
    private bool _initted;
    private Dictionary<int, SectorController> _sectorDict;
    private TerrainContainerManager _terrainContainerManager;
    private Transform _terrainContainer;
    
    public Action SectorGenerationDone;

    private void OnEnable()
    {
        terrainGraph.NodesChanged -= Init;
        terrainGraph.NodesChanged += Init;
        Init();
    }

    private void OnDisable()
    {
        UnInit();
    }

    private void OnDestroy()
    {
        UnInit();
    }

    private void OnValidate()
    {
        if (!active)
            return;
        Process();
    }

    [Button]
    private void ReInit()
    {
        if (!active)
            return;

        UnInit();
        Init();
    }

    public void Init()
    {
        if (!active)
            return;
        
        UnInit();

        
        
        _terrainContainerManager = gameObject.GetComponent<TerrainContainerManager>();
        if (_terrainContainerManager == null)
            _terrainContainerManager = gameObject.AddComponent<TerrainContainerManager>();

        _terrainContainer = _terrainContainerManager.Init();
        
        _sectorDict = new Dictionary<int, SectorController>();

        if (!ValidateVariables())
            return;

        DestroyAllSectorControllers();

        terrainGraph.NodesChanged += Init;
        terrainGraph.NodesUpdated += OnGraphUpdate;
        // terrainGraph.nodesChanged += OnGraphUpdate;
        terrainGraph.InitNodeIDs();

        _initted = true;
    }

    private void UnInit()
    {
        if (!active)
            return;

        if (!ValidateVariables())
            return;

        DestroyAllSectorControllers();

        terrainGraph.NodesChanged -= Init;
        terrainGraph.NodesUpdated -= OnGraphUpdate;
        terrainGraph.NodesChanged -= OnGraphUpdate;
        _initted = false;
    }

    private bool ValidateVariables()
    {
        // check for terrainGraph
        if (terrainGraph == null) throw new NoGraphException("Terrain Tester");
        
        // check graph for output node
        foreach (var node in terrainGraph.nodes)
            if (node.GetType() == typeof(OutputNode))
                return true;

        throw new NoOutputNodeException();
    }

    private void InitTerrainContainer()
    {
        
    }

    private void OnGraphUpdate()
    {
        SectorGenerationDone?.Invoke();
        Process();
    }

    private void Process()
    {
        if (!active)
            return;
        GenerateSectors();
    }

    [Button]
    public void GenerateSectors()
    {
        Debug.Log("botty");
        
        VectorTerrainGlobals.GlobalSeed = seed;
        DestroyAllSectorControllers();

        if (!_initted) throw new TerrainExceptions.NotInitialisedException("Terrain Tester");
        
        {
            var terrainGraphOutput = GetDataFromGraph(new TerrainGraphInput(0, 0));
            _sectorDict[0] = SectorController.New(terrainGraphOutput, _terrainContainer, visualiserConfig);
        }

        for (var i = 1; i < sectors; i++)
        {
            var terrainGraphOutput = GetDataFromGraph(new TerrainGraphInput(_sectorDict[i - 1]));
            _sectorDict[i] = SectorController.New(terrainGraphOutput, _terrainContainer, visualiserConfig);
        }
        
        SectorGenerationDone?.Invoke();
    }
    
    private void DestroyAllSectorControllers()
    {
        foreach (var sc in FindObjectsOfType<SectorController>()) sc.DestroyMe();
    }

    private TerrainGraphOutput GetDataFromGraph(TerrainGraphInput data)
    {
        var graphOutput = terrainGraph.GetGraphOutput(data, true);

        return graphOutput;
    }
}

[Serializable]
public class VisualiserConfig
{
    public bool DrawTerrain;
    public bool DrawTargets;
    public bool DrawVertexNormals;
    public bool DrawSegmentNormals;
    public bool DrawVerts;
    public bool DrawGrass;

    [Space] public bool DrawPlots;

    public bool DrawSignalPlots;
    public bool DrawWeightPlots;
    public bool DrawMaskPlots;
    public bool DrawFloatPlots;

    [Space] public Color TerrainColor;

    public Color TargetsColor;
    public Color VertexNormalsColor;
    public Color SegmentNormalsColor;
    public Color VertsColor;
    public Color GrassColor;

    [Space] public Color SignalPlotsColor;

    public Color WeightPlotsColor;
    public Color MaskPlotsColor;
    public Color FloatPlotsColor;

    [Space] public float TargetsScale = 1;

    public float VertexNormalsScale = 1;
    public float SegmentNormalsScale = 1;
    public float VertsScale = 1;
    public float plotYScale = 1;

    [Space] public float GrassDensity;
}