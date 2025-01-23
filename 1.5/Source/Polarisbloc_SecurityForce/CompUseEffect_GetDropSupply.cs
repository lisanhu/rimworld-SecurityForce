using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Polarisbloc_SecurityForce;

public class CompUseEffect_GetDropSupply : CompUseEffect
{
	private bool captain;

	private static readonly List<ThingDef> currencySuits = new List<ThingDef>
	{
		PSFDefOf.Polarisbloc_CygnusStandardTights,
		PSFDefOf.Polarisbloc_AlkaidStrategyAssistant,
		PSFDefOf.Polarisbloc_ThubanTacticalGoggles
	};

	private static readonly List<ThingDef> standardSuits = new List<ThingDef>
	{
		PSFDefOf.Apparel_PolarisShieldBelt_II,
		PSFDefOf.Polarisbloc_CygnusStandardArmor,
		PSFDefOf.Polarisbloc_CaniculaRifle
	};

	private static readonly List<ThingDef> captainSuits = new List<ThingDef>
	{
		PSFDefOf.Apparel_PolarisShieldBelt_IV,
		PSFDefOf.Polarisbloc_CygnusStandardArmorC,
		PSFDefOf.Polarisbloc_CaniculaSniper
	};

	private List<Thing> MakeSuits()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		QualityCategory quality = parent.GetComp<CompQuality>().Quality;
		List<Thing> list = new List<Thing>();
		list.Clear();
		foreach (ThingDef currencySuit in currencySuits)
		{
			Thing val = ThingMaker.MakeThing(currencySuit, (ThingDef)null);
			CompQuality obj = ThingCompUtility.TryGetComp<CompQuality>(val);
			if (obj != null)
			{
				obj.SetQuality(quality, (ArtGenerationContext)0);
			}
			list.Add(val);
		}
		if (captain)
		{
			foreach (ThingDef captainSuit in captainSuits)
			{
				Thing val2 = ThingMaker.MakeThing(captainSuit, (ThingDef)null);
				CompQuality obj2 = ThingCompUtility.TryGetComp<CompQuality>(val2);
				if (obj2 != null)
				{
					obj2.SetQuality(quality, (ArtGenerationContext)0);
				}
				list.Add(val2);
			}
		}
		else
		{
			foreach (ThingDef standardSuit in standardSuits)
			{
				Thing val3 = ThingMaker.MakeThing(standardSuit, (ThingDef)null);
				CompQuality obj3 = ThingCompUtility.TryGetComp<CompQuality>(val3);
				if (obj3 != null)
				{
					obj3.SetQuality(quality, (ArtGenerationContext)0);
				}
				list.Add(val3);
			}
		}
		Thing val4 = ThingMaker.MakeThing(PSFDefOf.Polarisbloc_EmergencyFood, (ThingDef)null);
		val4.stackCount = 5;
		list.Add(val4);
		return list;
	}

	public override void PostExposeData()
	{
		base.PostExposeData();
		Scribe_Values.Look<bool>(ref captain, "captain", false, false);
	}

	public override void PostPostMake()
	{
		base.PostPostMake();
		if (Rand.Chance(0.1f))
		{
			captain = true;
		}
	}

	public override string TransformLabel(string label)
	{
		if (!captain)
		{
			return label;
		}
		return "C'" + label;
	}

	public override IEnumerable<Gizmo> CompGetGizmosExtra()
	{
		if (Prefs.DevMode)
		{
			yield return (Gizmo)new Command_Toggle
			{
				defaultLabel = "captain",
				isActive = () => captain,
				toggleAction = delegate
				{
					captain = !captain;
				}
			};
		}
	}

	public override void DoEffect(Pawn usedBy)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		base.DoEffect(usedBy);
		_ = ((Thing)usedBy).MapHeld;
		foreach (Thing item in MakeSuits())
		{
			GenPlace.TryPlaceThing(item, ((Thing)usedBy).PositionHeld, ((Thing)usedBy).MapHeld, (ThingPlaceMode)1, (Action<Thing, int>)null, (Predicate<IntVec3>)null, default(Rot4));
		}
		bool flag = false;
		if (captain)
		{
			if (Rand.Chance(0.1f))
			{
				flag = true;
			}
		}
		else if (Rand.Chance(0.03f))
		{
			flag = true;
		}
		if (flag)
		{
			Thing obj = ThingMaker.MakeThing(PSFDefOf.PolarisBunnyGundamSculpture, ThingDefOf.Gold);
			CompQuality obj2 = ThingCompUtility.TryGetComp<CompQuality>(obj);
			if (obj2 != null)
			{
				obj2.SetQuality((QualityCategory)6, (ArtGenerationContext)0);
			}
			GenPlace.TryPlaceThing((Thing)(object)MinifyUtility.MakeMinified(obj), ((Thing)usedBy).PositionHeld, ((Thing)usedBy).MapHeld, (ThingPlaceMode)1, (Action<Thing, int>)null, (Predicate<IntVec3>)null, default(Rot4));
		}
		GenPlaceOtherRewards(usedBy);
	}

	private void GenPlaceOtherRewards(Pawn usedBy)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		int num = Rand.Range(800, 1500);
		List<Thing> list = new List<Thing>();
		while (num > 0)
		{
			Thing val = TrySpawnRandomReward(num);
			list.Add(val);
			num -= Mathf.RoundToInt(val.MarketValue * (float)val.stackCount);
		}
		foreach (Thing item in list)
		{
			GenPlace.TryPlaceThing(item, ((Thing)usedBy).PositionHeld, ((Thing)usedBy).MapHeld, (ThingPlaceMode)1, (Action<Thing, int>)null, (Predicate<IntVec3>)null, default(Rot4));
		}
	}

	private Thing TrySpawnRandomReward(int amountValue)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		ThingDef val = default(ThingDef);
		GenCollection.TryRandomElementByWeight<ThingDef>(DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) => x.thingSetMakerTags != null && x.BaseMarketValue >= 50f && x.recipeMaker == null), (Func<ThingDef, float>)((ThingDef x) => x.generateCommonality), out val);
		ThingDef val2 = GenStuff.RandomStuffFor(val);
		Thing val3 = ThingMaker.MakeThing(val, val2);
		CompQuality obj = ThingCompUtility.TryGetComp<CompQuality>(val3);
		if (obj != null)
		{
			obj.SetQuality(QualityUtility.GenerateQualityReward(), (ArtGenerationContext)1);
		}
		if (val3.def.Minifiable)
		{
			val3 = (Thing)(object)MinifyUtility.MakeMinified(val3);
		}
		val3.stackCount = 1;
		if (val3.def.stackLimit > 1 && val3.MarketValue < (float)amountValue)
		{
			val3.stackCount = Mathf.CeilToInt((float)amountValue / val3.MarketValue);
		}
		return val3;
	}
}
