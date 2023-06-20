using Shapes;
using Terrain;
using UnityEngine;
using VectorTerrain.Scripts.Types;

namespace VectorTerrain.Scripts.Sector
{
    public class SectorColliderController : MonoBehaviour
    {
        public new EdgeCollider2D collider2D;
        
        public void Init(SectorData sectorData, SectorColliderSettings settings)
        {
            if (collider2D == null) collider2D = gameObject.AddComponent<EdgeCollider2D>();
            
            PopulateCollider(sectorData);
        }
        
        private void PopulateCollider(SectorData sectorData)
        {
            collider2D.points = new Vector2[sectorData.Verts.Count];
            var points = new Vector2[sectorData.Verts.Count];
            for (int i = 0; i < sectorData.Verts.Count; i++)
            {
                var point = (Vector2)sectorData.Verts[i];
                points[i] = point;
            }
            collider2D.points = points;
        }
        
        
    }

    public class SectorColliderSettings
    {
        
    }
}