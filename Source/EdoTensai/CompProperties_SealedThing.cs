using System.Collections.Generic;
using Verse;

namespace EdoTensai
{
    public class CompProperties_SealedThing : CompProperties
    {
        public CompProperties_SealedThing()
        {
            compClass = typeof(CompSealedThing);
        }
    }

    public class CompSealedThing : ThingComp
    {
        private Thing StoredThing;
        public Thing Thing => StoredThing;

        public void StorePawn(Thing pawn)
        {
            StoredThing = pawn;
        }

        public bool HasStored()
        {
            return StoredThing != null;
        }

        public Thing ReleasePawn()
        {
            var pawn = StoredThing;
            StoredThing = null;
            return pawn;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (respawningAfterLoad)
            {
                StorePawn(StoredThing);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Deep.Look(ref StoredThing, "storedThing");
        }


        public override string CompInspectStringExtra()
        {
            if (StoredThing == null)
            {
                return "No thing stored";
            }

            TaggedString pawnInfo = "Stored thing: " + StoredThing.Label;
            return pawnInfo;
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selectionPawn)
        {
            foreach (FloatMenuOption option in base.CompFloatMenuOptions(selectionPawn))
            {
                yield return option;
            }

            Thing storedPawn = StoredThing;
            yield return new FloatMenuOption($"View stored thing info", () =>
            {
                Find.WindowStack.Add(new Dialog_InfoCard(storedPawn));
            });
        }
    }
}
