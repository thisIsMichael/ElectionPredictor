using ElectionPredictor.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ElectionPredictor
{
    class ProbabiliesManager
    {
        Dictionary<Party, PartyPrediction> PreviousElectionProbabilities = new Dictionary<Party, PartyPrediction>();
        Dictionary<AgeGroup, PartyPrediction> AgeProbabilities = new Dictionary<AgeGroup, PartyPrediction>();
        Dictionary<Gender, PartyPrediction> GenderProbabilities = new Dictionary<Gender, PartyPrediction>();
        Dictionary<ReferendumResult, PartyPrediction> ReferendumProbabilities = new Dictionary<ReferendumResult, PartyPrediction>();
        Dictionary<Region, PartyPrediction> RegionProbabilities = new Dictionary<Region, PartyPrediction>();
        Dictionary<SocialGrade, PartyPrediction> SocialGradeProbabilities = new Dictionary<SocialGrade, PartyPrediction>();

        public ProbabiliesManager()
        {
            LoadProbabilities();
        }

        public Party GetLikeliestParty(Party previousVote, AgeGroup age, Gender gender, ReferendumResult referendum, Region region, SocialGrade socialGrade)
        {
            var likeliestParty = Party.None;
            var highestScore = 0d;

            foreach (var party in Enum.GetValues(typeof(Party)).Cast<Party>())
            {
                if (party == Party.SNP && region != Region.Scotland)
                {
                    continue;
                }

                var isRegionalParty = (region == Region.Scotland && party == Party.SNP) || (region == Region.MidlandsWales && party == Party.PlaidCymru);
                var regionalMultiplier = isRegionalParty ? 2 : 1;
                var bonusPreviousParty = previousVote == party ? 2 : 1;

                var score = AgeProbabilities[age].GetVotingLikelihood(party) +
                            GenderProbabilities[gender].GetVotingLikelihood(party) +
                            ReferendumProbabilities[referendum].GetVotingLikelihood(party) + 
                            (RegionProbabilities[region].GetVotingLikelihood(party) * regionalMultiplier) +
                            SocialGradeProbabilities[socialGrade].GetVotingLikelihood(party);

                if (previousVote == Party.UKIP && party == Party.UKIP && referendum == ReferendumResult.Leave)
                {
                    var x = true;
                }

                if (PreviousElectionProbabilities.ContainsKey(previousVote))
                {
                    score += PreviousElectionProbabilities[previousVote].GetVotingLikelihood(party);
                    score /= 6;
                }
                else
                {
                    score /= 5;
                }

                if (previousVote == party || region == Region.Scotland || true)
                {
                    score *= 5;

                    Random random = new Random();
                    var rand = random.Next(100);
                    if (rand > 99 && previousVote == party)
                    {
                        score *= 100;

                    }
                    else if (rand > 40)
                    {
                        if (rand > 60 && region == Region.Scotland)
                        {
                            score += RegionProbabilities[region].GetVotingLikelihood(party) * 80;
                        }
                        else if (region != Region.Scotland)
                        {
                            if (rand > 75)
                                score += AgeProbabilities[age].GetVotingLikelihood(party) * 80;
                            else if (rand > 70)
                                score += GenderProbabilities[gender].GetVotingLikelihood(party) * 80;
                            else if (rand > 58)
                                score += SocialGradeProbabilities[socialGrade].GetVotingLikelihood(party) * 80;
                            else
                                score += RegionProbabilities[region].GetVotingLikelihood(party) * 80;
                        }
                    }
                }

                if (score > highestScore)
                {
                    likeliestParty = party;
                    highestScore = score;
                }
            }

            if (likeliestParty == Party.None)
            {
                throw new Exception("Couldnt find likeliest party");
            }

            return likeliestParty;
        }

        private void LoadProbabilities()
        {
            Load2017VotingProbabilities();
            LoadAgeProbabilities();
            LoadGenderProbabilities();
            LoadReferendumProbabilities();
            LoadRegionProbabilities();
            LoadSocialGradeProbabilies();
        }

        private void LoadSocialGradeProbabilies()
        {
            PartyPrediction givenABC1 = new PartyPrediction();
            PartyPrediction givenC2DE = new PartyPrediction();

            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Data", "yougov2017.csv")))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var party = (Party)Enum.Parse(typeof(Party), values[0]);

                    givenABC1.Add(party, int.Parse(values[14]));
                    givenC2DE.Add(party, int.Parse(values[15]));
                }
            }

            SocialGradeProbabilities.Add(SocialGrade.ABC1, givenABC1);
            SocialGradeProbabilities.Add(SocialGrade.C2DE, givenC2DE);
        }

        private void LoadRegionProbabilities()
        {
            PartyPrediction givenLondon = new PartyPrediction();
            PartyPrediction givenSouth = new PartyPrediction();
            PartyPrediction givenMidlands = new PartyPrediction();
            PartyPrediction givenNorth = new PartyPrediction();
            PartyPrediction givenScotland = new PartyPrediction();

            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Data", "yougov2017.csv")))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var party = (Party)Enum.Parse(typeof(Party), values[0]);

                    givenLondon.Add(party, int.Parse(values[16]));
                    givenSouth.Add(party, int.Parse(values[17]));
                    givenMidlands.Add(party, int.Parse(values[18]));
                    givenNorth.Add(party, int.Parse(values[19]));
                    givenScotland.Add(party, int.Parse(values[20]));
                }
            }

            RegionProbabilities.Add(Region.London, givenLondon);
            RegionProbabilities.Add(Region.South, givenSouth);
            RegionProbabilities.Add(Region.MidlandsWales, givenMidlands);
            RegionProbabilities.Add(Region.North, givenNorth);
            RegionProbabilities.Add(Region.Scotland, givenScotland);
        }

        private void LoadReferendumProbabilities()
        {
            PartyPrediction givenRemain = new PartyPrediction();
            PartyPrediction givenLeave = new PartyPrediction();

            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Data", "yougov2017.csv")))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var party = (Party)Enum.Parse(typeof(Party), values[0]);

                    givenRemain.Add(party, int.Parse(values[2]));
                    givenLeave.Add(party, int.Parse(values[3]));
                }
            }

            ReferendumProbabilities.Add(ReferendumResult.Remain, givenRemain);
            ReferendumProbabilities.Add(ReferendumResult.Leave, givenLeave);
        }

        private void LoadGenderProbabilities()
        {
            PartyPrediction givenMale = new PartyPrediction();
            PartyPrediction givenFemale = new PartyPrediction();

            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Data", "yougov2017.csv")))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var party = (Party)Enum.Parse(typeof(Party), values[0]);

                    givenMale.Add(party, int.Parse(values[8]));
                    givenFemale.Add(party, int.Parse(values[9]));
                }
            }

            GenderProbabilities.Add(Gender.Male, givenMale);
            GenderProbabilities.Add(Gender.Female, givenFemale);
        }

        private void LoadAgeProbabilities()
        {
            PartyPrediction given1824 = new PartyPrediction();
            PartyPrediction given2549 = new PartyPrediction();
            PartyPrediction given5064 = new PartyPrediction();
            PartyPrediction given65Plus = new PartyPrediction();

            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Data", "yougov2017.csv")))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var party = (Party)Enum.Parse(typeof(Party), values[0]);

                    given1824.Add(party, int.Parse(values[10]));
                    given2549.Add(party, int.Parse(values[11]));
                    given5064.Add(party, int.Parse(values[12]));
                    given65Plus.Add(party, int.Parse(values[13]));
                }
            }

            AgeProbabilities.Add(AgeGroup.A1824, given1824);
            AgeProbabilities.Add(AgeGroup.A2549, given2549);
            AgeProbabilities.Add(AgeGroup.A5064, given5064);
            AgeProbabilities.Add(AgeGroup.A65Plus, given65Plus);
        }

        private void Load2017VotingProbabilities()
        {
            PartyPrediction givenVotedCon17 = new PartyPrediction();
            PartyPrediction givenVotedLab17 = new PartyPrediction();
            PartyPrediction givenVotedLibDem17 = new PartyPrediction();

            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Data", "yougov2017.csv")))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var party = (Party)Enum.Parse(typeof(Party), values[0]);

                    givenVotedCon17.Add(party, int.Parse(values[4]));
                    givenVotedLab17.Add(party, int.Parse(values[5]));
                    givenVotedLibDem17.Add(party, int.Parse(values[6]));
                }
            }

            PreviousElectionProbabilities.Add(Party.Con, givenVotedCon17);
            PreviousElectionProbabilities.Add(Party.Lab, givenVotedLab17);
            PreviousElectionProbabilities.Add(Party.LibDem, givenVotedLibDem17);
        }
    }
}
