using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using VectorTerrain.Scripts;

public class VectorTerrainManager : MonoBehaviour
{
    public int seed;
    public Transform Focus;

    public bool async;

    private VectorTerrainGeneratorAsync generatorAsync;
    private VectorTerrainGenerator generator;


    private float _advanceThreshold = 0.51f;
    
    private async void Awake()
    {
        if (async)
        {
            generatorAsync = gameObject.GetComponent<VectorTerrainGeneratorAsync>();
            await generatorAsync.Init(seed);
        }
        else
        {
            generator = gameObject.GetComponent<VectorTerrainGenerator>();
            generator.Init(seed);
        }
    }

    private void LateUpdate()
    {
        if (async)
        {
            if (!generatorAsync.Initted) return;
            if (generatorAsync.AreTasksRunning()) return;

            var activeSector = generator.middleSectorController;
            var begin = activeSector.sectorData.LocalStart.x;
            var end = activeSector.sectorData.LocalEnd.x;
            var p = Focus.position.x;

            if (p > end)
                generator.Advance();
            else if (p < begin)
                generator.Subvance();
        }
        else
        {
            if (!generator.Initted) return;
            var activeSector = generator.middleSectorController;
            var begin = activeSector.sectorData.LocalStart.x;
            var end = activeSector.sectorData.LocalEnd.x;
            var p = Focus.position.x;

            if (p > end)
                generator.Advance();
            else if (p < begin)
                generator.Subvance();
        }
    }
}
