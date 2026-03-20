using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;

namespace tsoa.totems;

public class Building_TotemBounty : Building_AnimusTotem
{
    internal const float growthRate = 1.5f;

    public override void Tick()
    {
        if (!compRefuelable.HasFuel)
        {
            EndTotemEffect();
        }

        base.Tick();
    }

    public override void DoTotemEffect()
    {
        gameComp.activeBounty[this.Map] = this;
    }

    public override void EndTotemEffect()
    {
        gameComp.activeBounty.Remove(this.Map);
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthRate), MethodType.Getter)]
    public static class Harmony_Bounty
    {
        public static void Postfix(Plant __instance, ref float __result)
        {
            if (GameComponent_TotemTracker.Instance.activeBounty.ContainsKey(__instance.Map))
            {
                __result *= growthRate;
            }
        }
    }
}
