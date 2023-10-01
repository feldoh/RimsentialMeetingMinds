using HarmonyLib;
using Verse;

namespace RimsentialMeetingMinds
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            new Harmony("RimsentialMeetingMinds.Mod").PatchAll();
        }
    }
}
