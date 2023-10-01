using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RimsentialMeetingMinds.HarmonyPatches
{
    [HarmonyPatch(typeof(FactionDialogMaker), "FactionDialogFor")]
    public static class FactionDialogMaker_FactionDialogFor_Patch
    {
        public static void Postfix(Pawn negotiator, Faction faction, ref DiaNode __result)
        {
            if (__result != null && faction.leader != null && !faction.leader.Dead && !faction.leader.Spawned && !faction.def.permanentEnemy && !faction.def.naturalEnemy)
            {
                DiaOption diaOption = new DiaOption("RMM.InviteForVisit".Translate());
                diaOption.action = delegate()
                {
                    IncidentParms incidentParms = new IncidentParms();
                    incidentParms.target = negotiator.Map;
                    incidentParms.faction = faction;
                    if (faction.HostileTo(negotiator.Faction))
                    {
                        FactionRelation factionRelation = faction.RelationWith(negotiator.Faction);
                        FactionRelationKind kind2 = factionRelation.kind;
                        factionRelation.kind = FactionRelationKind.Neutral;
                        factionRelation.baseGoodwill = 0;
                        faction.Notify_RelationKindChanged(negotiator.Faction, kind2, false, "RMM.DiplomaticVisitUpcoming".Translate(), GlobalTargetInfo.Invalid,
                            out var sentLetter);
                        negotiator.Faction.RelationWith(faction).kind = FactionRelationKind.Neutral;
                        negotiator.Faction.Notify_RelationKindChanged(faction, kind2, false, "RMM.DiplomaticVisitUpcoming".Translate(), GlobalTargetInfo.Invalid, out sentLetter);
                    }

                    var dayCount = Rand.Range(2f, 3f);
                    var period = GenDate.TicksPerDay * dayCount;
                    Find.Storyteller.incidentQueue.Add(RimsentialMeetingMindsDefOf.RMM_DiplomaticVisit, (int)(Find.TickManager.TicksGame + period), incidentParms);
                    Messages.Message("RMM.WillVisit".Translate(faction.leader.Named("LEADER"), faction.Named("FACTION"), (int)dayCount), MessageTypeDefOf.NeutralEvent, true);
                };
                if (Find.Storyteller.incidentQueue.queuedIncidents.Any(x =>
                        x.FiringIncident.def == RimsentialMeetingMindsDefOf.RMM_DiplomaticVisit && x.FiringIncident.parms.faction == faction))
                {
                    diaOption.Disable("RMM.AlreadyVisiting".Translate());
                }

                diaOption.resolveTree = true;
                __result.options.Insert(__result.options.Count - 1, diaOption);
            }
        }
    }
}
