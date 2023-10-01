using System.Collections.Generic;
using RimWorld;
using Verse;
using IncidentWorker_VisitorGroup = Hospitality.IncidentWorker_VisitorGroup;

namespace RimsentialMeetingMinds
{
    public class IncidentWorker_DiplomaticVisit : IncidentWorker_VisitorGroup
    {
        public override IEnumerable<Pawn> GenerateNewPawns(IncidentParms parms, int preferredAmount)
        {
            foreach (var pawn in base.GenerateNewPawns(parms, preferredAmount))
            {
                yield return pawn;
            }

            yield return parms.faction.leader;
        }
    }
}
