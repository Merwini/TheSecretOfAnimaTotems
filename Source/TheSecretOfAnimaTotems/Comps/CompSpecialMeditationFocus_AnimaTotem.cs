using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tsoa.core;
using UnityEngine;
using Verse;

namespace tsoa.totems
{
    public class CompSpecialMeditationFocus_AnimaTotem : CompSpecialMeditationFocus_Anima
    {
        private const int TicksUntilEffect = 2500;
        private const int ResetPawnProgressAfterTicks = 120;
        private Dictionary<int, MeditationTracker> trackerDict = new Dictionary<int, MeditationTracker>();


        public override void CompTickLong()
        {
            base.CompTickLong();
            CleanDictionary();
        }

        public override void DoMeditationTick(Pawn pawn)
        {
            base.DoMeditationTick(pawn);

            if (pawn == null)
                return;

            if (UpdateTrackerForPawn(pawn))
            {
                ApplyOrReapplyHediffToPawn(pawn);
            }
        }

        private bool UpdateTrackerForPawn(Pawn pawn)
        {
            int id = pawn.thingIDNumber;
            int now = Find.TickManager.TicksGame;
            if (!trackerDict.TryGetValue(id, out var tr))
            {
                tr = new MeditationTracker
                {
                    accumulatedTicks = 0,
                    lastTickSeen = now
                };
                trackerDict[id] = tr;
            }
            else
            {
                if (now - tr.lastTickSeen > ResetPawnProgressAfterTicks)
                {
                    tr.accumulatedTicks = 0;
                }
            }

            tr.lastTickSeen = now;
            tr.accumulatedTicks++;

            return tr.accumulatedTicks >= TicksUntilEffect && !tr.alreadyApplied;
        }

        private void CleanDictionary()
        {
            int now = Find.TickManager.TicksGame;
            if (trackerDict.Count == 0) return;

            List<int> cleaningList = new List<int>();
            foreach (var kvp in trackerDict)
            {
                var tracker = kvp.Value;
                if (now - tracker.lastTickSeen > ResetPawnProgressAfterTicks)
                {
                    cleaningList.Add(kvp.Key);
                }
            }

            foreach (int item in cleaningList)
            {
                trackerDict.Remove(item);
            }
        }

        public void ApplyOrReapplyHediffToPawn(Pawn pawn)
        {
            Hediff_CairnEffect hediff = (Hediff_CairnEffect)HediffMaker.MakeHediff(TSOAT_DefOf.TSOA_CairnHediff, pawn);
            BuildHediffStage(hediff);

            Hediff existing = pawn.health.hediffSet.GetFirstHediffOfDef(TSOAT_DefOf.TSOA_CairnHediff);
            if (existing != null)
            {
                pawn.health.RemoveHediff(existing);
            }

            pawn.health.AddHediff(hediff);
            trackerDict.TryGetValue(pawn.thingIDNumber, out var tr);
            tr.alreadyApplied = true;
        }

        // Is this stupid?
        public void BuildHediffStage(Hediff_CairnEffect hediff)
        {
            List<Thing> linkedFacilities = CachedCompABGF.LinkedFacilities;
            if (linkedFacilities.NullOrEmpty())
                return;

            List<Building_AnimusCairn> cairns = linkedFacilities.OfType<Building_AnimusCairn>().ToList();
            if (cairns.NullOrEmpty())
                return;

            HediffStage hediffStage = new HediffStage();
            hediffStage.statOffsets = new List<StatModifier>();
            hediffStage.statFactors = new List<StatModifier>();

            foreach (Building_AnimusCairn cairn in cairns)
            {
                cairn.ApplyCairnEffect(hediffStage);
            }

            hediff.StoreStatModifiers(hediffStage.statOffsets, hediffStage.statFactors);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Collections.Look(ref trackerDict, "trackerDict", LookMode.Value, LookMode.Deep);
        }

        private class MeditationTracker : IExposable
        {
            public int accumulatedTicks;
            public int lastTickSeen;
            public bool alreadyApplied;

            public void ExposeData()
            {
                Scribe_Values.Look(ref accumulatedTicks, "accumulatedTicks", 0);
                Scribe_Values.Look(ref lastTickSeen, "lastTickSeen", 0);
                Scribe_Values.Look(ref alreadyApplied, "alreadyApplied", false);
            }
        }
    }
}
