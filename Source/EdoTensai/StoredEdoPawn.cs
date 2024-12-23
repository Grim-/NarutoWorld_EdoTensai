using Verse;

namespace EdoTensai
{
    public class StoredEdoPawn : IExposable
    {
        public Pawn pawn;
        public PawnEquipmentSnapshot equipment;

        public void ExposeData()
        {
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Deep.Look(ref equipment, "equipment");
        }
    }
}
