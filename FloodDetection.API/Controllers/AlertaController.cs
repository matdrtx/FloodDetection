using FloodDetection.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using FloodDetection.API.Services;

namespace FloodDetection.API.Controllers;

[ApiController]
[Route("api/v1/alertas")]
public class AlertaController : ControllerBase
{
    private readonly IDataService _dataService;

    public AlertaController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var alertas = await _dataService.GetAllAlertasAsync();
        var result = alertas.Select(a => new
        {
            a,
            links = new[]
            {
                new { rel = "self", href = Url.Action(nameof(GetById), new { id = a.Id }) }
            }
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var alerta = await _dataService.GetAlertaByIdAsync(id);
        return alerta == null ? NotFound() : Ok(alerta);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Alerta alerta)
    {
        alerta.Id = Guid.NewGuid();
        await _dataService.CreateAlertaAsync(alerta);
        return CreatedAtAction(nameof(GetById), new { id = alerta.Id }, alerta);
    }
}
