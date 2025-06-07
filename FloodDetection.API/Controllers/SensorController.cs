using Microsoft.AspNetCore.Mvc;
using FloodDetection.Domain.Entities;

[ApiController]
[Route("api/v1/sensores")]
public class SensorController : ControllerBase
{
    private static readonly List<Sensor> sensores = new();

    [HttpGet]
    public IActionResult GetAll()
    {
        var result = sensores.Select(s => new
        {
            s,
            links = new[]
            {
                new { rel = "self", href = Url.Action(nameof(GetById), new { id = s.Id }) }
            }
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var sensor = sensores.FirstOrDefault(s => s.Id == id);
        return sensor == null ? NotFound() : Ok(sensor);
    }

    [HttpPost]
    public IActionResult Create(Sensor sensor)
    {
        sensor.Id = Guid.NewGuid();
        sensores.Add(sensor);
        return CreatedAtAction(nameof(GetById), new { id = sensor.Id }, sensor);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var sensor = sensores.FirstOrDefault(s => s.Id == id);
        if (sensor == null) return NotFound();
        sensores.Remove(sensor);
        return NoContent();
    }
}
