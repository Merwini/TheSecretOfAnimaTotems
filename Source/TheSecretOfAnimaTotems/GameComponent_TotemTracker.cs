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

    public HashSet<Building_TotemMemory> activeMemory;
    public HashSet<Building_TotemTemper> activeTemper;
    public HashSet<Building_TotemBloodthirst> activeBloodthirst;
    public Dictionary<Map, HashSet<Building_TotemBounty>> activeBounty;
    public Dictionary<Map, HashSet<Building_TotemVersatility>> activeVersatility;

    public GameComponent_TotemTracker(Game game)
    {
    }

    public override void GameComponentTick()
    {
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
        activeBloodthirst ??= new HashSet<Building_TotemBloodthirst>();

        activeBounty ??= new Dictionary<Map, HashSet<Building_TotemBounty>>();
        activeVersatility ??= new Dictionary<Map, HashSet<Building_TotemVersatility>>();

        activeMemory.RemoveWhere(t => t == null || t.Destroyed || !t.Spawned);
        activeTemper.RemoveWhere(t => t == null || t.Destroyed || !t.Spawned);
        activeBloodthirst.RemoveWhere(t => t == null || t.Destroyed || !t.Spawned);

        CleanupBounty();
        CleanupVersatility();
    }

    public void CleanupBounty()
    {
        if (activeBounty.Count > 0)
        {
            List<Map> mapsToRemove = new List<Map>();

            foreach (var kvp in activeBounty)
            {
                Map map = kvp.Key;
                HashSet<Building_TotemBounty> buildings = kvp.Value;

                if (map == null || buildings == null || buildings.Count == 0)
                {
                    mapsToRemove.Add(map);
                    continue;
                }

                HashSet<Building_TotemBounty> buildingsToRemove = new HashSet<Building_TotemBounty>();
                foreach (Building_TotemBounty building in buildings)
                {
                    if (building == null || building.Destroyed || !building.Spawned || building.Map != map)
                    {
                        buildingsToRemove.Add(building);
                    }
                }
                if (buildingsToRemove.Count > 0)
                {
                    foreach (Building_TotemBounty building in buildingsToRemove)
                        buildings.Remove(building);
                }

                if (buildings.Count == 0)
                {
                    mapsToRemove.Add(map);
                }
            }
            if (mapsToRemove.Count > 0)
            {
                for (int i = 0; i < mapsToRemove.Count; i++)
                    activeBounty.Remove(mapsToRemove[i]);
            }
        }
    }

    public void CleanupVersatility()
    {
        if (activeVersatility.Count > 0)
        {
            List<Map> mapsToRemove = new List<Map>();

            foreach (var kvp in activeVersatility)
            {
                Map map = kvp.Key;
                HashSet<Building_TotemVersatility> buildings = kvp.Value;

                if (map == null || buildings == null)
                {
                    mapsToRemove.Add(map);
                    continue;
                }

                HashSet<Building_TotemVersatility> buildingsToRemove = new HashSet<Building_TotemVersatility>();
                foreach (Building_TotemVersatility building in buildings)
                {
                    if (building == null || building.Destroyed || !building.Spawned || building.Map != map)
                    {
                        buildingsToRemove.Add(building);
                    }
                }
                if (buildingsToRemove.Count > 0)
                {
                    foreach (Building_TotemVersatility building in buildingsToRemove)
                        buildings.Remove(building);
                }

                if (buildings.Count == 0)
                {
                    mapsToRemove.Add(map);
                }
            }
            if (mapsToRemove.Count > 0)
            {
                for (int i = 0; i < mapsToRemove.Count; i++)
                    activeVersatility.Remove(mapsToRemove[i]);
            }
        }
    }

    public override void ExposeData()
    {
        Scribe_Collections.Look(ref activeMemory, "activeMemory", LookMode.Reference);
        Scribe_Collections.Look(ref activeTemper, "activeTemper", LookMode.Reference);
        Scribe_Collections.Look(ref activeBloodthirst, "activeBloodthirst", LookMode.Reference);

        // TODO fix serialization
        Scribe_Collections.Look(ref activeBounty, "activeBounty", LookMode.Reference, LookMode.Reference);
        Scribe_Collections.Look(ref activeVersatility, "activeVersatility", LookMode.Reference, LookMode.Reference);

        base.ExposeData();
    }
}
