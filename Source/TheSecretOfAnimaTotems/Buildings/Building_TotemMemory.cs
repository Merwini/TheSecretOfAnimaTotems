using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;

namespace tsoa.totems;

public class Building_TotemMemory : Building_AnimusTotem
{
    internal const int lossDivisor = 2; // arbitrary, TODO balance

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
        gameComp.activeMemory.Add(this);
    }

    public override void EndTotemEffect()
    {
        gameComp.activeMemory.Remove(this);
    }

    [HarmonyPatch(typeof(SkillRecord), nameof(SkillRecord.Learn))]
    public static class Harmony_Memory
    {
        public static void Prefix(ref float xp)
        {
            if (xp < 0 && GameComponent_TotemTracker.Instance?.activeMemory.Count != 0)
            {
                xp /= lossDivisor;
            }
        }
    }
}