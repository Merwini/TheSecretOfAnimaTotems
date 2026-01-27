using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using tsoa.core;

namespace tsoa.totems;

public abstract class CompAnimusTotem : ThingComp
{
    private CompGroupedFacility compGF;
    public CompGroupedFacility CompGF
    {
        get
        {
            if (compGF == null)
            {
                compGF = parent.GetComp<CompGroupedFacility>();
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
}
