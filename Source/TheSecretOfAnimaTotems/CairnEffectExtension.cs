using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace tsoa.totems;

public class CairnEffectExtension : DefModExtension
{
    public bool isOffset;
    public bool isFactor;

    public List<StatModifier> modifiers;

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (var error in base.ConfigErrors())
            yield return error;

        if ((!isOffset && !isFactor) || (isOffset && isFactor))
        {
            yield return "Config error in CairnEffectExtension. Must either be a stat offset or a stat factor";
        }
    }
}
