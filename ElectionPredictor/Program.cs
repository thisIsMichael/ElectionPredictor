using ElectionPredictor.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace ElectionPredictor
{
    class Program
    {
        static void Main(string[] args)
        {
            var probabilies = new ProbabiliesManager();

            var voters = new VoterManager();
            voters.GenerateVoters();
            voters.GenerateLikelyVotingIntension(probabilies);

            var x = true;

            //Console.WriteLine("Hello World!");

            voters.OutputVotingIntention();

            ElectionMLModel model = new ElectionMLModel();
            model.TrainModel(voters.Voters);

            var testVoter = new Voter
            {
                AgeGroupEnum = AgeGroup.A65Plus,
                GenderEnum = Gender.Female,
                PreviousVoteEnum = Party.Con,
                ReferendumResultEnum = ReferendumResult.Leave,
                RegionEnum = Region.MidlandsWales,
                SocialGradeEnum = SocialGrade.ABC1
            };

            model.Predict(testVoter);

            Console.ReadLine();
        }



        
    }
}
