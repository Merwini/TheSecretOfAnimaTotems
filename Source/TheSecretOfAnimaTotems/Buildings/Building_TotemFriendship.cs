using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;

namespace tsoa.totems;

public class Building_TotemFriendship
{
    static Dictionary<Building_TotemFriendship, int> activeTotems = new Dictionary<Building_TotemFriendship, int>();

    static int GoodwillOffset
    {
        get
        {
            int num = 0;

            if (activeTotems.NullOrEmpty())
                return num;

            foreach (var kvp in activeTotems)
            {
                num = Math.Max(num, kvp.Value);
            }

            return num;
        }
    }

    [HarmonyPatch(typeof(GoodwillSituationManager), nameof(GoodwillSituationManager.GetNaturalGoodwill))]
    static class Harmony_FriendShip
    {
        public static void Postfix(Faction other, ref int __result)
        {
            if (other.def.permanentEnemy)
                return;

            __result = Math.Clamp(__result + GoodwillOffset, -100, 100);
        }
    }
}
