using System.Collections.Generic;
using System.Linq;
using Shapes;
using UnityEngine;
using VectorTerrain.Scripts.Types;

// [ExecuteAlways]
namespace Terrain
{
    /// <summary>
    /// Renders a list of Vertex2Node's with a Shapes Polyline
    /// </summary>
    public class TerrainShapesRenderer : MonoBehaviour

    // render horizonatlMag list of Vertex2Node's with horizonatlMag Shapes Polyline 


    {
        public Polyline pl;
        // public Polygon pg;  //todo not implemented

        public void Init(Vector2[] points)
        {
            Init(points, new PolyLineRenderSettings());
        }
        public void Init(List<Vertex2> points)
        {
            Init(points, new PolyLineRenderSettings());
        }
        public void Init(SectorData sectorData, PolyLineRenderSettings settings)
        {
            Init(sectorData.Verts, settings);
        }
        public void Init(List<Vertex2> points, PolyLineRenderSettings settings)
        {
            // Init(points.Select(v => v.Pos).ToList(), settings);
            Init(points.Select(v => v.Pos).ToArray(), settings);
        }
        public void Init(Vector2[] points, PolyLineRenderSettings settings)
        {
            if (pl == null) pl = gameObject.AddComponent<Polyline>();
            pl.Joins = PolylineJoins.Round;
            pl.Geometry = settings.Geometry;
            PopulatePolyLine(points);
            pl.Color = settings.Color;
            pl.Thickness = settings.Thickness;
        }

        
        private void PopulatePolyLine(Vector2[] points)
        {
            pl.points.Clear();

            foreach (var vert in points)
            {
                var thisPoint = new PolylinePoint((Vector2)vert, Color.white, 0.1f);
                pl.points.Add(thisPoint);
            }
            SetGlobalThickness(0.1f);
            pl.Closed = false;
            pl.meshOutOfDate = true;
        }
        
        // private void PopulatePolyLine(List<Vertex2> points)
        // {
        //     pl.points.Clear();
        //
        //     foreach (var vert in points)
        //     {
        //         var thisPoint = new PolylinePoint((Vector2)vert, Color.white, 0.1f);
        //         pl.points.Add(thisPoint);
        //     }
        //     SetGlobalThickness(0.1f);
        //     pl.Closed = false;
        //     pl.meshOutOfDate = true;
        // }
        
        

        private void SetGlobalThickness(float thicc)
        {
            pl.Thickness = thicc;
        }
    }

    public class PolyLineRenderSettings
    {
        public PolylineGeometry Geometry;
        public float Thickness = 1;
        public Color Color = UnityEngine.Color.cyan;
        
        public PolyLineRenderSettings()
        {
            Geometry = PolylineGeometry.Flat2D;
        }
        
        // public PolyLineRenderSettings(PolylineGeometry geometry)
        // {
        //     Geometry = geometry;
        // }
        
        public PolyLineRenderSettings(Color color)
        {
            this.Color = color;
            Geometry = PolylineGeometry.Flat2D;
        }
        
        public PolyLineRenderSettings(PolylineGeometry geometry, float thickness, Color color)
        {
            Geometry = geometry;
            Thickness = thickness;
            Color = color;
        }
        
        public PolyLineRenderSettings(float thickness, Color color)
        {
            Geometry = PolylineGeometry.Flat2D;
            Thickness = thickness;
            Color = color;
        }
    }
}