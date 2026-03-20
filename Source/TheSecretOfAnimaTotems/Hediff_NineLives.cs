using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace tsoa.totems;

public class Hediff_NineLives : HediffWithComps
{
    int livesRemaining = 9;
    bool IsActive => GameComponent_TotemTracker.Instance.activeFortune.Count != 0;

    public bool UseLife
    {
        get
        {
            if (livesRemaining > 0)
            {
                livesRemaining--;
                return true;
            }

            return false;
        }
    }

    public override string LabelInBrackets => IsActive ? "TSOA_FortuneLabel".Translate(livesRemaining) : "TSOA_FortuneLabelInactive".Translate(livesRemaining);

    public override void ExposeData()
    {
        Scribe_Values.Look(ref livesRemaining, "livesRemaining");

        base.ExposeData();
    }
}
