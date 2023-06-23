using System;
using UnityEngine;
using VectorTerrain.Scripts.Types;

namespace VectorTerrain.Scripts
{
    [ExecuteAlways]
    public class Temp: MonoBehaviour
    {
        public float botty;
        public FloatNoodle noodleboodle;

        public void OnValidate()
        {
            Debug.Log(botty);
            Debug.Log(noodleboodle.value);
        }
    }
}