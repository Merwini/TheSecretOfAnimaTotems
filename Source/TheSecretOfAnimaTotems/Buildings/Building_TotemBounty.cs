using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;

namespace tsoa.totems;

[StaticConstructorOnStartup]
public class Building_TotemBounty : Building_AnimusTotem
{
    internal static Dictionary<Map, Building_TotemBounty> activeTotems;
    internal const int growthRate = 2; // arbitrary, TODO balance

    static Building_TotemBounty()
    {
        activeTotems = new Dictionary<Map, Building_TotemBounty>();
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        if (activeTotems == null)
        {
            activeTotems = new Dictionary<Map, Building_TotemBounty>();
        }

        base.SpawnSetup(map, respawningAfterLoad);
    }

    protected override void Tick()
    {
        if (!compRefuelable.HasFuel)
        {
            activeTotems.Remove(this.Map);
        }

        base.Tick();
    }

    public override void DoTotemEffect()
    {
        activeTotems[this.Map] = this;
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthRate), MethodType.Getter)]
    public static class Harmony_Bounty
    {
        public static void Postfix(Plant __instance, ref float __result)
        {
            if (activeTotems.ContainsKey(__instance.Map))
            {
                __result *= growthRate;
            }
        }
    }
}
