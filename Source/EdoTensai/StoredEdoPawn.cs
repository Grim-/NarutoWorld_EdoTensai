using Verse;

namespace EdoTensai
{
    public class StoredEdoPawn : IExposable
    {
        public Pawn pawn;
        public PawnEquipmentSnapshot equipment;
        public float reanimationQuality = 1f;

        public void ExposeData()
        {
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Deep.Look(ref equipment, "equipment");
            Scribe_Values.Look(ref reanimationQuality, "reanimationQuality", 1f);
        }
    }
}
