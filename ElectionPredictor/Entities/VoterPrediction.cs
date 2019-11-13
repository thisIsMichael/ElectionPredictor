using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElectionPredictor.Entities
{
    public class VoterPrediction
    {
        [ColumnName("PredictedLabel")]
        public int PredictedLabel;

        [ColumnName("Score")]
        public float[] VotingIntention;

        public Party PredictedParty => (Party)PredictedLabel;
    }
}
