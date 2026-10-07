using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace tsoa.totems;

public class Building_TotemReflex : Building_AnimusTotem
{
    private bool isApplied = false;

    public override void Tick()
    { 
        base.Tick();
    }

    public override void DoTotemEffect()
    {
        HashSet<Building_TotemReflex> buildings = gameComp.activeReflex.TryGetValue(this.Map);
        if (buildings == null)
        {
            buildings = new HashSet<Building_TotemReflex>();
            gameComp.activeReflex[this.Map] = buildings;
        }
        else
        {
            buildings.Add(this);
        }

        if (CheckIfShouldApply())
        {
            if (!isApplied)
            {
                ApplyReflex(); 
            }
        }
        else
        {
            // hediff might actually still be applied, but hostiles are gone so I don't mind it refreshing if new hostiles appear
            isApplied = false;
        }
    }

    public override void EndTotemEffect()
    {
        HashSet<Building_TotemReflex> buildings = gameComp.activeReflex.TryGetValue(this.Map);
        if (buildings != null)
        {
            buildings.Remove(this);
        }
        gameComp.CleanupReflex();
    }

    private bool CheckIfShouldApply()
    {
        bool shouldApply = false;

        // TODO profiling how bad this is to call every tick
        Stopwatch watch = new Stopwatch();
        watch.Start();
        if (GenHostility.AnyHostileActiveThreatToPlayer(Map))
        {
            shouldApply = true;
        }
        watch.Stop();
        Log.Warning(watch.ElapsedMilliseconds + "ms to check for hostile threats");
        return shouldApply;
    }

    private void ApplyReflex()
    {
        if (Map == null || Current.Game == null)
            return;

        foreach (Pawn pawn in Map.mapPawns.FreeColonistsSpawned)
        {
            if (pawn.Dead || pawn.Destroyed || !pawn.Spawned)
                continue;

            Hediff_Reflex hediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(TSOAT_DefOf.TSOA_ReflexHediff) as Hediff_Reflex;
            if (hediff == null)
            {
                hediff = (Hediff_Reflex)HediffMaker.MakeHediff(TSOAT_DefOf.TSOA_ReflexHediff, pawn);
                pawn.health.AddHediff(hediff);
            }
            // Since I refactored to check for hostiles on map instead of triggering on hostile events, can't have it reapply. Should only be an issue if player is able to clean up hostiles and then gets more before first hediff disappears. Maybe fine
            //hediff.RefreshDuration();
        }

        isApplied = true;
    }

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Values.Look(ref isApplied, "isApplied", false);
    }
}
