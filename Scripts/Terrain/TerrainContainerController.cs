using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VectorTerrain.Scripts.Terrain
{
    public class TerrainContainerController: MonoBehaviour
    {

        private void OnDestroy()
        {
            DestroyChildren();  
        }


        [Button]
        private void DestroyChildren()
        {
            var children = new List<GameObject>();
            foreach (Transform child in transform) children.Add(child.gameObject);
            
            if(Application.isPlaying)
                children.ForEach(child => Destroy(child));
            else if (Application.isEditor)
                children.ForEach(child => DestroyImmediate(child));
        }
    }
}