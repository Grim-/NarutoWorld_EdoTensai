using RimWorld.Planet;
using Verse;
using Ability = TaranMagicFramework.Ability;

namespace EdoTensai
{
    public class ExtractDNA : Ability
    {
        public override void Start(bool consumeEnergy = true)
        {
            base.Start(consumeEnergy);
            if (this.curTarget.HasThing && this.curTarget.Thing is Corpse corpse)
            {
                Pawn targetPawn = corpse.InnerPawn;
                if (targetPawn != null)
                {
                    if (!Find.WorldPawns.Contains(targetPawn))
                    {
                        Find.WorldPawns.PassToWorld(targetPawn, PawnDiscardDecideMode.KeepForever);
                    }

                    Thing dnaSample = ThingMaker.MakeThing(EdoDefOf.EdoTensai_DNASample);
                    CompDNASample comp = dnaSample.TryGetComp<CompDNASample>();
                    if (comp != null)
                    {
                        comp.SetSourcePawn(targetPawn);
                        GenSpawn.Spawn(dnaSample, corpse.Position, this.pawn.Map);
                    }

                    if (!corpse.Destroyed)
                    {
                        corpse.Destroy(DestroyMode.Vanish);
                    }
                }
            }
        }
    }
}
