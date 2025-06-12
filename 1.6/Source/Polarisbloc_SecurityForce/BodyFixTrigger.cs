using System;
using System.Collections.Generic;
using Polarisbloc;
using RimWorld;
using Verse;

namespace Polarisbloc_SecurityForce;

public class BodyFixTrigger : Apparel
{
	public override void Notify_Equipped(Pawn pawn)
	{
		base.Notify_Equipped(pawn);
		if (Wearer.kindDef == PawnKindDefOf.WildMan || (Wearer).Faction == null || Wearer.kindDef.defaultFactionType != PSFDefOf.Polaribloc_SecuirityForce)
		{
			Destroy((DestroyMode)0);
		}
		else
		{
			if (Wearer == null)
			{
				return;
			}
			if (ModLister.GetActiveModWithIdentifier("Vanya.Polarisbloc.CoreLab", false) != null)
			{
				List<Hediff_CombatChip> list = new List<Hediff_CombatChip>();
				Wearer.health.hediffSet.GetHediffs<Hediff_CombatChip>(ref list, (Predicate<Hediff_CombatChip>)null);
				if (!GenList.NullOrEmpty<Hediff_CombatChip>((IList<Hediff_CombatChip>)list))
				{
					foreach (Hediff_CombatChip item in list)
					{
						Wearer.health.RemoveHediff((Hediff)(object)item);
					}
				}
				Wearer.health.AddHediff(HediffDef.Named("PolarisCombatChip_Currency"), Wearer.health.hediffSet.GetBrain(), (DamageInfo?)null, null);
			}
			CombatEnhancingDrugsApply(Wearer);
			foreach (Apparel item2 in Wearer.apparel.WornApparel)
			{
				CompBiocodable val = ThingCompUtility.TryGetComp<CompBiocodable>((Thing)(object)item2);
				if (val != null && !val.Biocoded)
				{
					val.CodeFor(Wearer);
				}
			}
			Wearer.apparel.Remove(this);
			Destroy((DestroyMode)0);
		}
	}

	private void CombatEnhancingDrugsApply(Pawn pawn)
	{
		if (ModLister.GetActiveModWithIdentifier("Vanya.Polarisbloc.CoreLab", false) != null)
		{
			Hediff val = HediffMaker.MakeHediff(HediffDef.Named("PolarisHealingPotion"), pawn, (BodyPartRecord)null);
			val.Severity = 2f;
			pawn.health.AddHediff(val, (BodyPartRecord)null, (DamageInfo?)null, null);
		}
		if (Rand.Chance(0.5f))
		{
			pawn.health.AddHediff(HediffDef.Named("GoJuiceHigh"), (BodyPartRecord)null, (DamageInfo?)null, null);
		}
		else
		{
			pawn.health.AddHediff(HediffDef.Named("YayoHigh"), (BodyPartRecord)null, (DamageInfo?)null, null);
		}
	}
}
