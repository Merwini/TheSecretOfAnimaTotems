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
    public Dictionary<Map, HashSet<Building_TotemBounty>> activeBounty;
    //public Dictionary<Map, Building_TotemPremonition> activePremonition;
    public HashSet<Building_TotemBloodthirst> activeBloodthirst;
    //public HashSet<Building_TotemFortune> activeFortune;
    public Dictionary<Map, HashSet<Building_TotemReflex>> activeReflex;

    //public List<Premonition> premonitions;

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
        activeBounty ??= new Dictionary<Map, HashSet<Building_TotemBounty>>();
        activeBloodthirst ??= new HashSet<Building_TotemBloodthirst>();
        activeReflex ??= new Dictionary<Map, HashSet<Building_TotemReflex>>();

        activeMemory.RemoveWhere(t => t == null || t.Destroyed || !t.Spawned);
        activeTemper.RemoveWhere(t => t == null || t.Destroyed || !t.Spawned);
        activeBloodthirst.RemoveWhere(t => t == null || t.Destroyed || !t.Spawned);
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

                if (map == null || buildings == null)
                {
                    mapsToRemove.Add(map);
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
            }
            if (mapsToRemove.Count > 0)
            {
                for (int i = 0; i < mapsToRemove.Count; i++)
                    activeBounty.Remove(mapsToRemove[i]);
            }
        }
    }

    public void CleanupReflex()
    {
        if (activeReflex.Count > 0)
        {
            List<Map> keysToRemove = new List<Map>();

            foreach (var kvp in activeReflex)
            {
                Map map = kvp.Key;
                HashSet<Building_TotemReflex> buildings = kvp.Value;

                if (map == null || buildings == null || buildings.Count == 0)
                {
                    keysToRemove.Add(map);
                }

                HashSet<Building_TotemReflex> buildingsToRemove = new HashSet<Building_TotemReflex>();
                foreach (Building_TotemReflex building in buildings)
                {
                    if (building == null || building.Destroyed || !building.Spawned || building.Map != map)
                    {
                        buildingsToRemove.Add(building);
                    }
                }
                if (buildingsToRemove.Count > 0)
                {
                    foreach (Building_TotemReflex building in buildingsToRemove)
                        buildings.Remove(building);
                }
            }

            if (keysToRemove.Count > 0)
            {
                for (int i = 0; i < keysToRemove.Count; i++)
                    activeReflex.Remove(keysToRemove[i]);
            }
        }
    }


    public override void ExposeData()
    {
        Scribe_Collections.Look(ref activeMemory, "activeMemory", LookMode.Reference);
        Scribe_Collections.Look(ref activeTemper, "activeTemper", LookMode.Reference);
        Scribe_Collections.Look(ref activeBounty, "activeBounty", LookMode.Reference, LookMode.Reference);
        Scribe_Collections.Look(ref activeBloodthirst, "activeBloodthirst", LookMode.Reference);
        Scribe_Collections.Look(ref activeReflex, "activeReflex", LookMode.Reference);

        base.ExposeData();
    }
}
