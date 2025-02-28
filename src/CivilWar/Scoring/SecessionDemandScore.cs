﻿using Diplomacy.CivilWar.Factions;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace Diplomacy.CivilWar
{
    class SecessionDemandScore : AbstractFactionDemandScoringModel
    {
        public float FiefDeficit => 10;

        private static readonly TextObject _TFiefDeficit = new TextObject(StringConstants.NotEnoughFiefs);
        private static readonly TextObject _TCalculating = new TextObject("{=Jc1mCVuY}Trait: Calculating");
        private static readonly TextObject _TClanTier = new TextObject("{=cjbVV7E3}Clan Tier Too Low");

        public SecessionDemandScore() : base(new SecessionScores()) { }

        protected override List<RebelFaction> GetPossibleFactions(Clan clan)
        {
            return new List<RebelFaction>() { new SecessionFaction(clan) };
        }

        protected override IEnumerable<Tuple<TextObject, float>> GetMemberScore(Clan clan, RebelFaction rebelFaction)
        {
            var generosityMultiplier = Math.Abs(clan.Leader.GetTraitLevel(DefaultTraits.Generosity) - DefaultTraits.Generosity.MaxValue);
            var scorePerFiefDeficit = generosityMultiplier * FiefDeficit;
            // happy with fiefs
            float fiefDeficit = (clan.Tier - 1 - clan.Fiefs.Count);
            if (fiefDeficit > 0)
            {
                yield return new Tuple<TextObject, float> (_TFiefDeficit, fiefDeficit * scorePerFiefDeficit);
            }
        }

        protected override IEnumerable<Tuple<TextObject, float>> GetLeaderOnlyScore(Clan clan, RebelFaction rebelFaction)
        {
            // ambition of faction sponsor
            yield return new Tuple<TextObject, float>(_TCalculating ,clan.Leader.GetTraitLevel(DefaultTraits.Calculating) * 20);

            // must be clan tier 4+
            if (clan.Tier < 4)
            {
                yield return new Tuple<TextObject, float>(_TClanTier, -999f);
            }
        }

        protected override IEnumerable<Tuple<TextObject, float>> GetMemberOnlyScore(Clan clan, RebelFaction rebelFaction)
        {
            yield break;
        }

        public class SecessionScores : IFactionDemandScores
        {
            public float Base => 10;
            public float NegativeLeaderRelationshipWeight => 1;
            public float KingdomSizeScoreMax => 50;
            public float KingdomSizeCastleScore => 1;
            public float KingdomSizeTownScore => 2;
        }
    }
}
