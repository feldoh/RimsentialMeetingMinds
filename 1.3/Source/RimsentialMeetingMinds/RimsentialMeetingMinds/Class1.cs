using HarmonyLib;
using Hospitality;
using RimWorld;
using RimWorld.Planet;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace RimsentialMeetingMinds
{
	[StaticConstructorOnStartup]
	public static class Core
	{
		static Core()
		{
			new Harmony("RimsentialMeetingMinds.Mod").PatchAll();
		}
	}

	[RimWorld.DefOf]
	public static class RimsentialMeetingMindsDefOf
	{
		public static IncidentDef RMM_DiplomaticVisit;
	}

	[HarmonyPatch(typeof(FactionDialogMaker), "FactionDialogFor")]
	public static class FactionDialogMaker_FactionDialogFor_Patch
	{
		public static void Postfix(Pawn negotiator, Faction faction, ref DiaNode __result)
		{
			if (__result != null && faction.leader != null && !faction.leader.Dead && !faction.leader.Spawned && !faction.def.permanentEnemy && !faction.def.naturalEnemy)
			{
				DiaOption diaOption = new DiaOption("RMM.InviteForVisit".Translate());
				diaOption.action = delegate ()
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
						faction.Notify_RelationKindChanged(negotiator.Faction, kind2, false, "RMM.DiplomaticVisitUpcoming".Translate(), GlobalTargetInfo.Invalid, out var sentLetter);
						negotiator.Faction.RelationWith(faction).kind = FactionRelationKind.Neutral;
						negotiator.Faction.Notify_RelationKindChanged(faction, kind2, false, "RMM.DiplomaticVisitUpcoming".Translate(), GlobalTargetInfo.Invalid, out sentLetter);
					}
					var dayCount = Rand.Range(2f, 3f);
					var period = GenDate.TicksPerDay * dayCount;
					Find.Storyteller.incidentQueue.Add(RimsentialMeetingMindsDefOf.RMM_DiplomaticVisit, (int)(Find.TickManager.TicksGame + period), incidentParms);
					Messages.Message("RMM.WillVisit".Translate(faction.leader.Named("LEADER"), faction.Named("FACTION"), (int)dayCount), MessageTypeDefOf.NeutralEvent, true);
				};
				if (Find.Storyteller.incidentQueue.queuedIncidents.Any(x => x.firingInc.def == RimsentialMeetingMindsDefOf.RMM_DiplomaticVisit && x.firingInc.parms.faction == faction))
				{
					diaOption.Disable("RMM.AlreadyVisiting".Translate());
				}
				diaOption.resolveTree = true;
				__result.options.Insert(__result.options.Count - 1, diaOption);
			}
		}
	}
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
