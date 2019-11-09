using ElectionPredictor.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ElectionPredictor
{
    public class VoterManager
    {
        public IList<Voter> Voters = new List<Voter>();

        public void GenerateVoters()
        {
            GenerateBlankVotersInRegions();
            ApplyGender();
            ApplyAgeGroups();
            ApplySocialGroups();
            ApplyEURefByRegion();
            ApplyPreviousVoteByRegion();
        }

        internal void OutputVotingIntention()
        {
            Dictionary<Party, int> intensions = new Dictionary<Party, int>();
            Dictionary<Region, Dictionary<Party, int>> regionalIntentions = new Dictionary<Region, Dictionary<Party, int>>();
            Dictionary<Region, int> regionCount = new Dictionary<Region, int>();

            foreach (var v in Voters)
            {
                if (!intensions.ContainsKey(v.Intention.Value))
                {
                    intensions.Add(v.Intention.Value, 0);
                }

                if (!regionalIntentions.ContainsKey(v.Region))
                {
                    regionalIntentions.Add(v.Region, new Dictionary<Party, int>());
                }

                if (!regionalIntentions[v.Region].ContainsKey(v.Intention.Value))
                {
                    regionalIntentions[v.Region].Add(v.Intention.Value, 0);
                }

                if (!regionCount.ContainsKey(v.Region))
                {
                    regionCount.Add(v.Region, 0);
                }

                intensions[v.Intention.Value] += 1;
                regionalIntentions[v.Region][v.Intention.Value] += 1;
                regionCount[v.Region] += 1;

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
                v.Intention = probabilies.GetLikeliestParty(v.PreviousVote.Value, v.AgeGroup.Value, v.Gender.Value, v.ReferendumResult.Value, v.Region, v.SocialGrade.Value);
            }
        }

        private void ApplyGender()
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

            var rand = new Random();
            foreach (var v in Voters)
            {
                var r = rand.Next(100);

                if (r <= malePercent)
                {
                    v.Gender = Gender.Male;
                }
                else
                {
                    v.Gender = Gender.Female;
                }
            }
        }

        private void ApplyPreviousVoteByRegion()
        {
            Dictionary<Region, Dictionary<Party, double>> previousVotePopulations = new Dictionary<Region, Dictionary<Party, double>>();
            Dictionary<Region, Dictionary<Party, double>> previousVotePercentages = new Dictionary<Region, Dictionary<Party, double>>();

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

                var thisRegionPercentages = previousVotePercentages[v.Region];

                bool decidedVote = false;
                foreach (var party in thisRegionPercentages.Keys)
                {
                    if (decidedVote)
                    {
                        break;
                    }

                    if (r <= thisRegionPercentages[party])
                    {
                        v.PreviousVote = party;
                        decidedVote = true;
                    }
                }

                if (decidedVote == false)
                {
                    throw new Exception("haven't decided how this person voted");
                }
                
            }
        }

        private void ApplyEURefByRegion()
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

            var rand = new Random();
            foreach (var v in Voters)
            {
                var r = rand.Next(100);

                if (r <= percentageLeave[v.Region])
                {
                    v.ReferendumResult = ReferendumResult.Leave;
                }
                else
                {
                    v.ReferendumResult = ReferendumResult.Remain;
                }
            }
        }

        private void ApplySocialGroups()
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

            var rand = new Random();
            foreach (var v in Voters)
            {
                var r = rand.Next(100);

                if (r <= percentageABC1[v.Region])
                {
                    v.SocialGrade = SocialGrade.ABC1;
                }
                else
                {
                    v.SocialGrade = SocialGrade.C2DE;
                }
            }
        }

        private void ApplyAgeGroups()
        {
            Dictionary<AgeGroup, double> ageGroupsPopulations = new Dictionary<AgeGroup, double>();
            Dictionary<AgeGroup, double> ageGroupsPercentages = new Dictionary<AgeGroup, double>();

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

            foreach (var group in ageGroupsPopulations.Keys)
            {
                ageGroupsPercentages.Add(group, Math.Round(100 * ageGroupsPopulations[group] / total));
            }

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
                    v.AgeGroup = AgeGroup.A1824;
                }
                else if (r <= a2549)
                {
                    v.AgeGroup = AgeGroup.A2549;
                }
                else if (r <= a5064)
                {
                    v.AgeGroup = AgeGroup.A5064;
                }
                else if (r <= a65Plus)
                {
                    v.AgeGroup = AgeGroup.A65Plus;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        private void GenerateBlankVotersInRegions()
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
                regionPercentages.Add(region, Math.Round(10000 * regionPopulations[region] / total));
            }

            foreach (var region in regionPercentages.Keys)
            {
                for (var n = 0; n < regionPercentages[region]; n += 1)
                {
                    Voters.Add(new Voter { Region = region });
                }
            }
        }
    }
}
