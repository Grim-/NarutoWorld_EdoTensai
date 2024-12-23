using RimWorld;
using System.Collections.Generic;
using TaranMagicFramework;
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
        public Pawn SourcePawn
        {
            get
            {
                if (sourcePawn == null)
                {
                    SetSourcePawn(GeneratePawn());
                }
                return sourcePawn;
            }
        }
        private Pawn GeneratePawn()
        {
            CompProperties_DNASample Props = (CompProperties_DNASample)props;
            string randomKind = Props.validPawnKinds.RandomElement();
            PawnKindDef kindDef = DefDatabase<PawnKindDef>.GetNamed(randomKind);
            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(kindDef));
            float level = Rand.Range(1000, 100000);
            pawn.GrantRandomXP(level);
            return pawn;
        }
        public void SetSourcePawn(Pawn pawn)
        {
            sourcePawn = pawn;
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
        }
        public override string CompInspectStringExtra()
        {
            if (sourcePawn != null)
                return "DNA Sample from: " + sourcePawn.LabelShortCap;
            return base.CompInspectStringExtra();
        }
    }

}
