using ElectionPredictor.Entities;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ElectionPredictor
{
    public class ElectionMLModel
    {
        private MLContext mlContext;
        private DataViewSchema dataViewSchema;
        private ITransformer model;
        private IList<Party> parties = new List<Party>();

        private static string modelSavePath = Path.Combine(Environment.CurrentDirectory, "Models", "model.zip");
        private static string modelPartiesSavePath = Path.Combine(Environment.CurrentDirectory, "Models", "modelParties.csv");

        public ElectionMLModel()
        {
            mlContext = new MLContext(seed: 0);
        }

        public void TrainModel(IList<Voter> voters)
        {
            var schema = SchemaDefinition.Create(typeof(Voter));
            //DataViewTypeManager.Register(NumberDataViewType.Int32, typeof(AgeGroup?));
            //DataViewTypeManager.Register(NumberDataViewType., typeof(Gender?));
            //DataViewTypeManager.Register(NumberDataViewType.Int32, typeof(Party?));
            //DataViewTypeManager.Register(NumberDataViewType.Int32, typeof(ReferendumResult?));
            //DataViewTypeManager.Register(NumberDataViewType.Int32, typeof(Region?));
            //DataViewTypeManager.Register(NumberDataViewType.Int32, typeof(SocialGrade?));
            var dataView = mlContext.Data.LoadFromEnumerable(voters, schema);

            //var binaryTrainer = mlContext.BinaryClassification.Trainers.SdcaLogisticRegression("Label", "Features");

            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "Label", inputColumnName: nameof(Voter.Intention))
                            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "AgeGroupEncoded", inputColumnName: nameof(Voter.AgeGroup)))
                            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "GenderEncoded", inputColumnName: nameof(Voter.Gender)))
                            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PreviousVoteEncoded", inputColumnName: nameof(Voter.PreviousVote)))
                            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "ReferendumResultEncoded", inputColumnName: nameof(Voter.ReferendumResult)))
                            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "RegionEncoded", inputColumnName: nameof(Voter.Region)))
                            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "SocialGradeEncoded", inputColumnName: nameof(Voter.SocialGrade)))
                            .Append(mlContext.Transforms.Concatenate("Features", "AgeGroupEncoded", "GenderEncoded", "PreviousVoteEncoded", "ReferendumResultEncoded", "RegionEncoded", "SocialGradeEncoded"))
                            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            model = pipeline.Fit(dataView);

            dataViewSchema = dataView.Schema;

            var transformed = model.Transform(dataView);
            var labels = transformed.GetColumn<int>(nameof(Voter.Intention));
            parties = labels.Distinct().Select(p => (Party)p).ToList();
            var x = true;
        }

        internal void PredictVotersIntentions(VoterManager canterburyTest)
        {
            var predictionFunction = mlContext.Model.CreatePredictionEngine<Voter, VoterPrediction>(model);

            foreach (var voter in canterburyTest.Voters)
            {
                var prediction = predictionFunction.Predict(voter);

                

                var summedProbabilities = new Dictionary<int, float>();
                for (int n = 0; n < parties.Count; n++)
                {
                    var previous = n == 0 ? 0 : summedProbabilities[n - 1];
                    summedProbabilities.Add(n, previous + prediction.VotingIntention[n]);
                }

                Random r = new Random();
                var prob = r.NextDouble();

                for (int n = 0; n < parties.Count; n++)
                {
                    var previous = n == 0 ? 0 : summedProbabilities[n - 1];

                    if (prob >= previous && prob < summedProbabilities[n])
                    {
                        voter.IntentionEnum = parties[n];
                        continue;
                    }
                }

                //voter.IntentionEnum = parties[prediction.PredictedLabel];
            }
        }

        //public void Evaluate()
        //{
        //    IDataView dataView = mlContext.Data.LoadFromTextFile<TaxiTrip>(_testDataPath, hasHeader: true, separatorChar: ',');

        //    var predictions = model.Transform(dataView);

        //    var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

        //    Console.WriteLine();
        //    Console.WriteLine($"*************************************************");
        //    Console.WriteLine($"*       Model quality metrics evaluation         ");
        //    Console.WriteLine($"*------------------------------------------------");
        //    Console.WriteLine($"*       RSquared Score:      {metrics.RSquared:0.##}");
        //    Console.WriteLine($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");
        //    Console.WriteLine($"*************************************************");
        //}

        public void Predict(Voter voter)
        {
            // Create prediction engine related to the loaded trained model
            var predictionFunction = mlContext.Model.CreatePredictionEngine<Voter, VoterPrediction>(model);

            //Score
            var prediction = predictionFunction.Predict(voter);

            var probabilities = new Dictionary<Party, float>();
            for (int n = 0; n < parties.Count; n++)
            {
                probabilities.Add(parties[n], prediction.VotingIntention[n]);
            }

            voter.IntentionEnum = prediction.PredictedParty;

            //Console.WriteLine($"**********************************************************************");
            //Console.WriteLine($"Predicted party: {prediction.VotingIntention}");
            //Console.WriteLine($"**********************************************************************");
        }

        public void SaveModelAsFile()
        {
            mlContext.Model.Save(model, dataViewSchema, modelSavePath);

            using (var file = File.CreateText(modelPartiesSavePath))
            {
                foreach (var party in parties.Select(p => p.ToString()))
                {
                    file.WriteLine(string.Join(",", party));
                }
            }
        }

        public void LoadModel()
        {
            if (model != null)
                throw new InvalidOperationException("Cannot load a model over an existing model");

            model = mlContext.Model.Load(modelSavePath, out dataViewSchema);

            using (var reader = new StreamReader(modelPartiesSavePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    var party = (Party)Enum.Parse(typeof(Party), line);
                    parties.Add(party);
                }
            }
        }

        //public Dictionary<Party, float> GetPredictedProbabilities(Voter
    }
}
