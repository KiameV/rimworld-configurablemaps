using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ConfigurableMaps
{
    public class WSFieldValues
    {
        public FieldValue<int>[] StoneMinMaxBuffers;
        public FieldValue<float>[] CommonalityBuffers;
    }

    public class StoneCommonality : IExposable
    {
        public const float DEFAULT_COMMONALITY = 50f;

        public string StoneDefName;
        public float Commonality;

        public ThingDef StoneDef;

        public void ExposeData()
        {
            Scribe_Values.Look(ref Commonality, "commonality", DEFAULT_COMMONALITY);
            Scribe_Values.Look(ref StoneDefName, "defName");
        }
    }

    public class WorldSettings : IExposable, IWindow<WSFieldValues>
    {
        const int DEFAULT_MIN_STONE = 2;
        const int DEFAULT_MAX_STONE = 3;

        public static int StoneMin = DEFAULT_MIN_STONE;
        public static int StoneMax = DEFAULT_MAX_STONE;
        public static bool CommonalityRandom = false;

        private static bool initCommonalities = false;
        public static List<StoneCommonality> Commonalities = new List<StoneCommonality>();

        private Vector2 scroll = Vector2.zero;
        private float lastY = 0;

        public string Name => "CM.WorldSettings".Translate();

        public void DoWindowContents(Rect rect, WSFieldValues fv)
        {
            Init();

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
                    WindowUtil.DrawInputWithSlider(rect.x, ref lastY, v);
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
            Scribe_Collections.Look(ref Commonalities, "Commonalities");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                Init();
        }

        public static void Init()
        {
            if (Commonalities == null)
                Commonalities = new List<StoneCommonality>();

            if (Commonalities.Count == 0 || !initCommonalities)
            {
                HashSet<string> existingStones = new HashSet<string>();
                SortedDictionary<string, StoneCommonality> loadedStones = new SortedDictionary<string, StoneCommonality>();
                foreach (var d in DefDatabase<ThingDef>.AllDefs)
                {
                    if (d.IsNonResourceNaturalRock)
                    {
                        initCommonalities = true;
                        existingStones.Add(d.defName);
                    }
                }
                if (initCommonalities)
                {
                    foreach (var c in Commonalities)
                    {
                        if (existingStones.Contains(c.StoneDefName))
                            loadedStones[c.StoneDefName] = c;
                    }
                    foreach (var d in existingStones)
                    {
                        if (!loadedStones.ContainsKey(d))
                        {
                            var def = DefDatabase<ThingDef>.GetNamed(d, false);
                            if (def != null)
                            {
                                loadedStones[d] = new StoneCommonality()
                                {
                                    StoneDef = def,
                                    StoneDefName = def.defName,
                                    Commonality = StoneCommonality.DEFAULT_COMMONALITY
                                };
                            }
                        }
                    }
                    Commonalities.Clear();
                    foreach (var c in loadedStones.Values)
                        Commonalities.Add(c);
                }
            }

            if (initCommonalities && Commonalities?.Count > 0 && Commonalities[0].StoneDef == null)
            {
                foreach (var c in Commonalities)
                    c.StoneDef = DefDatabase<ThingDef>.GetNamed(c.StoneDefName, false);
                Commonalities.RemoveAll(c => c.StoneDef == null);
            }
        }

        public WSFieldValues GetFieldValues()
        {
            Init();
            var commonalityBuffers = new FieldValue<float>[Commonalities.Count];
            for (int i = 0; i < Commonalities.Count; ++i)
            {
                var c = Commonalities[i];
                commonalityBuffers[i] = new FieldValue<float>(GetTranslation(c.StoneDefName, c.StoneDefName + ".label"), v => c.Commonality = v, () => c.Commonality, 0, 100, StoneCommonality.DEFAULT_COMMONALITY);
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
