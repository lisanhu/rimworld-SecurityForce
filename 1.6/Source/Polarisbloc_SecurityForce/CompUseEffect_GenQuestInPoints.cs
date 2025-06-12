using System.Collections.Generic;
using RimWorld;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;

namespace Polarisbloc_SecurityForce;

public class CompUseEffect_GenQuestInPoints : CompUseEffect
{
	private CompProperties_GenQuestInPoints Props => (CompProperties_GenQuestInPoints)props;

	public override void DoEffect(Pawn usedBy)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		base.DoEffect(usedBy);
		Slate val = new Slate();
		float num = Mathf.Max(Props.minPoints, (parent).MarketValue + Props.pointsOffset.RandomInRange);
		Pawn_SkillTracker skills = usedBy.skills;
		switch ((skills != null) ? new int?(skills.GetSkill(SkillDefOf.Intellectual).Level) : null)
		{
		case 15:
			num *= 1.1f;
			break;
		case 16:
			num *= 1.2f;
			break;
		case 17:
			num *= 1.3f;
			break;
		case 18:
			num *= 1.5f;
			break;
		case 19:
			num *= 1.75f;
			break;
		case 20:
			num *= 2f;
			break;
		}
		val.Set<float>("points", num, false);
		val.Set<Map>("map", ((Thing)usedBy).Map, false);
		if (!Props.quest.CanRun(val))
		{
			Map mapHeld = ((Thing)usedBy).MapHeld;
			List<Thing> list = ThingSetMakerDefOf.VisitorGift.root.Generate();
			IntVec3 val2 = DropCellFinder.RandomDropSpot(mapHeld, true);
			DropPodUtility.DropThingsNear(val2, mapHeld, (IEnumerable<Thing>)list, 110, false, true, true, true, true, (Faction)null);
			Find.LetterStack.ReceiveLetter(Translator.Translate("LetterLabelPolarisblocConsolationPrize"), Translator.Translate("PolarisblocConsolationPrize"), LetterDefOf.PositiveEvent, (new TargetInfo(val2, mapHeld, false)), (Faction)null, (Quest)null, (List<ThingDef>)null, (string)null);
		}
		else
		{
			QuestUtility.SendLetterQuestAvailable(QuestUtility.GenerateQuestAndMakeAvailable(Props.quest, val));
		}
	}
}
