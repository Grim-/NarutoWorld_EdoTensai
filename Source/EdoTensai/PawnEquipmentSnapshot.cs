using System.Collections.Generic;
using Verse;

namespace EdoTensai
{
    public class PawnEquipmentSnapshot : IExposable
    {
        public List<ThingDef> apparelDefs = new List<ThingDef>();
        public List<ThingDef> apparelStuff = new List<ThingDef>();

        public void ExposeData()
        {
            Scribe_Collections.Look(ref apparelDefs, "apparelDefs", LookMode.Def);
            Scribe_Collections.Look(ref apparelStuff, "apparelStuff", LookMode.Def);
        }
    }
}
