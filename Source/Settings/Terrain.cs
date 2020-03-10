using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ConfigurableMaps.Settings
{
    public class TerrainSettingsController : Mod
    {
        public TerrainSettingsController(ModContentPack content) : base(content)
        {
            base.GetSettings<TerrainSettings>();
        }

        public override string SettingsCategory()
        {
            return "RFR.FewerRuinsTerrain".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            TerrainSettings.DoWindowContents(inRect);
        }
    }

    public class TerrainSettings : ModSettings
    {
        public static float oreLevel = 2.5f;
        public static float geysersLevel = 1f;
        public static float chunksLevel = 1f;
        public static float mountainLevel = 2f;
        public static float waterLevel = 2f;
        public static float fertilityLevel = 2f;
        //public static float coastLevel = 2f;

        public static bool disallowIslands = false;
        public static bool allowFakeOres = true;

        public static void DoWindowContents(Rect inRect)
        {
            Listing_Standard list = new Listing_Standard();
            list.ColumnWidth = inRect.width;
            list.Begin(inRect);
            // ----------------- //
            // -- Ore Density -- //
            // ----------------- //
            list.Gap();
            if (oreLevel < 1) { list.Label("RFR.oreLevel".Translate() + "  " + "RFR.OreNone".Translate()); }
            else if (oreLevel < 2) { list.Label("RFR.oreLevel".Translate() + "  " + "RFR.OreLow".Translate()); }
            else if (oreLevel < 3)
            {
                GUI.contentColor = Color.yellow;
                list.Label("RFR.oreLevel".Translate() + "  " + "RFR.OreNormal".Translate());
                GUI.contentColor = Color.white;
            }
            else if (oreLevel < 4) { list.Label("RFR.oreLevel".Translate() + "  " + "RFR.OreHigh".Translate()); }
            else if (oreLevel < 5) { list.Label("RFR.oreLevel".Translate() + "  " + "RFR.OreVeryHigh".Translate()); }
            else
            {
                GUI.contentColor = Color.cyan;
                list.Label("RFR.oreLevel".Translate() + "  " + "RFR.OreRandom".Translate());
                GUI.contentColor = Color.white;
            }
            oreLevel = list.Slider(oreLevel, 0, 6);
            // ----------------------- //
            // -- Number of Geysers -- //
            // ----------------------- //
            list.Gap();
            if (geysersLevel < 1) { list.Label("RFR.geysersLevel".Translate() + "  " + "RFR.GeysersNone".Translate()); }
            else if (geysersLevel < 2) { list.Label("RFR.geysersLevel".Translate() + "  " + "RFR.GeysersLow".Translate()); }
            else if (geysersLevel < 3)
            {
                GUI.contentColor = Color.yellow;
                list.Label("RFR.geysersLevel".Translate() + "  " + "RFR.GeysersNormal".Translate());
                GUI.contentColor = Color.white;
            }
            else if (geysersLevel < 4) { list.Label("RFR.geysersLevel".Translate() + "  " + "RFR.GeysersHigh".Translate()); }
            else if (geysersLevel < 5) { list.Label("RFR.geysersLevel".Translate() + "  " + "RFR.GeysersVeryHigh".Translate()); }
            else
            {
                GUI.contentColor = Color.cyan;
                list.Label("RFR.geysersLevel".Translate() + "  " + "RFR.GeysersRandom".Translate());
                GUI.contentColor = Color.white;
            }
            geysersLevel = list.Slider(geysersLevel, 0, 6);
            // ---------------------------- //
            // -- Number of Stone Chunks -- //
            // ---------------------------- //
            list.Gap();
            if (chunksLevel < 1) { list.Label("RFR.chunksLevel".Translate() + "  " + "RFR.ChunksNone".Translate()); }
            else if (chunksLevel < 2) { list.Label("RFR.chunksLevel".Translate() + "  " + "RFR.ChunksLow".Translate()); }
            else if (chunksLevel < 3)
            {
                GUI.contentColor = Color.yellow;
                list.Label("RFR.chunksLevel".Translate() + "  " + "RFR.ChunksNormal".Translate());
                GUI.contentColor = Color.white;
            }
            else
            {
                GUI.contentColor = Color.cyan;
                list.Label("RFR.chunksLevel".Translate() + "  " + "RFR.ChunksRandom".Translate());
                GUI.contentColor = Color.white;
            }
            chunksLevel = list.Slider(chunksLevel, 0, 4);
            // -------------------- //
            // -- Mountain Level -- //
            // -------------------- //
            list.Gap();
            if (mountainLevel < 1) { list.Label("RFR.mountainLevel".Translate() + "  " + "RFR.MountainVeryLow".Translate()); }
            else if (mountainLevel < 2) { list.Label("RFR.mountainLevel".Translate() + "  " + "RFR.MountainLow".Translate()); }
            else if (mountainLevel < 3)
            {
                GUI.contentColor = Color.yellow;
                list.Label("RFR.mountainLevel".Translate() + "  " + "RFR.MountainNormal".Translate());
                GUI.contentColor = Color.white;
            }
            else if (mountainLevel < 4) { list.Label("RFR.mountainLevel".Translate() + "  " + "RFR.MountainHigh".Translate()); }
            else if (mountainLevel < 5) { list.Label("RFR.mountainLevel".Translate() + "  " + "RFR.MountainVeryHigh".Translate()); }
            else if (mountainLevel < 6) { list.Label("RFR.mountainLevel".Translate() + "  " + "RFR.MountainInsane".Translate()); }
            else
            {
                GUI.contentColor = Color.cyan;
                list.Label("RFR.mountainLevel".Translate() + "  " + "RFR.MountainRandom".Translate());
                GUI.contentColor = Color.white;
            }
            mountainLevel = list.Slider(mountainLevel, 0, 7);
            // ----------------- //
            // -- Water Level -- //
            // ----------------- //
            list.Gap();
            if (waterLevel < 1) { list.Label("RFR.waterLevel".Translate() + "  " + "RFR.WaterVeryLow".Translate()); }
            else if (waterLevel < 2) { list.Label("RFR.waterLevel".Translate() + "  " + "RFR.WaterLow".Translate()); }
            else if (waterLevel < 3)
            {
                GUI.contentColor = Color.yellow;
                list.Label("RFR.waterLevel".Translate() + "  " + "RFR.WaterNormal".Translate());
                GUI.contentColor = Color.white;
            }
            else if (waterLevel < 4) { list.Label("RFR.waterLevel".Translate() + "  " + "RFR.WaterHigh".Translate()); }
            else if (waterLevel < 5) { list.Label("RFR.waterLevel".Translate() + "  " + "RFR.WaterVeryHigh".Translate()); }
            else
            {
                GUI.contentColor = Color.cyan;
                list.Label("RFR.waterLevel".Translate() + "  " + "RFR.WaterRandom".Translate());
                GUI.contentColor = Color.white;
            }
            waterLevel = list.Slider(waterLevel, 0, 6);
            // -------------------- //
            // -- Soil Fertility -- //
            // -------------------- //
            list.Gap();
            if (fertilityLevel < 1) { list.Label("RFR.fertilityLevel".Translate() + "  " + "RFR.FertilityNone".Translate()); }
            else if (fertilityLevel < 2) { list.Label("RFR.fertilityLevel".Translate() + "  " + "RFR.FertilityLow".Translate()); }
            else if (fertilityLevel < 3)
            {
                GUI.contentColor = Color.yellow;
                list.Label("RFR.fertilityLevel".Translate() + "  " + "RFR.FertilityNormal".Translate());
                GUI.contentColor = Color.white;
            }
            else if (fertilityLevel < 4) { list.Label("RFR.fertilityLevel".Translate() + "  " + "RFR.FertilityHigh".Translate()); }
            else if (fertilityLevel < 5) { list.Label("RFR.fertilityLevel".Translate() + "  " + "RFR.FertilityVeryHigh".Translate()); }
            else
            {
                GUI.contentColor = Color.cyan;
                list.Label("RFR.fertilityLevel".Translate() + "  " + "RFR.FertilityRandom".Translate());
                GUI.contentColor = Color.white;
            }
            fertilityLevel = list.Slider(fertilityLevel, 0, 6);
            /*/ ----------------- //
            // -- Coast Level -- //
            // ----------------- //
            list.Gap();
            if (coastLevel < 1)
            {
                GUI.contentColor = Color.yellow;
                list.Label("RFR.coastLevel".Translate() + "  " + "RFR.CoastNormal".Translate());
                GUI.contentColor = Color.white;
            }
            else if (coastLevel < 2) { list.Label("RFR.coastLevel".Translate() + "  " + "RFR.CoastHigh1".Translate()); }
            else if (coastLevel < 3) { list.Label("RFR.coastLevel".Translate() + "  " + "RFR.CoastHigh2".Translate()); }
            else if (coastLevel < 4) { list.Label("RFR.coastLevel".Translate() + "  " + "RFR.CoastVeryHigh1".Translate()); }
            else if (coastLevel < 5) { list.Label("RFR.coastLevel".Translate() + "  " + "RFR.CoastVeryHigh2".Translate()); }
            else if (coastLevel < 6) { list.Label("RFR.coastLevel".Translate() + "  " + "RFR.CoastVeryHigh3".Translate()); }
            else if (coastLevel < 7) { list.Label("RFR.coastLevel".Translate() + "  " + "RFR.CoastInsane".Translate()); }
            else
            {
                GUI.contentColor = Color.cyan;
                list.Label("RFR.coastLevel".Translate() + "  " + "RFR.CoastRandom".Translate());
                GUI.contentColor = Color.white;
            }
            coastLevel = list.Slider(coastLevel, 0, 8);*/
            list.Gap();
            //list.CheckboxLabeled("RFR.DisallowIslands".Translate(), ref disallowIslands, "RFR.DisallowIslandsTip".Translate());
            //list.Gap();
            list.CheckboxLabeled("RFR.AllowFakeOres".Translate(), ref allowFakeOres, "RFR.AllowFakeOresTip".Translate());
            list.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            //Scribe_Values.Look(ref coastLevel, "coastLevel", 0.5f);
            Scribe_Values.Look(ref geysersLevel, "geysersLevel", 2.5f);
            Scribe_Values.Look(ref chunksLevel, "chunksLevel", 2.5f);
            Scribe_Values.Look(ref oreLevel, "oreLevel", 2.5f);
            Scribe_Values.Look(ref waterLevel, "waterLevel", 2.5f);
            Scribe_Values.Look(ref fertilityLevel, "fertilityLevel", 2.5f);
            Scribe_Values.Look(ref mountainLevel, "mountainLevel", 2.5f);
            Scribe_Values.Look(ref disallowIslands, "disallowIslands", false);
            Scribe_Values.Look(ref allowFakeOres, "allowFakeOres", true);
        }
    }
}