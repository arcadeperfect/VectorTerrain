using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using VectorTerrain.Scripts;

public class VectorTerrainManager : MonoBehaviour
{
    private VectorTerrainGeneratorAsync generator;
    public VectorTerrainGeneratorAsync Generator => generator;

    public Transform Focus;
    
    public int seed;

    // public float t;
    
    private float _advanceThreshold = 0.51f;
    
    private void Awake()
    {
        generator = GetComponent<VectorTerrainGeneratorAsync>();
        generator.Init(seed);
    }

    private void LateUpdate()
    {
        if (!generator.Initted) return;

        var activeSector = generator.middleSectorController;
        var begin = activeSector.sectorData.LocalStart.x;
        var end = activeSector.sectorData.LocalEnd.x;

        // float d = (end - begin);
        // float buffer = d * 0.25f;
        
        var p = Focus.position.x;

        if(p>end)
            generator.Advance();
        else if(p<begin)
            generator.Subvance();
        
        
        
        // if(p < begin + buffer && p > begin)
        //     return;
        //
        // if(p > end - buffer && p < end)
        //     return;
        //
        // if (p > end - buffer)
        // {
        //     generator.Advance();
        //     return;
        // }
        //
        // if (p < begin + buffer)
        // {
        //     generator.Subvance();
        //     return;
        // }
        
        
        // if(Math.Abs(p - b.x) < t || Math.Abs(p - e.x) < t)
        //     return;
        //
        // if(p > b1 && p < e1)
        //     return;
        //
        // if(p > e1)
        //     generator.Advance();
        // else if(p < b1)
        //     generator.Subvance();
        
    }

    private float SignedSimpleDist(float a, float b)
    {
        return a - b;
    }
    
    private float SimpleDist(float a, float b)
    {
        return Mathf.Abs(a - b);
    }
    
}
