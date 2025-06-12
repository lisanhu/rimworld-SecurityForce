using RimWorld;
using Verse;

namespace Polarisbloc_SecurityForce;

[DefOf]
public static class PSFDefOf
{
	public static ThingDef PolarisBunnyGundamSculpture;

	public static FactionDef Polaribloc_SecuirityForce;

	public static ThingDef Apparel_PolarisShieldBelt_II;

	public static ThingDef Polarisbloc_CygnusStandardArmor;

	public static ThingDef Polarisbloc_CygnusStandardTights;

	public static ThingDef Polarisbloc_AlkaidStrategyAssistant;

	public static ThingDef Polarisbloc_ThubanTacticalGoggles;

	public static ThingDef Polarisbloc_CaniculaRifle;

	public static ThingDef Polarisbloc_EmergencyFood;

	public static ThingDef Apparel_PolarisShieldBelt_IV;

	public static ThingDef Polarisbloc_CygnusStandardArmorC;

	public static ThingDef Polarisbloc_CaniculaSniper;

	static PSFDefOf()
	{
		DefOfHelper.EnsureInitializedInCtor(typeof(PSFDefOf));
	}
}
