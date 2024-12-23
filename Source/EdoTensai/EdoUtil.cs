using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaranMagicFramework;
using Verse;

namespace EdoTensai
{
    public static class EdoUtil
    {
        public static HediffComp_EdoTensaiPawn GetEdoTensaiPawn(this Pawn pawn)
        {
            if (pawn?.health?.hediffSet == null) 
                return null;

            return pawn.health.GetOrAddHediff(EdoDefOf.WN_EdoTensaiHediff)?.TryGetComp<HediffComp_EdoTensaiPawn>();
        }

        public static HediffComp_EdoTensaiController GetEdoTensaiController(this Pawn pawn)
        {
            if (pawn?.health?.hediffSet == null) 
                return null;

            return pawn.health.GetOrAddHediff(EdoDefOf.WN_EdoTensaiController)?.TryGetComp<HediffComp_EdoTensaiController>();
        }

        public static PawnEquipmentSnapshot CreateEquipmentSnapshot(this Pawn pawn)
        {
            PawnEquipmentSnapshot snapshot = new PawnEquipmentSnapshot();

            if (pawn.apparel != null)
            {
                foreach (Apparel apparel in pawn.apparel.WornApparel)
                {
                    snapshot.apparelDefs.Add(apparel.def);
                    snapshot.apparelStuff.Add(apparel.Stuff);
                }
            }

            return snapshot;
        }


        public static void GrantRandomXP(this Pawn pawn, float experience)
        {
            var unlockedClasses = pawn.GetUnlockedAbilityClasses().ToList();
            if (!unlockedClasses.Any()) return;

            float xpPerClass = experience / unlockedClasses.Count;
            foreach (var item in unlockedClasses)
            {
                item.GainXP(xpPerClass);
            }
        }


        public static void DestroyPawnAndCorpse(this Pawn pawn)
        {
            if (!pawn.Destroyed)
                pawn.Destroy();

            Corpse corpse = pawn.Corpse;
            if (corpse != null && !corpse.Destroyed)
                corpse.Destroy();
        }

        public static void RestoreEquipmentFromSnapshot(this Pawn pawn, PawnEquipmentSnapshot snapshot)
        {
            if (pawn.apparel != null)
            {
                pawn.apparel.DestroyAll();

                for (int i = 0; i < snapshot.apparelDefs.Count; i++)
                {
                    Apparel apparel = (Apparel)ThingMaker.MakeThing(snapshot.apparelDefs[i], snapshot.apparelStuff[i]);
                    pawn.apparel.Wear(apparel, false, true);
                }
            }
        }
    }
}
