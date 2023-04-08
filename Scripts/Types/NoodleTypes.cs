using System;
using System.Linq;
using Sirenix.OdinInspector;
using Terrain;
using UnityEngine;
using VectorTerrain.Scripts.Nodes.Floats;
using VectorTerrain.Scripts.Nodes.Mask;
using VectorTerrain.Scripts.Nodes.Weights;
using XNode;

namespace VectorTerrain.Scripts.Types
{
    [Serializable]
    public class AlgorithmNoodle
    {
    }

    [Serializable]
    public class SectorDataNoodle
    {
    }

    [Serializable]
    [InlineProperty]
    public class WeightNoodle
    {
        [Range(0, 1)] [HideLabel] public float value;
        [HideInInspector] public NodePort port; // todo make private, use constructor
        [HideInInspector] public Plot plot;

        public float GetWeight(Vector3 vectorSeed, bool generatePlot = true)
        {
            float returnMe;

            if (!port.IsConnected)
            {
                returnMe = value;
                // if(generatePlot) plot.YVals.Add(returnMe);
                return returnMe;
            }

            var node = port.Connection.node as ReturnWeightNode;
            returnMe = node.Get(vectorSeed);
            // if(generatePlot) plot.YVals.Add(returnMe);

            return returnMe;
        }
    }

    [Serializable]
    [InlineProperty]
    public class SignalNoodle
    {
        [Range(-1, 1)] [HideLabel] public float value;
        [HideInInspector] public NodePort port; // todo make private, use constructor
        [HideInInspector] public Plot plot;


        public float GetSignal(Vector3 vectorSeed, bool generatePlot = true)
        {
            float returnMe;
            if (!port.IsConnected)
            {
                returnMe = value;
                if (generatePlot) plot.YVals.Add(returnMe);
                return returnMe;
            }

            // Debug.Log("begin debug");
            // Debug.Log(port.Connection.node == null);
            // Debug.Log(port.Connection.node as ReturnSignalNode == null);
            var node = port.Connection.node as ReturnSignalNode;
            returnMe = node.Get(vectorSeed);
            if (generatePlot) plot.YVals.Add(returnMe);
            return returnMe;
        }
    }

    [Serializable]
    public class FloatNoodle
    {
        [HideLabel] public float value = 1;
        [HideInInspector] public NodePort port; // todo make private, use constructor
        [HideInInspector] public Plot plot;

        public float GetFloat(Vector3 vectorSeed)
        {
            float returnMe;
            if (!port.IsConnected)
            {
                returnMe = value;
                plot.YVals.Add(returnMe);
                return returnMe;
            }

            var node = port.Connection.node as ReturnFloatNode;
            returnMe = node.GetFloat(vectorSeed);
            plot.YVals.Add(returnMe);
            return returnMe;
        }
    }

    [Serializable]
    [InlineProperty]
    public class MaskNoodle
    {
        [HideLabel] public float value = 1;
        [HideInInspector] public NodePort port; // todo make private, use constructor
        [HideInInspector] public Plot plot;

        public float[] GetMask(Vector3 vectorSeed, int totalIterations)
        {
            var returnMe = new float[totalIterations];

            if (!port.IsConnected)
            {
                Array.Fill(returnMe, value);
                plot.YVals = returnMe.ToList();
                return returnMe;
            }

            var node = port.Connection.node as ReturnMaskNode;
            returnMe = node?.GetMask(vectorSeed, totalIterations);
            plot.YVals = returnMe.ToList();
            return returnMe;
        }

        public bool IsConnected()
        {
            return port.IsConnected;
        }
    }
}