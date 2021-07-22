using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ConfigurableMaps
{
    public class MSFieldValues
    {
        public FieldValue<float>[] TerrainFieldValues;
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

        public static RandomizableMultiplier MineableGold;
        public static RandomizableMultiplier MineableSilver;
        public static RandomizableMultiplier MineableUranium;
        public static RandomizableMultiplier MineableJade;
        public static RandomizableMultiplier MineableSteel;
        public static RandomizableMultiplier MineablePlasteel;
        public static RandomizableMultiplier MineableComponentsIndustrial;

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
                Fertility = new RandomizableMultiplier();
            Fertility.DefaultValue = 0;
            Fertility.RandomMin = -3;
            Fertility.RandomMax = 3;

            if (Water == null)
                Water = new RandomizableMultiplier();
            Water.DefaultValue = 0;
            Water.RandomMin = -0.75f;
            Water.RandomMax = 0.75f;

            if (MineableGold == null)
                MineableGold = new RandomizableMultiplier();
            if (MineableSilver == null)
                MineableSilver = new RandomizableMultiplier();
            if (MineableUranium == null)
                MineableUranium = new RandomizableMultiplier();
            if (MineableJade == null)
                MineableJade = new RandomizableMultiplier();
            if (MineableSteel == null)
                MineableSteel = new RandomizableMultiplier();
            if (MineablePlasteel == null)
                MineablePlasteel = new RandomizableMultiplier();

            if (MineableComponentsIndustrial == null)
                MineableComponentsIndustrial = new RandomizableMultiplier();

            if (Geysers == null)
                Geysers = new RandomizableMultiplier();
            Geysers.DefaultValue = 1;

            if (Mountain == null)
                Mountain = new RandomizableMultiplier();
            Mountain.Max = 1.4f;
            Mountain.DefaultValue = 0;
            Mountain.RandomMin = -0.15f;
            Mountain.RandomMax = 1.4f;

            if (AnimalDensity == null)
                AnimalDensity = new RandomizableMultiplier();
            AnimalDensity.RandomMax = 6;

            if (PlantDensity == null)
                PlantDensity = new RandomizableMultiplier();
            PlantDensity.RandomMax = 6;

            if (Ruins == null)
                Ruins = new RandomizableMultiplier();
            Ruins.RandomMax = 50f;

            if (Shrines == null)
                Shrines = new RandomizableMultiplier();
            Shrines.RandomMax = 50;

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
            Widgets.BeginScrollView(new Rect(rect.x, y, width, rect.height - y), ref terrainScroll, new Rect(0, 0, innerWidth, lastYTerrain));
            lastYTerrain = 0;
            for (int i = 0; i < fv.TerrainFieldValues.Length; ++i)
            {
                if (i < 4)
                    WindowUtil.DrawInputWithSlider(0, ref lastYTerrain, fv.TerrainFieldValues[i], "PowerConsumptionLow".Translate().CapitalizeFirst(), "PowerConsumptionHigh".Translate().CapitalizeFirst());
                else
                    WindowUtil.DrawInputRandomizableWithSlider(0, ref lastYTerrain, fv.TerrainFieldValues[i] as RandomizableFieldValue<float>);
                lastYTerrain += 5;
            }
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

            Scribe_Deep.Look(ref MineableGold, "MineableGold");
            Scribe_Deep.Look(ref MineableSilver, "MineableSilver");
            Scribe_Deep.Look(ref MineableUranium, "MineableUranium");
            Scribe_Deep.Look(ref MineableJade, "MineableJade");
            Scribe_Deep.Look(ref MineableSteel, "MineableSteel");
            Scribe_Deep.Look(ref MineablePlasteel, "MineablePlasteel");
            Scribe_Deep.Look(ref MineableComponentsIndustrial, "MineableComponentsIndustrial");
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
                TerrainFieldValues = new FieldValue<float>[11]
                {
                    new RandomizableMultiplierFieldValue("CM.FertilityLevel".Translate(), Fertility),
                    new RandomizableMultiplierFieldValue("CM.WaterLevel".Translate(), Water),
                    new RandomizableMultiplierFieldValue("CM.MountainLevel".Translate(), Mountain),
                    new RandomizableMultiplierFieldValue("CM.Geysers".Translate(), Geysers),
                    new RandomizableMultiplierFieldValue("CM.MineableGold".Translate(), MineableGold),
                    new RandomizableMultiplierFieldValue("CM.MineableSilver".Translate(), MineableSilver),
                    new RandomizableMultiplierFieldValue("CM.MineableUranium".Translate(), MineableUranium),
                    new RandomizableMultiplierFieldValue("CM.MineableJade".Translate(), MineableJade),
                    new RandomizableMultiplierFieldValue("CM.MineableSteel".Translate(), MineableSteel),
                    new RandomizableMultiplierFieldValue("CM.MineablePlasteel".Translate(), MineablePlasteel),
                    new RandomizableMultiplierFieldValue("CM.MineableComponentsIndustrial".Translate(), MineableComponentsIndustrial),
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
