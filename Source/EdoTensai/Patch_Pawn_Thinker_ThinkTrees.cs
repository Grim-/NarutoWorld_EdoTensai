using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace EdoTensai
{
    [StaticConstructorOnStartup]
    public static class EdoTensaiPatchClass
    {
        static EdoTensaiPatchClass()
        {
            var harmony = new Harmony("com.emo.narutomodpatches");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(SkillRecord))]
    [HarmonyPatch("Learn")]
    [HarmonyPatch(new[] { typeof(float), typeof(bool), typeof(bool) })]
    public static class SkillRecord_Learn_Patch
    {
        public static bool Prefix(SkillRecord __instance, ref float xp, bool direct = false, bool ignoreLearnRate = false)
        {
            try
            {
                Pawn pawn = __instance.Pawn;
                if (pawn != null)
                {
                    if (pawn.health.hediffSet.HasHediff(EdoDefOf.WN_EdoTensaiHediff))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error($"[Learn Prefix] Error in patch: {ex}");
                return true;
            }
        }
    }

    public static class Patch_Pawn_Thinker_ThinkTrees
    {
        [HarmonyPatch(typeof(Pawn_Thinker), "MainThinkTree", MethodType.Getter)]
        public static class Patch_MainThinkTree
        {
            public static bool Prefix(Pawn_Thinker __instance, ref ThinkTreeDef __result)
            {
                return HandleThinkTreePatch(__instance, ref __result, isConstant: false);
            }
        }

        [HarmonyPatch(typeof(Pawn_Thinker), "ConstantThinkTree", MethodType.Getter)]
        public static class Patch_ConstantThinkTree
        {
            public static bool Prefix(Pawn_Thinker __instance, ref ThinkTreeDef __result)
            {
                return HandleThinkTreePatch(__instance, ref __result, isConstant: true);
            }
        }

        public static Dictionary<HediffDef, ThinkTreeDef> ThinkTreeMap = new Dictionary<HediffDef, ThinkTreeDef>()
        {
            { EdoDefOf.WN_EdoTensaiHediff , EdoDefOf.WN_EdoTensaiTree},
        };

        private static bool HandleThinkTreePatch(Pawn_Thinker __instance, ref ThinkTreeDef __result, bool isConstant)
        {
            Pawn pawn = __instance.pawn;
            if (pawn == null || ThinkTreeMap == null) return true;

            if (!isConstant)
            {
                foreach (var item in ThinkTreeMap)
                {
                    if (pawn == null || item.Key == null || item.Value == null)
                    {
                        continue;
                    }

                    if (pawn.health.hediffSet.GetFirstHediffOfDef(item.Key) != null)
                    {
                        __result = item.Value;
                        return false;
                    }
                }
            }


            return true;
        }
    }


}
