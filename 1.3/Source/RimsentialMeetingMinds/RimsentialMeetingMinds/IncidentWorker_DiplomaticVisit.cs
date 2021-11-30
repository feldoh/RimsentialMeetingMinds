using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.AI;
using GuestUtility = Hospitality.Utilities.GuestUtility;
using Hospitality;
using Hospitality.Utilities;

namespace RimsentialMeetingMinds
{
    public class IncidentWorker_DiplomaticVisit : Hospitality.IncidentWorker_VisitorGroup
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
