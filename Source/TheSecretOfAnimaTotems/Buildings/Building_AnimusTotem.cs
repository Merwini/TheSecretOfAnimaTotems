using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tsoa.core;
using Verse;

namespace tsoa.totems;

public abstract class Building_AnimusTotem : Building
{
    internal GameComponent_TotemTracker gameComp;
    internal CompGroupedFacility compGroupedFacility;
    internal CompRefuelable compRefuelable;

    private bool Linked => compGroupedFacility.LinkedThings.Any();

    private Thing linkedTree;

    // Decided to go with a CompRefuelable instead of taking directly from the tree
    //private CompSpawnSubplant compSP;
    //public CompSpawnSubplant CompSpawnSubplant
    //{
    //    get
    //    {
    //        if (compSP == null)
    //        {
    //            List<Thing> linkedThings = compGF.LinkedThings;
    //            if (linkedThings.NullOrEmpty())
    //                return null;

    //            for (int i = 0; i < linkedThings.Count; i++)
    //            {
    //                CompSpawnSubplant comp = linkedThings[i].TryGetComp<CompSpawnSubplant>();
    //                if (comp != null)
    //                {
    //                    linkedTree = linkedThings[i];
    //                    compSP = comp;
    //                    break;
    //                }
    //            }
    //        }
    //        return compSP;
    //    }
    //}

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        compGroupedFacility = GetComp<CompGroupedFacility>();
        compRefuelable = GetComp<CompRefuelable>();
        gameComp = Current.Game.GetComponent<GameComponent_TotemTracker>();

        base.SpawnSetup(map, respawningAfterLoad);
    }

    protected override void Tick()
    {
        if (compRefuelable.HasFuel)
        {
            DoTotemEffect();
        }

        base.Tick();
    }

    public virtual void StartTotemEffect()
    {
    }

    public virtual void DoTotemEffect()
    {
    }

    public virtual void EndTotemEffect()
    {
    }
}
