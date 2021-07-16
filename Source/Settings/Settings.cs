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

    public class WorldSettings : IExposable
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

        public void DoWindowContents(Rect rect)
        {
            lastY = rect.y;
            Widgets.Label(new Rect(rect.x, lastY, rect.width, 28), "CM.StoneTypes".Translate());
            lastY += 30;
            WindowUtil.DrawIntInput(rect.x, ref lastY, "min".Translate().CapitalizeFirst(), v => StoneMin = v, ref buffers[0], DEFAULT_MIN_STONE);
            WindowUtil.DrawIntInput(rect.x, ref lastY, "max".Translate().CapitalizeFirst(), v => StoneMax = v, ref buffers[1], DEFAULT_MAX_STONE);
            if (StoneMin < 1)
                StoneMin = 1;
            if (StoneMax < StoneMin)
                StoneMax = StoneMin;

            lastY += 20;

            Widgets.Label(new Rect(rect.x, lastY, rect.width, 28), "CM.StoneCommonality".Translate());
            lastY += 30;

            Widgets.Label(new Rect(rect.x, lastY, 100, 28), "Randomize".Translate());
            Widgets.Checkbox(new Vector2(rect.x + 110, lastY - 2), ref CommonalityRandom);
            lastY += 30;

            if (!CommonalityRandom)
            {
                Widgets.BeginScrollView
                WindowUtil.DrawInputWithSlider(rect.x, ref lastY, "CM.Granite".Translate(), 0f, 100f, v => CommonalityGranite = v, ref buffers[2], DEFAULT_COMMONALITY);
                WindowUtil.DrawInputWithSlider(rect.x, ref lastY, "CM.Limestone".Translate(), 0f, 100f, v => CommonalityLimestone = v, ref buffers[3], DEFAULT_COMMONALITY);
                WindowUtil.DrawInputWithSlider(rect.x, ref lastY, "CM.Marble".Translate(), 0f, 100f, v => CommonalityMarble = v, ref buffers[4], DEFAULT_COMMONALITY);
                WindowUtil.DrawInputWithSlider(rect.x, ref lastY, "CM.Sandstone".Translate(), 0f, 100f, v => CommonalitySandstone = v, ref buffers[5], DEFAULT_COMMONALITY);
                WindowUtil.DrawInputWithSlider(rect.x, ref lastY, "CM.Slate".Translate(), 0f, 100f, v => CommonalitySlate = v, ref buffers[6], DEFAULT_COMMONALITY);
                // TODO Extra Stone Types
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

    public class Settings : ModSettings
    {
        public static WorldSettings WorldSettings;
        // public static float animalDensityLevel = 1f;
        // public static float plantDensityLevel = 1f;
        // public static float ruinsLevel = 1f;
        // public static float shrinesLevel = 1f;
        // public static float stoneType = 0.5f;

        public static void DoWindowContents(Rect rect)
        {
            WorldSettings.DoWindowContents(rect);
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
                WorldSettings = new WorldSettings();

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
