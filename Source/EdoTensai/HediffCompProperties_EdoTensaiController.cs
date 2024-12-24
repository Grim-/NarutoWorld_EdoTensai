using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace EdoTensai
{
    public class HediffCompProperties_EdoTensaiController : HediffCompProperties
    {
        public HediffCompProperties_EdoTensaiController()
        {
            compClass = typeof(HediffComp_EdoTensaiController);
        }
    }



    public class HediffComp_EdoTensaiController : HediffComp
    {
        private List<StoredEdoPawn> storedPawns = new List<StoredEdoPawn>();
        private List<Pawn> activePawns = new List<Pawn>();

        public List<Pawn> GetStoredPawns()
        {
            return storedPawns.Select(x => x.pawn).ToList();
        }

        public List<Pawn> GetActivePawns()
        {
            return new List<Pawn>(activePawns);
        }

        public bool IsPawnActive(Pawn storedPawn)
        {
            return activePawns.Contains(storedPawn);
        }

        public bool AddStoredPawn(Pawn pawn, PawnEquipmentSnapshot equipmentSnapshot, float quality = 1f)
        {
            if (!storedPawns.Any(x => x.pawn == pawn))
            {
                if (pawn.Spawned)
                    pawn.DeSpawn();

                if (!Find.WorldPawns.Contains(pawn))
                    Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.KeepForever);

                Log.Message($"Storing Pawn {pawn.Label} to {this.parent.pawn.Label} with quality {quality:P0}");
                storedPawns.Add(new StoredEdoPawn
                {
                    pawn = pawn,
                    equipment = equipmentSnapshot,
                    reanimationQuality = quality
                });
                SetupSummon(pawn, quality);
                return true;
            }
            return false;
        }
        public void SetupSummon(Pawn pawn, float quality)
        {
            Hediff edoHediff = pawn.health.GetOrAddHediff(EdoDefOf.WN_EdoTensaiHediff);
            HediffComp_EdoTensaiPawn edoTensaiPawn = edoHediff.TryGetComp<HediffComp_EdoTensaiPawn>();

            if (edoTensaiPawn != null)
            {
                edoTensaiPawn.parent.Severity = Mathf.Clamp(quality, 0.1f, 1.2f);
                edoTensaiPawn.SetSlaveMaster(this.parent.pawn);
            }
        }



        public void RemoveSummon(Pawn pawn, bool removeStoredPawn = true)
        {
            if (IsPawnActive(pawn))
            {
                UnsummonPawn(pawn);
            }

            if (removeStoredPawn)
            {
                storedPawns.RemoveAll(x => x.pawn == pawn);
            }
        }

        public bool SummonPawn(Pawn storedPawn, IntVec3 position = default(IntVec3), bool ShowCoffin = true)
        {
            var stored = storedPawns.FirstOrDefault(x => x.pawn == storedPawn);
            if (stored != null)
            {
                if (Find.WorldPawns.Contains(stored.pawn))
                    Find.WorldPawns.RemovePawn(stored.pawn);

                RestorePawn(stored);


                IntVec3 usePosition = position != default(IntVec3) ? position : CellFinder.RandomClosewalkCellNear(Pawn.Position, Pawn.Map, 3);

                if (EdoDefOf.EdoTensaiCoffinEffect != null && ShowCoffin)
                {
                    EdoDefOf.EdoTensaiCoffinEffect.SpawnMaintained(usePosition, Pawn.Map);
                }


                GenSpawn.Spawn(stored.pawn, usePosition, Pawn.Map);

                if (stored.pawn.Faction != Faction.OfPlayer)
                {
                    stored.pawn.SetFaction(Faction.OfPlayer);
                }

                HediffComp_EdoTensaiPawn edoComp = stored.pawn.GetEdoTensaiPawn();
                if (edoComp != null)
                {
                    edoComp.SetSlaveMaster(Pawn);
                }

                activePawns.Add(stored.pawn);
                return true;
            }
            return false;
        }
        public bool UnsummonPawn(Pawn pawn)
        {
            if (activePawns.Remove(pawn))
            {
                if (EdoDefOf.EdoTensaiCoffinEffect != null)
                {
                    EdoDefOf.EdoTensaiCoffinEffect.SpawnMaintained(pawn.Position, Pawn.Map);
                }

                if (pawn.Spawned)
                {
                    pawn.DeSpawn();
                }
                if (!Find.WorldPawns.Contains(pawn))
                    Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.KeepForever);
                return true;
            }
            return false;
        }

        public void RestorePawn(StoredEdoPawn stored)
        {
            Log.Message($"Restoring {stored.pawn.Label} health...");

            if (stored.pawn.Dead)
            {
                ResurrectionUtility.TryResurrect(stored.pawn);
                stored.pawn.ClearAllReservations();
            }

            List<Hediff> hediffsToRemove = stored.pawn.health.hediffSet.hediffs
                .Where(h => h.def.isBad)
                .ToList();

            foreach (Hediff hediff in hediffsToRemove)
            {
                stored.pawn.health.RemoveHediff(hediff);
            }

            if (stored.pawn.apparel != null)
            {
                stored.pawn.RestoreEquipmentFromSnapshot(stored.equipment);
            }

            if (stored.pawn.needs != null)
            {
                foreach (Need need in stored.pawn.needs.AllNeeds)
                {
                    need.CurLevel = need.MaxLevel;
                }
            }

            stored.pawn.mindState?.Reset();
        }
        private void ReleasePawn(Pawn summonedPawn)
        {
            if (!summonedPawn.Spawned)
            {
                GenSpawn.Spawn(summonedPawn, Pawn.Position, Pawn.Map);
            }

            summonedPawn.SetFaction(null);
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Collections.Look(ref storedPawns, "storedPawns", LookMode.Deep);
            Scribe_Collections.Look(ref activePawns, "activePawns", LookMode.Reference);
        }
    }
}
