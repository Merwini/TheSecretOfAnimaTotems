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
    private float grassConsumption = 0.5f;
    public float GrassConsumption => grassConsumption;

    private CompGroupedFacility compGF;

    private bool Linked => compGF.LinkedThings.Any();

    private Thing linkedTree;

    private CompSpawnSubplant compSP;
    public CompSpawnSubplant CompSpawnSubplant
    {
        get
        {
            if (compSP == null)
            {
                List<Thing> linkedThings = compGF.LinkedThings;
                if (linkedThings.NullOrEmpty())
                    return null;

                for (int i = 0; i < linkedThings.Count; i++)
                {
                    CompSpawnSubplant comp = linkedThings[i].TryGetComp<CompSpawnSubplant>();
                    if (comp != null)
                    {
                        linkedTree = linkedThings[i];
                        compSP = comp;
                        break;
                    }
                }
            }
            return compSP;
        }
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        compGF = GetComp<CompGroupedFacility>();

        if (respawningAfterLoad && Linked)
        {
            CompSpawnSubplant CompSpawnSubplant; // TODO does this to just initialize it?
        }

        base.SpawnSetup(map, respawningAfterLoad);
    }

    public virtual void StartTotemEffect()
    {
    }

    public virtual void EndTotemEffect()
    {
    }

    public override void ExposeData()
    {


        base.ExposeData();
    }
}
