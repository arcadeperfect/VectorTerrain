using System.Collections.Generic;
using ProceduralToolkit;
using Terrain;
using Unity.VisualScripting;
using UnityEngine;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Terrain;
using VectorTerrain.Scripts.Tester;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Types.Exceptions;

namespace VectorTerrain.Scripts.Sector
{
    public class SectorController : MonoBehaviour
    {
        public VisualiserConfig viz;
        public SectorData sectorData;

        [SerializeField] private GameObject startPositionObj;
        [SerializeField] private GameObject endPositionObj;

        public int Generation;


        public int pointCounter;
        public float distCounter;

        private bool _initted;

        // public Dictionary<string, List<Plot>> Plotz;
        public List<Plot> BottyPlot;
        public TerrainGraphOutput graphOutput;
        private TerrainGrassRenderer grassRenderer;
        private List<GameObject> Prefabs;

        private TerrainShapesRenderer shapesRenderer;
        private SectorColliderController colliderController;
        public Transform Start => startPositionObj.transform;
        public Transform End => endPositionObj.transform;

        private void OnDrawGizmos()
        {
            Visualise(BottyPlot);
        }

        public void SetColor(Color color)
        {
            for (var i = 0; i < sectorData.Verts.Count; i++)
            {
                var v = sectorData.Verts[i];
                v.Color = color;
                sectorData.Verts[i] = v;
            }

            var settings = new PolyLineRenderSettings(VectorTerrainGlobals.PolylineGeometry);
            shapesRenderer.Init(sectorData, settings);
        }

        // public static SectorController New(TerrainGraphOutput graphOutput, Transform parent)
        // {
        //     var v = Process(graphOutput, parent, new VisualiserConfig());
        //     // Debug.Log(v.BottyPlot[0].YVals[0]);
        //     return v;
        // }
        public static SectorController New(TerrainGraphOutput graphOutput, Transform parent, VisualiserConfig viz)
        {
            var v = Process(graphOutput, parent, viz);
            // Debug.Log(v.BottyPlot[0].YVals[0]);
            return v;
        }

        private static SectorController Process(TerrainGraphOutput graphOutput, Transform parent, VisualiserConfig viz)
        {
            if (graphOutput == null) return null;

            ValidateSectorData(graphOutput.SectorData);
            var newSector = new GameObject();
            newSector.transform.parent = parent;
            newSector.name = $"Terrain Object {graphOutput.Generation}";
            var controller = newSector.AddComponent<SectorController>();
            controller.InitSectorData(graphOutput.SectorData);
            controller.Generation = graphOutput.Generation;
            controller.viz = viz;
            controller.distCounter = graphOutput.TotalDistanceAtEnd;
            controller.pointCounter = graphOutput.TotalPointsAtEnd;
            // controller.Plotz = graphOutput.PlotListDict;
            controller.Prefabs = InstantiatePrefabs(graphOutput);
            controller.graphOutput = graphOutput; // todo just pass in this and be done with it, do not pass in all those variables above one by one. Also don't pass in the sectordata twice.
            controller.grassRenderer = controller.AddComponent<TerrainGrassRenderer>();
            controller.BottyPlot = new List<Plot>(graphOutput.PlotList);

            // Debug.Log(controller.BottyPlot[0].YVals[0]);

            controller._initted = true;

            return controller;
        }

        // public void TestPlots()
        // {
        //     Debug.Log(BottyPlot[0].YVals[0]);
        // }

        public void Visualise(List<Plot> plots)
        {
            if (!_initted) return;

            if (viz.DrawGrass)
            {
                Gizmos.color = viz.GrassColor;
                var grassPoints = TerrainGrassRenderer.ProcessGrassRegionsAbsolute(graphOutput, viz.GrassDensity);
                foreach (var point in grassPoints)
                    Gizmos.DrawLine(point.position, point.position + point.position.Normal * point.value);
                grassPoints = TerrainGrassRenderer.ProcessGrassRegionsNormalised(graphOutput, viz.GrassDensity);
                foreach (var point in grassPoints)
                    Gizmos.DrawLine(point.position, point.position + point.position.Normal * point.value);
            }

            if (viz.DrawTerrain)
                foreach (var segment in sectorData.Segments)
                {
                    Gizmos.color = viz.TerrainColor;
                    Gizmos.DrawLine(segment.a, segment.b);
                }

            if (viz.DrawVertexNormals)
            {
                Gizmos.color = viz.VertexNormalsColor;
                foreach (var vert in sectorData.Verts)
                    Gizmos.DrawLine(vert.Pos, vert.Pos + vert.Normal * viz.VertexNormalsScale);
            }

            if (viz.DrawSegmentNormals)
            {
                Gizmos.color = viz.SegmentNormalsColor;
                sectorData.ProcessSegments();
                foreach (var seg in sectorData.Segments)
                {
                    var p = seg.center;
                    var n = seg.direction.RotateCCW90();
                    Gizmos.DrawLine(p, p + n * viz.SegmentNormalsScale);
                }
            }

            if (viz.DrawVerts)
            {
                Gizmos.color = viz.VertsColor;
                foreach (var vert in sectorData.Verts) Gizmos.DrawWireSphere(vert.Pos, viz.VertsScale);
            }

            if (viz.DrawTargets)
            {
                Gizmos.color = viz.TargetsColor;
                foreach (var marker in sectorData.Markers)
                {
                    Gizmos.DrawWireSphere(marker.vert.Pos, viz.TargetsScale);
                    Gizmos.DrawLine(marker.vert.Pos, marker.vert.Pos + marker.vert.Normal * (viz.TargetsScale * 3f));
                }
            }

            if (viz.DrawPlots)
            {
                // Debug.Log(graphOutput.BottyPlot[0].YVals[0]);

                var c = 0;

                foreach (var plot in plots)
                {
                    // if(plot.YVals.Count == 0) {Debug.Log("warning empty plot");}

                    if (plot.YVals.Count == 0) continue;

                    if (plot.plotType == Plot.PlotType.Signal && !viz.DrawSignalPlots) continue;
                    if (plot.plotType == Plot.PlotType.Mask && !viz.DrawMaskPlots) continue;
                    if (plot.plotType == Plot.PlotType.Weight && !viz.DrawWeightPlots) continue;
                    if (plot.plotType == Plot.PlotType.Float && !viz.DrawFloatPlots) continue;

                    if (plot.plotType == Plot.PlotType.Signal) Gizmos.color = viz.SignalPlotsColor;
                    if (plot.plotType == Plot.PlotType.Mask) Gizmos.color = viz.MaskPlotsColor;
                    if (plot.plotType == Plot.PlotType.Weight) Gizmos.color = viz.WeightPlotsColor;
                    if (plot.plotType == Plot.PlotType.Float) Gizmos.color = viz.FloatPlotsColor;

                    var yOffset = viz.plotYScale * 2;
                    yOffset *= c;
                    plot.ScaleXVals(Start.position.x, End.position.x);

                    for (var i = 1; i < plot.Xvals.Count; i++)
                    {
                        var px = plot.Xvals[i - 1];
                        var py = plot.YVals[i - 1];
                        var x = plot.Xvals[i];
                        var y = plot.YVals[i];

                        Gizmos.DrawLine(new Vector3(px, py * viz.plotYScale + yOffset),
                            new Vector3(x, y * viz.plotYScale + yOffset));
                    }

                    c++;
                }
            }
        }

        public void InitSectorData(SectorData sectorData)
        {
            Prefabs = new List<GameObject>();

            this.sectorData = sectorData;

            startPositionObj = new GameObject();
            startPositionObj.name = "Start";
            startPositionObj.transform.parent = transform;

            endPositionObj = new GameObject();
            endPositionObj.name = "End";
            endPositionObj.transform.parent = transform;

            startPositionObj.transform.position = sectorData.LocalStart;
            endPositionObj.transform.position = sectorData.LocalEnd;

            shapesRenderer = gameObject.AddComponent<TerrainShapesRenderer>();
            var settings = new PolyLineRenderSettings(VectorTerrainGlobals.PolylineGeometry);
            shapesRenderer.Init(this.sectorData, settings);
            
            colliderController = gameObject.AddComponent<SectorColliderController>();
            var colliderSettings = new SectorColliderSettings();
            colliderController.Init(this.sectorData, colliderSettings);
        }

        private static void ValidateSectorData(SectorData sectorData)
        {
            if (sectorData.LocalEnd == null)
                throw new TerrainExceptions.InvalidSectorDataException("Local End not initialised");
            if (sectorData.LocalEnd == Vector2.zero && sectorData.LocalStart == Vector2.zero)
                throw new TerrainExceptions.InvalidSectorDataException("Local End invalid");
        }

        public void DestroyMe()
        {
            if (Application.isPlaying)
            {
                if (Prefabs != null)
                    foreach (var g in Prefabs)
                        Destroy(g);
                Destroy(gameObject);
            }
            else
            {
                if (Prefabs != null)
                    foreach (var g in Prefabs)
                        DestroyImmediate(g);
                DestroyImmediate(gameObject);
            }
        }

        private static List<GameObject> InstantiatePrefabs(TerrainGraphOutput graphOutput)
        {
            List<GameObject> returnMe = new();

            // var prefabManager = FindObjectOfType<TerrainPrefabManager>();
            // var v = graphOutput.sectorData.Markers;
            //
            // GameObject p = null;
            //
            // foreach (var marker in v)
            // {
            //     if (marker.markerType == 0)
            //     {
            //         p = Instantiate(prefabManager.prefab_00);
            //         p.transform.position = marker.vert.Pos;
            //         p.transform.parent = prefabManager.transform;
            //     }
            //
            //     if (p != null)
            //         returnMe.Add(p);
            // }
            return returnMe;
        }
    }
}