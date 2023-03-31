using System;
using Shapes;
using UnityEngine;
using VectorTerrain.Scripts;

public class TerrainShapesRenderSettings : MonoBehaviour
{

    public Color DefaultColor = Color.cyan;
    public PolylineGeometry Geometry = PolylineGeometry.Flat2D;

    private void OnValidate()
    {
        VectorTerrainGlobals.PolylineGeometry = Geometry;
    }
}
