using RimWorld;
using Verse;

namespace Polarisbloc_SecurityForce;

public class CygnusStandardArmor : Apparel
{
	public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (dinfo.Instigator == Wearer)
		{
			return true;
		}
		if (dinfo.Def == DamageDefOf.Extinguish)
		{
			return true;
		}
		if (dinfo.Def == DamageDefOf.SurgicalCut)
		{
			return false;
		}
		if (dinfo.Def == DamageDefOf.Smoke)
		{
			return true;
		}
		if (Rand.Value * 400f < (float)HitPoints)
		{
			TakeDamage(dinfo);
			MoteMaker.ThrowText(((Thing)Wearer).DrawPos, ((Thing)Wearer).Map, (Translator.Translate("PlrsTextMote_Absorbed")), 1.5f);
			return true;
		}
		return false;
	}
}
