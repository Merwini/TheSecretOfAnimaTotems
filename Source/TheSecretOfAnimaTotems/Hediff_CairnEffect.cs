using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace tsoa.totems
{
    public class Hediff_CairnEffect : HediffWithComps
    {
        private List<StatModifier> statOffsets;
        private List<StatModifier> statFactors;

        private HediffStage curStage;
        public override HediffStage CurStage
        {
            get
            {
                if (curStage == null)
                {
                    curStage = new HediffStage();
                    // TODO check if these need to be made null safe
                    curStage.statOffsets = statOffsets;
                    curStage.statFactors = statFactors;
                }
                return curStage;
            }
        }

        public void StoreStatModifiers(List<StatModifier> statOffsets, List<StatModifier> statFactors)
        {
            this.statOffsets = statOffsets;
            this.statFactors = statFactors;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            List<StatDef> statOffsetDefs = null;
            List<float> statOffsetValues = null;
            List<StatDef> statFactorDefs = null;
            List<float> statFactorValues = null;

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                statOffsetDefs = new List<StatDef>();
                statOffsetValues = new List<float>();
                statFactorDefs = new List<StatDef>();
                statFactorValues = new List<float>();

                if (!statOffsets.NullOrEmpty())
                {
                    foreach (StatModifier statModifier in statOffsets)
                    {
                        statOffsetDefs.Add(statModifier.stat);
                        statOffsetValues.Add(statModifier.value);
                    }
                }
                if (!statFactors.NullOrEmpty())
                {
                    foreach (StatModifier statModifier in statFactors)
                    {
                        statFactorDefs.Add(statModifier.stat);
                        statFactorValues.Add(statModifier.value);
                    }
                }
            }

            Scribe_Collections.Look(ref statOffsetDefs, "statOffsetDefs", LookMode.Def);
            Scribe_Collections.Look(ref statOffsetValues, "statOffsetValues", LookMode.Value);
            Scribe_Collections.Look(ref statFactorDefs, "statFactorDefs", LookMode.Def);
            Scribe_Collections.Look(ref statFactorValues, "statFactorValues", LookMode.Value);

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                if (!statOffsetDefs.NullOrEmpty())
                {
                    if (statOffsets == null)
                    {
                        statOffsets = new List<StatModifier>();
                    }

                    for (int i = 0; i < statOffsetDefs.Count; i++)
                    {
                        StatModifier sm = new StatModifier();
                        sm.stat = statOffsetDefs[i];
                        sm.value = statOffsetValues[i];
                        statOffsets.Add(sm);
                    }
                }
                if (!statFactorDefs.NullOrEmpty())
                {
                    if (statFactors == null)
                    {
                        statFactors = new List<StatModifier>();
                    }

                    for (int i = 0; i < statFactorDefs.Count; i++)
                    {
                        StatModifier sm = new StatModifier();
                        sm.stat = statFactorDefs[i];
                        sm.value = statFactorValues[i];
                        statFactors.Add(sm);
                    }
                }

                curStage = null;
            }
        }
    }
}
