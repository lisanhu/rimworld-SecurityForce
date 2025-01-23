using RimWorld;
using Verse;

namespace Polarisbloc_SecurityForce;

public class CompProperties_GenQuestInPoints : CompProperties_UseEffect
{
	public QuestScriptDef quest;

	public FloatRange pointsOffset = new FloatRange(-200f, 300f);

	public float minPoints = 300f;

	public CompProperties_GenQuestInPoints()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		compClass = typeof(CompUseEffect_GenQuestInPoints);
	}
}
