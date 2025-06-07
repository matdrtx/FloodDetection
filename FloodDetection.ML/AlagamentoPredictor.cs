using Microsoft.ML;
using Microsoft.ML.Data;

namespace FloodDetection.ML;

public class AlagamentoInput
{
    public float NivelAgua { get; set; }
    public float VelocidadeChuva { get; set; }
}

public class AlagamentoOutput
{
    [ColumnName("PredictedLabel")]
    public bool VaiAlagar { get; set; }
    public float Probability { get; set; }
    public float Score { get; set; }
}

public class AlagamentoPredictor
{
    private readonly MLContext _context;
    private readonly PredictionEngine<AlagamentoInput, AlagamentoOutput> _engine;

    public AlagamentoPredictor()
    {
        _context = new MLContext();

        // Simulação de histórico
        var dadosTreino = new[]
        {
            new AlagamentoInput { NivelAgua = 2, VelocidadeChuva = 1 },   // Sem alagamento
            new AlagamentoInput { NivelAgua = 3, VelocidadeChuva = 1 },
            new AlagamentoInput { NivelAgua = 7, VelocidadeChuva = 6 },   // Alagamento provável
            new AlagamentoInput { NivelAgua = 9, VelocidadeChuva = 8 }
        };

        var labels = new[] { false, false, true, true };

        var treino = _context.Data.LoadFromEnumerable(dadosTreino.Zip(labels, (input, label) => new AlagamentoTreino
        {
            NivelAgua = input.NivelAgua,
            VelocidadeChuva = input.VelocidadeChuva,
            Label = label
        }));

        var pipeline = _context.Transforms.Concatenate("Features", nameof(AlagamentoTreino.NivelAgua), nameof(AlagamentoTreino.VelocidadeChuva))
            .Append(_context.BinaryClassification.Trainers.SdcaLogisticRegression());

        var model = pipeline.Fit(treino);
        _engine = _context.Model.CreatePredictionEngine<AlagamentoInput, AlagamentoOutput>(model);
    }

    public AlagamentoOutput Prever(float nivelAgua, float velocidadeChuva)
    {
        return _engine.Predict(new AlagamentoInput
        {
            NivelAgua = nivelAgua,
            VelocidadeChuva = velocidadeChuva
        });
    }

    private class AlagamentoTreino
    {
        public float NivelAgua { get; set; }
        public float VelocidadeChuva { get; set; }
        public bool Label { get; set; }
    }
}
