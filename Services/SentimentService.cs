using Microsoft.ML;
using TaskAnalysisAPI.Services;

namespace TaskAnalysisAPI.Services
{
    public class SentimentService : ISentimentService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;

        public SentimentService()
        {
            _mlContext = new MLContext(seed: 0);
            // Entrenar un modelo simple con un dataset pequeño en memoria
            var samples = new List<SentimentData>
            {
                new SentimentData { Text = "La tarea fue completada correctamente", Label = true },
                new SentimentData { Text = "Excelente trabajo, muy satisfecho", Label = true },
                new SentimentData { Text = "Todo funciona bien", Label = true },
                new SentimentData { Text = "Muy buen resultado", Label = true },
                new SentimentData { Text = "No funciona correctamente", Label = false },
                new SentimentData { Text = "Fallo en el sistema", Label = false },
                new SentimentData { Text = "Mal trabajo, insatisfecho", Label = false },
                new SentimentData { Text = "La tarea no se completó", Label = false }
            };

            var data = _mlContext.Data.LoadFromEnumerable(samples);

            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.Text))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(SentimentData.Label), featureColumnName: "Features"));

            _model = pipeline.Fit(data);
        }

        public Task<bool> IsPositiveAsync(string text)
        {
            var predEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);
            var prediction = predEngine.Predict(new SentimentData { Text = text });
            return Task.FromResult(prediction.PredictedLabel);
        }

        private class SentimentData
        {
            public bool Label { get; set; }
            public string Text { get; set; } = string.Empty;
        }

        private class SentimentPrediction
        {
            [Microsoft.ML.Data.ColumnName("PredictedLabel")]
            public bool PredictedLabel { get; set; }
            public float Score { get; set; }
        }
    }
}
