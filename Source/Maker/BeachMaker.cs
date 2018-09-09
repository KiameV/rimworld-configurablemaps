using ConfigurableMaps.Settings;
using RimWorld;
using Verse;
using Verse.Noise;

namespace ConfigurableMaps.Maker
{
    internal static class CMBeachMaker
    {
        private const float PerlinFrequency = 0.03f;
        private const float MaxForDeepWater = 0.1f;
        private const float MaxForShallowWater = 0.45f;
        private const float MaxForSand = 1f;
        private static ModuleBase beachNoise;
        private readonly static FloatRange CoastWidthRange;

        static CMBeachMaker()
        {
#if DEBUG
            Log.Warning("CMBeachMaker Prefix");
#endif
            CMBeachMaker.CoastWidthRange = new FloatRange(20f, 60f);
        }

        public static TerrainDef BeachTerrainAt(IntVec3 loc, Map map, BiomeDef biome)
        {
#if TRACE
            Log.Warning("CMBeachMaker.BeachTerrainAt Prefix");
#endif
            /*/
            // Coast Level
            //
            float adjustment = 0.0f;
            float v = TerrainSettings.coastLevel;
            if (v < 0 || v > 7)
            {
                v = Rand.Value * 7;
            }
            if (v < 1) { }
            else if (v < 2) { adjustment = 0.5f; }
            else if (v < 3) { adjustment = 1.0f; }
            else if (v < 4) { adjustment = 1.5f; }
            else if (v < 5) { adjustment = 2.0f; }
            else if (v < 6) { adjustment = 2.5f; }
            else { adjustment = 3.0f; }
            if (CMBeachMaker.beachNoise == null)
            {
                return null;
            }
            float value = CMBeachMaker.beachNoise.GetValue(loc);
            if (value < (0.1f + adjustment))
            {
                return TerrainDefOf.WaterOceanDeep;
            }
            if (value < (0.45f + adjustment))
            {
                return TerrainDefOf.WaterOceanShallow;
            }
            if (value >= (1f + adjustment))
            {
                return null;
            }*/
            return (biome != BiomeDefOf.SeaIce ? TerrainDefOf.Sand : TerrainDefOf.Ice);
        }
        public static void Cleanup()
        {
#if TRACE
            Log.Warning("CMBeachMaker.Cleanup Prefix");
#endif
            CMBeachMaker.beachNoise = null;
        }
        public static void Init(Map map)
        {
#if TRACE
            Log.Warning("CMBeachMaker.Init Prefix");
#endif
            Rot4 rot4 = Find.World.CoastDirectionAt(map.Tile);
            if (!rot4.IsValid)
            {
                CMBeachMaker.beachNoise = null;
                return;
            }
            ModuleBase perlin = new Perlin(0.0299999993294477, 2, 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);
            perlin = new ScaleBias(0.5, 0.5, perlin);
            int size = map.Size.x;
            IntVec3 intVec3 = map.Size;
            NoiseDebugUI.StoreNoiseRender(perlin, "BeachMaker base", new IntVec2(size, intVec3.z));
            ModuleBase distFromAxi = new DistFromAxis(CMBeachMaker.CoastWidthRange.RandomInRange);
            if (rot4 == Rot4.North)
            {
                distFromAxi = new Rotate(0, 90, 0, distFromAxi);
                IntVec3 size1 = map.Size;
                distFromAxi = new Translate(0, 0, (double)(-size1.z), distFromAxi);
            }
            else if (rot4 == Rot4.East)
            {
                IntVec3 intVec31 = map.Size;
                distFromAxi = new Translate((double)(-intVec31.x), 0, 0, distFromAxi);
            }
            else if (rot4 == Rot4.South)
            {
                distFromAxi = new Rotate(0, 90, 0, distFromAxi);
            }
            distFromAxi = new ScaleBias(1, -1, distFromAxi);
            distFromAxi = new Clamp(-1, 2.5, distFromAxi);
            NoiseDebugUI.StoreNoiseRender(distFromAxi, "BeachMaker axis bias");
            CMBeachMaker.beachNoise = new Add(perlin, distFromAxi);
            NoiseDebugUI.StoreNoiseRender(CMBeachMaker.beachNoise, "beachNoise");
        }
    }
}
