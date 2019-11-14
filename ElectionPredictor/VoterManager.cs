using ElectionPredictor.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ElectionPredictor
{
    public class VoterManager
    {
        public IList<Voter> Voters = new List<Voter>();

        private bool votersGenerated = false;
        private string location;

        public void GenerateVotersNationally(int numberOfVoters)
        {
            if (votersGenerated)
                throw new InvalidOperationException("Voters already generated for this Voter Manager");

            votersGenerated = true;
            location = "National";

            GenerateBlankVotersInRegions(numberOfVoters);

            ApplyGender(LoadNationalGenderMalePercentage());
            
            ApplyAgeGroups(LoadNationalAgeGroups());

            ApplySocialGroupsByRegion(LoadPercentageABCSocialGradeByRegion());

            ApplyEURefByRegion(LoadReferendumResultPercentageLeaveByRegion());

            ApplyPreviousVoteByRegion(LoadPreviousVotePercentagesByRegion());
        }

        public void GenerateVotersForConstituency(Constituency constituency, int numberOfVoters)
        {
            if (votersGenerated)
                throw new InvalidOperationException("Voters already generated for this Voter Manager");

            votersGenerated = true;
            location = constituency.Name;

            GenerateBlankVotersInRegion(constituency.Region, numberOfVoters);

            ApplyGender(LoadNationalGenderMalePercentage());

            ApplyAgeGroups(constituency.Ages);

            ApplySocialGroups(constituency.SocialGrades[SocialGrade.ABC1]);

            ApplyEURef(constituency.ReferendumResults[ReferendumResult.Leave]);

            ApplyPreviousVote(constituency.PreviousVote);

        }

        internal void OutputConstituencyVotingIntention()
        {
            Dictionary<Party, int> intensions = new Dictionary<Party, int>();

            foreach (var v in Voters)
            {
                if (!intensions.ContainsKey(v.IntentionEnum.Value))
                {
                    intensions.Add(v.IntentionEnum.Value, 0);
                }

                intensions[v.IntentionEnum.Value] += 1;
            }

            var winner = intensions.OrderByDescending(party => party.Value).First().Key;

            Console.WriteLine($"Constituency: {location}. Winner: {winner.ToString()}");
            foreach (var party in intensions.Keys)
            {
                var count = 100d * intensions[party] / Voters.Count;
                Console.WriteLine($"Party: {party}. %: {count}");
            }

            Console.WriteLine();
        }

        internal void OutputNationalVotingIntention()
        {
            Dictionary<Party, int> intensions = new Dictionary<Party, int>();
            Dictionary<Region, Dictionary<Party, int>> regionalIntentions = new Dictionary<Region, Dictionary<Party, int>>();
            Dictionary<Region, int> regionCount = new Dictionary<Region, int>();

            foreach (var v in Voters)
            {
                if (!intensions.ContainsKey(v.IntentionEnum.Value))
                {
                    intensions.Add(v.IntentionEnum.Value, 0);
                }

                if (!regionalIntentions.ContainsKey(v.RegionEnum))
                {
                    regionalIntentions.Add(v.RegionEnum, new Dictionary<Party, int>());
                }

                if (!regionalIntentions[v.RegionEnum].ContainsKey(v.IntentionEnum.Value))
                {
                    regionalIntentions[v.RegionEnum].Add(v.IntentionEnum.Value, 0);
                }

                if (!regionCount.ContainsKey(v.RegionEnum))
                {
                    regionCount.Add(v.RegionEnum, 0);
                }

                intensions[v.IntentionEnum.Value] += 1;
                regionalIntentions[v.RegionEnum][v.IntentionEnum.Value] += 1;
                regionCount[v.RegionEnum] += 1;

            }

            Console.WriteLine("National Intention");
            foreach (var party in intensions.Keys)
            {
                var count = 100d * intensions[party] / Voters.Count;
                Console.WriteLine($"Party: {party}. %: {count}");
            }

            Console.WriteLine("################");
            Console.WriteLine();
            Console.WriteLine("Regional Breakdown");
            Console.WriteLine("################");

            foreach (var region in regionalIntentions.Keys)
            {
                Console.WriteLine($"Region: {region}");
                foreach (var party in regionalIntentions[region].Keys)
                {
                    var count = 100d * regionalIntentions[region][party] / regionCount[region];
                    Console.WriteLine($"Party: {party}. %: {count}");
                }
                Console.WriteLine();
            }
        }

        internal void GenerateLikelyVotingIntension(ProbabiliesManager probabilies)
        {
            foreach (var v in Voters)
            {
                v.IntentionEnum = probabilies.GetLikeliestParty(v.PreviousVoteEnum.Value, v.AgeGroupEnum.Value, v.GenderEnum.Value, v.ReferendumResultEnum.Value, v.RegionEnum, v.SocialGradeEnum.Value);
            }
        }

        private double LoadNationalGenderMalePercentage()
        {
            Dictionary<Gender, double> genderPopulatations = new Dictionary<Gender, double>();

            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Data", "Genders.csv")))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var gender = (Gender)Enum.Parse(typeof(Gender), values[0]);

                    var population = int.Parse(values[1]);

                    genderPopulatations.Add(gender, population);
                }
            }

            var male = genderPopulatations[Gender.Male];
            var total = male + genderPopulatations[Gender.Female];

            var malePercent = 100 * male / total;
            return malePercent;
        }

        private void ApplyGender(double percentMale)
        {
            var rand = new Random();
            foreach (var v in Voters)
            {
                var r = rand.Next(100);

                if (r <= percentMale)
                {
                    v.GenderEnum = Gender.Male;
                }
                else
                {
                    v.GenderEnum = Gender.Female;
                }
            }
        }

        private Dictionary<Region, Dictionary<Party, double>> LoadPreviousVotePercentagesByRegion()
        {
            Dictionary<Region, Dictionary<Party, double>> previousVotePopulations = new Dictionary<Region, Dictionary<Party, double>>();

            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Data", "Vote15ByRegion.csv")))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var region = (Region)Enum.Parse(typeof(Region), values[0]);

                    if (!previousVotePopulations.ContainsKey(region))
                    {
                        previousVotePopulations.Add(region, new Dictionary<Party, double>());
                    }

                    previousVotePopulations[region].Add(Party.Con, double.Parse(values[1]));
                    previousVotePopulations[region].Add(Party.Lab, double.Parse(values[2]));
                    previousVotePopulations[region].Add(Party.SNP, double.Parse(values[3]));
                    previousVotePopulations[region].Add(Party.LibDem, double.Parse(values[4]));
                    previousVotePopulations[region].Add(Party.PlaidCymru, double.Parse(values[5]));
                    previousVotePopulations[region].Add(Party.UKIP, double.Parse(values[6]));
                    previousVotePopulations[region].Add(Party.Green, double.Parse(values[7]));
                    previousVotePopulations[region].Add(Party.Other, double.Parse(values[8]));

                }
            }

            return previousVotePopulations;
        }

        private void ApplyPreviousVoteByRegion(Dictionary<Region, Dictionary<Party, double>> previousVotePopulations)
        {
            Dictionary<Region, Dictionary<Party, double>> previousVotePercentages = new Dictionary<Region, Dictionary<Party, double>>();

            foreach (var region in previousVotePopulations.Keys)
            {
                var thisRegion = previousVotePopulations[region];
                double total = 0;
                foreach (var party in thisRegion.Keys)
                {
                    total += thisRegion[party];
                }

                previousVotePercentages.Add(region, new Dictionary<Party, double>());
                var thisRegionPercentages = previousVotePercentages[region];

                var lastPercentage = 0d;

                foreach (var party in thisRegion.Keys)
                {
                    var thisPercentage = 100 * thisRegion[party] / total;
                    thisRegionPercentages.Add(party, lastPercentage + thisPercentage);
                    lastPercentage = lastPercentage + thisPercentage;
                }
            }

            var rand = new Random();
            foreach (var v in Voters)
            {
                var r = rand.Next(100);

                var thisRegionPercentages = previousVotePercentages[v.RegionEnum];

                bool decidedVote = false;
                foreach (var party in thisRegionPercentages.Keys)
                {
                    if (decidedVote)
                    {
                        break;
                    }

                    if (r <= thisRegionPercentages[party])
                    {
                        v.PreviousVoteEnum = party;
                        decidedVote = true;
                    }
                }

                if (decidedVote == false)
                {
                    throw new Exception("haven't decided how this person voted");
                }
                
            }
        }

        private void ApplyPreviousVote(Dictionary<Party, double> previousVotePopulations)
        {
            Dictionary<Party, double> previousVotePercentages = new Dictionary<Party, double>();

            double total = 0;
            foreach (var party in previousVotePopulations.Keys)
            {
                total += previousVotePopulations[party];
            }

            var lastPercentage = 0d;

            foreach (var party in previousVotePopulations.Keys)
            {
                var thisPercentage = 100 * previousVotePopulations[party] / total;
                previousVotePercentages.Add(party, lastPercentage + thisPercentage);
                lastPercentage = lastPercentage + thisPercentage;
            }

            var rand = new Random();
            foreach (var v in Voters)
            {
                var r = rand.Next(100);

                bool decidedVote = false;
                foreach (var party in previousVotePercentages.Keys)
                {
                    if (decidedVote)
                    {
                        break;
                    }

                    if (r <= previousVotePercentages[party])
                    {
                        v.PreviousVoteEnum = party;
                        decidedVote = true;
                    }
                }

                if (decidedVote == false)
                {
                    throw new Exception("haven't decided how this person voted");
                }

            }
        }

        private Dictionary<Region, double> LoadReferendumResultPercentageLeaveByRegion()
        {
            Dictionary<Region, Dictionary<ReferendumResult, double>> referendumPopulations = new Dictionary<Region, Dictionary<ReferendumResult, double>>();
            Dictionary<Region, double> percentageLeave = new Dictionary<Region, double>();

            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Data", "EURefByRegion.csv")))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var region = (Region)Enum.Parse(typeof(Region), values[0]);
                    var referendumResult = (ReferendumResult)Enum.Parse(typeof(ReferendumResult), values[1]);

                    var population = int.Parse(values[2]);

                    if (!referendumPopulations.ContainsKey(region))
                    {
                        referendumPopulations.Add(region, new Dictionary<ReferendumResult, double>());
                    }

                    referendumPopulations[region].Add(referendumResult, population);
                }
            }

            foreach (var region in referendumPopulations.Keys)
            {
                var thisRegion = referendumPopulations[region];
                var leave = thisRegion[ReferendumResult.Leave];
                var total = leave + thisRegion[ReferendumResult.Remain];

                var percent = 100 * leave / total;
                percentageLeave.Add(region, percent);
            }

            return percentageLeave;
        }

        private void ApplyEURefByRegion(Dictionary<Region, double> percentageLeave)
        { 
            var rand = new Random();
            foreach (var v in Voters)
            {
                var r = rand.Next(100);

                if (r <= percentageLeave[v.RegionEnum])
                {
                    v.ReferendumResultEnum = ReferendumResult.Leave;
                }
                else
                {
                    v.ReferendumResultEnum = ReferendumResult.Remain;
                }
            }
        }

        private void ApplyEURef(double percentageLeave)
        {
            var rand = new Random();
            foreach (var v in Voters)
            {
                var r = rand.Next(100);

                if (r <= percentageLeave)
                {
                    v.ReferendumResultEnum = ReferendumResult.Leave;
                }
                else
                {
                    v.ReferendumResultEnum = ReferendumResult.Remain;
                }
            }
        }

        private Dictionary<Region, double> LoadPercentageABCSocialGradeByRegion()
        {
            Dictionary<Region, Dictionary<SocialGrade, double>> socialGradePopulations = new Dictionary<Region, Dictionary<SocialGrade, double>>();
            Dictionary<Region, double> percentageABC1 = new Dictionary<Region, double>();

            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Data", "SocialGradeByRegion.csv")))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var region = (Region)Enum.Parse(typeof(Region), values[0]);
                    var socialGroup = (SocialGrade)Enum.Parse(typeof(SocialGrade), values[1]);

                    var population = int.Parse(values[2]);

                    if (!socialGradePopulations.ContainsKey(region))
                    {
                        socialGradePopulations.Add(region, new Dictionary<SocialGrade, double>());
                    }

                    socialGradePopulations[region].Add(socialGroup, population);
                }
            }

            foreach (var region in socialGradePopulations.Keys)
            {
                var thisRegion = socialGradePopulations[region];
                var abc1 = thisRegion[SocialGrade.ABC1];
                var total = abc1 + thisRegion[SocialGrade.C2DE];

                var percent = 100 * abc1 / total;
                percentageABC1.Add(region, percent);
            }

            return percentageABC1;
        }

        private void ApplySocialGroupsByRegion(Dictionary<Region, double> percentageABC1)
        {
            var rand = new Random();
            foreach (var v in Voters)
            {
                var r = rand.Next(100);

                if (r <= percentageABC1[v.RegionEnum])
                {
                    v.SocialGradeEnum = SocialGrade.ABC1;
                }
                else
                {
                    v.SocialGradeEnum = SocialGrade.C2DE;
                }
            }
        }

        private void ApplySocialGroups(double percentageABC1)
        {
            var rand = new Random();
            foreach (var v in Voters)
            {
                var r = rand.Next(100);

                if (r <= percentageABC1)
                {
                    v.SocialGradeEnum = SocialGrade.ABC1;
                }
                else
                {
                    v.SocialGradeEnum = SocialGrade.C2DE;
                }
            }
        }

        private Dictionary<AgeGroup, double> LoadNationalAgeGroups()
        {
            Dictionary<AgeGroup, double> ageGroupsPopulations = new Dictionary<AgeGroup, double>();

            int total = 0;

            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Data", "AgeGroups.csv")))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var group = (AgeGroup)Enum.Parse(typeof(AgeGroup), values[0]);
                    var population = int.Parse(values[1]);

                    ageGroupsPopulations.Add(group, population);
                    total += population;
                }
            }

            return ageGroupsPopulations;
        }

        private Dictionary<AgeGroup, double> FindPercentagesOfAgeGroups(Dictionary<AgeGroup, double> ageGroupsPopulations)
        {
            Dictionary<AgeGroup, double> ageGroupsPopulationsTurnout = ApplyTurnout(ageGroupsPopulations);
            Dictionary<AgeGroup, double> ageGroupsPercentages = new Dictionary<AgeGroup, double>();

            var total = ageGroupsPopulationsTurnout.Values.Sum();

            foreach (var group in ageGroupsPopulationsTurnout.Keys)
            {
                ageGroupsPercentages.Add(group, 100 * ageGroupsPopulationsTurnout[group] / total);
            }

            return ageGroupsPercentages;
        }

        private Dictionary<AgeGroup, double> ApplyTurnout(Dictionary<AgeGroup, double> ageGroupsPopulations)
        {
            Dictionary<AgeGroup, double> ageGroupsWithTurnout = new Dictionary<AgeGroup, double>();

            foreach (var group in ageGroupsPopulations.Keys)
            {
                ageGroupsWithTurnout[group] = ageGroupsPopulations[group] * TurnOutByAgeGroup(group);
            }

            return ageGroupsWithTurnout;
        }

        private double TurnOutByAgeGroup(AgeGroup group)
        {
            //return 1;

            switch(group)
            {
                case AgeGroup.A1824:
                    return 0.58;

                case AgeGroup.A2549:
                    return 0.625;

                case AgeGroup.A5064:
                    return 0.74;

                case AgeGroup.A65Plus:
                    return 0.805;

                default:
                    throw new Exception();
            }
        }

        private void ApplyAgeGroups(Dictionary<AgeGroup, double> ageGroupsPopulations)
        {
            var ageGroupsPercentages = FindPercentagesOfAgeGroups(ageGroupsPopulations);

            var a1824 = ageGroupsPercentages[AgeGroup.A1824];
            var a2549 = a1824 + ageGroupsPercentages[AgeGroup.A2549];
            var a5064 = a2549 + ageGroupsPercentages[AgeGroup.A5064];
            var a65Plus = 100;

            var rand = new Random();
            foreach (var v in Voters)
            {
                var r = rand.Next(100);

                if (r <= a1824)
                {
                    v.AgeGroupEnum = AgeGroup.A1824;
                }
                else if (r <= a2549)
                {
                    v.AgeGroupEnum = AgeGroup.A2549;
                }
                else if (r <= a5064)
                {
                    v.AgeGroupEnum = AgeGroup.A5064;
                }
                else if (r <= a65Plus)
                {
                    v.AgeGroupEnum = AgeGroup.A65Plus;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        private void GenerateBlankVotersInRegions(int numberToGenerate)
        {
            Dictionary<Region, double> regionPopulations = new Dictionary<Region, double>();
            Dictionary<Region, double> regionPercentages = new Dictionary<Region, double>();

            int total = 0;

            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Data", "Regions.csv")))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var region = (Region)Enum.Parse(typeof(Region), values[0]);
                    var population = int.Parse(values[1]);

                    regionPopulations.Add(region, population);
                    total += population;
                }
            }
            
            foreach (var region in regionPopulations.Keys)
            {
                regionPercentages.Add(region, Math.Round(numberToGenerate * regionPopulations[region] / total));
            }

            foreach (var region in regionPercentages.Keys)
            {
                for (var n = 0; n < regionPercentages[region]; n++)
                {
                    Voters.Add(new Voter { RegionEnum = region });
                }
            }
        }

        private void GenerateBlankVotersInRegion(Region region, int numberToGenerate)
        {
            for (var n = 0; n < numberToGenerate; n++)
            {
                Voters.Add(new Voter { RegionEnum = region });
            }
        }
    }
}
