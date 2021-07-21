using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ConfigurableMaps
{
    public class MSFieldValues
    {
        public RandomizableFieldValue<float> TerrainFertility;
        public RandomizableFieldValue<float> TerrainWaterLevel;
        public RandomizableFieldValue<float> TerrainMountainLevel;
        public RandomizableFieldValue<float> TerrainGeyserLevel;
        public RandomizableFieldValue<float>[] TerrainOre;
        public RandomizableFieldValue<float>[] ThingsFieldValues;
    }

    public enum ChunkLevelEnum
    {
        None,
        Low,
        Normal,
        Random
    }

    public class MapSettings : IExposable, IWindow<MSFieldValues>
    {
        // Terrain
        public static ChunkLevelEnum ChunkLevel = ChunkLevelEnum.Normal;
        public static RandomizableMultiplier Fertility;
        public static RandomizableMultiplier Water;
        public static RandomizableMultiplier Mountain;

        public static RandomizableMultiplier OreLevel;
        public static RandomizableMultiplier MinableSteel;
        public static RandomizableMultiplier MinablePlasteel;
        public static RandomizableMultiplier MinableComponentsIndustrial;

        public static RandomizableMultiplier Geysers;

        // coast level
        // caves

        // Things
        public static bool AreWallsMadeFromLocal = false;
        public static RandomizableMultiplier AnimalDensity;
        public static RandomizableMultiplier PlantDensity;
        public static RandomizableMultiplier Ruins;
        public static RandomizableMultiplier Shrines;

        private Vector2 terrainScroll = Vector2.zero, thingsScroll = Vector2.zero;
        private float lastYTerrain = 0, lastYThings;

        public string Name => "CM.MapSettings".Translate();

        public static void Initialize()
        {
            if (Fertility == null)
            {
                Fertility = new RandomizableMultiplier();
                Water = new RandomizableMultiplier();
                OreLevel = new RandomizableMultiplier();
                MinableSteel = new RandomizableMultiplier();
                MinablePlasteel = new RandomizableMultiplier();
                MinableComponentsIndustrial = new RandomizableMultiplier();
                Geysers = new RandomizableMultiplier();
                Mountain = new RandomizableMultiplier(1.4f);
                AnimalDensity = new RandomizableMultiplier();
                PlantDensity = new RandomizableMultiplier();
                Ruins = new RandomizableMultiplier();
                Shrines = new RandomizableMultiplier();
            }
        }

        public void DoWindowContents(Rect rect, MSFieldValues fv)
        {
            float half = rect.width * 0.5f;
            float width = half - 10f;
            float innerWidth = width - 16;
            float baseY = rect.y + 5;

            // Terrain
            float y = baseY;
            Widgets.Label(new Rect(0, y, innerWidth, 28), "CM.TerrainType".Translate());
            y += 30;
            WindowUtil.DrawEnumSelection(0, ref y, "CM.ChunksLevel", ChunkLevel, GetChunkLevelLabel, e => ChunkLevel = e);
            y += 5;
            WindowUtil.DrawInputWithSlider(0, ref y, fv.TerrainFertility, "PowerConsumptionLow".Translate().CapitalizeFirst(), "PowerConsumptionHigh".Translate().CapitalizeFirst());
            y += 5;
            WindowUtil.DrawInputWithSlider(0, ref y, fv.TerrainWaterLevel, "PowerConsumptionLow".Translate().CapitalizeFirst(), "PowerConsumptionHigh".Translate().CapitalizeFirst());
            y += 5;
            WindowUtil.DrawInputWithSlider(0, ref y, fv.TerrainMountainLevel, "PowerConsumptionLow".Translate().CapitalizeFirst(), "PowerConsumptionHigh".Translate().CapitalizeFirst());
            Widgets.DrawLineHorizontal(5, y, width - 10);
            y += 5;
            Widgets.BeginScrollView(new Rect(rect.x, y, width, rect.height - y), ref terrainScroll, new Rect(0, 0, innerWidth, lastYTerrain));
            lastYTerrain = 0;
            foreach (var v in fv.TerrainOre)
                WindowUtil.DrawInputRandomizableWithSlider(0, ref lastYTerrain, v);
            Widgets.EndScrollView();

            // Things
            y = baseY;
            Widgets.Label(new Rect(half, y, width, 28), "CM.ThingType".Translate());
            y += 30;

            WindowUtil.DrawBoolInput(half, ref y, "CM.WallsMadeFromLocal".Translate(), AreWallsMadeFromLocal, v => AreWallsMadeFromLocal = v);
            y += 5;
            Widgets.Label(new Rect(half, y, width, 28), "CM.Multipliers".Translate());
            y += 30;
            Widgets.BeginScrollView(new Rect(half, y, width, rect.height - y), ref thingsScroll, new Rect(0, 0, width - 16, lastYThings));
            lastYThings = 0;
            foreach (var v in fv.ThingsFieldValues)
                WindowUtil.DrawInputRandomizableWithSlider(0, ref lastYThings, v);
            Widgets.EndScrollView();
        }

        private string GetChunkLevelLabel(ChunkLevelEnum e)
        {
            switch (e)
            {
                case ChunkLevelEnum.None:
                    return "None".Translate();
                case ChunkLevelEnum.Low:
                    return "StoragePriorityLow".Translate().CapitalizeFirst();
                case ChunkLevelEnum.Normal:
                    return "StoragePriorityNormal".Translate().CapitalizeFirst();
            }
            return "CM.Random".Translate();
        }

        public void ExposeData()
        {
            Initialize();
            Scribe_Values.Look(ref ChunkLevel, "ChunkLevel", ChunkLevelEnum.Normal);
            Scribe_Deep.Look(ref Fertility, "Fertility");
            Scribe_Deep.Look(ref Water, "Water");
            Scribe_Values.Look(ref AreWallsMadeFromLocal, "AreWallsMadeFromLocal", false);

            Scribe_Deep.Look(ref OreLevel, "Ore");
            Scribe_Deep.Look(ref MinableSteel, "MinableSteel");
            Scribe_Deep.Look(ref MinablePlasteel, "MinablePlasteel");
            Scribe_Deep.Look(ref MinableComponentsIndustrial, "MinableComponentsIndustrial");
            Scribe_Deep.Look(ref Geysers, "Geysers");
            Scribe_Deep.Look(ref Mountain, "Mountain");

            Scribe_Deep.Look(ref AnimalDensity, "AnimalDensity");
            Scribe_Deep.Look(ref PlantDensity, "PlantDensity");
            Scribe_Deep.Look(ref Ruins, "Ruins");
            Scribe_Deep.Look(ref Shrines, "Shrines");
        }

        public MSFieldValues GetFieldValues()
        {
            Initialize();
            return new MSFieldValues()
            {
                TerrainFertility = new RandomizableMultiplierFieldValue("CM.FertilityLevel".Translate(), Fertility, -0.3f, 0.3f, 0f),
                TerrainWaterLevel = new RandomizableMultiplierFieldValue("CM.WaterLevel".Translate(), Water, -0.75f, 0.75f, 0f),
                TerrainMountainLevel = new RandomizableMultiplierFieldValue("CM.MountainLevel".Translate(), Mountain, -0.15f, 1f, 0.0f),
                TerrainGeyserLevel = new RandomizableMultiplierFieldValue("CM.Geysers".Translate(), Geysers),
                TerrainOre = new RandomizableFieldValue<float>[4]
                {
                    new RandomizableMultiplierFieldValue("CM.Ore".Translate(), OreLevel, 0f, 4f, 1f),
                    new RandomizableMultiplierFieldValue("CM.MinableSteel".Translate(), MinableSteel, 0f, 4f, 1f),
                    new RandomizableMultiplierFieldValue("CM.MinablePlasteel".Translate(), MinablePlasteel, 0f, 4f, 1f),
                    new RandomizableMultiplierFieldValue("CM.MinableComponentsIndustrial".Translate(), MinableComponentsIndustrial, 0f, 4f, 1f),
                },
                ThingsFieldValues = new RandomizableFieldValue<float>[4]
                {
                    new RandomizableMultiplierFieldValue("CM.AnimalDensity".Translate(), AnimalDensity),
                    new RandomizableMultiplierFieldValue("CM.PlantDensity".Translate(), PlantDensity),
                    new RandomizableMultiplierFieldValue("CM.Ruins".Translate(), Ruins),
                    new RandomizableMultiplierFieldValue("CM.Shrines".Translate(), Shrines),
                },
                // new RandomizableMultiplierFieldValue("CM.".Translate(), v => .Multiplier = v, () => .Multiplier, 0, 4, DEFAULT_MULTIPLIER, b => .IsRandom = b, () => .IsRandom),
            };
        }
    }
}
