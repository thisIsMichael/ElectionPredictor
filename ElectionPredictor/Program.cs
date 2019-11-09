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


            Console.ReadLine();
        }



        
    }
}
