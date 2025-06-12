using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Polarisbloc_SecurityForce;

[StaticConstructorOnStartup]
public class PolarisShieldBelt_IV : Apparel
{
	private float energy;

	private int ticksToReset = -1;

	private int lastKeepDisplayTick = -9999;

	private Vector3 impactAngleVect;

	private int lastAbsorbDamageTick = -9999;

	private const float MinDrawSize = 1.2f;

	private const float MaxDrawSize = 1.45f;

	private const float MaxDamagedJitterDist = 0.05f;

	private const int JitterDurationTicks = 8;

	private readonly int StartingTicksToReset = 1200;

	private readonly float EnergyOnReset = 1f;

	private readonly float EnergyLossPerDamage = 0.02f;

	private readonly int KeepDisplayingTicks = 600;

	private readonly float ApparelScorePerEnergyMax = 0.25f;

	private static readonly Material BubbleMat = MaterialPool.MatFrom("PolarisblocSF/UI/PolarisShieldIVBubble", ShaderDatabase.Transparent);

	private readonly int LightningColdDownTicks = 120;

	private readonly float maxDamageTakeOnce = 20f;

	private bool canLightning = true;

	private bool CanLightning
	{
		get
		{
			if (lastAbsorbDamageTick <= Find.TickManager.TicksGame - LightningColdDownTicks)
			{
				return canLightning;
			}
			return false;
		}
	}

	private bool CanForceActive => HitPoints > 20;

	private float EnergyMax => StatExtension.GetStatValue(this, StatDefOf.EnergyShieldEnergyMax, true, -1);

	private float EnergyGainPerTick => StatExtension.GetStatValue(this, StatDefOf.EnergyShieldRechargeRate, true, -1) / 60f;

	public float Energy => energy;

	public ShieldState ShieldState
	{
		get
		{
			if (ticksToReset <= 0)
			{
				return (ShieldState)0;
			}
			return (ShieldState)1;
		}
	}

	private bool ShouldDisplay
	{
		get
		{
			Pawn wearer = Wearer;
			if (((Thing)wearer).Spawned && !wearer.Dead && !wearer.Downed)
			{
				if (!wearer.InAggroMentalState && !wearer.Drafted && (!FactionUtility.HostileTo(((Thing)wearer).Faction, Faction.OfPlayer) || wearer.IsPrisoner))
				{
					return Find.TickManager.TicksGame < lastKeepDisplayTick + KeepDisplayingTicks;
				}
				return true;
			}
			return false;
		}
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look<float>(ref energy, "energy", 0f, false);
		Scribe_Values.Look<int>(ref ticksToReset, "ticksToReset", -1, false);
		Scribe_Values.Look<int>(ref lastKeepDisplayTick, "lastKeepDisplayTick", 0, false);
		Scribe_Values.Look<bool>(ref canLightning, "canLightning", true, false);
	}

	public override IEnumerable<Gizmo> GetWornGizmos()
	{
		if (Find.Selector.SingleSelectedThing != Wearer)
		{
			yield break;
		}
		if (Wearer.Drafted)
		{
			yield return (Gizmo)new Command_Toggle
			{
				hotKey = KeyBindingDefOf.Command_TogglePower,
				icon = (Texture)(object)((BuildableDef)def).uiIcon,
				defaultLabel = (Translator.Translate("PlrsLightningActiveLabel")),
				defaultDesc = (Translator.Translate("PlrsLightningActiveDESC")),
				isActive = () => canLightning,
				toggleAction = delegate
				{
					canLightning = !canLightning;
				}
			};
			if ((int)ShieldState != 0)
			{
				yield return (Gizmo)new Command_Action
				{
					action = delegate
					{
						//IL_0023: Unknown result type (might be due to invalid IL or missing references)
						if (CanForceActive)
						{
							HitPoints = HitPoints - 20;
							Reset();
						}
						else
						{
							Messages.Message((Translator.Translate("PlrsNoEnoughHitPointsToReset")), (Wearer), MessageTypeDefOf.NegativeEvent, true);
						}
					},
					defaultLabel = (Translator.Translate("PlrsForceResetLabel")),
					defaultDesc = (Translator.Translate("PlrsForceResetDESC")),
					icon = (Texture)(object)TexCommand.DesirePower,
					hotKey = KeyBindingDefOf.Misc7,
					Disabled = !CanForceActive,
					disabledReason = (Translator.Translate("PlrsNoEnoughHitPointsToReset"))
				};
			}
		}
		yield return (Gizmo)(object)new Gizmo_PolarisShield_IVStatus
		{
			shield = this
		};
	}

	public override float GetSpecialApparelScoreOffset()
	{
		return EnergyMax * ApparelScorePerEnergyMax;
	}

	protected override void Tick()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		base.Tick();
		if (Wearer == null)
		{
			energy = 0f;
		}
		else if ((int)ShieldState == 1)
		{
			ticksToReset--;
			if (ticksToReset <= 0)
			{
				Reset();
			}
		}
		else if ((int)ShieldState == 0)
		{
			energy += EnergyGainPerTick;
			if (energy > EnergyMax)
			{
				energy = EnergyMax;
			}
		}
	}

	public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if (dinfo.Def == DamageDefOf.SurgicalCut || dinfo.Def == DamageDefOf.ExecutionCut)
		{
			return false;
		}
		if (dinfo.Def == DamageDefOf.EMP)
		{
			if ((int)ShieldState == 1)
			{
				Reset();
			}
			energy += dinfo.Amount * 0.002f;
		}
		if (!dinfo.Def.harmsHealth)
		{
			return false;
		}
		if (dinfo.Instigator == Wearer)
		{
			AbsorbedDamage(dinfo);
			return true;
		}
		if ((int)ShieldState == 0)
		{
			TryShotLightning(dinfo);
			float amount = dinfo.Amount;
			if (amount > maxDamageTakeOnce)
			{
				amount = maxDamageTakeOnce;
			}
			energy -= amount * EnergyLossPerDamage;
			if (energy < 0f)
			{
				Break();
			}
			else
			{
				AbsorbedDamage(dinfo);
			}
			return true;
		}
		return false;
	}

	public void KeepDisplaying()
	{
		lastKeepDisplayTick = Find.TickManager.TicksGame;
	}

	private void AbsorbedDamage(DamageInfo dinfo)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		SoundStarter.PlayOneShot(SoundDefOf.EnergyShield_AbsorbDamage, (new TargetInfo(((Thing)Wearer).Position, ((Thing)Wearer).Map, false)));
		impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
		Vector3 val = GenThing.TrueCenter((Thing)(object)Wearer) + Vector3Utility.RotatedBy(impactAngleVect, 180f) * 0.5f;
		float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
		FleckMaker.Static(val, ((Thing)Wearer).Map, FleckDefOf.ExplosionFlash, num);
		int num2 = (int)num;
		for (int i = 0; i < num2; i++)
		{
			FleckMaker.ThrowDustPuff(val, ((Thing)Wearer).Map, Rand.Range(0.8f, 1.2f));
		}
		lastAbsorbDamageTick = Find.TickManager.TicksGame;
		KeepDisplaying();
	}

	private void Break()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		// SoundStarter.PlayOneShot(SoundDefOf.EnergyShield_Broken, (new TargetInfo(((Thing)Wearer).Position, ((Thing)Wearer).Map, false)));
		FleckMaker.Static(GenThing.TrueCenter((Thing)(object)Wearer), ((Thing)Wearer).Map, FleckDefOf.ExplosionFlash, 12f);
		for (int i = 0; i < 6; i++)
		{
			FleckMaker.ThrowDustPuff(GenThing.TrueCenter((Thing)(object)Wearer) + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f), ((Thing)Wearer).Map, Rand.Range(0.8f, 1.2f));
		}
		energy = 0f;
		ticksToReset = StartingTicksToReset;
	}

	private void Reset()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if ((Wearer).Spawned)
		{
			SoundStarter.PlayOneShot(SoundDefOf.EnergyShield_Reset, (new TargetInfo((Wearer).Position, (Wearer).Map, false)));
			FleckMaker.ThrowLightningGlow(GenThing.TrueCenter(Wearer), (Wearer).Map, 3f);
		}
		ticksToReset = -1;
		energy = EnergyOnReset;
	}

	public override void DrawWornExtras()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if ((int)ShieldState == 0 && ShouldDisplay)
		{
			float num = Mathf.Lerp(1.2f, 1.45f, energy);
			Vector3 val = Wearer.Drawer.DrawPos;
			val.y = Altitudes.AltitudeFor((AltitudeLayer)26);
			int num2 = Find.TickManager.TicksGame - lastAbsorbDamageTick;
			if (num2 < 8)
			{
				float num3 = (float)(8 - num2) / 8f * 0.05f;
				val += impactAngleVect * num3;
				num -= num3;
			}
			float num4 = Rand.Range(0, 360);
			Vector3 val2 = default(Vector3);
			val2 = new Vector3(num, 1f, num);
			Matrix4x4 val3 = default(Matrix4x4);
			val3.SetTRS(val, Quaternion.AngleAxis(num4, Vector3.up), val2);
			Graphics.DrawMesh(MeshPool.plane10, val3, BubbleMat, 0);
		}
	}

	private void TryShotLightning(DamageInfo dinfo)
	{
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		if (dinfo.Instigator == null || !canLightning || dinfo.Instigator.def == ThingDefOf.Fire)
		{
			return;
		}
		if (dinfo.Instigator.Faction != null)
		{
			if (dinfo.Instigator.Faction != (Wearer).Faction)
			{
				if (dinfo.Instigator is Building)
				{
					Map mapHeld = dinfo.Instigator.MapHeld;
					mapHeld.weatherManager.eventHandler.AddEvent((WeatherEvent)new WeatherEvent_LightningStrike(mapHeld, dinfo.Instigator.PositionHeld));
				}
				else if (CanLightning)
				{
					Map mapHeld2 = dinfo.Instigator.MapHeld;
					mapHeld2.weatherManager.eventHandler.AddEvent((WeatherEvent)new WeatherEvent_LightningStrike(mapHeld2, dinfo.Instigator.PositionHeld));
				}
			}
		}
		else if (dinfo.Instigator is Building)
		{
			Map mapHeld3 = dinfo.Instigator.MapHeld;
			mapHeld3.weatherManager.eventHandler.AddEvent((WeatherEvent)new WeatherEvent_LightningStrike(mapHeld3, dinfo.Instigator.PositionHeld));
		}
		else if (CanLightning)
		{
			Map mapHeld4 = dinfo.Instigator.MapHeld;
			mapHeld4.weatherManager.eventHandler.AddEvent((WeatherEvent)new WeatherEvent_LightningStrike(mapHeld4, dinfo.Instigator.PositionHeld));
		}
	}
}
