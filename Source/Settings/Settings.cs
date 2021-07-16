using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ConfigurableMaps.Settings
{
    public class SettingsController : Mod
    {
        public SettingsController(ModContentPack content) : base(content)
        {
            base.GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "ConfigurableMaps".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoWindowContents(inRect);
        }
    }

    public interface IWindow
    {
        string Name{ get; }
        void DoWindowContents(Rect inRect);
    }

    public class WorldSettings : IExposable, IWindow
    {
        const int DEFAULT_MIN_STONE = 2;
        const int DEFAULT_MAX_STONE = 3;
        const float DEFAULT_COMMONALITY = 50f;

        public int StoneMin = DEFAULT_MIN_STONE;
        public int StoneMax = DEFAULT_MAX_STONE;
        public bool CommonalityRandom = false;
        public float CommonalityGranite = DEFAULT_COMMONALITY;
        public float CommonalityLimestone = DEFAULT_COMMONALITY;
        public float CommonalityMarble = DEFAULT_COMMONALITY;
        public float CommonalitySandstone = DEFAULT_COMMONALITY;
        public float CommonalitySlate = DEFAULT_COMMONALITY;
        private Vector2 scroll = Vector2.zero;
        private float lastY = 0;
        private string[] buffers = new string[7];

        public WorldSettings()
        {
            BuildBuffers();
        }

        public string Name => "CM.WorldSettings".Translate();

        public void DoWindowContents(Rect rect)
        {
            float y = rect.y;
            Widgets.Label(new Rect(rect.x, y, rect.width, 28), "CM.StoneTypes".Translate());
            y += 30;
            WindowUtil.DrawIntInput(rect.x, ref y, "min".Translate().CapitalizeFirst(), v => StoneMin = v, ref buffers[0], DEFAULT_MIN_STONE);
            WindowUtil.DrawIntInput(rect.x, ref y, "max".Translate().CapitalizeFirst(), v => StoneMax = v, ref buffers[1], DEFAULT_MAX_STONE);
            if (StoneMin < 1)
                StoneMin = 1;
            if (StoneMax < StoneMin)
                StoneMax = StoneMin;

            y += 20;

            Widgets.Label(new Rect(rect.x, y, rect.width, 28), "CM.StoneCommonality".Translate());
            y += 30;

            Widgets.Label(new Rect(rect.x, y, 100, 28), "Randomize".Translate());
            Widgets.Checkbox(new Vector2(rect.x + 110, y - 2), ref CommonalityRandom);
            y += 30;

            if (!CommonalityRandom)
            {
                Widgets.BeginScrollView(new Rect(rect.x, y, rect.width, rect.height - y), ref scroll, new Rect(0, 0, rect.width - rect.x - 16, lastY));
                lastY = 0;
                WindowUtil.DrawInputWithSlider(rect.x, ref lastY, "CM.Granite".Translate(), 0f, 100f, v => CommonalityGranite = v, ref buffers[2], DEFAULT_COMMONALITY);
                WindowUtil.DrawInputWithSlider(rect.x, ref lastY, "CM.Limestone".Translate(), 0f, 100f, v => CommonalityLimestone = v, ref buffers[3], DEFAULT_COMMONALITY);
                WindowUtil.DrawInputWithSlider(rect.x, ref lastY, "CM.Marble".Translate(), 0f, 100f, v => CommonalityMarble = v, ref buffers[4], DEFAULT_COMMONALITY);
                WindowUtil.DrawInputWithSlider(rect.x, ref lastY, "CM.Sandstone".Translate(), 0f, 100f, v => CommonalitySandstone = v, ref buffers[5], DEFAULT_COMMONALITY);
                WindowUtil.DrawInputWithSlider(rect.x, ref lastY, "CM.Slate".Translate(), 0f, 100f, v => CommonalitySlate = v, ref buffers[6], DEFAULT_COMMONALITY);
                // TODO Extra Stone Types
                Widgets.EndScrollView();
            }
        }

        public void ExposeData()
        {
            if (StoneMin < 1)
                StoneMin = 1;
            if (StoneMax < StoneMin)
                StoneMax = StoneMin;

            Scribe_Values.Look(ref StoneMin, "StoneMin", DEFAULT_MIN_STONE);
            Scribe_Values.Look(ref StoneMax, "StoneMax", DEFAULT_MAX_STONE);
            Scribe_Values.Look(ref CommonalityRandom, "CommonalityRandom", false);
            Scribe_Values.Look(ref CommonalityGranite, "CommonalityGranite", DEFAULT_COMMONALITY);
            Scribe_Values.Look(ref CommonalityLimestone, "CommonalityLimestone", DEFAULT_COMMONALITY);
            Scribe_Values.Look(ref CommonalityMarble, "CommonalityMarble", DEFAULT_COMMONALITY);
            Scribe_Values.Look(ref CommonalitySandstone, "CommonalitySandstone", DEFAULT_COMMONALITY);
            Scribe_Values.Look(ref CommonalitySlate, "CommonalitySlate", DEFAULT_COMMONALITY);

            BuildBuffers();
        }

        private void BuildBuffers()
        {
            buffers[0] = StoneMin.ToString();
            buffers[1] = StoneMax.ToString();
            buffers[2] = CommonalityGranite.ToString("0.00");
            buffers[3] = CommonalityLimestone.ToString("0.00");
            buffers[4] = CommonalityMarble.ToString("0.00");
            buffers[5] = CommonalitySandstone.ToString("0.00");
            buffers[6] = CommonalitySlate.ToString("0.00");
        }
    }

    public class Randomizable<T>
    {
        public T Value, Min, Max, Default;
        public bool Randomize = false;
        public Randomizable(T value, T min, T max, T d)
        {
            this.Value = value;
            this.Min = min;
            this.Max = max;
            this.Default = d;
        }
    }

    public class MapSettings : IExposable, IWindow
    {
        const float DEFAULT_MULTIPLIER = 1f;
        // Terrain
        public static Randomizable<float> OreLevel = new Randomizable<float>(1, 0, 10, DEFAULT_MULTIPLIER); // GenStep_RocksFromGrid.Generate - genStep_ScatterLumpsMineable.countPer10kCellsRange = new FloatRange(num3, num3);
        public static Randomizable<float> GeysersLevel = new Randomizable<float>(1, 0, 10, DEFAULT_MULTIPLIER);
        public static Randomizable<float> ChunksLevel = new Randomizable<float>(1, 0, 10, DEFAULT_MULTIPLIER);
        public static Randomizable<float> MountainLevel = new Randomizable<float>(1, 0, 10, DEFAULT_MULTIPLIER);
        public static Randomizable<float> WaterLevel = new Randomizable<float>(1, 0, 10, DEFAULT_MULTIPLIER);
        public static Randomizable<float> FertilityLevel = new Randomizable<float>(1, 0, 10, DEFAULT_MULTIPLIER);

        public static bool allowFakeOres = true;

        // Things
        public static Randomizable<float> AnimalDensityLevel = new Randomizable<float>(1, 0, 10, DEFAULT_MULTIPLIER);
        public static Randomizable<float> PlantDensityLevel = new Randomizable<float>(1, 0, 10, DEFAULT_MULTIPLIER);
        public static Randomizable<float> RuinsLevel = new Randomizable<float>(1, 0, 10, DEFAULT_MULTIPLIER);
        public static Randomizable<float> ShrinesLevel = new Randomizable<float>(1, 0, 10, DEFAULT_MULTIPLIER);
        public static Randomizable<float> MiscWallStoneType = new Randomizable<float>(1, 0, 10, DEFAULT_MULTIPLIER);

        private Vector2 terrainScroll = Vector2.zero, thingsScroll = Vector2.zero;
        private float lastYTerrain = 0, lastYThings;
        private string[] buffersTerrain = new string[6], buffersThings = new string[5];

        public MapSettings()
        {
            BuildBuffers();
        }

        public string Name => "CM.MapSettings".Translate();

        public void DoWindowContents(Rect rect)
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
            WindowUtil.DrawInputRandomizableWithSlider(rect.x, ref lastYTerrain, "CM.OreLevel".Translate(), 0f, 10f, v => OreLevel.Value = v, ref buffersTerrain[0], ref OreLevel.Randomize, DEFAULT_MULTIPLIER);
            WindowUtil.DrawInputRandomizableWithSlider(rect.x, ref lastYTerrain, "CM.GeysersLevel".Translate(), 0f, 10f, v => GeysersLevel.Value = v, ref buffersTerrain[1], ref GeysersLevel.Randomize, DEFAULT_MULTIPLIER);
            WindowUtil.DrawInputRandomizableWithSlider(rect.x, ref lastYTerrain, "CM.ChunksLevel".Translate(), 0f, 10f, v => ChunksLevel.Value = v, ref buffersTerrain[2], ref ChunksLevel.Randomize, DEFAULT_MULTIPLIER);
            WindowUtil.DrawInputRandomizableWithSlider(rect.x, ref lastYTerrain, "CM.MountainLevel".Translate(), 0f, 10f, v => MountainLevel.Value = v, ref buffersTerrain[3], ref MountainLevel.Randomize, DEFAULT_MULTIPLIER);
            WindowUtil.DrawInputRandomizableWithSlider(rect.x, ref lastYTerrain, "CM.WaterLevel".Translate(), 0f, 10f, v => WaterLevel.Value = v, ref buffersTerrain[4], ref WaterLevel.Randomize, DEFAULT_MULTIPLIER);
            WindowUtil.DrawInputRandomizableWithSlider(rect.x, ref lastYTerrain, "CM.FertilityLevel".Translate(), 0f, 10f, v => FertilityLevel.Value = v, ref buffersTerrain[5], ref FertilityLevel.Randomize, DEFAULT_MULTIPLIER);
            Widgets.EndScrollView();

            // Things
            Widgets.BeginScrollView(new Rect(half, y, width, rect.height - y), ref thingsScroll, new Rect(0, 0, width - 16, lastYThings));
            lastYThings = 0;
            WindowUtil.DrawInputRandomizableWithSlider(rect.x, ref lastYThings, "CM.AnimalDensityLevel".Translate(), 0f, 10f, v => AnimalDensityLevel.Value = v, ref buffersThings[0], ref AnimalDensityLevel.Randomize, DEFAULT_MULTIPLIER);
            WindowUtil.DrawInputRandomizableWithSlider(rect.x, ref lastYThings, "CM.PlantDensityLevel".Translate(), 0f, 10f, v => PlantDensityLevel.Value = v, ref buffersThings[1], ref PlantDensityLevel.Randomize, DEFAULT_MULTIPLIER);
            WindowUtil.DrawInputRandomizableWithSlider(rect.x, ref lastYThings, "CM.RuinsLevel".Translate(), 0f, 10f, v => RuinsLevel.Value = v, ref buffersThings[2], ref RuinsLevel.Randomize, DEFAULT_MULTIPLIER);
            WindowUtil.DrawInputRandomizableWithSlider(rect.x, ref lastYThings, "CM.ShrinesLevel".Translate(), 0f, 10f, v => ShrinesLevel.Value = v, ref buffersThings[3], ref ShrinesLevel.Randomize, DEFAULT_MULTIPLIER);
            WindowUtil.DrawInputRandomizableWithSlider(rect.x, ref lastYThings, "CM.MiscWallStoneType".Translate(), 0f, 10f, v => MiscWallStoneType.Value = v, ref buffersThings[4], ref MiscWallStoneType.Randomize, DEFAULT_MULTIPLIER);
            Widgets.EndScrollView();
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref OreLevel.Value, "OreLevel", DEFAULT_MULTIPLIER);
            Scribe_Values.Look(ref GeysersLevel.Value, "GeysersLevel", DEFAULT_MULTIPLIER);
            Scribe_Values.Look(ref ChunksLevel.Value, "ChunksLevel", DEFAULT_MULTIPLIER);
            Scribe_Values.Look(ref MountainLevel.Value, "MountainLevel", DEFAULT_MULTIPLIER);
            Scribe_Values.Look(ref WaterLevel.Value, "WaterLevel", DEFAULT_MULTIPLIER);
            Scribe_Values.Look(ref FertilityLevel.Value, "FertilityLevel", DEFAULT_MULTIPLIER);

            BuildBuffers();
        }

        private void BuildBuffers()
        {
            buffersTerrain[0] = OreLevel.Value.ToString("0.00");
            buffersTerrain[1] = GeysersLevel.Value.ToString("0.00");
            buffersTerrain[2] = ChunksLevel.Value.ToString("0.00");
            buffersTerrain[3] = MountainLevel.Value.ToString("0.00");
            buffersTerrain[4] = WaterLevel.Value.ToString("0.00");
            buffersTerrain[5] = FertilityLevel.Value.ToString("0.00");

            buffersThings[0] = AnimalDensityLevel.Value.ToString("0.00");
            buffersThings[1] = PlantDensityLevel.Value.ToString("0.00");
            buffersThings[2] = RuinsLevel.Value.ToString("0.00");
            buffersThings[3] = ShrinesLevel.Value.ToString("0.00");
            buffersThings[4] = MiscWallStoneType.Value.ToString("0.00");
        }
    }

    public class Settings : ModSettings
    {
        public static WorldSettings WorldSettings;
        public static MapSettings MapSettings;

        private static IWindow selectedSetting;

        public static void DoWindowContents(Rect rect)
        {
            if (Widgets.ButtonText(new Rect(rect.x, rect.y, 300, 28), selectedSetting?.Name ?? "WorldChooseButton".Translate()))
            {
                List<FloatMenuOption> l = new List<FloatMenuOption>()
                {
                    new FloatMenuOption(WorldSettings.Name, () => selectedSetting = WorldSettings),
                    new FloatMenuOption(MapSettings.Name, () => selectedSetting = MapSettings)
                };
                Find.WindowStack.Add(new FloatMenu(l));
            }
            rect.y += 30f;
            selectedSetting?.DoWindowContents(rect);
            /*Listing_Standard list = new Listing_Standard();
            list.ColumnWidth = canvas.width;
            list.Begin(canvas);
            // ---------------------- //
            // -- Density of Ruins -- //
            // ---------------------- //
            list.Gap();
            if (ruinsLevel < 0.5) { list.Label("RFR.ruinsLevel".Translate() + "  " + "RFR.RuinsNone".Translate()); }
            else if (ruinsLevel < 1) { list.Label("RFR.ruinsLevel".Translate() + "  " + "RFR.RuinsVeryLow".Translate()); }
            else if (ruinsLevel < 1.5) { list.Label("RFR.ruinsLevel".Translate() + "  " + "RFR.RuinsLow".Translate()); }
            else if (ruinsLevel < 2)
            {
                GUI.contentColor = Color.yellow;
                list.Label("RFR.ruinsLevel".Translate() + "  " + "RFR.RuinsNormal".Translate());
                GUI.contentColor = Color.white;
            }
            else if (ruinsLevel < 3) { list.Label("RFR.ruinsLevel".Translate() + "  " + "RFR.RuinsHigh".Translate()); }
            else if (ruinsLevel < 4.5) { list.Label("RFR.ruinsLevel".Translate() + "  " + "RFR.RuinsVeryHigh".Translate()); }
            else if (ruinsLevel < 6) { list.Label("RFR.ruinsLevel".Translate() + "  " + "RFR.RuinsExtreme".Translate()); }
            else if (ruinsLevel < 8) { list.Label("RFR.ruinsLevel".Translate() + "  " + "RFR.RuinsInsane".Translate()); }
            else
            {
                GUI.contentColor = Color.cyan;
                list.Label("RFR.ruinsLevel".Translate() + "  " + "RFR.RuinsRandom".Translate());
                GUI.contentColor = Color.white;
            }
            ruinsLevel = list.Slider(ruinsLevel, 0, 9);

            // ------------------------ //
            // -- Density of Shrines -- //
            // ------------------------ //
            list.Gap();
            if (shrinesLevel < 1) { list.Label("RFR.shrinesLevel".Translate() + "  " + "RFR.ShrinesNone".Translate()); }
            else if (shrinesLevel < 2) { list.Label("RFR.shrinesLevel".Translate() + "  " + "RFR.ShrinesVeryLow".Translate()); }
            else if (shrinesLevel < 3) { list.Label("RFR.shrinesLevel".Translate() + "  " + "RFR.ShrinesLow".Translate()); }
            else if (shrinesLevel < 4)
            {
                GUI.contentColor = Color.yellow;
                list.Label("RFR.shrinesLevel".Translate() + "  " + "RFR.ShrinesNormal".Translate());
                GUI.contentColor = Color.white;
            }
            else if (shrinesLevel < 5) { list.Label("RFR.shrinesLevel".Translate() + "  " + "RFR.ShrinesHigh".Translate()); }
            else if (shrinesLevel < 6) { list.Label("RFR.shrinesLevel".Translate() + "  " + "RFR.ShrinesVeryHigh".Translate()); }
            else if (shrinesLevel < 7) { list.Label("RFR.shrinesLevel".Translate() + "  " + "RFR.ShrinesExtreme".Translate()); }
            else if (shrinesLevel < 8) { list.Label("RFR.shrinesLevel".Translate() + "  " + "RFR.ShrinesInsane".Translate()); }
            else
            {
                GUI.contentColor = Color.cyan;
                list.Label("RFR.shrinesLevel".Translate() + "  " + "RFR.ShrinesRandom".Translate());
                GUI.contentColor = Color.white;
            }
            shrinesLevel = list.Slider(shrinesLevel, 0, 9);

            // ---------------------- //
            // -- Ruins Stone Type -- //
            // ---------------------- //
            list.Gap();
            if (stoneType < 1)
            {
                GUI.contentColor = Color.yellow;
                list.Label("RFR.stoneType".Translate() + "  " + "RFR.StoneVanilla".Translate());
                GUI.contentColor = Color.white;
            }
            else if (stoneType < 2) { list.Label("RFR.stoneType".Translate() + "  " + "RFR.StoneMixed".Translate()); }
            else if (stoneType < 3) { list.Label("RFR.stoneType".Translate() + "  " + "RFR.StoneLocal".Translate()); }
            else
            {
                GUI.contentColor = Color.cyan;
                list.Label("RFR.stoneType".Translate() + "  " + "RFR.StoneRandom".Translate());
                GUI.contentColor = Color.white;
            }
            stoneType = list.Slider(stoneType, 0, 4);

            // -------------------- //
            // -- Animal Density -- //
            // -------------------- //
            list.Gap();
            if (animalDensityLevel < 1) { list.Label("RFR.animalDensityLevel".Translate() + "  " + "RFR.AnimalDensitySparse".Translate()); }
            else if (animalDensityLevel < 2)
            {
                GUI.contentColor = Color.yellow;
                list.Label("RFR.animalDensityLevel".Translate() + "  " + "RFR.AnimalDensityNormal".Translate());
                GUI.contentColor = Color.white;
            }
            else if (animalDensityLevel < 3) { list.Label("RFR.animalDensityLevel".Translate() + "  " + "RFR.AnimalDensityDense".Translate()); }
            else if (animalDensityLevel < 4) { list.Label("RFR.animalDensityLevel".Translate() + "  " + "RFR.AnimalDensityExtreme".Translate()); }
            else
            {
                GUI.contentColor = Color.cyan;
                list.Label("RFR.animalDensityLevel".Translate() + "  " + "RFR.AnimalDensityRandom".Translate());
                GUI.contentColor = Color.white;
            }
            animalDensityLevel = list.Slider(animalDensityLevel, 0, 5);

            // ------------------- //
            // -- Plant Density -- //
            // ------------------- //
            list.Gap();
            if (plantDensityLevel < 1) { list.Label("RFR.plantDensityLevel".Translate() + "  " + "RFR.PlantDensitySparse".Translate()); }
            else if (plantDensityLevel < 2)
            {
                GUI.contentColor = Color.yellow;
                list.Label("RFR.plantDensityLevel".Translate() + "  " + "RFR.PlantDensityNormal".Translate());
                GUI.contentColor = Color.white;
            }
            else if (plantDensityLevel < 3) { list.Label("RFR.plantDensityLevel".Translate() + "  " + "RFR.PlantDensityDense".Translate()); }
            else if (plantDensityLevel < 4) { list.Label("RFR.plantDensityLevel".Translate() + "  " + "RFR.PlantDensityExtreme".Translate()); }
            else
            {
                GUI.contentColor = Color.cyan;
                list.Label("RFR.plantDensityLevel".Translate() + "  " + "RFR.PlantDensityRandom".Translate());
                GUI.contentColor = Color.white;
            }
            plantDensityLevel = list.Slider(plantDensityLevel, 0, 5);
            list.End();*/
        }

        public override void ExposeData()
        {
            if (WorldSettings == null)
            {
                WorldSettings = new WorldSettings();
                MapSettings = new MapSettings();
            }

            base.ExposeData();
            Scribe_Deep.Look(ref WorldSettings, "worldSettings");
            /*
            Scribe_Values.Look(ref animalDensityLevel, "animalDensityLevel", 1.5f);
            Scribe_Values.Look(ref plantDensityLevel, "plantDensityLevel", 1.5f);
            Scribe_Values.Look(ref ruinsLevel, "ruinsLevel", 1.75f);
            Scribe_Values.Look(ref shrinesLevel, "shrinesLevel", 1.5f);
            Scribe_Values.Look(ref stoneType, "stoneType", 0.5f);

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                HarmonyPatches.UpdateDefs();
            }*/
        }
    }
}
