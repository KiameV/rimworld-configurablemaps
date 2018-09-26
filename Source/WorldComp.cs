using RimWorld.Planet;

namespace ConfigurableMaps
{
    class WorldComp : WorldComponent
    {
        public WorldComp(World world) : base(world) { }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            BiomeUtil.Init();
            BiomeUtil.UpdateBiomeStatsPerUserSettings();
        }
    }
}