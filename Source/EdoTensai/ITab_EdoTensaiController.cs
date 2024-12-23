using RimWorld;
using UnityEngine;
using Verse;

namespace EdoTensai
{
    public class ITab_EdoTensaiController : ITab
    {
        private Vector2 scrollPosition = Vector2.zero;
        private const float ROW_HEIGHT = 40f;
        private const float ICON_SIZE = 30f;
        private const float LABEL_WIDTH = 100f;
        private const float STATUS_WIDTH = 80f;
        private const float BUTTON_WIDTH = 80f;
        private const float COLUMN_SPACING = 5f;
        private const float SPACING = 5f;

        public ITab_EdoTensaiController()
        {
            this.labelKey = "TabEdoTensai";
            this.tutorTag = "EdoTensai";
            this.size = new Vector2(450f, 450f);
        }

        protected override void FillTab()
        {
            Rect rect = new Rect(0f, 0f, this.size.x, this.size.y).ContractedBy(10f);
            Rect viewRect = new Rect(0f, 0f, rect.width - 16f, 1000f);
            Pawn pawn = (Pawn)this.SelPawn;
            Hediff edoController = pawn.health.hediffSet.GetFirstHediffOfDef(EdoDefOf.WN_EdoTensaiController);
            HediffComp_EdoTensaiController controllerComp = edoController?.TryGetComp<HediffComp_EdoTensaiController>();

            Widgets.BeginScrollView(rect, ref scrollPosition, viewRect);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(viewRect);

            if (pawn != null && controllerComp != null)
            {
                var storedPawns = controllerComp.GetStoredPawns();
                listingStandard.Label($"Total Controlled Pawns: {storedPawns.Count}");
                listingStandard.GapLine();

                foreach (var storedPawn in storedPawns)
                {
                    DrawPawnRow(pawn, controllerComp, storedPawn, listingStandard);
                }
            }
            else
            {
                listingStandard.Label("No Edo Tensai Controller data available");
            }

            listingStandard.End();
            Widgets.EndScrollView();
        }

        private void DrawPawnRow(Pawn controller, HediffComp_EdoTensaiController controllerComp, Pawn storedPawn, Listing_Standard listingStandard)
        {
            Rect rowRect = listingStandard.GetRect(ROW_HEIGHT);
            var layout = new RowLayoutManager(rowRect);

            Rect iconRect = layout.NextRect(ICON_SIZE, COLUMN_SPACING);
            Rect labelRect = layout.NextRect(LABEL_WIDTH, COLUMN_SPACING);
            Rect statusRect = layout.NextRect(STATUS_WIDTH, COLUMN_SPACING);
            Rect summonButtonRect = layout.NextRect(BUTTON_WIDTH, COLUMN_SPACING);
            Rect removeButtonRect = layout.NextRect(BUTTON_WIDTH);


            Widgets.ThingIcon(iconRect, storedPawn);
            Widgets.HyperlinkWithIcon(iconRect, new Dialog_InfoCard.Hyperlink(storedPawn.def));


            Widgets.Label(labelRect, storedPawn.Label);


            bool isActive = controllerComp.IsPawnActive(storedPawn);
            Widgets.Label(statusRect, isActive ? "Active" : "Stored");


            if (isActive)
            {
                if (Widgets.ButtonText(summonButtonRect, "Unsummon"))
                {
                    controllerComp.UnsummonPawn(storedPawn);
                }
            }
            else
            {
                if (Widgets.ButtonText(summonButtonRect, "Summon"))
                {
                    controllerComp.SummonPawn(storedPawn);
                }
            }

            if (Widgets.ButtonText(removeButtonRect, "Release"))
            {
                controllerComp.RemoveSummon(storedPawn);
            }
        }

        public override bool IsVisible => base.IsVisible && this.SelPawn != null && this.SelPawn.health.hediffSet.HasHediff(EdoDefOf.WN_EdoTensaiController);
    }
}
