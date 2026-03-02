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
    public CompGroupedFacility CompGF
    {
        get
        {
            if (compGF == null)
            {
                compGF = GetComp<CompGroupedFacility>();
            }
            return compGF;
        }
    }

    private CompSpawnSubplant compSP;
    public CompSpawnSubplant CompSpawnSubplant
    {
        get
        {
            if (compSP == null)
            {
                List<Thing> linkedThings = CompGF.LinkedThings;
                if (linkedThings.NullOrEmpty())
                    return null;

                for (int i = 0; i < linkedThings.Count; i++)
                {
                    CompSpawnSubplant comp = linkedThings[i].TryGetComp<CompSpawnSubplant>();
                    if (comp != null)
                    {
                        compSP = comp;
                        break;
                    }
                }
            }
            return compSP;
        }
    }

    public abstract void DoTotemEffect();

    public virtual void EndTotemEffect()
    {
    }
}
