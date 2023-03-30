using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Types;
using Random = System.Random;

namespace VectorTerrain.Scripts.Terrain
{
    public class TerrainGrassRenderer : MonoBehaviour
    {
        public static List<GrassSpawnPoint> ProcessGrassRegionsNormalised(TerrainGraphOutput tg, float density)
        {
            var sd = tg.sectorData;
            List<GrassSpawnPoint> returnMe = new();
            var rnd = new Random(1);
            var regions = from region in sd.RegionsNormalised where region.ID == "grass" select region;

            foreach (var region in regions)
            {
                var range = region.points[^1].t * sd.TotalLengh - region.points[0].t * sd.TotalLengh;
                var bladeCount = (int) (density * range);

                for (var i = 0; i < bladeCount; i++)
                {
                    var t = Hutl.Map(i, 0, bladeCount, region.points[0].t, region.points[^1].t);
                    t += ((float) rnd.NextDouble() * 2 - 1) * 0.001f;

                    var value = GetValueFromRegionNormalised(t, region);

                    if ((float) rnd.NextDouble() > value)
                        continue;

                    var vertex = sd.Traverse(t, true);
                    var spawnPoint = new GrassSpawnPoint();
                    spawnPoint.color = Color.green;
                    spawnPoint.value = value;
                    spawnPoint.position = vertex;
                    returnMe.Add(spawnPoint);
                }
            }

            return returnMe;
        }

        public static List<GrassSpawnPoint> ProcessGrassRegionsAbsolute(TerrainGraphOutput tg, float density)
        {
            var sd = tg.sectorData;
            var beginDist = tg.totalDistanceAtStart;
            var endDist = tg.totalDistanceAtEnd;
            List<GrassSpawnPoint> returnMe = new();
            var rnd = new Random(1);
            var regions = from region in sd.RegionsAbsolute where region.ID == "grass" select region;

            foreach (var region in regions)
            {
                var p1 = region.points[0].t;
                var p2 = region.points[^1].t;
                var check = beginDist >= p1 && endDist < p2 || p1 >= beginDist && p1 < endDist ||
                            p2 >= beginDist && p2 < endDist;

                if (!check) continue;

                var range = p2 - p1;
                var bladeCount = (int) (density * range);

                var count = 100;

                for (var i = 0; i < bladeCount; i++)
                {
                    var t = Hutl.Map(i, 0, bladeCount, p1, p2);
                    t += ((float) rnd.NextDouble() * 2 - 1) * 0.001f;
                    if (t < beginDist || t >= endDist)
                        continue;

                    var vertex = sd.Traverse(t - beginDist, false);
                    var spawnPoint = new GrassSpawnPoint();
                    spawnPoint.color = Color.blue;
                    spawnPoint.value = GetValueFromRegionAbsolute(t, region);
                    spawnPoint.position = vertex;
                    returnMe.Add(spawnPoint);
                }
            }

            return returnMe;
        }

        public static float GetValueFromRegionNormalised(float t, SectorData.RegionNormalised r)
        {
            float v = 0;

            for (var i = 0; i < r.points.Count - 1; i++)
            {
                var p1 = r.points[i];
                var p2 = r.points[i + 1];
                if (t >= p1.t && t < p2.t)
                {
                    var l = Hutl.Map(t, p1.t, p2.t, 0, 1);
                    v = Mathf.Lerp(p1.value, p2.value, l);
                }
            }

            return v;
        }

        public static float GetValueFromRegionAbsolute(float t, SectorData.RegionAbsolute r)
        {
            float v = 0;

            for (var i = 0; i < r.points.Count - 1; i++)
            {
                var p1 = r.points[i];
                var p2 = r.points[i + 1];
                if (t >= p1.t && t < p2.t)
                {
                    var l = Hutl.Map(t, p1.t, p2.t, 0, 1);
                    v = Mathf.Lerp(p1.value, p2.value, l);
                }
            }

            return v;
        }

        public class GrassSpawnPoint
        {
            public Color color;
            public Vertex2 position;
            public float value;
        }
    }
}