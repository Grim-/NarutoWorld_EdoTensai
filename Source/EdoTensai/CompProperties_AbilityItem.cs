using TaranMagicFramework;
using Verse;

namespace EdoTensai
{
    public class CompProperties_AbilityItem : CompProperties
    {
        public TaranMagicFramework.AbilityDef abilityDef;

        public CompProperties_AbilityItem()
        {
            this.compClass = typeof(Comp_AbilityItem);
        }
    }


    public class Comp_AbilityItem : ThingComp
    {
        public CompProperties_AbilityItem Props => (CompProperties_AbilityItem)props;
        private bool grantedAbility = false;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref grantedAbility, "grantedAbility", false);
        }

        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);

            var compAbilities = pawn.GetComp<CompAbilities>();
            if (compAbilities?.GetAbilityClass(NarutoMod.WN_DefOf.WN_Ninjutsu) is var abilityClass && abilityClass != null)
            {
                if (!abilityClass.Learned(Props.abilityDef))
                {
                    abilityClass.LearnAbility(Props.abilityDef, false, 0);
                    grantedAbility = true;
                }
            }
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            base.Notify_Unequipped(pawn);

            if (grantedAbility)
            {
                var compAbilities = pawn.GetComp<CompAbilities>();
                if (compAbilities?.GetAbilityClass(NarutoMod.WN_DefOf.WN_Ninjutsu) is var abilityClass && abilityClass != null)
                {
                    if (abilityClass.Learned(Props.abilityDef))
                    {
                        abilityClass.RemoveAbility(abilityClass.GetLearnedAbility(Props.abilityDef));
                        grantedAbility = false;
                    }

                }
            }
        }
    }
}
