using System;
using System.Collections.Generic;
using UnityEngine;
using Drawing;
using VectorTerrain.Scripts.Types;

namespace VectorTerrain.Scripts
{
    // [ExecuteAlways]
    public class DebugVisualiser: MonoBehaviour
    {
        private VectorTerrainManager manager;


        private void OnEnable()
        {
            Awake();
        }

        private void Awake()
        {
            manager = gameObject.GetComponent<VectorTerrainManager>();
            if (manager == null) throw new Exception("Manager not found");
        }
        void Update()
        {
            if(manager.SectorDict == null) return;

            foreach (var pair in manager.SectorDict)
            {
                var v = pair.Value.sectorData.AverageLine;
                if(v == null) Debug.Log("averageLine was null");
                else DrawVertexes(v);
            }
        }
        void DrawVertexes(List<Vertex2>Verts)
        {

            for (int i = 1; i < Verts.Count; i++)
            {
                // Draw.Line(Verts[i-1], Verts[i], Color.red);
                Debug.DrawLine(Verts[i-1], Verts[i], Color.red);
            }
        }
    }
}