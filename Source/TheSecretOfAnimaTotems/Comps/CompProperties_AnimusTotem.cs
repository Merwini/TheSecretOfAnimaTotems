using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tsoa.core;
using Verse;

namespace tsoa.totems;

public class CompProperties_AnimusTotem : CompProperties
{
    public float progressConsumption = 0.5f;

    public CompProperties_AnimusTotem()
    {
        this.compClass = typeof(CompAnimusTotem);
    }
}
