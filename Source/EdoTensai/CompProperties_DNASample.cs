using RimWorld;
using System.Collections.Generic;
using TaranMagicFramework;
using UnityEngine;
using Verse;

namespace EdoTensai
{
    public class CompProperties_DNASample : CompProperties
    {
        public List<string> validPawnKinds = new List<string>
        {
            "WN_Uchiha_Player",
            "WN_Hyuuga_Player",
            "WN_Sabaku_Player",
            "WN_Sarutobi_Player",
            "WN_Yuki_Player",
            "WN_Aburame_Player"
        };

        public CompProperties_DNASample()
        {
            compClass = typeof(CompDNASample);
        }
    }


    public class CompDNASample : ThingComp
    {
        private Pawn sourcePawn;
        private PawnEquipmentSnapshot pawnData;
        private float reanimationQuality = 1f;

        public Pawn SourcePawn
        {
            get
            {
                if (sourcePawn == null)
                {
                    Log.Message($"Generating new Pawn for DNA Sample");

                    float Quality = Rand.Range(0.2f, 0.7f);
                    SetSourcePawn(GeneratePawn(Quality), Quality);
                }
                return sourcePawn;
            }
        }

        public float ReanimationQuality => reanimationQuality;

        private Pawn GeneratePawn(float quality = 1f)
        {
            CompProperties_DNASample Props = (CompProperties_DNASample)props;
            string randomKind = Props.validPawnKinds.RandomElement();
            PawnKindDef kindDef = DefDatabase<PawnKindDef>.GetNamed(randomKind);
            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(kindDef));
            float level = Mathf.Lerp(100, 5000, quality);
            pawn.GrantRandomXP(level);
            return pawn;
        }

        public void SetSourcePawn(Pawn pawn, float quality = 1f)
        {
            sourcePawn = pawn;
            reanimationQuality = quality;
            if (pawn.apparel != null)
            {
                pawnData = new PawnEquipmentSnapshot();
                foreach (Apparel apparel in pawn.apparel.WornApparel)
                {
                    pawnData.apparelDefs.Add(apparel.def);
                    pawnData.apparelStuff.Add(apparel.Stuff);
                }
            }
        }

        public PawnEquipmentSnapshot GetPawnData() => pawnData;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref sourcePawn, "sourcePawn");
            Scribe_Deep.Look(ref pawnData, "pawnData");
            Scribe_Values.Look(ref reanimationQuality, "reanimationQuality", 1f);
        }
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selectionPawn)
        {
            foreach (FloatMenuOption option in base.CompFloatMenuOptions(selectionPawn))
            {
                yield return option;
            }

            Pawn storedPawn = SourcePawn;
            yield return new FloatMenuOption($"View stored pawn info", () =>
            {
                Find.WindowStack.Add(new Dialog_InfoCard(storedPawn));
            });
        }

        public override string CompInspectStringExtra()
        {
            if (SourcePawn != null)
                return "DNA Sample from: " + SourcePawn.GetInspectString() + $" (Quality: {reanimationQuality:P0})";
            return base.CompInspectStringExtra();
        }
    }

}
