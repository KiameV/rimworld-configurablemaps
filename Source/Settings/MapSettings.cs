using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ConfigurableMaps
{
    public class MSFieldValues
    {
        public RandomizableFieldValue<float>[] TerrainFieldValues;
        public RandomizableFieldValue<float>[] ThingsFieldValues;
    }

    public enum ChunkLevel
    {
        None,
        Low,
        Normal,
        Random,
    }

    public class MapSettings : IExposable, IWindow<MSFieldValues>
    {
        const float DEFAULT_MULTIPLIER = 1f;
        // Terrain
        // TODO public static bool allowFakeOres = true;
        public static ChunkLevel ChunkLevel;
        public static float OreMultiplier;
        public static bool IsOreMultiplierRandom;
        public static float GeysersMultiplier;
        public static bool IsGeysersMultiplierRandom;
        public static float MountainMultiplier;
        public static bool IsMountainMultiplierRandom;
        public static float WaterMultiplier;
        public static bool IsWaterMultiplierRandom;
        public static float FertilityMultiplier;
        public static bool IsFertilityMultiplierRandom;

        // coast level
        // caves

        // Things
        public static bool AreWallsMadeFromLocal;
        public static float AnimalDensityMultiplier;
        public static bool IsAnimalDensityMultiplierRandom;
        public static float PlantDensityMultiplier;
        public static bool IsPlantDensityMultiplierRandom;
        public static float RuinsMultiplier;
        public static bool IsRuinsMultiplierRandom;
        public static float ShrinesMultiplier;
        public static bool IsShrinesMultiplierRandom;

        private Vector2 terrainScroll = Vector2.zero, thingsScroll = Vector2.zero;
        private float lastYTerrain = 0, lastYThings;

        public string Name => "CM.MapSettings".Translate();

        public void DoWindowContents(Rect rect, MSFieldValues fv)
        {
            float y = rect.y;
            float half = rect.width * 0.5f;
            float width = half - 5f;
            Widgets.Label(new Rect(rect.x, y, width, 28), "CM.TerrainTypeMultipliers".Translate());
            Widgets.Label(new Rect(half, y, width, 28), "CM.ThingTypeMultipliers".Translate());
            y += 30;

            // Terrain
            Widgets.BeginScrollView(new Rect(rect.x, y, width, rect.height - y), ref terrainScroll, new Rect(0, 0, width - 16, lastYTerrain));
            lastYTerrain = 0;
            DrawChunksSelection(ref lastYTerrain, width);
            foreach (var v in fv.TerrainFieldValues)
                WindowUtil.DrawInputRandomizableWithSlider(0, ref lastYTerrain, v);
            Widgets.EndScrollView();

            // Things
            Widgets.BeginScrollView(new Rect(half, y, width, rect.height - y), ref thingsScroll, new Rect(0, 0, width - 16, lastYThings));
            lastYThings = 0;
            WindowUtil.DrawBoolInput(0, ref lastYThings, "CM.WallsMadeFromLocal", AreWallsMadeFromLocal, v => AreWallsMadeFromLocal = v);
            foreach (var v in fv.ThingsFieldValues)
                WindowUtil.DrawInputRandomizableWithSlider(0, ref lastYThings, v);
            Widgets.EndScrollView();
        }

        private void DrawChunksSelection(ref float lastYTerrain, float width)
        {
            Widgets.Label(new Rect(0, lastYTerrain, 150, 28), "CM.chunksLevel".Translate());
            if (Widgets.ButtonText(new Rect(160, lastYTerrain, 100, 28), GetChunkLevelLabel(ChunkLevel)))
            {
                List<FloatMenuOption> l = new List<FloatMenuOption>(4)
                {
                    new FloatMenuOption(GetChunkLevelLabel(ChunkLevel.None), () => ChunkLevel = ChunkLevel.None),
                    new FloatMenuOption(GetChunkLevelLabel(ChunkLevel.Low), () => ChunkLevel = ChunkLevel.Low),
                    new FloatMenuOption(GetChunkLevelLabel(ChunkLevel.Normal), () => ChunkLevel = ChunkLevel.Normal),
                    new FloatMenuOption(GetChunkLevelLabel(ChunkLevel.Random), () => ChunkLevel = ChunkLevel.Random),
                };
                Find.WindowStack.Add(new FloatMenu(l));
            }
            lastYTerrain += 35;
        }

        private string GetChunkLevelLabel(ChunkLevel c)
        {
            switch (c)
            {
                case ChunkLevel.None:
                    return "None".Translate();
                case ChunkLevel.Low:
                    return "StoragePriorityLow".Translate().CapitalizeFirst();
                case ChunkLevel.Normal:
                    return "StoragePriorityNormal".Translate().CapitalizeFirst();
            }
            return "CM.Random".Translate();
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref ChunkLevel, "CM.ChunkLevel", ChunkLevel.Normal);

            Scribe_Values.Look(ref OreMultiplier, "CM.OreMultiplier", DEFAULT_MULTIPLIER);
            Scribe_Values.Look(ref IsOreMultiplierRandom, "CM.IsOreMultiplierRandom", false);
            Scribe_Values.Look(ref GeysersMultiplier, "CM.GeysersMultiplier", DEFAULT_MULTIPLIER);
            Scribe_Values.Look(ref IsGeysersMultiplierRandom, "CM.IsGeysersMultiplierRandom", false);
            Scribe_Values.Look(ref MountainMultiplier, "CM.MountainMultiplier", DEFAULT_MULTIPLIER);
            Scribe_Values.Look(ref IsMountainMultiplierRandom, "CM.IsMountainMultiplierRandom", false);
            Scribe_Values.Look(ref WaterMultiplier, "CM.WaterMultiplier", DEFAULT_MULTIPLIER);
            Scribe_Values.Look(ref IsWaterMultiplierRandom, "CM.IsWaterMultiplierRandom", false);
            Scribe_Values.Look(ref FertilityMultiplier, "CM.FertilityMultiplier", DEFAULT_MULTIPLIER);
            Scribe_Values.Look(ref IsFertilityMultiplierRandom, "CM.IsFertilityMultiplierRandom", false);

            Scribe_Values.Look(ref AnimalDensityMultiplier, "CM.AnimalDensityMultiplier", DEFAULT_MULTIPLIER);
            Scribe_Values.Look(ref IsAnimalDensityMultiplierRandom, "CM.IsAnimalDensityMultiplierRandom", false);
            Scribe_Values.Look(ref PlantDensityMultiplier, "CM.PlantDensityMultiplier", DEFAULT_MULTIPLIER);
            Scribe_Values.Look(ref IsPlantDensityMultiplierRandom, "CM.IsPlantDensityMultiplierRandom", false);
            Scribe_Values.Look(ref RuinsMultiplier, "CM.RuinsMultiplier", DEFAULT_MULTIPLIER);
            Scribe_Values.Look(ref IsRuinsMultiplierRandom, "CM.IsRuinsMultiplierRandom", false);
            Scribe_Values.Look(ref ShrinesMultiplier, "CM.ShrinesMultiplier", DEFAULT_MULTIPLIER);
            Scribe_Values.Look(ref IsShrinesMultiplierRandom, "CM.IsShrinesMultiplierRandom", false);
            Scribe_Values.Look(ref AreWallsMadeFromLocal, "CM.AreWallsMadeFromLocal", false);
        }

        public MSFieldValues GetFieldValues()
        {
            return new MSFieldValues()
            {
                TerrainFieldValues = new RandomizableFieldValue<float>[5]
                {
                    new RandomizableFieldValue<float>("CM.OreMultiplier".Translate(), v => OreMultiplier = v, () => OreMultiplier, 0, 4, DEFAULT_MULTIPLIER, b => IsOreMultiplierRandom = b, () => IsOreMultiplierRandom),
                    new RandomizableFieldValue<float>("CM.GeysersMultiplier".Translate(), v => GeysersMultiplier = v, () => GeysersMultiplier, 0, 4, DEFAULT_MULTIPLIER, b => IsGeysersMultiplierRandom = b, () => IsGeysersMultiplierRandom),
                    new RandomizableFieldValue<float>("CM.MountainMultiplier".Translate(), v => MountainMultiplier = v, () => MountainMultiplier, 0, 4, DEFAULT_MULTIPLIER, b => IsMountainMultiplierRandom = b, () => IsMountainMultiplierRandom),
                    new RandomizableFieldValue<float>("CM.WaterMultiplier".Translate(), v => WaterMultiplier = v, () => WaterMultiplier, 0, 4, DEFAULT_MULTIPLIER, b => IsWaterMultiplierRandom = b, () => IsWaterMultiplierRandom),
                    new RandomizableFieldValue<float>("CM.FertilityMultiplier".Translate(), v => FertilityMultiplier = v, () => FertilityMultiplier, 0, 4, DEFAULT_MULTIPLIER, b => IsFertilityMultiplierRandom = b, () => IsFertilityMultiplierRandom),
                },
                ThingsFieldValues = new RandomizableFieldValue<float>[4]
                {
                    new RandomizableFieldValue<float>("CM.AnimalDensityMultiplier".Translate(), v => AnimalDensityMultiplier = v, () => AnimalDensityMultiplier, 0, 4, DEFAULT_MULTIPLIER, b => IsAnimalDensityMultiplierRandom = b, () => IsAnimalDensityMultiplierRandom),
                    new RandomizableFieldValue<float>("CM.PlantDensityMultiplier".Translate(), v => PlantDensityMultiplier = v, () => PlantDensityMultiplier, 0, 4, DEFAULT_MULTIPLIER, b => IsPlantDensityMultiplierRandom = b, () => IsPlantDensityMultiplierRandom),
                    new RandomizableFieldValue<float>("CM.RuinsMultiplier".Translate(), v => RuinsMultiplier = v, () => RuinsMultiplier, 0, 4, DEFAULT_MULTIPLIER, b => IsRuinsMultiplierRandom = b, () => IsRuinsMultiplierRandom),
                    new RandomizableFieldValue<float>("CM.ShrinesMultiplier".Translate(), v => ShrinesMultiplier = v, () => ShrinesMultiplier, 0, 4, DEFAULT_MULTIPLIER, b => IsShrinesMultiplierRandom = b, () => IsShrinesMultiplierRandom),
                },
            };
        }
    }
}
