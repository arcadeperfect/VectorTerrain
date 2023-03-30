using System;
using System.Collections.Generic;
using System.Linq;
using ProceduralToolkit;
using UnityEngine;

namespace VectorTerrain.Scripts.Types
{
    [Serializable]
    public class SectorData
    {
        /*
     SectorData is the pure data required to instantiate a sector
     
     It does not create any gameObjects, and must be thread safe (no Unity API, although Vectors are allowed)
     
     It currently generates:
     
     - a list of Vertex2Node objects which contain:
            Vector2 for coords
            Color
     
     - A GameObject / transform for the first vertex
            this allows for easy access to the local / global methods of that transform which is handy
            
    - A record of the globalSeed value
            this can be used as an IDD to record relevant data relevant to this particular sector
    
     */

        public float zOffset; // todo should this be here?
        private float? _totalDistance = null;

        public float TotalLengh
        {
            get
            {
                if (_totalDistance == null)
                    ProcessDistances();
                if (_totalDistance == null)
                    throw new NullReferenceException();
                return (float) _totalDistance;
            }
        }

        public int Count => _verts.Count;
        private Dictionary<string, SeedContainer> _beginSeeds;

        public Dictionary<string, SeedContainer> BeginSeeds
        {
            get => _beginSeeds;
            set => _beginSeeds = value;
        }

        private Dictionary<string, SeedContainer> _endSeeds;

        public Dictionary<string, SeedContainer> EndSeeds
        {
            get => _endSeeds;
            set => _endSeeds = value;
        }

        private Vector2 _localStart;
        public Vector2 LocalStart => _localStart;
        private Vector2 _localEnd;
        public Vector2 LocalEnd => _localEnd;
        private List<Vertex2> _verts;

        public List<Vertex2> Verts
        {
            get => _verts;
            set
            {
                _verts = value;
                // Process();
            }
        }

        public List<PosMarker> Markers { get; set; }
        public List<SectorSegment> Segments { get; set; }
        public List<RegionNormalised> RegionsNormalised { get; set; }
        public List<RegionAbsolute> RegionsAbsolute { get; set; }

        // public List<Attr> VertexProperties;
        // public List<Attr> SegmentAttrs;
        // public Dictionary<string, List<Attr>> SegmentAttrs;


        // private List<SectorSegment> _segmentAttributes;
        // public List<SectorSegment> SegmentAttributes
        // {
        //     get => _segmentAttributes;
        // }



        // constructors
        public SectorData()
        {
            Verts = new List<Vertex2>();
            InitCollections();
        }

        public SectorData(bool initVecs)
        {
            if (initVecs)
                Verts = new List<Vertex2>() {Vector2.zero, Vector2.right};
            Verts = !initVecs ? new() : new List<Vertex2>() {Vector2.zero, Vector2.right};
            InitCollections();
        }

        public SectorData(List<Vertex2> verts)
        {
            Verts = verts;
            InitCollections();
        }

        // Methods
        private void InitCollections()
        {
            Markers = new();
            Segments = null;
            RegionsNormalised = new();
            RegionsAbsolute = new();
        }

        public void Process()
        {
            //todo why is this is getting called on empty sectors
            if (_verts.Count == 0)
            {
                throw new SectorDataEmptyException();
            }

            _localStart = _verts[0];
            _localEnd = _verts[^1];


        }

        public class SectorDataEmptyException : Exception
        {
            public SectorDataEmptyException()
            {
            }
        }

        public void SetPos(List<Vertex2> Verts)
        {
            for (int i = 0; i < Verts.Count; i++)
            {
                var v = Verts[i];
                v.Pos = new Vector2(1, 2);
                Verts[i] = v;
            }
        }

        public void ProcessDistances()
        {
            float totalDistance = 0;

            var u = _verts[0];
            u.Dist = 0;
            _verts[0] = u;

            Vertex2 initVert = _verts[0];
            initVert.TotalDist = 0;
            _verts[0] = initVert;

            for (int i = 1; i < _verts.Count; i++)
            {
                Vector2 current = _verts[i].Pos;
                Vector2 previous = _verts[i - 1].Pos;
                Vertex2 temp = _verts[i];
                temp.Dist = Vector2.Distance(current, previous);
                totalDistance += (float) temp.Dist;
                temp.TotalDist = totalDistance;
                _verts[i] = temp;
            }

            this._totalDistance = totalDistance;
        }

        public void ProcessSegments()
        {
            if (Verts.Count == 0)
                return;
            Segments = new();
            for (int i = 1; i < Verts.Count; i++)
            {
                SectorSegment seg = new SectorSegment(_verts, i - 1, i);
                Segments.Add(seg);
                // var v = new Vector2();
                // v.RotateCCW90()
            }
        }

        public void ProcessNormals()
        {
            ProcessSegments(); // todo could me optimised to not always run
            for (int i = 0; i < Verts.Count; i++)
            {
                if (i == 0 || i == _verts.Count - 1) //todo handling first and last normals will be annoying, do it later
                    continue;

                var s1 = Segments[i - 1];
                var s2 = Segments[i];
                var n1 = s1.direction.RotateCCW90();
                var n2 = s2.direction.RotateCCW90();
                var n = (n1.normalized + n2.normalized).normalized;
                var v = _verts[i];
                v.Normal = n;
                _verts[i] = v;
            }
        }

        public void SetColor(Color color)
        {
            for (int i = 0; i < _verts.Count; i++)
            {
                var v = _verts[i];
                v.Color = color;
                _verts[i] = v;
            }
        }
    
        public void MakeBackwards()
        {
            List<Vertex2> reversedVerts = new List<Vertex2>();
            var pivot = Verts[0].Pos;
			
            foreach (var vert in Verts)
            {
                var v = vert;
                v.x -= pivot.x;
                v.x *= -1;
                v.x += pivot.x;
                reversedVerts.Add(v);
            }
            Verts = reversedVerts;

            _localStart = _verts[^1];
            _localEnd = _verts[0];
        }

        // public VertexSegment SegmentAtPointNormalized(float t)
        // {
        //     if (Segments == null) ProcessSegments();
        //     if (totalDistance == null) ProcessDistances();
        //     if (Segments == null || totalDistance == null) throw new NullReferenceException();
        //     
        //     float u = t * (float)totalDistance;
        //
        //     VertexSegment? found = null;
        //     foreach (var v in Segments)
        //     {
        //         if (verts[v.indexB].TotalDist < u)
        //             continue;
        //         found = v;
        //         break;
        //     }
        //
        //     if (found != null) return (VertexSegment) found;
        //     throw new NullReferenceException();
        // }
        public List<int> IndicesesOfPointsWithinRangeNormalized(float a, float b)
        {
            if (_totalDistance == null)
                ProcessDistances();

            List<int> ReturnMe = new();

            var c = a * _totalDistance;
            var d = b * _totalDistance;

            for (int i = 0; i < Verts.Count; i++)
            {
                if (Verts[i].TotalDist >= c && Verts[i].TotalDist <= d)
                    ReturnMe.Add(i);
            }

            return ReturnMe;
        }

        public List<Vertex2> PointsWithinRangeNormalized(float a, float b)
        {
            if (_totalDistance == null)
                ProcessDistances();

            List<Vertex2> ReturnMe = new();

            var c = a * _totalDistance;
            var d = b * _totalDistance;

            foreach (var vert in _verts)
            {
                if (vert.TotalDist >= c && vert.TotalDist <= d)
                    ReturnMe.Add(vert);
            }

            return ReturnMe;
        }

        public int IndexOfNearestPointToNormalised(float p)
        {
            return _verts.IndexOf((Vertex2) NearestPointToNormalised(p));
        }

        public Vertex2 NearestPointToNormalised(float p)
        {
            if (_totalDistance == null)
                ProcessDistances();

            float pDist = (float) _totalDistance * p;

            Vertex2? closest = null;
            float currentClosestDist = float.MaxValue;

            foreach (var v in _verts)
            {
                float distFromPoint = Mathf.Abs((float) v.TotalDist - pDist);

                if (distFromPoint < currentClosestDist)
                {
                    currentClosestDist = distFromPoint;
                    closest = v;
                }
            }

            if (closest == null)
                throw new NullReferenceException();

            return (Vertex2) closest;
        }

        public Vertex2 Traverse(float targetDistance, bool normalised)
        {
            return Traverse(this, targetDistance, normalised);
        }

        public static Vertex2 Traverse(SectorData sectorData, float targetDistance, bool normalized)
        {
            var verts = sectorData._verts;

            if (sectorData._totalDistance == null) sectorData.ProcessDistances();

            var totalDistance = (float) sectorData._totalDistance;

            if (targetDistance < 0)
                throw new TraversException("target distance was negative  -> " + targetDistance + " : " + totalDistance);
        
            if (targetDistance > totalDistance)
                throw new TraversException("target distance was greater than totalDistance  -> " + targetDistance + " : " + totalDistance);

            if (normalized)
                targetDistance = totalDistance * targetDistance;

            targetDistance = Mathf.Clamp(targetDistance, 0, totalDistance);

            Vertex2 lerped = new Vertex2();

            float distanceSoFar = 0;
            for (int i = 0; i < verts.Count - 1; i++)
            {
                if (i < verts.Count - 1 && verts[i + 1].Dist == null)
                    throw new TraversException("Next vertex distance was null");

                float thisDist = (float) verts[i].Dist;
                float nextDist = (float) verts[i + 1].Dist;

                distanceSoFar += thisDist;

                // continue looping until we know the target point is closer than the next vert
                float z = distanceSoFar + nextDist;
                if (z < targetDistance)
                    continue;

                // if we are here,the target point lies before the next vert
                float remainingDistance = targetDistance - distanceSoFar;
                float lerpIndex = remainingDistance / nextDist;

                lerped = Vertex2.Lerp(verts[i], verts[i + 1], lerpIndex);

                break;
            }

            return lerped;
        }

        public class TraversException : Exception
        {
            public TraversException(string message) : base(message)
            {
            }
        }

        public static SectorData Lerp(SectorData a, SectorData b, float t)
        {

            if (a.Verts.Count != b.Verts.Count)
            {
                Debug.Log("vert counts did not match");
                return null;
            }

            SectorData n = new SectorData();


            for (int i = 0; i < a.Verts.Count; i++)
            {
                n._verts.Add(Vertex2.Lerp(a._verts[i], b._verts[i], t));
            }

            n.Process();
            return n;
        }
        // public static SectorData Lerp(SectorData a, SectorData b, List<float> t)
        // {
        //     if (a.Verts.Count != b.Verts.Count)
        //     {
        //         return TraverseLerp(a, b, t);
        //     }
        //
        //     SectorData n = new SectorData();
        //
        //     for (int i = 0; i < a.Verts.Count; i++)
        //     {
        //         n._verts.Add(Vertex2.Lerp(a._verts[i], b._verts[i], t[i]));
        //     }
        //     
        //     n.Process();
        //     return n;
        // }
        // public static SectorData Lerp(SectorData a, SectorData b, float[] t)
        // {
        //     if (a.Verts.Count != b.Verts.Count)
        //     {
        //         return TraverseLerp(a, b, t);
        //     }
        //
        //     SectorData n = new SectorData();
        //
        //     for (int i = 0; i < a.Verts.Count; i++)
        //     {
        //         n._verts.Add(Vertex2.Lerp(a._verts[i], b._verts[i], t[i]));
        //     }
        //     
        //     n.Process();
        //     return n;
        // }

        public static List<Vertex2> Lerp(List<Vertex2> a, List<Vertex2> b, List<float> t)
        {
            int count = Mathf.Min(a.Count, b.Count);
            count = Mathf.Min(count, t.Count);

            if (t.Sum() == 0) return a;
            if (t.Sum() > count - 0.9999999f) return b;

            Vertex2[] returnMe = new Vertex2[count];

            for (int i = 0; i < a.Count; i++)
            {
                returnMe[i] = Vertex2.Lerp(a[i], b[i], t[i]);
                // n._verts.Add(Vertex2.Lerp(a._verts[i], b._verts[i], t[i]));
            }

            return returnMe.ToList();
        }

        public static Vertex2[] Lerp(Vertex2[] a, Vertex2[] b, float[] t)
        {
            int count = Mathf.Min(a.Length, b.Length);
            count = Mathf.Min(count, t.Length);

            Vertex2[] returnMe = new Vertex2[count];

            for (int i = 0; i < a.Length; i++)
            {
                returnMe[i] = Vertex2.Lerp(a[i], b[i], t[i]);
                // n._verts.Add(Vertex2.Lerp(a._verts[i], b._verts[i], t[i]));
            }

            return returnMe;
        }

        // public static SectorData TraverseLerp(SectorData a, SectorData b, List<float> t)
        // {
        //     return TraverseLerp(a, b, t.ToArray());
        // }

        // public static SectorData TraverseLerp(SectorData a, SectorData b, float[] t)
        // {
        //     
        //     // to lerp between vert lists of different lengths, we resample both input lists to contain the same 
        //     // number of elements as the which list, then lerp those. 
        //     
        //     //todo rather than taking the output vert count from which, resample which inputs and lerp output count from a to b
        //     
        //     List<Vertex2> newVerts = new();
        //
        //     for (int i = 0; i < t.Length; i++)
        //     {
        //         var t1 = (float)i / (float)t.Length;
        //
        //         var w1 = a.Traverse(t1, true);
        //         var w2 = b.Traverse(t1, true);
        //         newVerts.Add(Vertex2.Lerp(w1, w2, t[i]));
        //     }
        //
        //     
        //     SectorData n = new SectorData();
        //     n.Verts = newVerts;
        //     n.Process();
        //     return n;
        // }

        public class RegionNormalised
        {
            public string ID;
            public List<RegionPoint> points = new();

            public void AddPoint(float t, float v)
            {
                points.Add(new RegionPoint(t, v));
            }
        }

        public class RegionAbsolute
        {
            public string ID;
            public List<RegionPoint> points = new();

            public void AddPoint(float t, float v)
            {
                points.Add(new RegionPoint(t, v));
            }
        }

        public class RegionPoint
        {
            public float value;
            public float t;

            public RegionPoint(float t, float v)
            {
                this.value = v;
                this.t = t;
            }

            public override string ToString()
            {
                return $"{t} : {value}";
            }

        
        }

    }

    public class PosMarker
    {
        public Vertex2 vert;

        public int markerType;

        // public float absoluteT;
        // public float localizedT;
    
        public PosMarker(int markerType)
        {
            this.markerType = markerType;

        }
    
        public PosMarker(int MarkerType, Vertex2 Vert)
        {
            vert = Vert;
            markerType = MarkerType;
        }
    }
}