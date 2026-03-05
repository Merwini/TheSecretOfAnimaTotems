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
public class Building_TotemTemper : Building_AnimusTotem
{
    internal static HashSet<Building_TotemTemper> activeTotems;
    internal const int moodDivisor = 2; // arbitrary, TODO balance

    static Building_TotemTemper()
    {
        activeTotems = new HashSet<Building_TotemTemper>();
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        if (activeTotems == null)
        {
            activeTotems = new HashSet<Building_TotemTemper>();
        }

        base.SpawnSetup(map, respawningAfterLoad);
    }

    protected override void Tick()
    { 
        if (!compRefuelable.HasFuel)
        {
            activeTotems.Remove(this);
        }

        base.Tick();
    }

    public override void DoTotemEffect()
    {
        activeTotems.Add(this);
    }

    [HarmonyPatch(typeof(ThoughtDef), nameof(ThoughtDef.DurationTicks), MethodType.Getter)]
    public static class Harmony_Temper
    {
        public static void Postfix(ThoughtDef __instance, ref int __result)
        {
            if (activeTotems.Count == 0)
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