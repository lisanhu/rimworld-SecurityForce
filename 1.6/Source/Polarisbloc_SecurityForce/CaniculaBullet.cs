using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Polarisbloc_SecurityForce;

public class CaniculaBullet : Bullet
{
	private void RefDinfo(ref DamageInfo dinfo, Thing hitThing)
	{
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		dinfo.SetIgnoreInstantKillProtection(true);
		dinfo.SetIgnoreArmor(true);
		Pawn val = (Pawn)(object)((hitThing is Pawn) ? hitThing : null);
		if (val == null)
		{
			return;
		}
		if (val != null)
		{
			if (val.RaceProps.FleshType == FleshTypeDefOf.Insectoid)
			{
				dinfo.SetAmount(dinfo.Amount * 6f);
			}
			BodyPartRecord brain = val.health.hediffSet.GetBrain();
			if (brain != null)
			{
				float num = 0f;
				num = (((float)DamageAmount <= 6.9f) ? 0.05f : (((float)DamageAmount <= 8f) ? 0.1f : (((float)DamageAmount <= 10f) ? 0.15f : (((float)DamageAmount <= 13f) ? 0.2f : (((float)DamageAmount <= 15f) ? 0.25f : ((!((float)DamageAmount <= 19f)) ? 0.35f : 0.3f))))));
				if (Rand.Chance(num))
				{
					dinfo.SetHitPart(brain);
				}
			}
			if (ModsConfig.IdeologyActive)
			{
				Thing launcher = Launcher;
				Pawn val2 = (Pawn)(object)((launcher is Pawn) ? launcher : null);
				if (val2 != null)
				{
					Pawn_EquipmentTracker equipment = val2.equipment;
					Thing val3 = (Thing)(object)((equipment != null) ? equipment.Primary : null);
					if (val3 != null && ReliquaryUtility.IsRelic(val3))
					{
						dinfo.SetHitPart(brain);
					}
				}
			}
		}
		DamageInfo val4 = default(DamageInfo);
		val4 = new DamageInfo(dinfo);
		val4.Def = DamageDefOf.EMP;
		DamageInfo val5 = val4;
		hitThing.TakeDamage(val5);
	}

	protected override void Impact(Thing hitThing, bool blockedByShield = false)
	{
		Map map = Map;
		IntVec3 position = Position;
		base.Impact(hitThing, blockedByShield);
		BattleLogEntry_RangedImpact val = new BattleLogEntry_RangedImpact(launcher, hitThing, intendedTarget.Thing, equipmentDef, def, targetCoverDef);
		Find.BattleLog.Add((LogEntry)(object)val);
		NotifyImpact(hitThing, map, position);
		if (hitThing != null)
		{
			Thing launcher = this.launcher;
			Pawn val2 = (Pawn)(object)((launcher is Pawn) ? launcher : null);
			bool flag = val2 == null || !val2.Drafted;
			DamageDef damageDef = def.projectile.damageDef;
			float num = DamageAmount;
			float armorPenetration = ArmorPenetration;
			Quaternion exactRotation = ExactRotation;
			DamageInfo dinfo = new DamageInfo();
			dinfo = new DamageInfo(damageDef, num, armorPenetration, exactRotation.eulerAngles.y, launcher, (BodyPartRecord)null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing, flag, true);
			RefDinfo(ref dinfo, hitThing);
			hitThing.TakeDamage(dinfo).AssociateWithLog((LogEntry_DamageResult)(object)val);
			Pawn val3 = (Pawn)(object)((hitThing is Pawn) ? hitThing : null);
			if (val3 != null && val3.stances != null)
			{
				val3.stances.stagger.Notify_BulletImpact(this);
			}
			if (def.projectile.extraDamages != null)
			{
				DamageInfo val4 = default(DamageInfo);
				foreach (ExtraDamage extraDamage in def.projectile.extraDamages)
				{
					if (Rand.Chance(extraDamage.chance))
					{
						DamageDef def = extraDamage.def;
						float amount = extraDamage.amount;
						float num2 = extraDamage.AdjustedArmorPenetration();
						exactRotation = ExactRotation;
						val4 = new DamageInfo(def, amount, num2, exactRotation.eulerAngles.y, launcher, (BodyPartRecord)null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing, flag, true);
						hitThing.TakeDamage(val4).AssociateWithLog((LogEntry_DamageResult)(object)val);
					}
				}
			}
			if (Rand.Chance(def.projectile.bulletChanceToStartFire) && (val3 == null || Rand.Chance(FireUtility.ChanceToAttachFireFromEvent((Thing)(object)val3))))
			{
				FireUtility.TryAttachFire(hitThing, def.projectile.bulletFireSizeRange.RandomInRange, launcher);
			}
			return;
		}
		if (!blockedByShield)
		{
			SoundStarter.PlayOneShot(SoundDefOf.BulletImpact_Ground, (new TargetInfo(Position, map, false)));
			if (GridsUtility.GetTerrain(Position, map).takeSplashes)
			{
				FleckMaker.WaterSplash(ExactPosition, map, Mathf.Sqrt((float)DamageAmount) * 1f, 4f);
			}
			else
			{
				FleckMaker.Static(ExactPosition, map, FleckDefOf.ShotHit_Dirt, 1f);
			}
		}
		if (Rand.Chance(def.projectile.bulletChanceToStartFire))
		{
			FireUtility.TryStartFireIn(Position, map, (def.projectile.bulletFireSizeRange).RandomInRange, launcher);
		}
	}

	private void NotifyImpact(Thing hitThing, Map map, IntVec3 position)
	{
		BulletImpactData val = default(BulletImpactData);
		val.bullet = this;
		val.hitThing = hitThing;
		val.impactPosition = position;
		BulletImpactData val2 = val;
		if (hitThing != null)
		{
			hitThing.Notify_BulletImpactNearby(val2);
		}
		int num = 9;
		for (int i = 0; i < num; i++)
		{
			IntVec3 val3 = position + GenRadial.RadialPattern[i];
			if (!GenGrid.InBounds(val3, map))
			{
				continue;
			}
			List<Thing> thingList = GridsUtility.GetThingList(val3, map);
			for (int j = 0; j < thingList.Count; j++)
			{
				if (thingList[j] != hitThing)
				{
					thingList[j].Notify_BulletImpactNearby(val2);
				}
			}
		}
	}
}
