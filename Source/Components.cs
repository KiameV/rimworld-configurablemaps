using RimWorld.Planet;
using Verse;

namespace ConfigurableMaps
{
    class WorldComp : WorldComponent
    {
        public static float AnimalMultiplier = -1, PlantMultiplier = -1;

        public WorldComp(World world) : base(world) { }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref AnimalMultiplier, "animalMultiplier", -1);
            Scribe_Values.Look(ref PlantMultiplier, "plantMultiplier", -1);
            Settings.Reload();
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();

            CurrentSettings.ApplySettings(AnimalMultiplier, PlantMultiplier);
        }
    }
}
