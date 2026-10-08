using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;

namespace tsoa.totems;

public class Building_TotemBloodthirst : Building_AnimusTotem
{
    public override void DoTotemEffect()
    {
        RegisterTotem();
    }

    public override void RegisterTotem()
    {
        gameComp.activeBloodthirst.Add(this);
    }

    public override void EndTotemEffect()
    {
        gameComp.activeBloodthirst.Remove(this);
    }

    [HarmonyPatch(typeof(RecordsUtility), nameof(RecordsUtility.Notify_PawnKilled))]
    public static class RecordsUtility_Notify_PawnKilled_Postfix
    {
        public static void Postfix(Pawn killed, Pawn killer)
        {
            if (GameComponent_TotemTracker.Instance.activeBloodthirst.Count == 0)
                return;

            if (!killer.IsColonist)
                return;

            if (killer.Dead || killer.Destroyed || !killer.Spawned)
                return;

            Hediff_Bloodthirst hediff = killer.health?.hediffSet?.GetFirstHediffOfDef(TSOAT_DefOf.TSOA_BloodthirstHediff) as Hediff_Bloodthirst;
            if (hediff == null)
            {
                hediff = (Hediff_Bloodthirst)HediffMaker.MakeHediff(TSOAT_DefOf.TSOA_BloodthirstHediff, killer);
                killer.health.AddHediff(hediff);
            }
            hediff.AddKill();
        }
    }
}
