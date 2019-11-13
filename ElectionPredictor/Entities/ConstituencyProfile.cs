using System;
using System.Collections.Generic;
using System.Text;

namespace ElectionPredictor.Entities
{
    public class ConstituencyProfile
    {
        Dictionary<AgeGroup, double> Ages;

        Dictionary<ReferendumResult, double> ReferendumResults;

        Dictionary<Party, double> PreviousVote;

        Region Region;

        Dictionary<SocialGrade, double> SocialGrades;

        public static ConstituencyProfile CanterburyExample()
        {
            return new ConstituencyProfile
            {
                Ages = new Dictionary<AgeGroup, double>()
                {
                    { AgeGroup.A1824, 23839 },
                    {AgeGroup.A2549, 33701 },
                    { AgeGroup.A5064, 19403 },
                    { AgeGroup.A65Plus, 22434 }
                },
                ReferendumResults = new Dictionary<ReferendumResult, double>()
                {
                    { ReferendumResult.Leave, 51 },
                    {ReferendumResult.Remain, 49 }
                },
                PreviousVote = new Dictionary<Party, double>()
                {
                    { Party.Con, 42.9 },
                    { Party.Lab, 24.5 },
                    { Party.UKIP, 13.6 },
                    { Party.LibDem, 11.6 },
                    { Party.Green, 7.0 },
                    { Party.Other, 0.3 }
                },
                Region = Region.South,
                SocialGrades = new Dictionary<SocialGrade, double>()
                {
                    { SocialGrade.ABC1, 59.6 },
                    { SocialGrade.C2DE, 40.4 }
                }
            };
        }
    }
}
