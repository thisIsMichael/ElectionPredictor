using System;
using System.Collections.Generic;
using System.Text;

namespace ElectionPredictor.Entities
{
    public class ConstituencyProfile
    {
        public string Name;

        public Dictionary<AgeGroup, double> Ages;

        public Dictionary<ReferendumResult, double> ReferendumResults;

        public Dictionary<Party, double> PreviousVote;

        public Region Region;

        public Dictionary<SocialGrade, double> SocialGrades;

        public static ConstituencyProfile CanterburyExample
        {
            get
            {
                return new ConstituencyProfile
                {
                    Name = "Canterbury",
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

        public static ConstituencyProfile BatterseaExample
        {
            get
            {
                return new ConstituencyProfile
                {
                    Name = "Battersea",
                    Ages = new Dictionary<AgeGroup, double>()
                    {
                        { AgeGroup.A1824, 8990 },
                        {AgeGroup.A2549, 63006 },
                        { AgeGroup.A5064, 14817 },
                        { AgeGroup.A65Plus, 9985 }
                    },
                    ReferendumResults = new Dictionary<ReferendumResult, double>()
                    {
                        { ReferendumResult.Leave, 23 },
                        {ReferendumResult.Remain, 77 }
                    },
                    PreviousVote = new Dictionary<Party, double>()
                    {
                        { Party.Con, 52.4 },
                        { Party.Lab, 36.8    },
                        { Party.UKIP, 3.1 },
                        { Party.LibDem, 4.4 },
                        { Party.Green, 3.3 }
                    },
                    Region = Region.London,
                    SocialGrades = new Dictionary<SocialGrade, double>()
                    {
                        { SocialGrade.ABC1, 75.3 },
                        { SocialGrade.C2DE, 24.7 }
                    }
                };
            }
        }

        public static ConstituencyProfile MorayExample
        {
            get
            {
                return new ConstituencyProfile
                {
                    Name = "Moray",
                    Ages = new Dictionary<AgeGroup, double>()
                    {
                        { AgeGroup.A1824, 7246 },
                        {AgeGroup.A2549, 28955 },
                        { AgeGroup.A5064, 20584 },
                        { AgeGroup.A65Plus, 20423 }
                    },
                    ReferendumResults = new Dictionary<ReferendumResult, double>()
                    {
                        { ReferendumResult.Leave, 49.9 },
                        {ReferendumResult.Remain, 50.1 }
                    },
                    PreviousVote = new Dictionary<Party, double>()
                    {
                        { Party.Con, 31.1 },
                        { Party.Lab, 9.9    },
                        { Party.UKIP, 3.9 },
                        { Party.LibDem, 2.8 },
                        { Party.Green, 2.7 },
                        { Party.SNP, 49.5 }
                    },
                    Region = Region.Scotland,
                    SocialGrades = new Dictionary<SocialGrade, double>()
                    {
                        { SocialGrade.ABC1, 43.6 },
                        { SocialGrade.C2DE, 56.3 }
                    }
                };
            }
        }

        public static ConstituencyProfile NuneatonExample
        {
            get
            {
                return new ConstituencyProfile
                {
                    Name = "Nuneaton",
                    Ages = new Dictionary<AgeGroup, double>()
                    {
                        { AgeGroup.A1824, 7191 },
                        {AgeGroup.A2549, 30608 },
                        { AgeGroup.A5064, 18738 },
                        { AgeGroup.A65Plus, 17348 }
                    },
                    ReferendumResults = new Dictionary<ReferendumResult, double>()
                    {
                        { ReferendumResult.Leave, 66 },
                        {ReferendumResult.Remain, 34 }
                    },
                    PreviousVote = new Dictionary<Party, double>()
                    {
                        { Party.Con, 45.5 },
                        { Party.Lab, 34.9 },
                        { Party.UKIP, 14.4 },
                        { Party.LibDem, 1.8 },
                        { Party.Green, 2.8 },
                        { Party.Other, 0.6 }
                    },
                    Region = Region.MidlandsWales,
                    SocialGrades = new Dictionary<SocialGrade, double>()
                    {
                        { SocialGrade.ABC1, 43.5 },
                        { SocialGrade.C2DE, 56.5 }
                    }
                };
            }
        }

        public static ConstituencyProfile PeterboroughExample
        {
            get
            {
                return new ConstituencyProfile
                {
                    Name = "Peterborough",
                    Ages = new Dictionary<AgeGroup, double>()
                    {
                        { AgeGroup.A1824, 9367 },
                        {AgeGroup.A2549, 43917 },
                        { AgeGroup.A5064, 20867 },
                        { AgeGroup.A65Plus, 18520 }
                    },
                    ReferendumResults = new Dictionary<ReferendumResult, double>()
                    {
                        { ReferendumResult.Leave, 60.9 },
                        {ReferendumResult.Remain, 39.1 }
                    },
                    PreviousVote = new Dictionary<Party, double>()
                    {
                        { Party.Con, 39.7 },
                        { Party.Lab, 35.6 },
                        { Party.UKIP, 15.9 },
                        { Party.LibDem, 3.8 },
                        { Party.Green, 2.6 },
                        { Party.Other, 2.5 }
                    },
                    Region = Region.MidlandsWales,
                    SocialGrades = new Dictionary<SocialGrade, double>()
                    {
                        { SocialGrade.ABC1, 46.3 },
                        { SocialGrade.C2DE, 53.7 }
                    }
                };
            }
        }

        public static ConstituencyProfile SouthamptonItchenExample
        {
            get
            {
                return new ConstituencyProfile
                {
                    Name = "Southampton Itchen",
                    Ages = new Dictionary<AgeGroup, double>()
                    {
                        { AgeGroup.A1824, 16289 },
                        {AgeGroup.A2549, 37723 },
                        { AgeGroup.A5064, 17872 },
                        { AgeGroup.A65Plus, 15811 }
                    },
                    ReferendumResults = new Dictionary<ReferendumResult, double>()
                    {
                        { ReferendumResult.Leave, 53.8 },
                        {ReferendumResult.Remain, 46.2 }
                    },
                    PreviousVote = new Dictionary<Party, double>()
                    {
                        { Party.Con, 41.7 },
                        { Party.Lab, 36.5 },
                        { Party.UKIP, 13.4 },
                        { Party.LibDem, 3.6 },
                        { Party.Green, 4.2 },
                        { Party.Other, 0.5 }
                    },
                    Region = Region.South,
                    SocialGrades = new Dictionary<SocialGrade, double>()
                    {
                        { SocialGrade.ABC1, 51.5 },
                        { SocialGrade.C2DE, 48.5 }
                    }
                };
            }
        }
    }
}
