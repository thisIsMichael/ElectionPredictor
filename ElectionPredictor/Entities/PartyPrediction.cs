using System;
using System.Collections.Generic;
using System.Text;

namespace ElectionPredictor.Entities
{
    public class PartyPrediction : Dictionary<Party, double>
    {
        public double GetVotingLikelihood(Party party) => this.ContainsKey(party) ? this[party] : 0;
    }
}
