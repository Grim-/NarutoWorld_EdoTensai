using RimWorld;
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
                    if (targetPawn.health.hediffSet.HasHediff(EdoDefOf.WN_EdoTensaiHediff))
                    {

                        Messages.Message($"cant target edo tensai pawns", MessageTypeDefOf.NeutralEvent);                    
                        return;
                    }


                    if (!Find.WorldPawns.Contains(targetPawn))
                    {
                        Find.WorldPawns.PassToWorld(targetPawn, PawnDiscardDecideMode.KeepForever);
                    }

                    Thing dnaSample = ThingMaker.MakeThing(EdoDefOf.EdoTensai_DNASample);
                    CompDNASample comp = dnaSample.TryGetComp<CompDNASample>();
                    if (comp != null)
                    {
                        float medicalSkill = this.pawn.skills.GetSkill(SkillDefOf.Medicine).Level;
                        float quality = (medicalSkill / 20f) * 1.2f;

                        comp.SetSourcePawn(targetPawn, quality);
                        GenSpawn.Spawn(dnaSample, corpse.Position, this.pawn.Map);

                        Messages.Message($"DNA extracted with {quality:P0} reanimation potential.", MessageTypeDefOf.NeutralEvent);
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
