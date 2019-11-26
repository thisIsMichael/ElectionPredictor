using ElectionPredictor.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ElectionPredictor
{
    class Program
    {
        const int numberOfNationalVotersToGenerate = 100000;
        const int numberOfLocalVotersToGenerate = 10000;
        const int iterations = 100;
        private static PredictionType predictionType = PredictionType.Scotland2019;
        private static Election ElectionToPredict
        {
            get 
            {
                if (predictionType == PredictionType.EnglandWales2017 || 
                    predictionType == PredictionType.Scotland2017)
                {
                    return Election.e2017;
                }
                else
                {
                    return Election.e2019;
                }
            }
        }

        private static string logPath = Path.Combine(Environment.CurrentDirectory, "Output", "modelOutput.txt");
        private static string winningPath = Path.Combine(Environment.CurrentDirectory, "Output", "winningStatistics.csv");
        private static string conPath = Path.Combine(Environment.CurrentDirectory, "Output", "conStatistics.csv");
        private static string labPath = Path.Combine(Environment.CurrentDirectory, "Output", "labStatistics.csv");
        private static string ldPath = Path.Combine(Environment.CurrentDirectory, "Output", "ldStatistics.csv");
        private static string snpPath = Path.Combine(Environment.CurrentDirectory, "Output", "snpStatistics.csv");
        private static string actualWinners = Path.Combine(Environment.CurrentDirectory, "Output", "2017Results.csv");

        private static Dictionary<string, List<string>> combinedResults = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> combinedCon = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> combinedLab = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> combinedLD = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> combinedUKIP = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> combinedSNP = new Dictionary<string, List<string>>();




        const bool LoadInFromFile = false;

        static void Main(string[] args)
        {
            OutputController output = new OutputController(logPath);
            ElectionMLModel model = null;

            for (int n = 1; n <= iterations; n++)
            {
                Console.WriteLine($"Run {n}");

                

                model = new ElectionMLModel();

                if (LoadInFromFile)
                {
                    model.LoadModel();
                }
                else
                {
                    var probabilies = new ProbabiliesManager(predictionType);

                    var voters = new VoterManager(predictionType);
                    voters.GenerateVotersNationally(numberOfNationalVotersToGenerate);
                    voters.GenerateLikelyVotingIntension(probabilies);

                    var x = true;

                    //Console.WriteLine("Hello World!");

                    voters.OutputNationalVotingIntention();

                    Console.WriteLine("Building model...");
                    Console.WriteLine();
                    model.TrainModel(voters.Voters);

                    //var testVoter = new Voter
                    //{
                    //    AgeGroupEnum = AgeGroup.A65Plus,
                    //    GenderEnum = Gender.Female,
                    //    PreviousVoteEnum = Party.Con,
                    //    ReferendumResultEnum = ReferendumResult.Leave,
                    //    RegionEnum = Region.MidlandsWales,
                    //    SocialGradeEnum = SocialGrade.ABC1
                    //};

                    //model.Predict(testVoter);
                }
                /*
                var nuneaton = new VoterManager();
                nuneaton.GenerateVotersForConstituency(Constituency.NuneatonExample, 10000);
                model.PredictVotersIntentions(nuneaton);
                nuneaton.OutputConstituencyVotingIntention();

                var batterseaTest = new VoterManager();
                batterseaTest.GenerateVotersForConstituency(Constituency.BatterseaExample, 10000);
                model.PredictVotersIntentions(batterseaTest);
                batterseaTest.OutputConstituencyVotingIntention();

                var peterboroughTest = new VoterManager();
                peterboroughTest.GenerateVotersForConstituency(Constituency.PeterboroughExample, 10000);
                model.PredictVotersIntentions(peterboroughTest);
                peterboroughTest.OutputConstituencyVotingIntention();

                var canterburyTest = new VoterManager();
                canterburyTest.GenerateVotersForConstituency(Constituency.CanterburyExample, 10000);
                model.PredictVotersIntentions(canterburyTest);
                canterburyTest.OutputConstituencyVotingIntention();

                var southamptonItchenTest = new VoterManager();
                southamptonItchenTest.GenerateVotersForConstituency(Constituency.SouthamptonItchenExample, 10000);
                model.PredictVotersIntentions(southamptonItchenTest);
                southamptonItchenTest.OutputConstituencyVotingIntention();

                var yorkCentralTest = new VoterManager();
                yorkCentralTest.GenerateVotersForConstituency(Constituency.YorkCentralExample, 10000);
                model.PredictVotersIntentions(yorkCentralTest);
                yorkCentralTest.OutputConstituencyVotingIntention();

                var yorkOuterTest = new VoterManager();
                yorkOuterTest.GenerateVotersForConstituency(Constituency.YorkOuterExample, 10000);
                model.PredictVotersIntentions(yorkOuterTest);
                yorkOuterTest.OutputConstituencyVotingIntention();*/

                var totalSeats = new Dictionary<Party, int>();
                int countDif = 0;

                var constituencyIntentions = new Dictionary<string, VoterManager>();

                var includeEnglandWales = predictionType == PredictionType.EnglandWales2017 || predictionType == PredictionType.EnglandWales2019;
                var includeScotland = predictionType == PredictionType.Scotland2017 || predictionType == PredictionType.Scotland2019;

                foreach (var constituency in ConstituencyManager.GetConstituencies(ElectionToPredict, includeEnglandWales, includeScotland))
                {
                    //Console.WriteLine($"Predicting for {constituency.Name}...");
                    var constituencyVoters = new VoterManager(predictionType);
                    constituencyVoters.GenerateVotersForConstituency(constituency, 10000);
                    model.PredictVotersIntentions(constituencyVoters);


                    if (!totalSeats.ContainsKey(constituencyVoters.Winner))
                    {
                        totalSeats.Add(constituencyVoters.Winner, 1);
                    }
                    else
                    {
                        totalSeats[constituencyVoters.Winner] = totalSeats[constituencyVoters.Winner] + 1;
                    }

                    if (constituencyVoters.IsDifferent)
                    {
                        countDif++;
                        constituencyVoters.OutputConstituencyVotingIntention(true);
                    }

                    constituencyIntentions.Add(constituency.Name.Replace(",", ""), constituencyVoters);
                }

                Console.WriteLine();
                Console.WriteLine("Totals -------------------");
                Console.WriteLine("--------------------------");

                if (totalSeats.Values.Max() > 400)
                {
                    Console.WriteLine("Ignoring model as infeasible");
                    n--;
                    continue;
                }

                int con = totalSeats.ContainsKey(Party.Con) ? totalSeats[Party.Con] : 0;
                int lab = totalSeats.ContainsKey(Party.Lab) ? totalSeats[Party.Lab] : 0;
                int ld = totalSeats.ContainsKey(Party.LibDem) ? totalSeats[Party.LibDem] : 0;

                string results = $"{con},{lab},{ld},";
                output.WriteLine(results);

                AddRecordOfPrediction(constituencyIntentions);

                //foreach (var result in totalSeats.OrderByDescending(x => x.Value))
                //{
                //    output.WriteLine($"Party: {result.Key.ToString()}. Seats: {result.Value}");
                //}

                //output.WriteLine();
                //output.WriteLine($"Number different: {countDif}");
                //output.WriteLine();
            }

            using (OutputController combinedOutput = new OutputController(winningPath))
            {
                foreach (var name in combinedResults.Keys)
                {
                    string winners = $"{name},{string.Join(",", combinedResults[name])}";
                    combinedOutput.WriteLine(winners);
                }
            }

            using (OutputController conOutput = new OutputController(conPath))
            {
                foreach (var name in combinedCon.Keys)
                {
                    string conVotes = $"{name},{string.Join(",", combinedCon[name])}";
                    conOutput.WriteLine(conVotes);
                }
            }

            using (OutputController labOutput = new OutputController(labPath))
            {
                foreach (var name in combinedLab.Keys)
                {
                    string labVotes = $"{name},{string.Join(",", combinedLab[name])}";
                    labOutput.WriteLine(labVotes);
                }
            }

            using (OutputController ldOutput = new OutputController(ldPath))
            {
                foreach (var name in combinedLD.Keys)
                {
                    string ldVotes = $"{name},{string.Join(",", combinedLD[name])}";
                    ldOutput.WriteLine(ldVotes);
                }
            }

            using (OutputController snpOutput = new OutputController(snpPath))
            {
                foreach (var name in combinedSNP.Keys)
                {
                    string snpVotes = $"{name},{string.Join(",", combinedSNP[name])}";
                    snpOutput.WriteLine(snpVotes);
                }
            }

            using (OutputController actualResults = new OutputController(actualWinners))
            {
                foreach (var c in ConstituencyManager.GetConstituencies(Election.e2017, true, false))
                {
                    actualResults.WriteLine($"{c.Name.Replace(",", "")},{c.ActualWinner.ToString()}");
                }
            }

            var save = Console.ReadLine();
            if (!String.IsNullOrWhiteSpace(save))
            {
                model.SaveModelAsFile();
            }

        }

        private static void AddRecordOfPrediction(Dictionary<string, VoterManager> constituencyIntentions)
        {
            foreach (var id in constituencyIntentions.Keys)
            {
                if (!combinedResults.ContainsKey(id))
                {
                    combinedResults.Add(id, new List<string>());
                }

                combinedResults[id].Add(constituencyIntentions[id].Winner.ToString());

                if (!combinedCon.ContainsKey(id))
                {
                    combinedCon.Add(id, new List<string>());
                }

                combinedCon[id].Add(constituencyIntentions[id].Intentions[Party.Con].ToString());

                if (!combinedLab.ContainsKey(id))
                {
                    combinedLab.Add(id, new List<string>());
                }

                combinedLab[id].Add(constituencyIntentions[id].Intentions[Party.Lab].ToString());

                if (!combinedLD.ContainsKey(id))
                {
                    combinedLD.Add(id, new List<string>());
                }

                combinedLD[id].Add(constituencyIntentions[id].Intentions[Party.LibDem].ToString());

                if (!combinedSNP.ContainsKey(id))
                {
                    combinedSNP.Add(id, new List<string>());
                }

                combinedSNP[id].Add(constituencyIntentions[id].Intentions[Party.SNP].ToString());
            }
        }



        
    }
}
