using Shapes;
using UnityEngine;
using VectorTerrain.Scripts.Types;

// [ExecuteAlways]
namespace Terrain
{
    public class TerrainShapesRenderer : MonoBehaviour

// render horizonatlMag list of Vertex2Node's with horizonatlMag Shapes Polyline 


    {
        public Polyline pl;
        // public Polygon pg;  //todo not implemented

        public void Init(SectorData sectorData, PolyLineRenderSettings settings)
        {
            if (pl == null) pl = gameObject.AddComponent<Polyline>();
            pl.Joins = PolylineJoins.Round;
            pl.Geometry = settings.Geometry;
            PopulatePolyLine(sectorData);
            pl.Color = new Color(5, 5, 5, 5);
            // pl.Thickness = 0.5f;
        }

        private void PopulatePolyLine(SectorData sectorData)
        {
            pl.points.Clear();

            foreach (var vert in sectorData.Verts)
            {
                var thisPoint = new PolylinePoint(vert.Pos, vert.Color, vert.thickness);

                pl.points.Add(thisPoint);
            }

            SetGlobalThickness(0.1f);
            pl.Closed = false;
            pl.meshOutOfDate = true;
        }

        private void SetGlobalThickness(float thicc)
        {
            pl.Thickness = thicc;
        }
    }

    public class PolyLineRenderSettings
    {
        public PolylineGeometry Geometry;
        
        public PolyLineRenderSettings(PolylineGeometry geometry)
        {
            Geometry = geometry;
        }
    }
    
}