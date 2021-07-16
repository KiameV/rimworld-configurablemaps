using UnityEngine;
using Verse;

namespace ConfigurableMaps.Settings
{
  /*  public class ThingsSettingsController : Mod
    {
        public ThingsSettingsController(ModContentPack content) : base(content)
        {
            base.GetSettings<ThingsSettings>();
        }

        public override string SettingsCategory()
        {
            return "RFR.FewerRuinsThings".Translate();
        }

        public override void DoSettingsWindowContents(Rect canvas)
        {
            BiomeUtil.Init();
            ThingsSettings.DoWindowContents(canvas);
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            BiomeUtil.UpdateBiomeStatsPerUserSettings();
        }
    }

    public class ThingsSettings : ModSettings
    {
        public static float animalDensityLevel = 1f;
        public static float plantDensityLevel = 1f;
        public static float ruinsLevel = 1f;
        public static float shrinesLevel = 1f;
        public static float stoneType = 0.5f;

        public static void DoWindowContents(Rect canvas)
        {
            Listing_Standard list = new Listing_Standard();
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
            list.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref animalDensityLevel, "animalDensityLevel", 1.5f);
            Scribe_Values.Look(ref plantDensityLevel, "plantDensityLevel", 1.5f);
            Scribe_Values.Look(ref ruinsLevel, "ruinsLevel", 1.75f);
            Scribe_Values.Look(ref shrinesLevel, "shrinesLevel", 1.5f);
            Scribe_Values.Look(ref stoneType, "stoneType", 0.5f);

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                HarmonyPatches.UpdateDefs();
            }
        }
    }*/
}
