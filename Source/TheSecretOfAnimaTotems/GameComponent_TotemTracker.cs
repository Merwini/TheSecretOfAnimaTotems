using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace tsoa.totems;

public class GameComponent_TotemTracker : GameComponent
{
    public static GameComponent_TotemTracker Instance;

    // TotemFriendship doesn't need global tracking
    public HashSet<Building_TotemMemory> activeMemory;
    public HashSet<Building_TotemTemper> activeTemper;
    public Dictionary<Map, Building_TotemBounty> activeBounty;
    public Dictionary<Map, Building_TotemPremonition> activePremonition;

    public List<Premonition> premonitions;

    public GameComponent_TotemTracker(Game game)
    {
    }

    public override void GameComponentTick()
    {
        if (premonitions.Count > 0)
        {
            for (int i = premonitions.Count - 1; i >= 0; i--)
            {
                Premonition premonition = premonitions[i];
                FiringIncident inc = premonition.Incident;
                Map map = (Map)inc.parms.target;

                if (map == null && !map.Parent.Spawned)
                {
                    premonitions.RemoveAt(i);
                }

                if (Find.TickManager.TicksGame >= premonition.FireTick)
                {
                    inc.parms.questTag = "premonition"; // so it doesn't get re-delayed by the Prefix
                    if (Find.Storyteller.TryFire(inc))
                    {
                        premonitions.RemoveAt(i);
                    }
                    else if (!premonition.Delay()) // has failed to fire 4 times, give up
                    {
                        premonitions.RemoveAt(i);
                        Log.Error("Premonition has failed to come true");
                    }
                }
            }
        }

        base.GameComponentTick();
    }

    public override void FinalizeInit()
    {
        Instance = this;

        Cleanup();
        base.FinalizeInit();
    }

    public void Cleanup()
    {
        activeMemory ??= new HashSet<Building_TotemMemory>();
        activeTemper ??= new HashSet<Building_TotemTemper>();
        activeBounty ??= new Dictionary<Map, Building_TotemBounty>();
        activePremonition ??= new Dictionary<Map, Building_TotemPremonition>();

        premonitions ??= new List<Premonition>();

        activeMemory.RemoveWhere(t => t == null || t.Destroyed || !t.Spawned);
        activeTemper.RemoveWhere(t => t == null || t.Destroyed || !t.Spawned);

        if (activeBounty.Count > 0)
        {
            List<Map> keysToRemove = null;

            foreach (var kvp in activeBounty)
            {
                Map map = kvp.Key;
                Building_TotemBounty building = kvp.Value;

                if (map == null || building == null || building.Destroyed || !building.Spawned || building.Map != map)
                {
                    keysToRemove ??= new List<Map>();
                    keysToRemove.Add(map);
                }
            }

            if (keysToRemove != null)
            {
                for (int i = 0; i < keysToRemove.Count; i++)
                    activeBounty.Remove(keysToRemove[i]);
            }
        }

        if (activePremonition.Count > 0)
        {
            List<Map> keysToRemove = null;

            foreach (var kvp in activePremonition)
            {
                Map map = kvp.Key;
                Building_TotemPremonition building = kvp.Value;

                if (map == null || building == null || building.Destroyed || !building.Spawned || building.Map != map)
                {
                    keysToRemove ??= new List<Map>();
                    keysToRemove.Add(map);
                }
            }

            if (keysToRemove != null)
            {
                for (int i = 0; i < keysToRemove.Count; i++)
                    activePremonition.Remove(keysToRemove[i]);
            }
        }
    }

    public override void ExposeData()
    {
        Scribe_Collections.Look(ref activeMemory, "activeMemory", LookMode.Reference);
        Scribe_Collections.Look(ref activeTemper, "activeTemper", LookMode.Reference);
        Scribe_Collections.Look(ref activeBounty, "activeBounty", LookMode.Reference, LookMode.Reference);
        Scribe_Collections.Look(ref activePremonition, "activePremonition", LookMode.Reference, LookMode.Reference);

        Scribe_Collections.Look(ref premonitions, "premonitions", LookMode.Deep);

        base.ExposeData();
    }
}
