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
    public override void DoTotemEffect()
    {
        RegisterTotem();
    }


    public override void RegisterTotem()
    {
        gameComp.activeTemper.Add(this);
    }

    public override void EndTotemEffect()
    {
        gameComp.activeTemper.Remove(this);
    }

    [HarmonyPatch(typeof(Thought), nameof(Thought.DurationTicks), MethodType.Getter)]
    public static class Thought_DurationTicks_Postfix
    {
        public static void Postfix(Thought __instance, ref int __result)
        {
            // Is this the most efficient order for the early returns?
            if (!__instance.pawn.IsPlayerControlled)
                return;

            if (GameComponent_TotemTracker.Instance.activeTemper.Count == 0)
                return;

            ThoughtStage stage = __instance.CurStage;
            if (stage == null)
                return;

            float mood = stage.baseMoodEffect;
            if (mood < 0)
            {
                __result = (int)(__result / 2); // TODO balance
            }
            else
            {
                __result = (int)(__result * 1.5f);
            }
        }
    }
}