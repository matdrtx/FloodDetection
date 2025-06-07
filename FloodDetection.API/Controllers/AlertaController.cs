using FloodDetection.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FloodDetection.API.Controllers;

[ApiController]
[Route("api/v1/alertas")]
public class AlertaController : ControllerBase
{
    private static readonly List<Alerta> alertas = new();

    [HttpGet]
    public IActionResult GetAll()
    {
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
    public IActionResult GetById(Guid id)
    {
        var alerta = alertas.FirstOrDefault(a => a.Id == id);
        return alerta == null ? NotFound() : Ok(alerta);
    }

    [HttpPost]
    public IActionResult Create(Alerta alerta)
    {
        alerta.Id = Guid.NewGuid();
        alertas.Add(alerta);
        return CreatedAtAction(nameof(GetById), new { id = alerta.Id }, alerta);
    }
}
