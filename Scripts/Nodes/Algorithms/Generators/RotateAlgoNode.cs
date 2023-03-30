using Nodez.BaseNodes;
using UnityEngine;
using VectorTerrain.Scripts.Types;
using XNode;

namespace VectorTerrain.Scripts.Nodes.Algorithms.Generators
{
    [CreateNodeMenu("Algorithms/Rotate")]
    public class RotateAlgoNode : AlgorithmNode
    {		
        [Input(typeConstraint = Node.TypeConstraint.Strict, connectionType = Node.ConnectionType.Override)]
        public SignalNoodle signal = new();

        public float signalOffset = 0;
        
        [Input(typeConstraint = Node.TypeConstraint.Strict, connectionType = Node.ConnectionType.Override)]
        public FloatNoodle seedMult = new();
        
        [Input(typeConstraint = Node.TypeConstraint.Strict, connectionType = Node.ConnectionType.Override)]
        public FloatNoodle rotation = new();

        public float rotationOffset = 0f;
        
        [Input(typeConstraint = Node.TypeConstraint.Strict, connectionType = Node.ConnectionType.Override)]
        public FloatNoodle magnitude = new();
        
        
        private const float ITERATION_MULT = 0.001f;

        public override Vertex2 Result(Vector3 vectorSeed, SectorData dataSoFar)
        {
            return GetPoint(vectorSeed, dataSoFar);
        }

        private Vertex2 GetPoint(Vector3 vectorSeed, SectorData dataSoFar)
        {
            var rotation = this.rotation.GetFloat(vectorSeed);
            var seedMult = this.seedMult.GetFloat(vectorSeed);
            var magnitude = this.magnitude.GetFloat(vectorSeed);

            rotation += rotationOffset;
            
            seedMult = Mathf.Max(0.01f, seedMult);

            var seed = vectorSeed.x * seedMult * ITERATION_MULT;
            // var _signal = GetSignal(new Vector3(seed, vectorSeed.y, vectorSeed.z));
            var _signal = signal.GetSignal(new Vector3(seed, vectorSeed.y, vectorSeed.z));
            _signal += signalOffset;
            
            // _signal = signal.Get(new Vector3(seed, vectorSeed.y, vectorSeed.z));

            magnitude = Mathf.Max(0.01f, magnitude);


            var rotateFrom = Vector2.right; // begin with right vector, rotate from here
            var degrees = Hutl.Map(_signal, -1, 1, -rotation, rotation);

            rotateFrom = Hutl.rotatePointDegrees(rotateFrom, degrees);
            rotateFrom *= magnitude;
            
            return new Vertex2(rotateFrom);
        }

        // protected override void Init()
        // {
        //     base.Init();
        //     InitNoodles();
        // }

        // protected void InitNoodles()
        // {
        //     // base.Init();
        //
        //     Debug.Log("rotate algo init");
        //     
        //     foreach (var fieldInfo in GetType().GetFields())
        //     {
        //         if (fieldInfo.HasAttribute(typeof(InputAttribute)))
        //         {
        //             NodePort port = GetPort(fieldInfo.Name);
        //             
        //             if (fieldInfo.FieldType == typeof(FloatNoodle))
        //             {
        //                 var floatNoodle = fieldInfo.GetValue(this) as FloatNoodle;
        //                 // floatNoodle = new();
        //                 floatNoodle.port = port;
        //                 // floatNoodle = new(port);
        //                 // Debug.Log(floatNoodle.port);
        //             }
        //         }
        //     }
        //     
        //     // if (idd == Guid.Empty)
        //     // {
        //     //     idd = Guid.NewGuid();
        //     // }
        // }
        
    }
}
