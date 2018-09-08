using UnityEngine;
using Verse;

namespace ConfigurableMaps.Settings
{
    public class WorldSettingsController : Mod
    {
        public WorldSettingsController(ModContentPack content) : base(content)
        {
            base.GetSettings<WorldSettings>();
        }

        public override string SettingsCategory()
        {
            return "RFR.FewerRuinsWorld".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Util.Init();
            WorldSettings.DoWindowContents(inRect);
        }
    }

    public class WorldSettings : ModSettings
    {
        public static float stoneMin = 2.5f;
        public static float stoneMax = 3.5f;
        public static float graniteCommonality = 2.5f;
        public static float limestoneCommonality = 2.5f;
        public static float marbleCommonality = 2.5f;
        public static float sandstoneCommonality = 2.5f;
        public static float slateCommonality = 2.5f;
        public static float extraStoneCommonality = 2.5f;

        public static void DoWindowContents(Rect canvas)
        {
            Listing_Standard list = new Listing_Standard();
            list.ColumnWidth = canvas.width;
            list.Begin(canvas);
            list.Gap();
            list.Label("RFR.stoneMin".Translate() + "  " + (int)stoneMin);
            stoneMin = list.Slider(stoneMin, 1, 8.99f);
            list.Label("RFR.stoneMax".Translate() + "  " + (int)stoneMax);
            stoneMax = list.Slider(stoneMax, 1, 8.99f);
            Text.Font = GameFont.Tiny;
            list.Label("RFR.stoneNotes".Translate());
            Text.Font = GameFont.Small;
            list.Gap(24);
            if (graniteCommonality < 1) { list.Label("RFR.graniteCommonality".Translate() + "  " + "RFR.Rare".Translate()); }
            else if (graniteCommonality < 2) { list.Label("RFR.graniteCommonality".Translate() + "  " + "RFR.Average".Translate()); }
            else { list.Label("RFR.graniteCommonality".Translate() + "  " + "RFR.Common".Translate()); }
            graniteCommonality = list.Slider(graniteCommonality, 0, 3);
            if (limestoneCommonality < 1) { list.Label("RFR.limestoneCommonality".Translate() + "  " + "RFR.Rare".Translate()); }
            else if (limestoneCommonality < 2) { list.Label("RFR.limestoneCommonality".Translate() + "  " + "RFR.Average".Translate()); }
            else { list.Label("RFR.limestoneCommonality".Translate() + "  " + "RFR.Common".Translate()); }
            limestoneCommonality = list.Slider(limestoneCommonality, 0, 3);
            if (marbleCommonality < 1) { list.Label("RFR.marbleCommonality".Translate() + "  " + "RFR.Rare".Translate()); }
            else if (marbleCommonality < 2) { list.Label("RFR.marbleCommonality".Translate() + "  " + "RFR.Average".Translate()); }
            else { list.Label("RFR.marbleCommonality".Translate() + "  " + "RFR.Common".Translate()); }
            marbleCommonality = list.Slider(marbleCommonality, 0, 3);
            if (sandstoneCommonality < 1) { list.Label("RFR.sandstoneCommonality".Translate() + "  " + "RFR.Rare".Translate()); }
            else if (sandstoneCommonality < 2) { list.Label("RFR.sandstoneCommonality".Translate() + "  " + "RFR.Average".Translate()); }
            else { list.Label("RFR.sandstoneCommonality".Translate() + "  " + "RFR.Common".Translate()); }
            sandstoneCommonality = list.Slider(sandstoneCommonality, 0, 3);
            if (slateCommonality < 1) { list.Label("RFR.slateCommonality".Translate() + "  " + "RFR.Rare".Translate()); }
            else if (slateCommonality < 2) { list.Label("RFR.slateCommonality".Translate() + "  " + "RFR.Average".Translate()); }
            else { list.Label("RFR.slateCommonality".Translate() + "  " + "RFR.Common".Translate()); }
            slateCommonality = list.Slider(slateCommonality, 0, 3);
            list.Gap();
            if (extraStoneCommonality < 1) { list.Label("RFR.extraStoneCommonality".Translate() + "  " + "RFR.Rare".Translate()); }
            else if (extraStoneCommonality < 2) { list.Label("RFR.extraStoneCommonality".Translate() + "  " + "RFR.Average".Translate()); }
            else { list.Label("RFR.extraStoneCommonality".Translate() + "  " + "RFR.Common".Translate()); }
            extraStoneCommonality = list.Slider(extraStoneCommonality, 0, 3);
            list.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref stoneMin, "stoneMin", 2.5f);
            if (stoneMax < stoneMin) { stoneMax = stoneMin; }
            Scribe_Values.Look(ref stoneMax, "stoneMax", 3.5f);
            Scribe_Values.Look(ref graniteCommonality, "graniteCommonality", 2.5f);
            Scribe_Values.Look(ref limestoneCommonality, "limestoneCommonality", 2.5f);
            Scribe_Values.Look(ref marbleCommonality, "marbleCommonality", 2.5f);
            Scribe_Values.Look(ref sandstoneCommonality, "sandstoneCommonality", 2.5f);
            Scribe_Values.Look(ref slateCommonality, "slateCommonality", 2.5f);
            Scribe_Values.Look(ref extraStoneCommonality, "extraStoneCommonality", 2.5f);
        }
    }
}