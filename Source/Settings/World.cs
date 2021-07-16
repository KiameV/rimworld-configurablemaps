using System.Text;
using UnityEngine;
using Verse;

namespace ConfigurableMaps.Settings
{/*
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
            BiomeUtil.Init();
            WorldSettings.DoWindowContents(inRect);
        }
    }

    public class WorldSettings : ModSettings
    {
        public static float stoneMin = 2.5f;
        public static float stoneMax = 3.5f;
        public static float graniteCommonality = 50;
        public static float limestoneCommonality = 50;
        public static float marbleCommonality = 50;
        public static float sandstoneCommonality = 50;
        public static float slateCommonality = 50;
        public static float extraStoneCommonality = 50;

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
            if (stoneMin > stoneMax)
                stoneMax = stoneMin;
            else if (stoneMax < stoneMin)
                stoneMin = stoneMax;

            Text.Font = GameFont.Tiny;
            list.Label("RFR.stoneNotes".Translate());
            Text.Font = GameFont.Small;
            list.Gap(24);
            graniteCommonality = DrawSlider(list, graniteCommonality, "RFR.graniteCommonality");
            limestoneCommonality = DrawSlider(list, limestoneCommonality, "RFR.limestoneCommonality");
            marbleCommonality = DrawSlider(list, marbleCommonality, "RFR.marbleCommonality");
            sandstoneCommonality = DrawSlider(list, sandstoneCommonality, "RFR.sandstoneCommonality");
            slateCommonality = DrawSlider(list, slateCommonality, "RFR.slateCommonality");
            extraStoneCommonality = DrawSlider(list, extraStoneCommonality, "RFR.extraStoneCommonality");
            list.End();

            if (Widgets.ButtonText(new Rect(canvas.x + 10, canvas.yMax - 100, 100, 32), "Reset"))
            {
                stoneMin = 2.5f;
                stoneMax = 3.5f;
                graniteCommonality = limestoneCommonality = marbleCommonality = sandstoneCommonality = slateCommonality = extraStoneCommonality = 50;
            }
        }

        private static float DrawSlider(Listing_Standard l, float commonality, string label)
        {
            StringBuilder sb = new StringBuilder(label.Translate());
            sb.Append(" ");
            if (commonality < 25)
            {
                sb.Append("RFR.Rare".Translate());
            }
            else if (commonality > 75)
            {
                sb.Append("RFR.Common".Translate());
            }
            else
            {
                sb.Append("RFR.Average".Translate());
            }
            l.Label(sb.ToString());
            return l.Slider(commonality, 0, 100);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref stoneMin, "stoneMin", 2.5f);
            if (stoneMax < stoneMin) { stoneMax = stoneMin; }
            Scribe_Values.Look(ref stoneMax, "stoneMax", 3.5f);
            Scribe_Values.Look(ref graniteCommonality, "graniteCommonality", 50f);
            Scribe_Values.Look(ref limestoneCommonality, "limestoneCommonality", 50f);
            Scribe_Values.Look(ref marbleCommonality, "marbleCommonality", 50f);
            Scribe_Values.Look(ref sandstoneCommonality, "sandstoneCommonality", 50f);
            Scribe_Values.Look(ref slateCommonality, "slateCommonality", 50f);
            Scribe_Values.Look(ref extraStoneCommonality, "extraStoneCommonality", 50f);
        }
    }*/
}