using FloodDetection.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using FloodDetection.API.Services;

namespace FloodDetection.API.Controllers;

[ApiController]
[Route("api/v1/leituras")]
public class LeituraController : ControllerBase
{
    private readonly IDataService _dataService;

    public LeituraController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var leituras = await _dataService.GetAllLeiturasAsync();
        var result = leituras.Select(l => new
        {
            l,
            links = new[]
            {
                new { rel = "self", href = Url.Action(nameof(GetById), new { id = l.Id }) }
            }
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var leitura = await _dataService.GetLeituraByIdAsync(id);
        return leitura == null ? NotFound() : Ok(leitura);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Leitura leitura)
    {
        leitura.Id = Guid.NewGuid();
        leitura.Timestamp = DateTime.UtcNow;

        await _dataService.CreateLeituraAsync(leitura);

        // Publica a leitura na fila do RabbitMQ
        EnviarParaFila(leitura);

        return CreatedAtAction(nameof(GetById), new { id = leitura.Id }, leitura);
    }

    private void EnviarParaFila(Leitura leitura)
    {
        try
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "leituras", durable: false, exclusive: false, autoDelete: false);

            var json = JsonSerializer.Serialize(leitura);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "",
                                 routingKey: "leituras",
                                 basicProperties: null,
                                 body: body);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao enviar leitura para a fila RabbitMQ: {ex.Message}");
        }
    }
}
