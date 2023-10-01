using HarmonyLib;
using Hospitality;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RimsentialMeetingMinds.HarmonyPatches
{
    [HarmonyPatch(typeof(LordToil_VisitPoint), "Leave")]
    public static class LordToil_VisitPoint_Leave_Patch
    {
        public static Lord curLord;

        public static void Prefix(LordToil_VisitPoint __instance)
        {
            curLord = __instance.lord;
        }

        public static void Postfix(LordToil_VisitPoint __instance)
        {
            curLord = null;
        }
    }

    [HarmonyPatch(typeof(LordToil_VisitPoint), "DisplayLeaveMessage")]
    public static class LordToil_VisitPoint_DisplayLeaveMessage_Patch
    {
        public static void Postfix(float score, Faction faction, int visitorCount, Map currentMap, bool sentAway)
        {
            var targetGoodwill = faction.HasGoodwill ? LordToil_VisitPoint.AffectGoodwill(score, faction, visitorCount) : 25;
            var leaderPresent = LordToil_VisitPoint_Leave_Patch.curLord.ownedPawns.Any(x => x.Faction.leader == x);
            if (leaderPresent)
            {
                if (targetGoodwill >= 90)
                    faction.TryAffectGoodwillWith(Faction.OfPlayer, 50);
                else if (targetGoodwill >= 50)
                    faction.TryAffectGoodwillWith(Faction.OfPlayer, 25);
                else if (targetGoodwill <= -25)
                    faction.TryAffectGoodwillWith(Faction.OfPlayer, -50);
                else if (targetGoodwill <= 5)
                    faction.TryAffectGoodwillWith(Faction.OfPlayer, -25);
            }
        }
    }
}
