using System;
using System.Collections.Generic;
using Nodez;
using Nodez.Nodes;
using Sirenix.OdinInspector;
using UnityEngine;
using VectorTerrain.Scripts;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Sector;


[ExecuteAlways]
public class TerrainTester : SerializedMonoBehaviour
{
    public bool active;

    public VisualiserConfig visualiserConfig;

    [Required] public TerrainGraph terrainGraph;
    public int seed;
    public int sectors;
    private Dictionary<int, SectorController> _sectorDict;
    private bool _initted = false;

    public Action SectorGenerationDone;

    private void OnValidate()
    {
        if (!active)
            return;
        Process();
    }

    private void OnEnable()
    {
        terrainGraph.NodesChanged -= Init;
        terrainGraph.NodesChanged += Init;
        Init();
    }

    private void OnDestroy()
    {
        UnInit();
    }

    private void OnDisable()
    {
        UnInit();
    }

    [Button]
    void ReInit()
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
        _sectorDict = new();

        if (!ValidateVariables())
            return;

        DestroyAllSectorControllers();

        terrainGraph.NodesChanged += Init;
        terrainGraph.NodesUpdated += OnGraphUpdate;
        // terrainGraph.nodesChanged += OnGraphUpdate;
        terrainGraph.InitNodeIDs();

        _initted = true;
    }

    void UnInit()
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
        if (terrainGraph != null)
        {
            foreach (var node in terrainGraph.nodes)
            {
                if (node.GetType() == typeof(OutputNode))
                    return true;
            }

            Debug.Log("no output node fuckface");
            return false;
        }

        print("connect a terrainGraph fuckface");
        return false;
    }

    void OnGraphUpdate()
    {
        SectorGenerationDone?.Invoke();
        Process();
    }

    void Process()
    {
        if (!active)
            return;
        GenerateSectors();
    }

    [Button]
    public void GenerateSectors()
    {
        Globals.GlobalSeed = this.seed;
        DestroyAllSectorControllers();

        if (!_initted)
        {
            print("not initted fuckface");
            return;
        }

        {
            var terrainGraphOutput = GetDataFromGraph(new TerrainGraphInput(0, 0));
            _sectorDict[0] = SectorController.New(terrainGraphOutput, transform, visualiserConfig);
        }
        
        for (int i = 1; i < sectors; i++)
        {
            var terrainGraphOutput = GetDataFromGraph(new TerrainGraphInput(_sectorDict[i - 1]));
            _sectorDict[i] = SectorController.New(terrainGraphOutput, transform, visualiserConfig);
        }

        
        SectorGenerationDone?.Invoke();
    }

    private void DestroyAllSectorControllers()
    {
        foreach (var sc in GameObject.FindObjectsOfType<SectorController>())
        {
            sc.DestroyMe();
        }
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
    [Space]
    public bool DrawPlots;
    public bool DrawSignalPlots;
    public bool DrawWeightPlots;
    public bool DrawMaskPlots;
    public bool DrawFloatPlots;
    [Space]
    public Color TerrainColor;
    public Color TargetsColor;
    public Color VertexNormalsColor;
    public Color SegmentNormalsColor;
    public Color VertsColor;
    public Color GrassColor;
    [Space] 
    public Color SignalPlotsColor;
    public Color WeightPlotsColor;
    public Color MaskPlotsColor;
    public Color FloatPlotsColor;

    [Space] 
    public float TargetsScale = 1;
    public float VertexNormalsScale = 1;
    public float SegmentNormalsScale = 1;
    public float VertsScale = 1;
    public float plotYScale =1;
    [Space] 
    public float GrassDensity;
}