﻿using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using VectorTerrain.Scripts.Attributes;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Utils;

namespace VectorTerrain.Scripts.Nodes.Filters
{
    [CreateNodeMenu("Filters/Blur")]
    public class BlurNode: GeometryModifierNode
    {
        [Input(typeConstraint = TypeConstraint.Strict)]
        
        [Test(4)]
        public FloatNoodle BlurAmount = new FloatNoodle(1f);

        

        [Input(typeConstraint = TypeConstraint.Strict)] [Range(1, 100)]
        public int windowSize = 0;
        protected override SectorData Process(SectorData input)
        { 
            var blurAmount = GetInputValue(nameof(BlurAmount), BlurAmount).value;
            if(blurAmount == 0f) return input;
            
            int w = windowSize;
            if (w % 2 == 0) w++;

            // var blurredVerts = VertexProcessing.Gaussian(input.Verts, blurAmount, w);
            input.Verts = VertexProcessing.Gaussian(input.Verts, blurAmount, w);;

            return input;
        }
    }
}