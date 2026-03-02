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
            StatModifier modifier = new StatModifier();
            modifier.stat = extension.statDef;
            modifier.value = extension.value;

            if (extension.isOffset)
            {
                stage.statOffsets.Add(modifier);
            }
            else if (extension.isFactor)
            {
                stage.statFactors.Add(modifier);
            }
        }
    }
}
