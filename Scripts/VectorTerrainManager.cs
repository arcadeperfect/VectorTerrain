using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VectorTerrain.Scripts;

public class VectorTerrainManager : MonoBehaviour
{
    private VectorTerrainGeneratorAsync generator;
    public VectorTerrainGeneratorAsync Generator => generator;

    public Transform Focus;
    
    public int seed;

    private void Awake()
    {
        generator = GetComponent<VectorTerrainGeneratorAsync>();
    }

    // [Button]
    // private void Start()
    // {
    //     generator.Init(seed);
    // }

    [Button]
    void Advance()
    {
        generator.Advance();
    }
}
