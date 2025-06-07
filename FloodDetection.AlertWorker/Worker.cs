using System.Text;
using System.Text.Json;
using FloodDetection.Domain.Entities;
using FloodDetection.ML;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FloodDetection.AlertWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AlagamentoPredictor _predictor = new();

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "leituras", durable: false, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var leitura = JsonSerializer.Deserialize<Leitura>(json);

                if (leitura != null)
                {
                    var resultado = _predictor.Prever((float)leitura.NivelAgua, (float)leitura.VelocidadeChuva);
                    var nivel = resultado.VaiAlagar ? (resultado.Probability > 0.7 ? "Crítico" : "Médio") : "Baixo";

                    var alerta = new Alerta
                    {
                        Id = Guid.NewGuid(),
                        Local = leitura.SensorId.ToString(),
                        NivelRisco = nivel,
                        AcaoRecomendada = nivel switch
                        {
                            "Crítico" => "Evacuação imediata",
                            "Médio" => "Monitorar a região",
                            _ => "Nenhuma ação necessária"
                        }
                    };

                    _logger.LogInformation($"[ALERTA] {alerta.NivelRisco}: {alerta.AcaoRecomendada} (Sensor {alerta.Local})");
                }
            };

            channel.BasicConsume(queue: "leituras", autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }
    }
}
