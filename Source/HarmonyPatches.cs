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
    [StaticConstructorOnStartup]
    public class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("com.configurablemaps.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            //LongEventHandler.QueueLongEvent(new Action(UpdateDefs), "LibraryStartup", false, null);
            HashSet<string> mods = new HashSet<string>();
            foreach (var m in ModsConfig.ActiveModsInLoadOrder)
                mods.Add(m.Name);
            Settings.detectedFertileFields = mods.Contains("[RF] Fertile Fields");
            Settings.detectedCuprosStones = mods.Contains("Cupro's Stones");
            Settings.detectedImpassableMaps = mods.Contains("[KV] Impassable Map Maker");
        }
    }

    [HarmonyPatch(typeof(Page_CreateWorldParams), "DoWindowContents")]
    public static class Patch_Page_CreateWorldParams_DoWindowContents
    {
        static void Postfix(Rect rect)
        {
            float y = rect.y + rect.height - 78f;
            Text.Font = GameFont.Small;
            string label = "ConfigurableMaps".Translate();
            if (Widgets.ButtonText(new Rect(0, y, 150, 32), label))
            {
                //Find.WindowStack.TryRemove(typeof(EditWindow_Log));
                if (!Find.WindowStack.TryRemove(typeof(SettingsWindow)))
                {
                    Find.WindowStack.Add(new SettingsWindow());
                }
            }
        }
    }

    [HarmonyPatch(typeof(GenStep_Scatterer), "CountFromPer10kCells", null)]
    public static class GenStep_Scatterer_CountFromPer10kCells
    {
        public static void Prefix(ref float countPer10kCells)
        {
            if (DefsUtil.Enable && !DefsUtil.LumpsApplied)
            {
                if (Environment.StackTrace.Contains("GenStep_ScatterLumpsMineable"))
                {
                    float prev = countPer10kCells;
                    countPer10kCells *= MapSettings.OreLevels.GetMultiplier();
                    Log.Message($"[Configurable Maps] OreLevel = {countPer10kCells}. Was {prev}");
                    DefsUtil.LumpsApplied = true;
                }
            }
        }
    }

    [HarmonyPatch(typeof(MapGenerator), "GenerateMap")]
    public class MapGenerator_Generate
    {
        [HarmonyPriority(Priority.First)]
        public static void Prefix(MapParent parent, MapGeneratorDef mapGenerator)
        {
            foreach (var q in Current.Game.questManager.QuestsListForReading)
            {
                if (q.State == QuestState.Ongoing)
                {
                    foreach (var qt in q.QuestLookTargets)
                    {
                        if (qt.Tile == parent.Tile)
                        {
                            DefsUtil.Enable = false;
                            Log.Message("[Configurable Maps] this tile has a quest on it. Disabling map modifications.");
                            return;
                        }
                    }
                }
            }

            try
            {
                DefsUtil.Update();
            }
            catch
            {
                Log.Error("[Configurable Maps] failed to apply map settings.");
            }

            GenStep_RockChunks_GrowLowRockFormationFrom.ChunkLevel = MapSettings.ChunkLevel;
            if (MapSettings.ChunkLevel == ChunkLevelEnum.Random)
            {
                GenStep_RockChunks_GrowLowRockFormationFrom.ChunkLevel = (ChunkLevelEnum)Rand.RangeInclusive(0, Enum.GetNames(typeof(ChunkLevelEnum)).Length - 1);
            }
        }
        [HarmonyPriority(Priority.First)]
        public static void Postfix()
        {
            DefsUtil.Enable = true;
            DefsUtil.Restore();
        }
    }

    [HarmonyPatch(typeof(WorldGenerator), "GenerateWorld")]
    public class WorldGenerator_Generate
    {
        struct WeightedStoneType
        {
            public ThingDef Def;
            public float Weight;
            public WeightedStoneType(ThingDef def, float weight)
            {
                this.Def = def;
                this.Weight = weight;
            }
        }

        [HarmonyPriority(Priority.First)]
        public static void Prefix()
        {
            try
            {
                DefsUtil.Update();
            }
            catch
            {
                Log.Error("[Configurable Maps] failed to apply map settings.");
            }
        }

        private static List<WeightedStoneType> weighted = null;
        [HarmonyPriority(Priority.First)]
        public static void Postfix()
        {
            weighted?.Clear();
            weighted = null;
        }

        public static List<ThingDef> GetRandomStone(int wanted)
        {
            Init();
            List<ThingDef> result;
            if (weighted.Count < wanted)
            {
                string msg = "[Configurable Maps] Not enough weighted stone types > 0, using them all.";
                Log.ErrorOnce(msg, msg.GetHashCode());
                result = new List<ThingDef>(weighted.Count);
                var defs = DefDatabase<ThingDef>.AllDefs.Where((ThingDef d) => d.IsNonResourceNaturalRock).ToList();
                for (int i = 0; i < wanted && i < defs.Count; ++i)
                    result.Add(defs[i]);
                return result;
            }

            List<WeightedStoneType> w = new List<WeightedStoneType>(weighted.Count);
            foreach (WeightedStoneType wst in weighted)
            {
                if (wst.Weight > 0)
                    w.Add(wst);
            }

            if (w.Count == wanted)
            {
                result = new List<ThingDef>(wanted);
                foreach (var i in w)
                    if (i.Weight > 0)
                        result.Add(i.Def);
            }
            if (w.Count < wanted)
            {
                string msg = "[Configurable Maps] Not enough weighted stone types > 0, getting first found.";
                Log.ErrorOnce(msg, msg.GetHashCode());
                result = new List<ThingDef>(wanted);
                var defs = DefDatabase<ThingDef>.AllDefs.Where((ThingDef d) => d.IsNonResourceNaturalRock).ToList();
                for (int i = 0; i < wanted && i < defs.Count; ++i)
                    result.Add(defs[i]);
                return result;
            }
            
            result = new List<ThingDef>(wanted);
            while (result.Count < wanted)
            {
                float sum = 0;
                foreach (WeightedStoneType wst in w)
                    sum += wst.Weight;
                sum /= w.Count;
                sum = Rand.RangeInclusive((int)(sum * 3), (int)(sum * 7));

                while (sum > 0)
                {
                    for (int i = 0; i < w.Count; ++i)
                    {
                        sum -= w[i].Weight;
                        if (sum <= 0)
                        {
                            result.Add(w[i].Def);
                            w.RemoveAt(i);
                            break;
                        }

                    }
                }
            }
            return result;
        }

        private static void Init()
        {
            if (weighted != null && weighted.Count > 0)
                return;

            var stoneDefs = DefDatabase<ThingDef>.AllDefs.Where((ThingDef d) => d.IsNonResourceNaturalRock).ToList();
            weighted = new List<WeightedStoneType>(stoneDefs.Count);

            foreach (var d in stoneDefs)
            {
                if (WorldSettings.CommonalityRandom)
                {
                    var i = Rand.RangeInclusive(0, 10);
                    weighted.Add(new WeightedStoneType(d, i));
                }
                else
                {
                    switch (d.defName.ToLower())
                    {
                        case "granite":
                            if (WorldSettings.CommonalityGranite > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalityGranite));
                            break;
                        case "limestone":
                            if (WorldSettings.CommonalityLimestone > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalityLimestone));
                            break;
                        case "marble":
                            if (WorldSettings.CommonalityMarble > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalityMarble));
                            break;
                        case "sandstone":
                            if (WorldSettings.CommonalitySandstone > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalitySandstone));
                            break;
                        case "slate":
                            if (WorldSettings.CommonalitySlate > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalitySlate));
                            break;
                        case "claystone":
                            if (WorldSettings.CommonalityClaystone > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalityClaystone));
                            break;
                        case "andesite":
                            if (WorldSettings.CommonalityAndesite > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalityAndesite));
                            break;
                        case "syenite":
                            if (WorldSettings.CommonalitySyenite > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalitySyenite));
                            break;
                        case "gneiss":
                            if (WorldSettings.CommonalityGneiss > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalityGneiss));
                            break;
                        case "quartzite":
                            if (WorldSettings.CommonalityQuartzite > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalityQuartzite));
                            break;
                        case "schis":
                            if (WorldSettings.CommonalitySchist > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalitySchist));
                            break;
                        case "gabbro":
                            if (WorldSettings.CommonalityGabbro > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalityGabbro));
                            break;
                        case "diorite":
                            if (WorldSettings.CommonalityDiorite > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalityDiorite));
                            break;
                        case "dunite":
                            if (WorldSettings.CommonalityDunite > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalityDunite));
                            break;
                        case "pegmatite":
                            if (WorldSettings.CommonalityPegmatite > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalityPegmatite));
                            break;
                        default:
                            Log.Message($"[Configurable Maps] unknown stone type {d.defName}. Using weight {WorldSettings.CommonalityOther}");
                            if (WorldSettings.CommonalityOther > 0)
                                weighted.Add(new WeightedStoneType(d, WorldSettings.CommonalityOther));
                            break;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(BaseGenUtility), "RandomCheapWallStuff", new Type[] { typeof(TechLevel), typeof(bool) })]
    public static class BaseGenUtility_RandomCheapWallStuff
    {
        public static void Postfix(ref ThingDef __result)
        {
            if (__result == ThingDefOf.WoodLog)
                return;
            if (__result.defName.ToLower().Contains("steel") && Rand.Int % 3 > 0)
                return;
            try
            {
                Map map = BaseGen.globalSettings.map;
                ThingDef rockType = Find.World.NaturalRockTypesIn(map.Tile).RandomElement<ThingDef>();
                if (rockType != null)
                {
                    var def = DefDatabase<ThingDef>.GetNamed("Blocks" + rockType.defName, false);
                    if (def != null)
                        __result = def;
                    else
                        Log.Warning("Configurable Maps: Failed to find Block for rock type " + rockType.defName);
                }
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(GenStep_RockChunks), "GrowLowRockFormationFrom", null)]
    public static class GenStep_RockChunks_GrowLowRockFormationFrom
    {
        public static ChunkLevelEnum ChunkLevel = ChunkLevelEnum.Normal;
        public static bool Prefix()
        {
            if (ChunkLevel == ChunkLevelEnum.None)
                return false;
            if (ChunkLevel == ChunkLevelEnum.Low)
                return Rand.Value < 0.5f;
            return true;
        }
    }

    [HarmonyPatch(typeof(TerrainThreshold), "TerrainAtValue", null)]
    public static class TerrainThreshold_TerrainAtValue
    {
        public static bool Prefix(List<TerrainThreshold> threshes, float val, ref TerrainDef __result)
        {
            // val is fertility
            float mod;
            var orig = __result;
            __result = null;
            float chosenFertility = MapSettings.Fertility.GetMultiplier();
            if (threshes[0].min < -900)
            {
                //
                // TerrainsByFertility
                // 
                mod = 0.87f - chosenFertility;
                if (mod < 0)
                    mod = 0;
                if (threshes[0].terrain.defName == "Soil")
                {
                    if (val >= -999f && val < mod)
                        __result = TerrainDef.Named("Soil");
                    else if (val >= mod && val < 999f)
                        __result = TerrainDef.Named("SoilRich");
                }
                else if (threshes[0].terrain.defName == "Sand" && (threshes[0].max < 0.5f))
                {
                    mod += 0.03f;
                    if (val >= -999f && val < 0.45f)
                        __result = TerrainDef.Named("Sand");
                    else if (val >= 0.45f && val < mod)
                        __result = TerrainDef.Named("Soil");
                    else if (val >= mod && val < 999f)
                        __result = TerrainDef.Named("SoilRich");
                }
                else
                    return true;

                if (__result == null)
                {
                    __result = orig;
                    return true;
                }
                return false;
            }

            //
            // TerrainPatchMakers
            //
            float chosenWaterLevel = MapSettings.Water.GetMultiplier() * -1;
            if (Math.Abs(chosenWaterLevel) < 0.01)
            {
                // Use base game's code
                return true;
            }
            chosenFertility *= -2;
            for (int i = 0; i < threshes.Count; i++)
            {
                if (threshes[0].terrain.defName == "SoilRich")
                {
                    if (i == 0)
                    {
                        if ((threshes[i].min + chosenWaterLevel) <= val && (threshes[i].max + chosenWaterLevel + chosenFertility) > val)
                        {
                            __result = threshes[i].terrain;
                        }
                    }
                    if (i == 1)
                    {
                        if ((threshes[i].min + chosenWaterLevel + chosenFertility) <= val && (threshes[i].max + chosenWaterLevel) > val)
                        {
                            __result = threshes[i].terrain;
                        }
                    }
                    else
                    {
                        if ((threshes[i].min + chosenWaterLevel) <= val && (threshes[i].max + chosenWaterLevel) > val)
                        {
                            __result = threshes[i].terrain;
                        }
                    }
                }
                else
                {
                    if ((threshes[i].min + chosenWaterLevel) <= val && (threshes[i].max + chosenWaterLevel) > val)
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
        [HarmonyPriority(Priority.High)]
        public static bool Prefix(Map map)
        {
            if (!DefsUtil.Enable)
                return true;
            if (Settings.detectedImpassableMaps && map.TileInfo.hilliness == Hilliness.Impassable)
                return true;

            NoiseRenderer.renderSize = new IntVec2(map.Size.x, map.Size.z);
            ModuleBase input = new Perlin(0.020999999716877937, 2.0, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
            input = new ScaleBias(0.5, 0.5, input);
            NoiseDebugUI.StoreNoiseRender(input, "elev base");
            float num = 1f;
            switch (map.TileInfo.hilliness)
            {
                case Hilliness.Flat:
                    num = MapGenTuning.ElevationFactorFlat;
                    break;
                case Hilliness.SmallHills:
                    num = MapGenTuning.ElevationFactorSmallHills;
                    break;
                case Hilliness.LargeHills:
                    num = MapGenTuning.ElevationFactorLargeHills;
                    break;
                case Hilliness.Mountainous:
                    num = MapGenTuning.ElevationFactorMountains;
                    break;
                case Hilliness.Impassable:
                    num = MapGenTuning.ElevationFactorImpassableMountains;
                    break;
            }
            input = new Multiply(input, new Const(num + MapSettings.Mountain.GetMultiplier()));
            NoiseDebugUI.StoreNoiseRender(input, "elev world-factored");
            if (map.TileInfo.hilliness == Hilliness.Mountainous || map.TileInfo.hilliness == Hilliness.Impassable)
            {
                ModuleBase input2 = new DistFromAxis((float)map.Size.x * 0.42f);
                input2 = new Clamp(0.0, 1.0, input2);
                input2 = new Invert(input2);
                input2 = new ScaleBias(1.0, 1.0, input2);
                Rot4 random;
                do
                {
                    random = Rot4.Random;
                }
                while (random == Find.World.CoastDirectionAt(map.Tile));
                if (random == Rot4.North)
                {
                    input2 = new Rotate(0.0, 90.0, 0.0, input2);
                    input2 = new Translate(0.0, 0.0, -map.Size.z, input2);
                }
                else if (random == Rot4.East)
                {
                    input2 = new Translate(-map.Size.x, 0.0, 0.0, input2);
                }
                else if (random == Rot4.South)
                {
                    input2 = new Rotate(0.0, 90.0, 0.0, input2);
                }
                else
                {
                    _ = random == Rot4.West;
                }
                NoiseDebugUI.StoreNoiseRender(input2, "mountain");
                input = new Add(input, input2);
                NoiseDebugUI.StoreNoiseRender(input, "elev + mountain");
            }
            float b = (map.TileInfo.WaterCovered ? 0f : float.MaxValue);
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            foreach (IntVec3 allCell in map.AllCells)
            {
                elevation[allCell] = Mathf.Min(input.GetValue(allCell), b);
            }
            ModuleBase input3 = new Perlin(0.020999999716877937, 2.0, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
            input3 = new ScaleBias(0.5, 0.5, input3);
            NoiseDebugUI.StoreNoiseRender(input3, "noiseFert base");
            MapGenFloatGrid fertility = MapGenerator.Fertility;
            foreach (IntVec3 allCell2 in map.AllCells)
            {
                fertility[allCell2] = input3.GetValue(allCell2);
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
            Rand.PushState();
            Rand.Seed = tile;
            try
            {
                if (WorldSettings.StoneMin <= 0)
                    WorldSettings.StoneMin = 1;
                if (WorldSettings.StoneMax < WorldSettings.StoneMin)
                    WorldSettings.StoneMax = WorldSettings.StoneMin;
                int rockTypesInTile = Rand.RangeInclusive(WorldSettings.StoneMin, WorldSettings.StoneMax);
                __result = WorldGenerator_Generate.GetRandomStone(rockTypesInTile);
            }
            finally
            {
                Rand.PopState();
            }
            return false;
        }
    }

    /*[HarmonyPatch(typeof(RockNoises), "Init", null)]
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
    }*/
}