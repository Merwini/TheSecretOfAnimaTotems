using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using tsoa.core;

namespace tsoa.totems
{
    public class CompProperties_SpecialMeditationFocus_AnimaTotem : CompProperties_SpecialMeditationFocus_Anima
    {
        public CompProperties_SpecialMeditationFocus_AnimaTotem()
        {
            this.compClass = typeof(CompSpecialMeditationFocus_AnimaTotem);
        }
    }
}
