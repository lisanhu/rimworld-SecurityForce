using System.Collections.Generic;
using System.Linq;
using Polarisbloc_SecurityForce;
using Verse;

namespace RimWorld;

public class CompUseEffect_FinishPSFShieldResearchProject : CompUseEffect
{
	public override void DoEffect(Pawn usedBy)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		base.DoEffect(usedBy);
		if (Rand.Chance(0.12f))
		{
			ResearchProjectDef named = DefDatabase<ResearchProjectDef>.GetNamed("PolarisShield", true);
			ResearchProjectDef researchProj;
			if (named != null && !named.IsFinished)
			{
				FinishInstantly(named);
			}
			else if (TryRandomlyUnfinishedResearch(out researchProj))
			{
				FinishInstantly(researchProj);
			}
		}
		else if (Rand.Chance(0.3f))
		{
			Map mapHeld = ((Thing)usedBy).MapHeld;
			List<Thing> list = ThingSetMakerDefOf.ResourcePod.root.Generate();
			IntVec3 val = DropCellFinder.RandomDropSpot(mapHeld, true);
			DropPodUtility.DropThingsNear(val, mapHeld, (IEnumerable<Thing>)list, 110, false, true, true, true, true, (Faction)null);
			Find.LetterStack.ReceiveLetter(Translator.Translate("LetterLabelPolarisblocConsolationPrize"), Translator.Translate("PolarisblocConsolationPrize"), LetterDefOf.PositiveEvent, (new TargetInfo(val, mapHeld, false)), (Faction)null, (Quest)null, (List<ThingDef>)null, (string)null);
		}
		else if (Rand.Chance(0.01f))
		{
			Map mapHeld2 = ((Thing)usedBy).MapHeld;
			List<Thing> list2 = new List<Thing>();
			Thing item = ThingMaker.MakeThing(PSFDefOf.PolarisBunnyGundamSculpture, ThingDefOf.Gold);
			list2.Add(item);
			IntVec3 val2 = DropCellFinder.RandomDropSpot(mapHeld2, true);
			DropPodUtility.DropThingsNear(val2, mapHeld2, (IEnumerable<Thing>)list2, 110, false, true, true, true, true, (Faction)null);
			Find.LetterStack.ReceiveLetter(Translator.Translate("LetterLabelPolarisblocSurprised"), Translator.Translate("PolarisblocSurprised"), LetterDefOf.PositiveEvent, (new TargetInfo(val2, mapHeld2, false)), (Faction)null, (Quest)null, (List<ThingDef>)null, (string)null);
		}
		else
		{
			Messages.Message((TranslatorFormattedStringExtensions.Translate("MessageFailToUnscrambleMemoryStick", (((Entity)usedBy).LabelShort))), MessageTypeDefOf.NegativeEvent, true);
		}
	}

	public override AcceptanceReport CanBeUsedBy(Pawn p)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		string failReason = (Translator.Translate("PolarisPlannedToRemove"));
		return new AcceptanceReport(failReason);
	}

	private bool TryRandomlyUnfinishedResearch(out ResearchProjectDef researchProj)
	{
		return GenCollection.TryRandomElement<ResearchProjectDef>(DefDatabase<ResearchProjectDef>.AllDefs.Where((ResearchProjectDef x) => !x.IsFinished), out researchProj);
	}

	private void FinishInstantly(ResearchProjectDef proj)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Find.ResearchManager.FinishProject(proj, false, (Pawn)null, true);
		Messages.Message((TranslatorFormattedStringExtensions.Translate("MessageResearchProjectFinishedByItem", (((Def)proj).label))), MessageTypeDefOf.PositiveEvent, true);
	}
}
