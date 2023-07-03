using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VectorTerrain.Scripts;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Sector;
using VectorTerrain.Scripts.Types;

public class VectorTerrainManager : MonoBehaviour
{
    public int seed;
    public Transform focus;
    public bool async;
    public TerrainGraph graph;
    
    private VectorTerrainGeneratorAsync _generatorAsync;
    private VectorTerrainGenerator _generator;
    
    public Dictionary<int, SectorController> SectorDict => _generatorAsync.SectorDict;


    private async void Awake()
    {
        if (async)
        {
            _generator = null;
            _generatorAsync = gameObject.GetComponent<VectorTerrainGeneratorAsync>();
            if (_generatorAsync == null) throw new Exception("GeneratorAsync component not found");
            await _generatorAsync.InitAsync(seed);
        }
        
        else
        {
            _generatorAsync = null;
            _generator = gameObject.GetComponent<VectorTerrainGenerator>();
            if (_generator == null) throw new Exception("Generator component not found");
            _generator.Init(seed);
        }
    }
    
    
    private void LateUpdate()
    {
        if (async)
        {
            if (!_generatorAsync.Initted) return;
            if (_generatorAsync.AreTasksRunning()) return;
            if (_generatorAsync == null) Debug.Log("generator async was null"); //todo remove when no longer needed
            
            var activeSector = _generatorAsync.MiddleSectorController;
            var begin = activeSector.sectorData.LocalStart.x;
            var end = activeSector.sectorData.LocalEnd.x;
            var p = focus.position.x;

            if (p > end)
                Advance();
            else if (p < begin)
                Subvance();
        }
        else
        {
            if (!_generator.Initted) return;
            if (_generator == null) Debug.Log("generator was null"); //todo remove when no longer needed
            
            var activeSector = _generator.middleSectorController;
            var begin = activeSector.sectorData.LocalStart.x;
            var end = activeSector.sectorData.LocalEnd.x;
            var p = focus.position.x;

            if (p > end) 
                Advance();
            else if (p < begin) 
                Subvance();
        }
    }

    [Button]
    void Generate()
    {
        if (async)
        {
            if (_generatorAsync == null)
            {
                Awake();
                return;
            }
            _generatorAsync.InitAsync(seed);
            // _generatorAsync.Init(seed);

        }

        else
        {
            if (_generator == null)
            {
                Awake();
                return;
            }
            _generator.Init(seed);
        }
    }

    [Button]
    void Advance()
    {
        if (async)
        {
            _generatorAsync.Advance();
        }

        else
        {
            _generator.Advance();   
        }
    }

    [Button]
    void Subvance()
    {
        if (async)
        {
            _generatorAsync.Subvance();
        }

        else
        {
            _generator.Subvance();   
        }
    }
}
