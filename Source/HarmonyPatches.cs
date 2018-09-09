using ConfigurableMaps.Settings;
using Harmony;
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
    public class HarmonyPatches
    {
        public static bool detectedFertileFields = false;
        public static bool detectedCuprosStones = false;

        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("com.configurablemaps.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            if (ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.Name.Contains("[RF] Fertile Fields")))
            {
                detectedFertileFields = true;
            }
            if (ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.Name.Contains("Cupro's Stones")))
            {
                detectedCuprosStones = true;
            }
        }
    }

    [HarmonyPatch(typeof(Game), "InitNewGame")]
    static class Patch_Game_InitNewGame
    {
        static void Prefix()
        {
            Log.Warning("Patch_Game_InitNewGame Prefix");
            Util.Init();
            Util.UpdateBiomeStatsPerUserSettings();
        }
    }

    [HarmonyPatch(typeof(GenStep_RockChunks), "GrowLowRockFormationFrom", null)]
    public static class GenStep_RockChunks_GrowLowRockFormationFrom
    {
        public static bool Prefix()
        {
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

    struct BiomeOriginalValues
    {
        public readonly float AnimalDensity;
        public readonly float PlantDensity;
        public BiomeOriginalValues(float animalDensity, float plantDensity)
        {
            this.AnimalDensity = animalDensity;
            this.PlantDensity = plantDensity;
        }
    }

    static class Util
    {
        public static readonly Dictionary<BiomeDef, BiomeOriginalValues> BiomeStats = new Dictionary<BiomeDef, BiomeOriginalValues>();

        public static void Init()
        {
            if (BiomeStats.Count == 0)
            {
                foreach(BiomeDef def in DefDatabase<BiomeDef>.AllDefs)
                {
                    BiomeStats.Add(def, new BiomeOriginalValues(def.animalDensity, def.plantDensity));
                }
            }
        }

        public static void UpdateBiomeStatsPerUserSettings()
        {
            Init();

            float v = ThingsSettings.animalDensityLevel;
            if (v > 4)
            {
                v = Rand.Value * 4;
            }
            float animalModifier;
            if (v < 1) { animalModifier = 0.5f; }
            else if (v < 2) { animalModifier = 1; }
            else if (v < 3) { animalModifier = 2; }
            else { animalModifier = 4; }

            v = ThingsSettings.plantDensityLevel;
            if (v > 4)
            {
                v = Rand.Value * 4;
            }
            float plantModifier;
            if (v < 1) { plantModifier = 0.5f; }
            else if (v < 2) { plantModifier = 1; }
            else if (v < 3) { plantModifier = 2; }
            else { plantModifier = 4; }

            BiomeOriginalValues orig;
            foreach (BiomeDef def in DefDatabase<BiomeDef>.AllDefs)
            {
                if (BiomeStats.TryGetValue(def, out orig))
                {
                    def.animalDensity = orig.AnimalDensity * animalModifier;
                    def.plantDensity = orig.PlantDensity * plantModifier;
                }
            }
            /*BiomeDefOf.AridShrubland.animalDensity = 1.3f * animalModifier;
            BiomeDefOf.AridShrubland.plantDensity = 0.16f * plantModifier;
            BiomeDefOf.BorealForest.animalDensity = 1.9f * animalModifier;
            BiomeDefOf.BorealForest.plantDensity = 0.3f * plantModifier;
            BiomeDef.Named("ColdBog").animalDensity = 0.8f * animalModifier;
            BiomeDef.Named("ColdBog").plantDensity = 0.52f * plantModifier;
            BiomeDefOf.Desert.animalDensity = 0.25f * animalModifier;
            BiomeDefOf.Desert.plantDensity = 0.05f * plantModifier;
            BiomeDef.Named("ExtremeDesert").animalDensity = 0.05f * animalModifier;
            BiomeDef.Named("ExtremeDesert").plantDensity = 0.002f * plantModifier;
            BiomeDefOf.IceSheet.animalDensity = 0.1f * animalModifier;
            BiomeDefOf.IceSheet.plantDensity = 0f * plantModifier;
            BiomeDefOf.SeaIce.animalDensity = 0.05f * animalModifier;
            BiomeDefOf.SeaIce.plantDensity = 0f * plantModifier;
            BiomeDefOf.TemperateForest.animalDensity = 2.8f * animalModifier;
            BiomeDefOf.TemperateForest.plantDensity = 0.55f * plantModifier;
            BiomeDef.Named("TemperateSwamp").animalDensity = 2.8f * animalModifier;
            BiomeDef.Named("TemperateSwamp").plantDensity = 0.55f * plantModifier;
            BiomeDefOf.TropicalRainforest.animalDensity = 4.0f * animalModifier;
            BiomeDefOf.TropicalRainforest.plantDensity = 0.85f * plantModifier;
            BiomeDef.Named("TropicalSwamp").animalDensity = 4.0f * animalModifier;
            BiomeDef.Named("TropicalSwamp").plantDensity = 0.85f * plantModifier;
            BiomeDefOf.Tundra.animalDensity = 0.8f * animalModifier;
            BiomeDefOf.Tundra.plantDensity = 0.14f * plantModifier;
            List<BiomeDef> Biomes = DefDatabase<BiomeDef>.AllDefsListForReading;
            for (int i = Biomes.Count - 1; i >= 0; i--)
            {
                if (Biomes[i].defName.Contains("BorealArchi"))
                {
                    Biomes[i].animalDensity = 1.9f * animalModifier;
                    Biomes[i].plantDensity = 0.3f * plantModifier;
                }
                if (Biomes[i].defName.Contains("DesertArchi"))
                {
                    Biomes[i].animalDensity = 1.3f * animalModifier;
                    Biomes[i].plantDensity = 0.16f * plantModifier;
                }
                if (Biomes[i].defName.Contains("TemperateArchi"))
                {
                    Biomes[i].animalDensity = 2.8f * animalModifier;
                    Biomes[i].plantDensity = 0.55f * plantModifier;
                }
                if (Biomes[i].defName.Contains("TropicalArchi"))
                {
                    Biomes[i].animalDensity = 4.0f * animalModifier;
                    Biomes[i].plantDensity = 0.85f * plantModifier;
                }
                if (Biomes[i].defName.Contains("TundraArchi"))
                {
                    Biomes[i].animalDensity = 0.8f * animalModifier;
                    Biomes[i].plantDensity = 0.14f * plantModifier;
                }
                if (Biomes[i].defName.Contains("ColdBogArchi"))
                {
                    Biomes[i].animalDensity = 0.8f * animalModifier;
                    Biomes[i].plantDensity = 0.52f * plantModifier;
                }
                if (Biomes[i].defName.Contains("TemperateSwampArchi"))
                {
                    Biomes[i].animalDensity = 2.8f * animalModifier;
                    Biomes[i].plantDensity = 0.55f * plantModifier;
                }
                if (Biomes[i].defName.Contains("TropicalSwampArchi"))
                {
                    Biomes[i].animalDensity = 4.0f * animalModifier;
                    Biomes[i].plantDensity = 0.85f * plantModifier;
                }
                if (Biomes[i].defName == "Cave")
                {
                    Biomes[i].animalDensity = 3.0f * animalModifier;
                    Biomes[i].plantDensity = 0f * plantModifier;
                }
            }*/
        }
    }

    [HarmonyPatch(typeof(GenStep_ScatterRuinsSimple), "ScatterAt", null)]
    public static class GenStep_ScatterRuinsSimple_ScatterAt
    {
        public static bool Prefix(IntVec3 c, Map map)
        {
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
    }

    [HarmonyPatch(typeof(BaseGenUtility), "RandomCheapWallStuff", new Type[] { typeof(TechLevel), typeof(bool) })]
    public static class BaseGenUtility_RandomCheapWallStuff
    {
        public static bool Prefix(TechLevel techLevel, ref ThingDef __result, bool notVeryFlammable = false)
        {
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
                else if (v < 2) { min = min / 2; }
                else if (v < 3) { }
                else if (v < 4) { min = min * 2; }
                else { min = min * 4; }
                float max = min;
                countPer10kCells = Rand.Range(min, max);
            }
            //
            // Ruins
            //
            if (countPer10kCells > 1.95 && countPer10kCells < 3.85)
            {
                float min = 2;
                float max = 4;
                float v = ThingsSettings.ruinsLevel;
                if (v < 0 || v > 8)
                {
                    v = (Rand.Value) * 8;
                }
                if (v < 1) { min = 0; max = 0; }
                else if (v < 2) { min = 0; max = 1; }
                else if (v < 3) { min = 1; max = 2; }
                else if (v < 4) { min = 2; max = 4; }
                else if (v < 5) { min = 4; max = 8; }
                else if (v < 6) { min = 8; max = 16; }
                else if (v < 7) { min = 16; max = 32; }
                else { min = 32; max = 64; }
                countPer10kCells = Rand.Range(min, max);
            }
            //
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
            //
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
            }
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
        [HarmonyPriority(Priority.High)]
        public static bool Prefix(int tile, ref List<ThingDef> __result)
        {
            if (HarmonyPatches.detectedCuprosStones)
            {
                return true;
            }
            Rand.PushState();
            Rand.Seed = tile;
            List<ThingDef> list = (
                from d in DefDatabase<ThingDef>.AllDefs
                where (d.category != ThingCategory.Building || !d.building.isNaturalRock ? false : !d.building.isResourceRock)
                select d).ToList<ThingDef>();
            int count = Rand.RangeInclusive((int)WorldSettings.stoneMin, (int)WorldSettings.stoneMax);
            if (count > list.Count)
            {
                count = list.Count;
            }
            List<ThingDef> thingDefs = new List<ThingDef>();
            for (int i = 0; i < count; i++)
            {
                ThingDef thingDef = list.RandomElement<ThingDef>();
                if (thingDef.defName == "Granite")
                {
                    if (WorldSettings.graniteCommonality < 1)
                    {
                        if (Rand.Value < 0.90) { i--; continue; }
                    }
                    if (WorldSettings.graniteCommonality < 2)
                    {
                        if (Rand.Value < 0.45) { i--; continue; }
                    }
                }
                else if (thingDef.defName == "Limestone")
                {
                    if (WorldSettings.limestoneCommonality < 1)
                    {
                        if (Rand.Value < 0.90) { i--; continue; }
                    }
                    if (WorldSettings.limestoneCommonality < 2)
                    {
                        if (Rand.Value < 0.45) { i--; continue; }
                    }
                }
                else if (thingDef.defName == "Marble")
                {
                    if (WorldSettings.marbleCommonality < 1)
                    {
                        if (Rand.Value < 0.90) { i--; continue; }
                    }
                    if (WorldSettings.marbleCommonality < 2)
                    {
                        if (Rand.Value < 0.45) { i--; continue; }
                    }
                }
                else if (thingDef.defName == "Sandstone")
                {
                    if (WorldSettings.sandstoneCommonality < 1)
                    {
                        if (Rand.Value < 0.90) { i--; continue; }
                    }
                    if (WorldSettings.sandstoneCommonality < 2)
                    {
                        if (Rand.Value < 0.45) { i--; continue; }
                    }
                }
                else if (thingDef.defName == "Slate")
                {
                    if (WorldSettings.slateCommonality < 1)
                    {
                        if (Rand.Value < 0.90) { i--; continue; }
                    }
                    if (WorldSettings.slateCommonality < 2)
                    {
                        if (Rand.Value < 0.45) { i--; continue; }
                    }
                }
                else
                {
                    if (WorldSettings.extraStoneCommonality < 1)
                    {
                        if (Rand.Value < 0.90) { i--; continue; }
                    }
                    if (WorldSettings.extraStoneCommonality < 2)
                    {
                        if (Rand.Value < 0.45) { i--; continue; }
                    }
                }
                list.Remove(thingDef);
                thingDefs.Add(thingDef);
            }
            Rand.PopState();
            __result = thingDefs;
            return false;
        }
    }

    [HarmonyPatch(typeof(RockNoises), "Init", null)]
    public static class RockNoises_Init
    {
        public static bool Prefix(Map map)
        {
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
        public static bool Prefix()
        {
            if (TerrainSettings.allowFakeOres)
            {
                ThingDefOf.MineableSteel.building.mineableScatterCommonality = 1.0f;
                ThingDef.Named("MineablePlasteel").building.mineableScatterCommonality = 0.05f;
                ThingDef.Named("MineableComponents").building.mineableScatterCommonality = 1.0f;
            }
            else
            {
                ThingDefOf.MineableSteel.building.mineableScatterCommonality = 0f;
                ThingDef.Named("MineablePlasteel").building.mineableScatterCommonality = 0f;
                ThingDef.Named("MineableComponents").building.mineableScatterCommonality = 0f;
            }
            return true;
        }
    }
}