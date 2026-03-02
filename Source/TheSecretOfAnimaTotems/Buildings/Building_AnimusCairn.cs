using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace tsoa.totems
{
    public class Building_AnimusCairn : Building
    {
        private CairnEffectExtension effectExtension;
        public CairnEffectExtension EffectExtension
        {
            get
            {
                if (effectExtension == null)
                {
                    CairnEffectExtension extension = def.GetModExtension<CairnEffectExtension>();
                    if (extension == null)
                    {
                        Log.Error($"Building_AnimusCairn of def {def.defName} from {def.modContentPack} has no CairnEffectExtension");
                    }
                    else
                    {
                        effectExtension = extension;
                    }
                }
                return effectExtension;
            }
        }

        public void ApplyCairnEffect(HediffStage stage)
        {
            CairnEffectExtension extension = EffectExtension;

            if (!extension.statOffsets.NullOrEmpty())
            {
                foreach (StatModifier modifier in extension.statOffsets)
                {
                    stage.statOffsets.Add(modifier);
                }
            }

            if (!extension.statFactors.NullOrEmpty())
            {
                foreach (StatModifier modifier in extension.statFactors)
                {
                    stage.statFactors.Add(modifier);
                }
            }
        }
    }
}
