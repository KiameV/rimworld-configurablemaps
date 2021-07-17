using UnityEngine;
using Verse;

namespace ConfigurableMaps
{
    public class WSFieldValues
    {
        public FieldValue<int>[] StoneMinMaxBuffers;
        public FieldValue<float>[] CommonalityBuffers;
    }

    public class WorldSettings : IExposable, IWindow<WSFieldValues>
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

        public float CommonalityClaystone = DEFAULT_COMMONALITY;
        public float CommonalityAndesite = DEFAULT_COMMONALITY;
        public float CommonalitySyenite = DEFAULT_COMMONALITY;
        public float CommonalityGneiss = DEFAULT_COMMONALITY;
        public float CommonalityQuartzite = DEFAULT_COMMONALITY;
        public float CommonalitySchist = DEFAULT_COMMONALITY;
        public float CommonalityGabbro = DEFAULT_COMMONALITY;
        public float CommonalityDiorite = DEFAULT_COMMONALITY;
        public float CommonalityDunite = DEFAULT_COMMONALITY;
        public float CommonalityPegmatite = DEFAULT_COMMONALITY;

        private Vector2 scroll = Vector2.zero;
        private float lastY = 0;

        public string Name => "CM.WorldSettings".Translate();

        public void DoWindowContents(Rect rect, WSFieldValues fv)
        {
            float y = rect.y;
            Widgets.Label(new Rect(rect.x, y, rect.width, 28), "CM.StoneTypes".Translate());
            y += 30;
            foreach (var v in fv.StoneMinMaxBuffers)
                WindowUtil.DrawIntInput(rect.x, ref y, v);

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
                foreach (var v in fv.CommonalityBuffers)
                    WindowUtil.DrawFloatInput(rect.x, ref lastY, v);
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
            Scribe_Values.Look(ref CommonalityClaystone, "CommonalityClaystone", DEFAULT_COMMONALITY);
            Scribe_Values.Look(ref CommonalityAndesite, "CommonalityAndesite", DEFAULT_COMMONALITY);
            Scribe_Values.Look(ref CommonalitySyenite, "CommonalitySyenite", DEFAULT_COMMONALITY);
            Scribe_Values.Look(ref CommonalityGneiss, "CommonalityGneiss", DEFAULT_COMMONALITY);
            Scribe_Values.Look(ref CommonalityQuartzite, "CommonalityQuartzite", DEFAULT_COMMONALITY);
            Scribe_Values.Look(ref CommonalitySchist, "CommonalitySchist", DEFAULT_COMMONALITY);
            Scribe_Values.Look(ref CommonalityGabbro, "CommonalityGabbro", DEFAULT_COMMONALITY);
            Scribe_Values.Look(ref CommonalityDiorite, "CommonalityDiorite", DEFAULT_COMMONALITY);
            Scribe_Values.Look(ref CommonalityDunite, "CommonalityDunite", DEFAULT_COMMONALITY);
            Scribe_Values.Look(ref CommonalityPegmatite, "CommonalityPegmatite", DEFAULT_COMMONALITY);
        }

        public WSFieldValues GetFieldValues()
        {
            var commonalityBuffers = new FieldValue<float>[5]
            {
                new FieldValue<float>(GetTranslation("Granite", "Granite.label"), v => CommonalityGranite = v, () => CommonalityGranite, 0, 100, DEFAULT_COMMONALITY),
                new FieldValue<float>(GetTranslation("Limestone", "Limestone.label"), v => CommonalityLimestone = v, () => CommonalityLimestone, 0, 100, DEFAULT_COMMONALITY),
                new FieldValue<float>(GetTranslation("Marble", "Marble.label"), v => CommonalityMarble = v, () => CommonalityMarble, 0, 100, DEFAULT_COMMONALITY),
                new FieldValue<float>(GetTranslation("Sandstone", "Sandstone.label"), v => CommonalitySandstone = v, () => CommonalitySandstone, 0, 100, DEFAULT_COMMONALITY),
                new FieldValue<float>(GetTranslation("Slate", "Slate.label").Translate(), v => CommonalitySlate = v, () => CommonalitySlate, 0, 100, DEFAULT_COMMONALITY),
            };
            if (Settings.detectedCuprosStones)
            {
                commonalityBuffers = new FieldValue<float>[15]
                {
                    commonalityBuffers[0],
                    commonalityBuffers[1],
                    commonalityBuffers[2],
                    commonalityBuffers[3],
                    commonalityBuffers[4],
                    new FieldValue<float>(GetTranslation("Claystone", "Claystone.label").Translate(), v => CommonalityClaystone = v, () => CommonalityClaystone, 0, 100, DEFAULT_COMMONALITY),
                    new FieldValue<float>(GetTranslation("Andesite", "Andesite.label").Translate(), v => CommonalityAndesite = v, () => CommonalityAndesite, 0, 100, DEFAULT_COMMONALITY),
                    new FieldValue<float>(GetTranslation("Syenite", "Syenite.label").Translate(), v => CommonalitySyenite = v, () => CommonalitySyenite, 0, 100, DEFAULT_COMMONALITY),
                    new FieldValue<float>(GetTranslation("Gneiss", "Gneiss.label").Translate(), v => CommonalityGneiss = v, () => CommonalityGneiss, 0, 100, DEFAULT_COMMONALITY),
                    new FieldValue<float>(GetTranslation("Quartzite", "Quartzite.label").Translate(), v => CommonalityQuartzite = v, () => CommonalityQuartzite, 0, 100, DEFAULT_COMMONALITY),
                    new FieldValue<float>(GetTranslation("Schist", "Schist.label").Translate(), v => CommonalitySchist = v, () => CommonalitySchist, 0, 100, DEFAULT_COMMONALITY),
                    new FieldValue<float>(GetTranslation("Gabbro", "Gabbro.label").Translate(), v => CommonalityGabbro = v, () => CommonalityGabbro, 0, 100, DEFAULT_COMMONALITY),
                    new FieldValue<float>(GetTranslation("Diorite", "Diorite.label").Translate(), v => CommonalityDiorite = v, () => CommonalityDiorite, 0, 100, DEFAULT_COMMONALITY),
                    new FieldValue<float>(GetTranslation("Dunite", "Dunite.label").Translate(), v => CommonalityDunite = v, () => CommonalityDunite, 0, 100, DEFAULT_COMMONALITY),
                    new FieldValue<float>(GetTranslation("Pegmatite", "Pegmatite.label").Translate(), v => CommonalityPegmatite = v, () => CommonalityPegmatite, 0, 100, DEFAULT_COMMONALITY)
                };
            }
            return new WSFieldValues()
            {
                StoneMinMaxBuffers = new FieldValue<int>[2]
                {
                    new FieldValue<int>("min".Translate(), v => StoneMin = v, () => StoneMin, 0, 10, DEFAULT_MIN_STONE),
                    new FieldValue<int>("max".Translate(), v => StoneMax = v, () => StoneMax, 0, 10, DEFAULT_MAX_STONE)
                },
                CommonalityBuffers = commonalityBuffers
            };
        }

        private string GetTranslation(string eng, string toTranslate)
        {
            if (toTranslate.TryTranslate(out TaggedString t))
                return t;
            return eng;
        }
    }
}
