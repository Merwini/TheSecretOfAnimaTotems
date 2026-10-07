using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;
using LudeonTK;

namespace tsoa.totems;

public class Building_TotemFortune : Building_AnimusTotem
{
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
        gameComp.activeFortune.Add(this);
    }

    public override void EndTotemEffect()
    {
        gameComp.activeFortune.Remove(this);
    }

    [HarmonyPatch(typeof(DamageWorker_AddInjury), nameof(DamageWorker_AddInjury.FinalizeAndAddInjury), new Type[] { typeof(Pawn), typeof(Hediff_Injury), typeof(DamageInfo), typeof(DamageWorker.DamageResult) })]
    public static class DamageWorker_AddInjury_FinalizeAndAddInjury_Prefix
    {
        public static bool Prefix(Pawn pawn, Hediff_Injury injury, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            if (GameComponent_TotemTracker.Instance == null || GameComponent_TotemTracker.Instance.activeFortune == null)
                return true;

            if (GameComponent_TotemTracker.Instance.activeFortune.Count == 0)
                return true;

            if (!pawn.IsColonist)
                return true;

            bool wouldDie = pawn.health.WouldDieAfterAddingHediff(injury);
            if (!wouldDie)
                return true;

            Hediff_NineLives hediff = pawn.health?.hediffSet?.GetFirstHediff<Hediff_NineLives>();
            if (hediff == null)
            {
                hediff = (Hediff_NineLives)HediffMaker.MakeHediff(TSOAT_DefOf.TSOA_NineLivesHediff, pawn);
                pawn.health.AddHediff(hediff);
            }
            else if (!hediff.UseLife)
            {
                return true;
            }

            Messages.Message("TSOA_FortuneMessage".Translate(), MessageTypeDefOf.NegativeHealthEvent);
            DebugToolsPawns.DamageUntilDown(pawn);
            var injuries = pawn.health.hediffSet.GetHediffsTendable();
            foreach (Hediff inj in injuries)
            {
                if (inj.Bleeding)
                    inj.Tended(0.5f, 1f);
            }

            return false;
        }
    }
}
