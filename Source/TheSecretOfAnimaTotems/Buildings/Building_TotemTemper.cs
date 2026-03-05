using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;

namespace tsoa.totems;

public class Building_TotemTemper : Building_AnimusTotem
{
    internal const int moodDivisor = 2; // arbitrary, TODO balance

    protected override void Tick()
    { 
        if (!compRefuelable.HasFuel)
        {
            gameComp.activeTemper.Remove(this);
        }

        base.Tick();
    }

    public override void DoTotemEffect()
    {
        gameComp.activeTemper.Add(this);
    }

    [HarmonyPatch(typeof(ThoughtDef), nameof(ThoughtDef.DurationTicks), MethodType.Getter)]
    public static class Harmony_Temper
    {
        public static void Postfix(ThoughtDef __instance, ref int __result)
        {
            if (GameComponent_TotemTracker.Instance.activeTemper.Count == 0)
                return;

            List<ThoughtStage> stages = __instance.stages;
            if (stages.NullOrEmpty())
                return;

            float mood = stages[0].baseMoodEffect;
            if (mood < 0)
            {
                __result = __result / 2;
            }
            else
            {
                __result = __result * 2;
            }
        }
    }
}