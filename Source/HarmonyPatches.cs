using ConfigurableMaps.Settings;
using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace ConfigurableMaps
{
/*	[StaticConstructorOnStartup]
    public class HarmonyPatches
	{
		public static bool detectedFertileFields = false;
        public static bool detectedCuprosStones = false;

        static HarmonyPatches()
        {
			var harmony = new Harmony("com.configurablemaps.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            LongEventHandler.QueueLongEvent(new Action(UpdateDefs), "LibraryStartup", false, null);
            if (ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.Name.Contains("[RF] Fertile Fields")))
            {
                detectedFertileFields = true;
            }
            if (ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.Name.Contains("Cupro's Stones")))
            {
                detectedCuprosStones = true;
            }
        }

        internal static void UpdateDefs()
        {
            Log.Message("Updating Defs");
            foreach (var step in DefDatabase<GenStepDef>.AllDefsListForReading)
            {
                if (step.defName == "ScatterRuinsSimple")
                {
                    Log.Message("   ScatterRuinsSimple");
                    float v = ThingsSettings.ruinsLevel;
                    if (v < 0 || v > 8)
                        v = Rand.Value * 8;
                    if (v < 1)
                        v = 0;
                    if (step.genStep is GenStep_ScatterRuinsSimple s)
                        s.countPer10kCellsRange = new FloatRange(v, v * 2);
                    else
                        Log.Warning("Failed to apply ScatterRuinsSimple");
                }
                else if (step.defName == "ScatterShrines")
                {
                    Log.Message("   ScatterShrines");
                    float min, max, v = ThingsSettings.shrinesLevel;
                    if (v < 0 || v > 8)
                    {
                        v = Rand.Value * 8;
                    }
                    if (v < 1) { min = 0; max = 0; }
                    else if (v < 2) { min = 0; max = 0.06f; }
                    else if (v < 3) { min = 0.06f; max = 0.12f; }
                    else if (v < 4) { min = 0.12f; max = 0.24f; }
                    else if (v < 5) { min = 0.24f; max = 0.48f; }
                    else if (v < 6) { min = 0.48f; max = 0.96f; }
                    else if (v < 7) { min = 0.96f; max = 1.92f; }
                    else { min = 1.92f; max = 3.84f; }
                    if (step.genStep is GenStep_ScatterShrines s)
                        s.countPer10kCellsRange = new FloatRange(min, max);
                    else
                        Log.Warning("Failed to apply ScatterShrines");
                }
            }
        }
    }

    [HarmonyPatch(typeof(Game), "InitNewGame")]
    static class Patch_Game_InitNewGame
    {
        static void Prefix()
        {
#if DEBUG
            Log.Warning("Patch_Game_InitNewGame Prefix");
#endif
            BiomeUtil.Init();
            BiomeUtil.UpdateBiomeStatsPerUserSettings();
        }
    }

    [HarmonyPatch(typeof(GenStep_RockChunks), "GrowLowRockFormationFrom", null)]
    public static class GenStep_RockChunks_GrowLowRockFormationFrom
    {
        public static bool Prefix()
        {
#if DEBUG
            Log.Warning("GenStep_RockChunks_GrowLowRockFormationFrom Prefix");
#endif
            float v = TerrainSettings.chunksLevel;
            if (v < 0 || v > 3)
            {
                v = Rand.Value * 3;
            }

            if (v < 1)
            {
                return false;
            }
            if (v < 2 && 
                Rand.Value < 0.5f)
            {
                return false;
            }
            return true;
        }
    }

    /*[HarmonyPatch(typeof(GenStep_Plants), "Generate", null)]
    public static class GenStep_Plants_Generate
    {
        public static bool Prefix(Map map)
        {
            if (ThingsSettings.animalDensityLevel < 0 ||
                ThingsSettings.plantDensityLevel < 0)
            {
                Util.UpdateBiomeStatsPerUserSettings();
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(GenPlantReproduction), "TryFindReproductionDestination", null)]
    public static class GenPlantReproduction_TryFindReproductionDestination
    {
        public static bool Prefix(Map map)
        {
            if ((Controller_Things.animalDensityLevel < 0) || (Controller_Things.plantDensityLevel < 0))
            {
                SetBiodensity.SetBiodensityLevels();
            }
            return true;
        }
    }*/

    /*[HarmonyPatch(typeof(GenStep_ScatterRuinsSimple), "ScatterAt", null)]
    public static class GenStep_ScatterRuinsSimple_ScatterAt
    {
        public static bool Prefix(IntVec3 c, Map map)
        {
#if DEBUG
            Log.Warning("GenStep_ScatterRuinsSimple_ScatterAt Prefix");
#endif
            BaseGen.globalSettings.map = map;
            ThingDef thingDef = BaseGenUtility.RandomCheapWallStuff(null, true);
            if (Rand.Value < 0.9f)
            {
                int sizeX = Rand.Range(5, 11);
                int sizeZ = Rand.Range(5, 11);
                CellRect cellRect = new CellRect(c.x, c.z, sizeX, sizeZ);
                CellRect cellRect1 = cellRect.ClipInsideMap(map);
                MethodInfo locationCheck = typeof(GenStep_ScatterRuinsSimple).GetMethod("CanPlaceAncientBuildingInRange", BindingFlags.NonPublic | BindingFlags.Instance);
                bool b = (bool)locationCheck.Invoke(new GenStep_ScatterRuinsSimple(), new object[] { cellRect1, map });
                if (b.Equals(true))
                {
                    BaseGen.symbolStack.Push("ancientRuins", cellRect1);
                    BaseGen.Generate();
                }
            }
            else
            {
                bool flag = Rand.Bool;
                int randomInRange = Rand.Range(3, 11);
                CellRect cellRect2 = new CellRect(c.x, c.z, (!flag ? 1 : randomInRange), (!flag ? randomInRange : 1));
                MethodInfo locationCheck = typeof(GenStep_ScatterRuinsSimple).GetMethod("CanPlaceAncientBuildingInRange", BindingFlags.NonPublic | BindingFlags.Instance);
                bool b = (bool)locationCheck.Invoke(new GenStep_ScatterRuinsSimple(), new object[] { cellRect2.ExpandedBy(1), map });
                if (b.Equals(true))
                {
                    typeof(GenStep_ScatterRuinsSimple).GetMethod("MakeLongWall", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(new GenStep_ScatterRuinsSimple(), new object[] { c, map, randomInRange, flag, thingDef });
                }
            }
            return false;
        }
    }* /

    [HarmonyPatch(typeof(BaseGenUtility), "RandomCheapWallStuff", new Type[] { typeof(TechLevel), typeof(bool) })]
    public static class BaseGenUtility_RandomCheapWallStuff
    {
        public static bool Prefix(TechLevel techLevel, ref ThingDef __result, bool notVeryFlammable = false)
        {
#if DEBUG
            Log.Warning("BaseGenUtility_RandomCheapWallStuff Prefix");
#endif
            if (techLevel.IsNeolithicOrWorse())
            {
                __result = ThingDefOf.WoodLog;
                return false;
            }
            ThingDef wallType = (from wallStuff in DefDatabase<ThingDef>.AllDefsListForReading
                                 where (!BaseGenUtility.IsCheapWallStuff(wallStuff) ? false : (!notVeryFlammable ? true : wallStuff.BaseFlammability < 0.5f))
                                 select wallStuff).RandomElement<ThingDef>();
            Map map = BaseGen.globalSettings.map;
            float v = ThingsSettings.stoneType;
            if (v < 0 || v > 3)
            {
                v = Rand.Value * 3;
            }
            if (map == null || wallType.defName.Contains("Steel")
              || v < 1
              || (v < 2 && Rand.Value < 0.33))
            {
                __result = wallType;
                return false;
            }
            ThingDef rockType = Find.World.NaturalRockTypesIn(map.Tile).RandomElement<ThingDef>();
            wallType = ThingDef.Named("Blocks" + rockType.defName);
            __result = wallType;
            return false;
        }
    }

    [HarmonyPatch(typeof(GenStep_Scatterer), "CountFromPer10kCells", null)]
    public static class GenStep_Scatterer_CountFromPer10kCells
    {
        public static bool Prefix(float countPer10kCells, Map map, ref int __result, int mapSize = -1)
        {
#if DEBUG
            Log.Warning("GenStep_Scatterer_CountFromPer10kCells Prefix");
#endif
            if (mapSize < 0)
            {
                mapSize = map.Size.x;
            }
            //
            // Ore
            //
            if (countPer10kCells > 3.95)
            {
                float min = countPer10kCells;
                float v = TerrainSettings.oreLevel;
                if (v < 0 || v > 5)
                {
                    v = (Rand.Value) * 5;
                }
                if (v < 1) { min = 0; }
                else if (v < 2) { min *= 0.5f; }
                else if (v < 3) { }
                else if (v < 4) { min *= 2; }
                else { min *= 4; }
                float max = min;
                countPer10kCells = Rand.Range(min, max);
            }
            /* /
            // Ruins
            //
            if (countPer10kCells > 1.95 && countPer10kCells < 3.85)
            {
                float v = ThingsSettings.ruinsLevel;
                if (v < 0 || v > 8)
                {
                    v = Rand.Value * 8;
                }
                if (v < 1)
                {
                    v = 0;
                }
                
                countPer10kCells = Rand.Range(v, v * 2);
            }
            /* /
            // Geysers
            //
            if (countPer10kCells > 0.65 && countPer10kCells < 1.05)
            {
                float min = 0.7f;
                float max = 1;
                float v = TerrainSettings.geysersLevel;
                if (v < 0 || v > 5)
                {
                    v = (Rand.Value) * 5;
                }
                if (v < 1) { min = 0; max = 0; }
                else if (v < 2) { min = 0.35f; max = 0.5f; }
                else if (v < 3) { min = 0.7f; max = 1; }
                else if (v < 4) { min = 1.4f; max = 2; }
                else { min = 2.8f; max = 4; }
                countPer10kCells = Rand.Range(min, max);
            }
            /* /
            // Shrines
            //
            if (countPer10kCells > 0.10 && countPer10kCells < 0.30)
            {
                float min = 0.12f;
                float max = 0.24f;
                float v = ThingsSettings.shrinesLevel;
                if (v < 0 || v > 8)
                {
                    v = Rand.Value * 8;
                }
                if (v < 1) { min = 0; max = 0; }
                else if (v < 2) { min = 0; max = 0.06f; }
                else if (v < 3) { min = 0.06f; max = 0.12f; }
                else if (v < 4) { min = 0.12f; max = 0.24f; }
                else if (v < 5) { min = 0.24f; max = 0.48f; }
                else if (v < 6) { min = 0.48f; max = 0.96f; }
                else if (v < 7) { min = 0.96f; max = 1.92f; }
                else { min = 1.92f; max = 3.84f; }
                countPer10kCells = Rand.Range(min, max);
            }* /
            int num = Mathf.RoundToInt(10000f / countPer10kCells);
            __result = Mathf.RoundToInt((float)(mapSize * mapSize) / (float)num);
            return false;
        }
    }

    [HarmonyPatch(typeof(TerrainThreshold), "TerrainAtValue", null)]
    public static class TerrainThreshold_TerrainAtValue
    {
        public static bool Prefix(List<TerrainThreshold> threshes, float val, ref TerrainDef __result)
        {
#if TRACE
            Log.Warning("TerrainAtValue Prefix");
#endif
            __result = null;
            if (threshes[0].min < -900)
            {
                //
                // TerrainsByFertility
                // 
                float mod = 0.0f;
                if (TerrainSettings.fertilityLevel < 1) { mod = 0.30f; }
                else if (TerrainSettings.fertilityLevel < 2) { mod = 0.15f; }
                else if (TerrainSettings.fertilityLevel < 3) { }
                else if (TerrainSettings.fertilityLevel < 4) { mod = -0.15f; }
                else { mod = -0.30f; }
                if (threshes[0].terrain.defName == "Soil")
                {
                    float val1 = 0.87f + mod;
                    if (val >= -999f && val < val1) { __result = TerrainDef.Named("Soil"); }
                    else if (val >= val1 && val < 999f) { __result = TerrainDef.Named("SoilRich"); }
                }
                else if (threshes[0].terrain.defName == "Sand" && (threshes[0].max < 0.5f))
                {
                    float val1 = 0.90f + mod;
                    if (val >= -999f && val < 0.45f) { __result = TerrainDef.Named("Sand"); }
                    else if (val >= 0.45f && val < val1) { __result = TerrainDef.Named("Soil"); }
                    else if (val >= val1 && val < 999f) { __result = TerrainDef.Named("SoilRich"); }
                }
                else
                {
                    for (int i = 0; i < threshes.Count; i++)
                    {
                        if (threshes[i].min <= val && threshes[i].max > val)
                        {
                            __result = threshes[i].terrain;
                        }
                    }
                }
                return false;
            }
            //
            // TerrainPatchMakers
            //
            float adjustment;
            float v = TerrainSettings.waterLevel;
            if (v < 0 || v > 5)
            {
                v = Rand.Value * 5;
            }
            if (v < 1) { adjustment = 0.75f; }
            else if (v < 2) { adjustment = 0.33f; }
            else if (v < 3) { adjustment = 0.0f; }
            else if (v < 4) { adjustment = -0.25f; }
            else { adjustment = -0.5f; }

            float richSoil;
            v = TerrainSettings.fertilityLevel;
            if (v < 0 || v > 5)
            {
                v = Rand.Value * 5;
            }
            if (v < 1) { richSoil = -0.06f; }
            else if (v < 2) { richSoil = -0.03f; }
            else if (v < 3) { richSoil = 0.0f; }
            else if (v < 4) { richSoil = 0.03f; }
            else { richSoil = 0.06f; }
            for (int i = 0; i < threshes.Count; i++)
            {
                if (threshes[0].terrain.defName == "SoilRich")
                {
                    if (i == 0)
                    {
                        if ((threshes[i].min + adjustment) <= val && (threshes[i].max + adjustment + richSoil) > val)
                        {
                            __result = threshes[i].terrain;
                        }
                    }
                    if (i == 1)
                    {
                        if ((threshes[i].min + adjustment + richSoil) <= val && (threshes[i].max + adjustment) > val)
                        {
                            __result = threshes[i].terrain;
                        }
                    }
                    else
                    {
                        if ((threshes[i].min + adjustment) <= val && (threshes[i].max + adjustment) > val)
                        {
                            __result = threshes[i].terrain;
                        }
                    }
                }
                else
                {
                    if ((threshes[i].min + adjustment) <= val && (threshes[i].max + adjustment) > val)
                    {
                        __result = threshes[i].terrain;
                    }
                }
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(GenStep_ElevationFertility), "Generate", null)]
    public static class GenStep_ElevationFertility_Generate
    {
        private const float ElevationFreq = 0.021f;
        private const float FertilityFreq = 0.021f;
        private const float EdgeMountainSpan = 0.42f;
        public static bool Prefix(Map map)
        {
#if DEBUG
            Log.Warning("GenStep_ElevationFertility_Generate Prefix");
#endif
            Rot4 random;
            NoiseRenderer.renderSize = new IntVec2(map.Size.x, map.Size.z);
            ModuleBase perlin = new Perlin(0.0209999997168779, 2, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
            perlin = new ScaleBias(0.5, 0.5, perlin);
            NoiseDebugUI.StoreNoiseRender(perlin, "elev base");
            float elevationFactorFlat = 1f;
            float elevationScale = 1f;
            switch (map.TileInfo.hilliness)
            {
                case Hilliness.Flat:
                    elevationFactorFlat = MapGenTuning.ElevationFactorFlat;
                    elevationScale = 0.1f;
                    break;
                case Hilliness.SmallHills:
                    elevationFactorFlat = MapGenTuning.ElevationFactorSmallHills;
                    elevationScale = 0.3f;
                    break;
                case Hilliness.LargeHills:
                    elevationFactorFlat = MapGenTuning.ElevationFactorLargeHills;
                    elevationScale = 0.6f;
                    break;
                case Hilliness.Mountainous:
                    elevationFactorFlat = MapGenTuning.ElevationFactorMountains;
                    break;
                case Hilliness.Impassable:
                    elevationFactorFlat = MapGenTuning.ElevationFactorImpassableMountains;
                    break;
            }

            if (TerrainSettings.mountainLevel < 1)
            {
                elevationFactorFlat -= (0.15f * elevationScale);
            }
            else if (TerrainSettings.mountainLevel < 2)
            {
                elevationFactorFlat -= (0.1f * elevationScale);
            }
            else if (TerrainSettings.mountainLevel < 3) { }
            else if (TerrainSettings.mountainLevel < 4)
            {
                elevationFactorFlat += (0.2f * elevationScale);
            }
            else if (TerrainSettings.mountainLevel < 5)
            {
                elevationFactorFlat += (0.6f * elevationScale);
            }
            else
            {
                elevationFactorFlat += (1.4f * elevationScale);
            }
            perlin = new Multiply(perlin, new Const((double)elevationFactorFlat));
            NoiseDebugUI.StoreNoiseRender(perlin, "elev world-factored");
            if (map.TileInfo.hilliness == Hilliness.Mountainous || map.TileInfo.hilliness == Hilliness.Impassable)
            {
                IntVec3 size = map.Size;
                ModuleBase distFromAxi = new DistFromAxis((float)size.x * 0.42f);
                distFromAxi = new Clamp(0, 1, distFromAxi);
                distFromAxi = new Invert(distFromAxi);
                distFromAxi = new ScaleBias(1, 1, distFromAxi);
                do
                {
                    random = Rot4.Random;
                }
                while (random == Find.World.CoastDirectionAt(map.Tile));
                if (random == Rot4.North)
                {
                    distFromAxi = new Rotate(0, 90, 0, distFromAxi);
                    IntVec3 intVec3 = map.Size;
                    distFromAxi = new Translate(0, 0, (double)(-intVec3.z), distFromAxi);
                }
                else if (random == Rot4.East)
                {
                    IntVec3 size1 = map.Size;
                    distFromAxi = new Translate((double)(-size1.x), 0, 0, distFromAxi);
                }
                else if (random == Rot4.South)
                {
                    distFromAxi = new Rotate(0, 90, 0, distFromAxi);
                }
                else if (random == Rot4.West) { }
                NoiseDebugUI.StoreNoiseRender(distFromAxi, "mountain");
                perlin = new Add(perlin, distFromAxi);
                NoiseDebugUI.StoreNoiseRender(perlin, "elev + mountain");
            }
            float single = (!map.TileInfo.WaterCovered ? Single.MaxValue : 0f);
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            foreach (IntVec3 allCell in map.AllCells)
            {
                elevation[allCell] = Mathf.Min(perlin.GetValue(allCell), single);
            }
            ModuleBase scaleBia = new Perlin(0.0209999997168779, 2, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
            scaleBia = new ScaleBias(0.5, 0.5, scaleBia);
            NoiseDebugUI.StoreNoiseRender(scaleBia, "noiseFert base");
            MapGenFloatGrid fertility = MapGenerator.Fertility;
            foreach (IntVec3 allCell1 in map.AllCells)
            {
                fertility[allCell1] = scaleBia.GetValue(allCell1);
            }
            return false;
        }
    }


    [HarmonyPatch(typeof(World), "NaturalRockTypesIn", null)]
    public static class World_NaturalRockTypesIn
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(int tile, ref IEnumerable<ThingDef> __result)
        {
            List<StonePercentChance> possible = GetStoneTypes();
            if (possible.Count == 0)
            {
                Log.Warning("No rocks selected. Will use base game's logic.");
                return true;
            }

            Rand.PushState();
            Rand.Seed = tile;
            try
            {
                int rockTypesInTile = Rand.RangeInclusive((int)WorldSettings.stoneMin, (int)WorldSettings.stoneMax);
                if (rockTypesInTile > possible.Count)
                    rockTypesInTile = possible.Count;
                var result = new List<ThingDef>(rockTypesInTile);
                int i = 0;
                while (i < 50 && result.Count < rockTypesInTile && possible.Count > 0)
                {
                    var s = RandomWeightedSelect(possible);
                    result.Add(s.Def);
                    possible.Remove(s);
                    ++i;
                }
                __result = result;
            }
            finally
            {
                Rand.PopState();
            }
            return false;
        }

        private static StonePercentChance RandomWeightedSelect(List<StonePercentChance> possible)
        {
            float totalWeight = 0;
            foreach (var st in possible)
                totalWeight += st.Chance;
            float r = Rand.RangeInclusive(0, (int)totalWeight);
            StonePercentChance p = possible[0];
            while (r > 0)
            {
                for (int i = 0; i < possible.Count && r > 0; ++i)
                {
                    p = possible[i];
                    r -= p.Chance;
                }
            }
            return p;
        }

        private struct StonePercentChance
        {
            public readonly float Chance;
            public readonly ThingDef Def;
            public StonePercentChance(float Chance, ThingDef d)
            {
                this.Chance = Chance;
                this.Def = d;
            }
        }

        private static List<StonePercentChance> GetStoneTypes()
        {
            List<StonePercentChance> l = new List<StonePercentChance>();
            foreach (ThingDef d in DefDatabase<ThingDef>.AllDefs.Where((ThingDef td) => td.IsNonResourceNaturalRock).ToList())
            {
                if (d.defName == "Granite")
                {
                    AddStoneType(l, WorldSettings.graniteCommonality, d);
                }
                else if (d.defName == "Limestone")
                {
                    AddStoneType(l, WorldSettings.limestoneCommonality, d);
                }
                else if (d.defName == "Marble")
                {
                    AddStoneType(l, WorldSettings.marbleCommonality, d);
                }
                else if (d.defName == "Sandstone")
                {
                    AddStoneType(l, WorldSettings.sandstoneCommonality, d);
                }
                else if (d.defName == "Slate")
                {
                    AddStoneType(l, WorldSettings.slateCommonality, d);
                }
                else if (WorldSettings.extraStoneCommonality > 0)
                {
                    AddStoneType(l, WorldSettings.extraStoneCommonality, d);
                }
            }
            return l;
        }

        private static void AddStoneType(List<StonePercentChance> l, float chance, ThingDef d)
        {
            if (chance < 0)
                chance = 0;
            else if (chance > 100)
                chance = 100;
            l.Add(new StonePercentChance(chance, d));
        }
    }

    [HarmonyPatch(typeof(RockNoises), "Init", null)]
    public static class RockNoises_Init
    {
        public static bool Prefix(Map map)
        {
#if DEBUG
            Log.Warning("RockNoises_Init Prefix");
#endif
            double multiplier = 0.5d * Find.World.NaturalRockTypesIn(map.Tile).ToList().Count;
            int octaves = Rand.RangeInclusive(3, 8);
            RockNoises.rockNoises = new List<RockNoises.RockNoise>();
            foreach (ThingDef thingDef in Find.World.NaturalRockTypesIn(map.Tile))
            {
                RockNoises.RockNoise rockNoise = new RockNoises.RockNoise()
                {
                    rockDef = thingDef,
                    noise = new Perlin((multiplier * 0.00499999988824129), 2, 0.5, octaves, Rand.Range(0, 2147483647), QualityMode.Medium)
                };
                RockNoises.rockNoises.Add(rockNoise);
                NoiseDebugUI.StoreNoiseRender(rockNoise.noise, string.Concat(rockNoise.rockDef, " score"), map.Size.ToIntVec2);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(GenStep_ScatterLumpsMineable), "ScatterAt", null)]
    public static class GenStep_ScatterLumpsMineable_ScatterAt
    {
		private static float originalPlasteelCommonality = -1;
		private static float originalSteelCommonality = -1;
		private static float originalComponentsIndustrialCommonality = -1;

        [HarmonyPriority(Priority.First)]
        public static void Prefix()
        {
			ThingDef plasteel = ThingDef.Named("MineablePlasteel");
			ThingDef steel = ThingDefOf.MineableSteel;
			ThingDef compInd = ThingDefOf.MineableComponentsIndustrial;

			if (originalPlasteelCommonality == -1)
			{
				originalPlasteelCommonality = plasteel.building.mineableScatterCommonality;
				originalSteelCommonality = steel.building.mineableScatterCommonality;
				originalComponentsIndustrialCommonality = compInd.building.mineableScatterCommonality;
            }

            if (TerrainSettings.allowFakeOres)
            {
                steel.building.mineableScatterCommonality = originalSteelCommonality;
                plasteel.building.mineableScatterCommonality = originalPlasteelCommonality;
				compInd.building.mineableScatterCommonality = originalComponentsIndustrialCommonality;
            }
            else
            {
                steel.building.mineableScatterCommonality = 0f;
                plasteel.building.mineableScatterCommonality = 0f;
                compInd.building.mineableScatterCommonality = 0f;
            }
        }

		[HarmonyPriority(Priority.Last)]
		public static void Postfix()
		{
			if (!TerrainSettings.allowFakeOres)
			{
				ThingDefOf.MineableSteel.building.mineableScatterCommonality = originalSteelCommonality;
				ThingDef.Named("MineablePlasteel").building.mineableScatterCommonality = originalPlasteelCommonality;
				ThingDefOf.MineableComponentsIndustrial.building.mineableScatterCommonality = originalComponentsIndustrialCommonality;
			}
			originalPlasteelCommonality = -1;
			originalSteelCommonality = -1;
			originalComponentsIndustrialCommonality = -1;
		}
    }*/
}