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
public class Building_TotemMemory : Building_AnimusTotem
{
    internal static HashSet<Building_TotemMemory> activeTotems;
    internal const int lossDivisor = 2; // arbitrary, TODO balance

    static Building_TotemMemory()
    {
        activeTotems = new HashSet<Building_TotemMemory>();
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        if (activeTotems == null)
        {
            activeTotems = new HashSet<Building_TotemMemory>(); // in the rare event that multiple totems exist
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

    [HarmonyPatch(typeof(SkillRecord), nameof(SkillRecord.Learn))]
    public static class Harmony_Memory
    {
        public static void Prefix(float xp)
        {
            if (xp < 0 && activeTotems.Count != 0)
            {
                xp /= lossDivisor;
            }
        }
    }
}