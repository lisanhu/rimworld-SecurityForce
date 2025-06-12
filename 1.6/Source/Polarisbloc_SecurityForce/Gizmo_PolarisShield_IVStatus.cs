using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Polarisbloc_SecurityForce;

[StaticConstructorOnStartup]
internal class Gizmo_PolarisShield_IVStatus : Gizmo
{
	public PolarisShieldBelt_IV shield;

	private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.6f, 0.4f));

	private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

	public Gizmo_PolarisShield_IVStatus()
	{
		Order = -100f;
	}

	public override float GetWidth(float maxWidth)
	{
		return 140f;
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms gizmoRenderParms)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Rect overRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Find.WindowStack.ImmediateWindow(1221393, overRect, (WindowLayer)0, (Action)delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			Rect val;
			Rect val2 = (val = GenUI.ContractedBy(GenUI.AtZero(overRect), 6f));
			val.height = overRect.height / 2f;
			Text.Font = (GameFont)0;
			Widgets.Label(val, ((Entity)shield).LabelCap);
			Rect val3 = val2;
			val3.yMin = overRect.height / 2f;
			float num = shield.Energy / Mathf.Max(1f, StatExtension.GetStatValue((Thing)(object)shield, StatDefOf.EnergyShieldEnergyMax, true, -1));
			Widgets.FillableBar(val3, num, FullShieldBarTex, EmptyShieldBarTex, false);
			Text.Font = (GameFont)1;
			Text.Anchor = (TextAnchor)4;
			Widgets.Label(val3, (shield.Energy * 100f).ToString("F0") + " / " + (StatExtension.GetStatValue((Thing)(object)shield, StatDefOf.EnergyShieldEnergyMax, true, -1) * 100f).ToString("F0"));
			Text.Anchor = (TextAnchor)0;
		}, true, false, 1f, (Action)null);
		return new GizmoResult((GizmoState)0);
	}
}
