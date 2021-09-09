using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ConfigurableMaps
{
    public class MSFieldValues
    {
        public List<FieldValue<float>> TerrainFieldValues;
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
        public static RandomizableMultiplier0 Fertility;
        public static RandomizableMultiplier0 Water;
        public static RandomizableMultiplier0 Mountain;
        public static RandomizableMultiplier Geysers;

        public static RandomizableMultiplier OreLevels;
        public static List<RandomizableMultiplier> Mineables;


        // coast level
        // caves

        // Things
        public static bool AreWallsMadeFromLocal = false;
        public static RandomizableMultiplier AnimalDensity;
        public static RandomizableMultiplier PlantDensity;
        public static RandomizableMultiplier Ruins;
        public static RandomizableMultiplier Shrines;
        public static RandomizableMultiplier AncientPipelineSection;
        public static RandomizableMultiplier AncientJunkClusters;
        /*public static bool EnableAncientUtilBuildings;
        public static bool EnableAncientMechanoidRemains;
        public static bool EnableAncientTurrets;
        public static bool EnableAncientMechs;
        public static bool EnableAncientLandingPad;
        public static bool EnableAncientFences;*/

        private Vector2 terrainScroll = Vector2.zero, thingsScroll = Vector2.zero;
        private float lastYTerrain = 0, lastYThings;
        private static bool initMineables = false;

        public string Name => "CM.MapSettings".Translate();

        public static void Initialize()
        {
            if (Fertility == null)
                Fertility = new RandomizableMultiplier0();
            Fertility.DefaultValue = 0;
            Fertility.RandomMin = -3;
            Fertility.RandomMax = 3;

            if (Water == null)
                Water = new RandomizableMultiplier0();
            Water.DefaultValue = 0;
            Water.RandomMin = -0.75f;
            Water.RandomMax = 0.75f;

            var ms = MineableStuff.GetMineables();
            if (Mineables == null && ms != null && ms.Count > 0)
            {
                Mineables = new List<RandomizableMultiplier>(ms.Count);
                foreach (var m in ms)
                {
                    Mineables.Add(new RandomizableMultiplier()
                    {
                        ThingDef = m,
                        ThingDefName = m.defName,
                        DefaultValue = m.building.mineableScatterCommonality,
                    });
                }
            }
            else if (!initMineables && ms.Count > 0)
            {
                initMineables = true;
                for (int i = Mineables.Count - 1; i >= 0; --i)
                {
                    var m = Mineables[i];
                    if (m.ThingDef == null && m.ThingDefName != "")
                    {
                        m.ThingDef = DefDatabase<ThingDef>.GetNamed(m.ThingDefName, false);
                        if (m.ThingDef == null)
                        {
                            Mineables.RemoveAt(i);
                            Log.Message($"[Configurable Maps] Unable to load mineable thing def {m.ThingDefName}");
                        }
                    }
                }
                foreach (var m in ms)
                {
                    bool found = false;
                    foreach (var r in Mineables)
                    {
                        found = r.ThingDefName == m.defName;
                        if (found)
                        {
                            r.ThingDef = m;
                            r.DefaultValue = m.building.mineableScatterCommonality;
                            break;
                        }
                    }
                    if (!found)
                    {
                        Mineables.Add(new RandomizableMultiplier()
                        {
                            ThingDef = m,
                            ThingDefName = m.defName
                        });
                    }
                }
            }

            if (Geysers == null)
                Geysers = new RandomizableMultiplier();
            Geysers.DefaultValue = 1;

            if (Mountain == null)
                Mountain = new RandomizableMultiplier0();
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

            if (AncientPipelineSection == null)
                AncientPipelineSection = new RandomizableMultiplier();
            AncientPipelineSection.RandomMax = 50;

            if (AncientJunkClusters == null)
                AncientJunkClusters = new RandomizableMultiplier();
            AncientJunkClusters.RandomMax = 50;

            if (OreLevels == null)
                OreLevels = new RandomizableMultiplier();
            OreLevels.DefaultValue = 1;
            OreLevels.Min = 0;
            OreLevels.RandomMin = 0;
            OreLevels.RandomMax = 6;

            int resetCount = 0;
            if (Mineables != null)
            {
                foreach (var m in Mineables)
                {
                    if (m.GetMultiplier() == 1.0f)
                        ++resetCount;
                }
                if (resetCount > 3)
                {
                    Log.Message("[Configurable Maps] Correcting default values for mineables to their default values.");
                    foreach (var m in Mineables)
                        m.SetMultiplier(m.DefaultValue);
                }
            }

            resetCount = 0;
            resetCount += (Fertility?.GetMultiplier() == 1f) ? 1 : 0;
            resetCount += (Water?.GetMultiplier() == 1f) ? 1 : 0;
            resetCount += (Mountain?.GetMultiplier() == 1f) ? 1 : 0;
            if (resetCount >= 2)
            {
                Fertility.SetMultiplier(Fertility.DefaultValue);
                Water.SetMultiplier(Water.DefaultValue);
                Mountain.SetMultiplier(Mountain.DefaultValue);
                Log.Message("[Configurable Maps] Correcting default values for Fertility, Water, and Mountain to their default values.");
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
            Widgets.BeginScrollView(new Rect(rect.x, y, width, rect.height - y), ref terrainScroll, new Rect(0, 0, innerWidth, lastYTerrain));
            lastYTerrain = 0;
            for (int i = 0; i < fv.TerrainFieldValues.Count; ++i)
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

            /*if (ModsConfig.IdeologyActive)
            {
                Widgets.Label(new Rect(0, lastYThings, width - 16, 28), "CM.AncientThingsEnabled".Translate() + ":");
                lastYThings += 30;
                CheckBox(5, ref lastYThings, "CM.AncientUtilBuilding".Translate(), ref EnableAncientUtilBuildings);
                CheckBox(5, ref lastYThings, "CM.AncientMechanoidRemains".Translate(), ref EnableAncientMechanoidRemains);
                CheckBox(5, ref lastYThings, "CM.AncientTurrets".Translate(), ref EnableAncientTurrets);
                CheckBox(5, ref lastYThings, "CM.AncientMechs".Translate(), ref EnableAncientMechs);
                CheckBox(5, ref lastYThings, "CM.AncientLandingPad".Translate(), ref EnableAncientLandingPad);
                CheckBox(5, ref lastYThings, "CM.AncientFences".Translate(), ref EnableAncientFences);
            }*/
            Widgets.EndScrollView();
        }

        private bool CheckBox(float x, ref float y, string label, ref bool b)
        {
            bool v = b;
            Widgets.Label(new Rect(x, y, 200, 28), label);
            Widgets.Checkbox(215, y, ref b);
            y += 30;
            if (v != b && b == true)
            {
                Messages.Message("CM.RequiresRestartIfAMapWasCreated".Translate(), MessageTypeDefOf.CautionInput);
            }
            return v != b;
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
            Scribe_Values.Look(ref AreWallsMadeFromLocal, "AreWallsMadeFromLocal", false);

            Scribe_Deep.Look(ref Fertility, "Fertility");
            Scribe_Deep.Look(ref Water, "Water");
            Scribe_Deep.Look(ref Geysers, "Geysers");
            Scribe_Deep.Look(ref Mountain, "Mountain");

            Scribe_Deep.Look(ref OreLevels, "OreLevels");
            Scribe_Collections.Look(ref Mineables, "Mineables", LookMode.Deep);

            Scribe_Deep.Look(ref AnimalDensity, "AnimalDensity");
            Scribe_Deep.Look(ref PlantDensity, "PlantDensity");
            Scribe_Deep.Look(ref Ruins, "Ruins");
            Scribe_Deep.Look(ref Shrines, "Shrines");
            Scribe_Deep.Look(ref AncientPipelineSection, "AncientPipelineSection");
            Scribe_Deep.Look(ref AncientJunkClusters, "AncientJunkClusters");

            /*Scribe_Values.Look(ref EnableAncientUtilBuildings, "EnableAncientUtilBuildings", true);
            Scribe_Values.Look(ref EnableAncientMechanoidRemains, "MechanoidRemains", true);
            Scribe_Values.Look(ref EnableAncientTurrets, "EnableAncientTurrets", true);
            Scribe_Values.Look(ref EnableAncientMechs, "EnableAncientMechs", true);
            Scribe_Values.Look(ref EnableAncientLandingPad, "EnableAncientLandingPad", true);
            Scribe_Values.Look(ref EnableAncientFences, "EnableAncientFences", true);*/
        }

        public MSFieldValues GetFieldValues()
        {
            Initialize();
            var l = new List<FieldValue<float>>(6 + Mineables.Count)
            {
                new RandomizableMultiplierFieldValue("CM.FertilityLevel".Translate(), Fertility),
                new RandomizableMultiplierFieldValue("CM.WaterLevel".Translate(), Water),
                new RandomizableMultiplierFieldValue("CM.MountainLevel".Translate(), Mountain),
                new RandomizableMultiplierFieldValue("CM.Geysers".Translate(), Geysers),
                new RandomizableMultiplierFieldValue("CM.OreLevels".Translate(), OreLevels),
            };
            foreach (var m in Mineables)
            {
                if (m.ThingDef != null)
                {
                    l.Add(new RandomizableMultiplierFieldValue(m.ThingDef.label, m));
                }
            }

            return new MSFieldValues()
            {
                TerrainFieldValues = l,
                ThingsFieldValues = new RandomizableFieldValue<float>[6]
                    {
                        new RandomizableMultiplierFieldValue("CM.AnimalDensity".Translate(), AnimalDensity),
                        new RandomizableMultiplierFieldValue("CM.PlantDensity".Translate(), PlantDensity),
                        new RandomizableMultiplierFieldValue("CM.Ruins".Translate(), Ruins),
                        new RandomizableMultiplierFieldValue("CM.Shrines".Translate(), Shrines),
                        new RandomizableMultiplierFieldValue("CM.AncientPipelineSection".Translate(), AncientPipelineSection),
                        new RandomizableMultiplierFieldValue("CM.AncientJunkClusters".Translate(), AncientJunkClusters),
                    },
            };
        }
    }
}
