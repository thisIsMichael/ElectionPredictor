using System;
using System.Collections.Generic;
using System.Text;

namespace ElectionPredictor.Entities
{
    public class Voter
    {
        public AgeGroup? AgeGroup { get; set; }

        public Gender? Gender { get; set; }

        public Party? Intention { get; set; }

        public Party? PreviousVote { get; set; }

        public ReferendumResult? ReferendumResult { get; set; }

        public Region Region { get; set; }

        public SocialGrade? SocialGrade { get; set; }
    }
}
