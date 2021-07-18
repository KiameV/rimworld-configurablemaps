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
            if (ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.Name.Contains("[RF] Fertile Fields")))
            {
                Settings.detectedFertileFields = true;
            }
            if (ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.Name.Contains("Cupro's Stones")))
            {
                Settings.detectedCuprosStones = true;
            }
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
                Find.WindowStack.TryRemove(typeof(EditWindow_Log));
                if (!Find.WindowStack.TryRemove(typeof(SettingsWindow)))
                {
                    Find.WindowStack.Add(new SettingsWindow());
                }
            }
        }
    }

    [HarmonyPatch(typeof(Page_SelectStartingSite), "DoWindowContents")]
    public static class Patch_Page_SelectStartingSite_DoWindowContents
    {
        static void Postfix()
        {
            Vector2 BottomButSize = new Vector2(150f, 38f);
            int num = (TutorSystem.TutorialMode ? 4 : 5);
            int num2 = ((num < 4 || !((float)UI.screenWidth < 540f + (float)num * (BottomButSize.x + 10f))) ? 1 : 2);
            int num3 = Mathf.CeilToInt((float)num / (float)num2);
            float num4 = BottomButSize.x * (float)num3 + 10f * (float)(num3 + 1);
            float num5 = (float)num2 * BottomButSize.y + 10f * (float)(num2 + 1);
            Rect rect = new Rect(((float)UI.screenWidth - num4) / 2f, (float)UI.screenHeight - num5 - 4f, num4, num5);
            float num6 = rect.xMin + 10f;
            float num7 = rect.yMin - 50f;
            Text.Font = GameFont.Small;
            if (Widgets.ButtonText(new Rect(num6, num7, BottomButSize.x, BottomButSize.y), "ConfigurableMaps".Translate()))
            {
                Find.WindowStack.TryRemove(typeof(EditWindow_Log));
                if (!Find.WindowStack.TryRemove(typeof(SettingsWindow)))
                {
                    Find.WindowStack.Add(new SettingsWindow());
                }
            }
        }
    }


    [HarmonyPatch(typeof(MainMenuDrawer), "DoMainMenuControls")]
    static class Patch_MainMenuDrawer_DoMainMenuControls
    {
        static void Postfix(Rect rect, bool anyMapFiles)
        {
            if (Current.ProgramState == ProgramState.Entry)
            {
                Text.Font = GameFont.Small;
                float y = ((rect.yMax + rect.yMin) / 2.0f) - 8.5f + 40;
                float x = Math.Max(0, rect.xMax - 598);
                //float x = Math.Max(0, rect.xMax - 915);
                Rect r = new Rect(x, y, 140, 45);
                if (Widgets.ButtonText(r, "ConfigurableMaps".Translate(), true, false, true))
                {
                    Find.WindowStack.Add(new SettingsWindow());
                }
            }
        }
    }

    // CommonMapGenerator.xml
    // def: Caves - GenStep_Caves
    // def: RocksFromGrid - GenStep_RocksFromGrid
    // def: RocksFromGrid_NoMinerals - GenStep_RocksFromGrid
    // def: Terrain - GenStep_Terrain
    // def: CavesTerrain - GenStep_CavesTerrain
    // def: Roads - GenStep_Roads
    // def: RockChunks - GenStep_RockChunks

    [HarmonyPatch(typeof(MapGenerator), "GenerateMap")]
    public class MapGenerator_Generate
    {
        public static System.Random r;
        public static void Prefix()
        {
            r = new System.Random(DateTime.Now.Millisecond);
            DefsUtil.Update(r);

            GenStep_RockChunks_GrowLowRockFormationFrom.ChunkLevel = MapSettings.ChunkLevel;
            if (MapSettings.ChunkLevel == ChunkLevelEnum.Random)
            {
                GenStep_RockChunks_GrowLowRockFormationFrom.ChunkLevel = (ChunkLevelEnum)r.Next(0, Enum.GetNames(typeof(ChunkLevelEnum)).Length - 1);
            }
        }
        public static void Postfix()
        {
            r = null;
            DefsUtil.Restore();
        }
    }

    [HarmonyPatch(typeof(BaseGenUtility), "RandomCheapWallStuff", new Type[] { typeof(TechLevel), typeof(bool) })]
    public static class BaseGenUtility_RandomCheapWallStuff
    {
        public static void Postfix(ref ThingDef __result)
        {
            if (__result == ThingDefOf.WoodLog)
                return;
            if (__result.defName.ToLower().Contains("steel") && MapGenerator_Generate.r.Next() % 3 > 0)
                return;

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

            var orig = __result;
            __result = null;
            if (threshes[0].min < -900)
            {
                //
                // TerrainsByFertility
                // 
                float mod = 0.87f - MapSettings.Fertility;
                if (mod < 0)
                    mod = 0;
                /*switch(MapSettings.Fertility)
                {
                    case FertilityLevelEnum.Rare:
                        mod += 0.3f;
                        break;
                    case FertilityLevelEnum.Uncommon:
                        mod += 0.15f;
                        break;
                    case FertilityLevelEnum.Common:
                        mod += -0.15f;
                        break;
                    case FertilityLevelEnum.Abundant:
                        mod += -0.3f;
                        break;
                }*/
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

            /*/
            // TerrainPatchMakers
            //
            float adjustment;
            switch (MapSettings.Water)
            {
                case 
            }
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
                        if ((threshes[i].min + adjustment) <= fertility && (threshes[i].max + adjustment + richSoil) > fertility)
                        {
                            __result = threshes[i].terrain;
                        }
                    }
                    if (i == 1)
                    {
                        if ((threshes[i].min + adjustment + richSoil) <= fertility && (threshes[i].max + adjustment) > fertility)
                        {
                            __result = threshes[i].terrain;
                        }
                    }
                    else
                    {
                        if ((threshes[i].min + adjustment) <= fertility && (threshes[i].max + adjustment) > fertility)
                        {
                            __result = threshes[i].terrain;
                        }
                    }
                }
                else
                {
                    if ((threshes[i].min + adjustment) <= fertility && (threshes[i].max + adjustment) > fertility)
                    {
                        __result = threshes[i].terrain;
                    }
                }
            }
            return false;*/
            return true;
        }
    }
}
/*
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
}
}*/