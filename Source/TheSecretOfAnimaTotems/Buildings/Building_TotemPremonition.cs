using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace tsoa.totems;

public class Building_TotemPremonition : Building_AnimusTotem
{
    internal static FloatRange premonitionRange = new FloatRange(7500, 15000); // 3-6 hours

    public override void Tick()
    {
        if (!compRefuelable.HasFuel)
        {
            gameComp.activePremonition.Remove(this.Map);
        }

        base.Tick();
    }

    public override void DoTotemEffect()
    {
        gameComp.activePremonition[this.Map] = this;
    }

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        gameComp.activePremonition.Remove(this.Map);
        base.Destroy(mode);
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        gameComp.activePremonition.Remove(this.Map);
        base.DeSpawn(mode);
    }
}

public class Premonition : IExposable
{
    private const int delayTicks = 2500; // 1 hour

    public Premonition(FiringIncident incident, int delayTicks)
    {
        this.incident = incident;
        capturedTick = Find.TickManager.TicksGame;
        fireTick = capturedTick + delayTicks;
    }

    private FiringIncident incident;
    public FiringIncident Incident => incident;

    private int capturedTick = -1;
    private int fireTick = -1;
    public int FireTick => fireTick;

    private int timeToLive = 3;

    // Gets 3 extra chances to fire. If it can't fire after 1+3 tries, dies.
    public bool Delay()
    {
        if (timeToLive > 0)
        {
            fireTick += delayTicks;
            timeToLive--;
            return true;
        }
        return false;
    }

    public void ExposeData()
    {
        Scribe_Deep.Look(ref incident, "incident");
        Scribe_Values.Look(ref fireTick, "fireTick", -1);
        Scribe_Values.Look(ref capturedTick, "capturedTick", -1);
        Scribe_Values.Look(ref timeToLive, "timeToLive", 0);
    }
}

[HarmonyPatch(typeof(IncidentWorker_RaidEnemy), nameof(IncidentWorker_RaidEnemy.TryExecuteWorker))]
public static class Harmony_TotemPremonition
{
    public static bool Prefix(IncidentWorker_Raid __instance, IncidentParms parms, ref bool __result)
    {
        if (__instance?.def == null)
            return true;

        if (parms?.target is not Map map)
            return true;

        GameComponent_TotemTracker gameComp = GameComponent_TotemTracker.Instance;

        if (!gameComp.activePremonition.ContainsKey(map))
            return true;

        // Don't do premonition for quest-related raids, so as not to break quests
        if (!parms.questTag.NullOrEmpty())
            return true;

        int delayTicks = (int)Building_TotemPremonition.premonitionRange.RandomInRange;

        gameComp.premonitions.Add(new Premonition(new FiringIncident(__instance.def, null, parms), delayTicks));

        SendPremonitionLetter(map, delayTicks);

        // result is true so it treats it as if the raid successfully spawned
        __result = true;
        return false;
    }

    private static void SendPremonitionLetter(Map map, int delayTicks)
    {
        float hoursF = delayTicks / (float)GenDate.TicksPerHour;
        int hours = Mathf.Clamp(Mathf.RoundToInt(hoursF), 1, 24);

        string label = "TSOA_PremonitionLabel".Translate();
        string text = "TSOA_PremonitionDescription".Translate();

        Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.ThreatBig, new TargetInfo(map.Center, map));
    }
}