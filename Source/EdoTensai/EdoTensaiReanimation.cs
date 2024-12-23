using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Ability = TaranMagicFramework.Ability;

namespace EdoTensai
{
    public class EdoTensaiReanimation : Ability
    {
        public override void Start(bool consumeEnergy = true)
        {
            base.Start(consumeEnergy);

            CompDNASample dnaSample = this.pawn.inventory.innerContainer
                .FirstOrDefault(t => t.TryGetComp<CompDNASample>() != null)?.TryGetComp<CompDNASample>();
            Pawn sacrifice = this.curTarget.Pawn;

            if (dnaSample == null)
            {
                Log.Error($"No DNA Samples found to reanimate");
                return;
            }
            if (sacrifice == null)
            {
                Log.Error($"No sacrifice found to reanimate");
                return;
            }

            Pawn targetPawn = dnaSample.SourcePawn;
            if (targetPawn == null)
            {
                Log.Error($"No SourcePawn DNA found to reanimate");
                return;
            }


            PawnEquipmentSnapshot storedPawnData = dnaSample.GetPawnData();

            HediffComp_EdoTensaiController controllerHediff = this.pawn.GetEdoTensaiController();
            if (controllerHediff == null)
            {
                Log.Error($"No EdoTensai controller hediff found on caster");
                return;
            }

            EdoDefOf.EdoTensaiRitualEffect.Spawn(sacrifice.Position, this.pawn.Map);

            sacrifice.Kill(null, controllerHediff.parent);

            sacrifice.DestroyPawnAndCorpse();

            if (controllerHediff.AddStoredPawn(targetPawn, storedPawnData))
            {
                Messages.Message($"{targetPawn.Label} has been reanimated through Edo Tensei!", MessageTypeDefOf.NeutralEvent);
                dnaSample.parent.Destroy();

                if (controllerHediff.SummonPawn(targetPawn))
                {

                }
            }
        }
    }
}
