using RimWorld;
using TaranMagicFramework;
using UnityEngine;
using Verse;

namespace EdoTensai
{
    public class HediffCompProperties_EdoTensaiPawn : HediffCompProperties
    {
        public int RegenerationTicks = 250;
        public int TicksBeforeBerserkWithoutMaster = 2000;
        public Color Color = new Color(0.2f, 0.7f, 0.2f, 1f);

        public HediffCompProperties_EdoTensaiPawn()
        {
            compClass = typeof(HediffComp_EdoTensaiPawn);
        }
    }

    public class HediffComp_EdoTensaiPawn : HediffComp
    {
        public HediffCompProperties_EdoTensaiPawn Props => (HediffCompProperties_EdoTensaiPawn)props;

        protected Faction OriginalFaction;

        protected Pawn Master;
        protected int CurrentTick = 0;
        protected int TicksWithoutMaster = 0;


        private CompAbilities AbilityComp;


        public void SetSlaveMaster(Pawn Master)
        {
            this.Master = Master;
                  CurrentTick = 0;
            TicksWithoutMaster = 0;
            AbilityComp = this.parent.pawn.GetComp<CompAbilities>();
            ApplyZombieColor(this.Pawn);
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            AbilityComp = this.parent.pawn.GetComp<CompAbilities>();
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);

            AbilityComp = this.parent.pawn.GetComp<CompAbilities>();
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            RestoreOriginalColor(Pawn);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            Pawn pawn = parent.pawn;
            if (pawn == null) return;

            CurrentTick++;
            if (CurrentTick >= Props.RegenerationTicks)
            {
                OnRegenTick(pawn);
                CurrentTick = 0;
            }

            if (Master != null && Master.Dead)
            {
                TicksWithoutMaster++;
                if (TicksWithoutMaster >= Props.TicksBeforeBerserkWithoutMaster)
                {
                    Log.Message($"Zombify: Master dead for {Props.TicksBeforeBerserkWithoutMaster}.");
                    TriggerBerserk(pawn);
                }
            }
            else
            {
                TicksWithoutMaster = 0;
            }


            HandleNeedsAndSupress(pawn);
        }

        private void OnRegenTick(Pawn pawn)
        {
            if (AbilityComp != null)
            {
                foreach (var item in AbilityComp.abilityResources)
                {
                    item.Value.energy = item.Value.MaxEnergy;
                }
            }


            if (pawn.health?.hediffSet?.hediffs != null)
            {
                Hediff hediffToHeal = pawn.health.hediffSet.hediffs
                    .FirstOrDefault(h => h.def.isBad && h.def != EdoDefOf.WN_EdoTensaiHediff);

                if (hediffToHeal != null)
                {
                    HealthUtility.Cure(hediffToHeal);
                }
            }
        }

        private void HandleNeedsAndSupress(Pawn pawn)
        {
            foreach (Need need in pawn.needs.AllNeeds)
            {
                need.CurLevel = need.MaxLevel;
            }

        }
        
        private void ApplyZombieColor(Pawn pawn)
        {
            pawn.story.skinColorOverride = Color.grey;
        }

        private void RestoreOriginalColor(Pawn pawn)
        {
            pawn.story.skinColorOverride = null;
        }


        private void TriggerBerserk(Pawn pawn)
        {
            pawn.guest.SetGuestStatus(OriginalFaction, GuestStatus.Guest);
            pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, null, true, true);
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_References.Look(ref Master, "pawnSlaveMaster");
            Scribe_Values.Look(ref CurrentTick, "slaveCurrentRegenTick");
            Scribe_Values.Look(ref TicksWithoutMaster, "slaveTickWithoutMaster");
            Scribe_References.Look(ref OriginalFaction, "slaveOriginalFaction");
        }
    }

}
