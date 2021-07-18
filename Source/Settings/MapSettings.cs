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

    public enum ChunkLevelEnum
    {
        None,
        Low,
        Normal,
        Random
    }

    public enum FertilityLevelEnum
    {
        Rare,
        Uncommon,
        Normal,
        Common,
        Abundant,
        Random
    }

    public class MapSettings : IExposable, IWindow<MSFieldValues>
    {
        // Terrain
        public static ChunkLevelEnum ChunkLevel = ChunkLevelEnum.Normal;
        public static FertilityLevelEnum Fertility = FertilityLevelEnum.Normal;

        public static RandomizableMultiplier Ore;
        public static RandomizableMultiplier MinableSteel;
        public static RandomizableMultiplier MinablePlasteel;
        public static RandomizableMultiplier MinableComponentsIndustrial;
        public static RandomizableMultiplier Geysers;
        public static RandomizableMultiplier Mountain;
        public static RandomizableMultiplier Water;

        // coast level
        // caves

        // Things
        public static bool AreWallsMadeFromLocal = false;
        public static RandomizableMultiplier AnimalDensity;
        public static RandomizableMultiplier PlantDensity;
        public static RandomizableMultiplier CaveHives;
        public static RandomizableMultiplier Ruins;
        public static RandomizableMultiplier Shrines;

        private Vector2 terrainScroll = Vector2.zero, thingsScroll = Vector2.zero;
        private float lastYTerrain = 0, lastYThings;

        public string Name => "CM.MapSettings".Translate();

        private void Initialize()
        {
            if (Ore == null)
            {
                Ore = new RandomizableMultiplier();
                MinableSteel = new RandomizableMultiplier();
                MinablePlasteel = new RandomizableMultiplier();
                MinableComponentsIndustrial = new RandomizableMultiplier();
                Geysers = new RandomizableMultiplier();
                Mountain = new RandomizableMultiplier();
                Water = new RandomizableMultiplier();
                AnimalDensity = new RandomizableMultiplier();
                PlantDensity = new RandomizableMultiplier();
                CaveHives = new RandomizableMultiplier();
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
            Widgets.BeginScrollView(new Rect(rect.x, y, innerWidth, rect.height - y), ref terrainScroll, new Rect(0, 0, width - 16, lastYTerrain));
            lastYTerrain = 0;
            WindowUtil.DrawEnumSelection(0, ref lastYTerrain, "CM.chunksLevel", ChunkLevel, GetChunkLevelLabel, e => ChunkLevel = e);
            WindowUtil.DrawEnumSelection(0, ref lastYTerrain, "CM.fertilityLevel", Fertility, GetFertilityLabel, e => Fertility = e);
            lastYTerrain += 5;
            Widgets.Label(new Rect(rect.x, lastYTerrain, innerWidth, 28), "CM.TerrainTypeMultipliers".Translate());
            lastYTerrain += 30;
            foreach (var v in fv.TerrainFieldValues)
                WindowUtil.DrawInputRandomizableWithSlider(0, ref lastYTerrain, v);
            Widgets.EndScrollView();

            // Things
            y = baseY;
            WindowUtil.DrawBoolInput(half, ref y, "CM.WallsMadeFromLocal", AreWallsMadeFromLocal, v => AreWallsMadeFromLocal = v);
            y += 5;
            Widgets.Label(new Rect(half, y, width, 28), "CM.ThingTypeMultipliers".Translate());
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

        private string GetFertilityLabel(FertilityLevelEnum e)
        {
            switch (e)
            {
                case FertilityLevelEnum.Rare:
                    return "PlanetPopulation_Low".Translate().CapitalizeFirst();
                case FertilityLevelEnum.Uncommon:
                    return "PowerConsumptionLow".Translate().CapitalizeFirst();
                case FertilityLevelEnum.Normal:
                    return "StoragePriorityNormal".Translate().CapitalizeFirst();
                case FertilityLevelEnum.Common:
                    return "PowerConsumptionHigh".Translate().CapitalizeFirst();
                case FertilityLevelEnum.Abundant:
                    return "PsychicDroneLevel_BadExtreme".Translate().CapitalizeFirst();
            }
            return "CM.Random".Translate();
        }

        public void ExposeData()
        {
            Initialize();
            Scribe_Values.Look(ref ChunkLevel, "ChunkLevel", ChunkLevelEnum.Normal);
            Scribe_Values.Look(ref Fertility, "Fertility", FertilityLevelEnum.Normal);
            Scribe_Values.Look(ref AreWallsMadeFromLocal, "AreWallsMadeFromLocal", false);

            Scribe_Deep.Look(ref Ore, "Ore");
            Scribe_Deep.Look(ref MinableSteel, "MinableSteel");
            Scribe_Deep.Look(ref MinablePlasteel, "MinablePlasteel");
            Scribe_Deep.Look(ref MinableComponentsIndustrial, "MinableComponentsIndustrial");
            Scribe_Deep.Look(ref Geysers, "Geysers");
            Scribe_Deep.Look(ref Mountain, "Mountain");
            Scribe_Deep.Look(ref Water, "Water");

            Scribe_Deep.Look(ref AnimalDensity, "AnimalDensity");
            Scribe_Deep.Look(ref PlantDensity, "PlantDensity");
            Scribe_Deep.Look(ref CaveHives, "CaveHives");
            Scribe_Deep.Look(ref Ruins, "Ruins");
            Scribe_Deep.Look(ref Shrines, "Shrines");
        }

        public MSFieldValues GetFieldValues()
        {
            Initialize();
            return new MSFieldValues()
            {
                TerrainFieldValues = new RandomizableFieldValue<float>[7]
                {
                    new RandomizableMultiplierFieldValue("CM.Ore".Translate(), Ore),
                    new RandomizableMultiplierFieldValue("CM.MinableSteel".Translate(), MinableSteel),
                    new RandomizableMultiplierFieldValue("CM.MinablePlasteel".Translate(), MinablePlasteel),
                    new RandomizableMultiplierFieldValue("CM.MinableComponentsIndustrial".Translate(), MinableComponentsIndustrial),
                    new RandomizableMultiplierFieldValue("CM.Geysers".Translate(), Geysers),
                    new RandomizableMultiplierFieldValue("CM.Mountain".Translate(), Mountain),
                    new RandomizableMultiplierFieldValue("CM.Water".Translate(), Water),
                },
                ThingsFieldValues = new RandomizableFieldValue<float>[5]
                {
                    new RandomizableMultiplierFieldValue("CM.AnimalDensity".Translate(), AnimalDensity),
                    new RandomizableMultiplierFieldValue("CM.PlantDensity".Translate(), PlantDensity),
                    new RandomizableMultiplierFieldValue("CM.CaveHives".Translate(), CaveHives),
                    new RandomizableMultiplierFieldValue("CM.Ruins".Translate(), Ruins),
                    new RandomizableMultiplierFieldValue("CM.Shrines".Translate(), Shrines),
                },
                // new RandomizableMultiplierFieldValue("CM.".Translate(), v => .Multiplier = v, () => .Multiplier, 0, 4, DEFAULT_MULTIPLIER, b => .IsRandom = b, () => .IsRandom),
            };
        }
    }
}
