using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;

namespace tsoa.totems;

[StaticConstructorOnStartup]
public class HarmonyPatches
{
    static HarmonyPatches()
    {
        Harmony harmony = new Harmony("tsoa.totems");
        harmony.PatchAll();
    }
}
