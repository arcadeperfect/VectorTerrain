using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using VectorTerrain.Scripts;

public class VectorTerrainManager : MonoBehaviour
{
    // private VectorTerrainGeneratorAsync generator;
    // public VectorTerrainGeneratorAsync Generator => generator;

    // private VectorTerrainGenerator generator;
    //
    // private VectorTerrainGeneratorAsync generatorAsync;

    
    // private ITerrainGenerator generator;

    private VectorTerrainGeneratorAsync generator;
    
    public Transform Focus;
    
    public int seed;

    // public float t;

    public bool async;
    
    private float _advanceThreshold = 0.51f;
    
    private async void Awake()
    {
        // if(async)
        //     generator = gameObject.GetComponent<VectorTerrainGeneratorAsync>() as ITerrainGenerator;
        // else
        //     generator = gameObject.GetComponent<VectorTerrainGenerator>() as ITerrainGenerator;
        
        generator = gameObject.GetComponent<VectorTerrainGeneratorAsync>();
        await generator.Init(seed);
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
    
        // if(Input.GetKeyDown(KeyCode.S)) generator.Subvance();
        // if(Input.GetKeyDown(KeyCode.A)) generator.Advance();


        Debug.Log(generator.AreTasksRunning());
        if (generator.AreTasksRunning()) return;
        
        if(p>end)
            generator.Advance();
        else if(p<begin)
            generator.Subvance();
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
