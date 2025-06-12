using System.Linq;
using RimWorld;
using Verse;

namespace Polarisbloc_SecurityForce;

public class CygnusStandardTights : Apparel
{
	private int TicksBetweenTend = 300;

	private int tendTicks;

	protected override void Tick()
	{
		base.Tick();
		if (Wearer != null)
		{
			tendTicks++;
			if (CheckAutoTendAvaliable())
			{
				tendTicks = 0;
				AutoTendWearer(Wearer);
			}
		}
	}

	private void AutoTendWearer(Pawn pawn)
	{
		Hediff val = default(Hediff);
		if (GenCollection.TryRandomElement<Hediff>(pawn.health.hediffSet.hediffs.Where((Hediff x) => x.TendableNow(false) && (x is Hediff_Injury || x is Hediff_MissingPart)), out val))
		{
			val.Tended(Rand.Range(0.6f, 1f), 1f, 0);
		}
	}

	private bool CheckAutoTendAvaliable()
	{
		if (tendTicks >= TicksBetweenTend)
		{
			return true;
		}
		return false;
	}
}
