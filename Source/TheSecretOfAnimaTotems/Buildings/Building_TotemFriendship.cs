using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;

namespace tsoa.totems;

public class Building_TotemFriendship : Building_AnimusTotem
{
    List<Faction> affectableFactions;
    private const int goodwillTicks = 30000; // half a day
    private const int tickRate = 250; // rare
    private const int maxGoodwillAboveNatural = 50; // arbitrary, TODO balance
    private int ticksToNextGoodwill = goodwillTicks;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        affectableFactions = new List<Faction>();

        foreach (Faction faction in Find.FactionManager.AllFactionsListForReading.Where(f => !f.def.permanentEnemy))
        {
            affectableFactions.Add(faction);
        }

        base.SpawnSetup(map, respawningAfterLoad);
    }

    public override void TickRare()
    {
        if (ticksToNextGoodwill > 0)
        {
            ticksToNextGoodwill -= tickRate;
        }
        else
        {
            for (int i = 0; i < affectableFactions.Count; i++)
            {
                Faction faction = affectableFactions[i];
                Faction player = Faction.OfPlayer;
                if (faction.GoodwillWith(player) < faction.NaturalGoodwill + maxGoodwillAboveNatural)
                {
                    faction.TryAffectGoodwillWith(Faction.OfPlayer, 1);
                }
            }

            ticksToNextGoodwill = goodwillTicks;
        }

        base.TickRare();
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref ticksToNextGoodwill, "ticksToNextGoodwill", goodwillTicks);

        base.ExposeData();
    }
}
