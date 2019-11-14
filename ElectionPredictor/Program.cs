using ElectionPredictor.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace ElectionPredictor
{
    class Program
    {
        const int numberOfNationalVotersToGenerate = 100000;
        const int numberOfLocalVotersToGenerate = 10000;

        const bool LoadInFromFile = true;

        static void Main(string[] args)
        {
            Election electionToPredict = Election.e2017;

            /*ElectionMLModel model = new ElectionMLModel();

            if (LoadInFromFile)
            {
                model.LoadModel();
            }
            else
            { 
                var probabilies = new ProbabiliesManager();

                var voters = new VoterManager();
                voters.GenerateVotersNationally(numberOfNationalVotersToGenerate);
                voters.GenerateLikelyVotingIntension(probabilies);

                var x = true;

                //Console.WriteLine("Hello World!");

                voters.OutputNationalVotingIntention();

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
            yorkOuterTest.OutputConstituencyVotingIntention();

            var save = Console.ReadLine();
            if (!String.IsNullOrWhiteSpace(save))
            {
                model.SaveModelAsFile();
            }*/

            ConstituencyManager.GetConstituencies(electionToPredict);
        }



        
    }
}
