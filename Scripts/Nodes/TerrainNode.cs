using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.Utilities;
using Terrain;
using Unity.VisualScripting;
using UnityEngine;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Nodes.Floats;
using VectorTerrain.Scripts.Nodes.Weights;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Types.Interfaces;
using XNode;

namespace VectorTerrain.Scripts.Nodes
{
    public abstract class TerrainNode : Node, IRealTimeUpdate, IPersistentSeed
    {
        [HideInInspector] [CanBeNull] public string ID;

        private bool _noodlesInitted;

        protected SeedContainer thisNodeSeedContainer;

        protected TerrainGraph Graph => graph as TerrainGraph;
        protected int GlobalSeed => Graph.GlobalSeed;
        public int Generation => Graph.Generation;
        protected TerrainGraphInput TerrainGraphInput => Graph.terrainGraphInput;
        protected float BeginDist => TerrainGraphInput.totalDistanceSoFar;
        public float BeginPoints => TerrainGraphInput.totalPointsSoFar;

        public Vector3 VectorSeed => SeedContainer.vectorSeed;

        public virtual void OnDestroy()
        {
            Graph.GenerationStart -= OnGenerationStart;
            Graph.GenerationStart -= OnGenerationEnd;
        }

        public SeedContainer SeedContainer
        {
            get => thisNodeSeedContainer;
            set => thisNodeSeedContainer = value;
        }

        public event Action NodeUpdateEvent;

        public void resetSeedContainer()
        {
            thisNodeSeedContainer.Reset();
        }

        protected override void Init()
        {
            base.Init();

            Graph.GenerationStart += OnGenerationStart;
            Graph.GenerationEnd += OnGenerationEnd;
            InitNoodles();
            Graph.OnNodesChanged();
        }

        public void InitNoodles()
        {
            foreach (var fieldInfo in GetType().GetFields())
            {
                if (!fieldInfo.HasAttribute(typeof(InputAttribute))) continue;

                var port = GetPort(fieldInfo.Name);

                // Init WeightNoodles

                if (fieldInfo.FieldType == typeof(WeightNoodle))
                {
                    var noodle = fieldInfo.GetValue(this) as WeightNoodle;

                    if (noodle == null)
                        throw new NullReferenceException("WeightNoodle is null, did you forget to initialise?");

                    noodle.port = port;
                    noodle.plot = new Plot(Plot.PlotType.Weight);
                    Graph.graphPlotList.Add(noodle.plot);
                }

                // Init SignalNoodles

                if (fieldInfo.FieldType == typeof(SignalNoodle))
                {
                    var noodle = fieldInfo.GetValue(this) as SignalNoodle;

                    if (noodle == null)
                        throw new NullReferenceException("SignalNoodle is null, did you forget to initialise?");

                    noodle.port = port;
                    noodle.plot = new Plot(Plot.PlotType.Signal);
                    Graph.graphPlotList.Add(noodle.plot);
                }

                // Init FloatNoodles

                if (fieldInfo.FieldType == typeof(FloatNoodle))
                {
                    var noodle = fieldInfo.GetValue(this) as FloatNoodle;

                    noodle.port = port;
                    noodle.plot = new Plot(Plot.PlotType.Float);
                    Graph.graphPlotList.Add(noodle.plot);
                }

                // Init MaskNoodles

                if (fieldInfo.FieldType == typeof(MaskNoodle))
                {
                    var noodle = fieldInfo.GetValue(this) as MaskNoodle;

                    noodle.port = port;
                    noodle.plot = new Plot(Plot.PlotType.Mask);
                    Graph.graphPlotList.Add(noodle.plot);
                }
            }
        }

        protected virtual void OnGenerationStart()
        {
            InitNoodles(); //todo do I need to do this every time?
            foreach (var signalNoodle in GetInputSignalNoodles()) signalNoodle.plot.Clear();
        }

        private List<SignalNoodle> GetInputSignalNoodles()
        {
            List<SignalNoodle> returnMe = new();

            foreach (var fieldInfo in GetType().GetFields())
            {
                if (!fieldInfo.HasAttribute(typeof(InputAttribute))) continue;

                var port = GetPort(fieldInfo.Name);

                if (fieldInfo.FieldType == typeof(SignalNoodle))
                {
                    var signalNoodle = fieldInfo.GetValue(this) as SignalNoodle;
                    returnMe.Add(signalNoodle);
                }
            }

            return returnMe;
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }

        public virtual void OnGenerationEnd()
        {
        }

        public void Updated()
        {
            Graph.OnNodesUpdated();
            NodeUpdateEvent?.Invoke();
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);
            NodeUpdateEvent?.Invoke();
            Graph.OnNodesUpdated();
        }

        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);
            NodeUpdateEvent?.Invoke();
            Graph.OnNodesUpdated();
        }

        protected List<NodePort> GetSignalInputPorts()
        {
            List<NodePort> returnMe = new();
            foreach (var port in Inputs)
                if (port.ValueType == typeof(SignalNoodle))
                    returnMe.Add(port);
            return returnMe;
        }

        [Obsolete]
        protected float GetSignal(Vector3 vectorSeed)
        {
            return GetSignal(0, vectorSeed);
        }

        [Obsolete]
        protected float GetSignal(int portIndex, Vector3 vectorSeed)
        {
            var port = GetSignalInputPorts()[portIndex];
            return GetSignal(port, vectorSeed);
        }

        [Obsolete]
        protected float GetSignal(string fieldName, Vector3 vectorSeed)
        {
            var port = GetPort(fieldName);
            return GetSignal(port, vectorSeed);
        }

        [Obsolete]
        private float GetSignal(NodePort port, Vector3 vectorSeed)
        {
            if (port.IsConnected)
            {
                var signalNode = (ReturnSignalNode) port.Connection.node;
                return signalNode.Get(vectorSeed);
            }

            var fieldInfo = GetType().GetField(port.fieldName);
            var signalNoodle = fieldInfo.GetValue(this) as SignalNoodle;

            if (signalNoodle != null) return signalNoodle.value;

            throw new NullReferenceException("unable to find signalNoodle");
        }

        [Obsolete]
        public float GetFloat(string fieldName, Vector3 vectorSeed)
        {
            var port = GetPort(fieldName);
            return GetFloat(port, vectorSeed);
        }

        [Obsolete]
        private float GetFloat(NodePort port, Vector3 vectorSeed)
        {
            if (port.IsConnected)
            {
                var floatNode = (ReturnFloatNode) port.Connection.node;
                return floatNode.GetFloat(vectorSeed);
            }

            var fieldInfo = GetType().GetField(port.fieldName);
            var floatNoodle = fieldInfo.GetValue(this) as FloatNoodle;

            if (floatNoodle != null) return floatNoodle.value;

            throw new NullReferenceException("unable to find signalNoodle");
        }

        [Obsolete]
        protected List<ReturnSignalNode> GetConnectedSignalInputNodes()
        {
            List<ReturnSignalNode> returnMe = new();
            foreach (var input in Inputs)
                if (input.IsConnected && input.Connection.node.GetType().InheritsFrom(typeof(ReturnSignalNode)))
                    returnMe.Add(input.Connection.node as ReturnSignalNode);
            return returnMe;
        }

        protected List<IReturnSectorData> GetGeometryInputNodes()
        {
            List<IReturnSectorData> returnMe = new();
            foreach (var input in Inputs)
                if (input.IsConnected && input.Connection.node.GetType().InheritsFrom(typeof(IReturnSectorData)))
                    returnMe.Add(input.Connection.node as IReturnSectorData);
            return returnMe;
        }

        // protected List<NodePort> GetMaskInputPorts()
        // {
        //     List<NodePort> returnMe = new();
        //     foreach (var port in Inputs)
        //     {
        //         
        //         if(port.ValueType == typeof(MaskNoodle))
        //             returnMe.Add(port);
        //     }
        //     return returnMe;
        // }

        // protected List<IReturnMask> GetMaskInputNodes()
        // {
        //     List<IReturnMask> returnMe = new();
        //     foreach (var input in Inputs)
        //     {
        //         if (input.IsConnected && input.Connection.node.GetType().InheritsFrom(typeof(IReturnMask)))
        //             returnMe.Add(input.Connection.node as IReturnMask);
        //     }
        //     return returnMe;
        // }
        // protected IReturnMask GetMaskNode(string inputName)
        // {
        //     foreach (var input in Inputs)
        //     {
        //         if (input.IsConnected && input.fieldName == inputName)
        //             return input.Connection.node as IReturnMask;
        //     }
        //     return null;
        // }
    }
}