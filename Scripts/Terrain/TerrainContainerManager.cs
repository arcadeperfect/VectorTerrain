using UnityEngine;

namespace VectorTerrain.Scripts.Terrain
{
    public class TerrainContainerManager: MonoBehaviour
    {
        public Transform Init()
        {
            var v = FindObjectsOfType<TerrainContainerManager>();

            foreach (var container in v)
            {
                if (container != this)
                {
                    if(Application.isPlaying)
                        Destroy(container.gameObject);
                    else if(Application.isEditor)
                        DestroyImmediate(container.gameObject);
                }
            }

            var w = FindObjectsOfType<TerrainContainerController>();

            foreach (var containerController in w)
            {
                if (Application.isPlaying)
                    Destroy(containerController.gameObject);
                else if(Application.isEditor)
                    DestroyImmediate(containerController.gameObject);
            }
            
            GameObject terrainContainer = new GameObject("TerrainContainer");
            terrainContainer.AddComponent<TerrainContainerController>();

            return terrainContainer.transform;
        }
    }
}