using Microsoft.AspNetCore.Mvc;
using FloodDetection.Domain.Entities;
using FloodDetection.API.Services;

[ApiController]
[Route("api/v1/sensores")]
public class SensorController : ControllerBase
{
    private readonly IDataService _dataService;

    public SensorController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sensores = await _dataService.GetAllSensoresAsync();
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
    public async Task<IActionResult> GetById(Guid id)
    {
        var sensor = await _dataService.GetSensorByIdAsync(id);
        return sensor == null ? NotFound() : Ok(sensor);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Sensor sensor)
    {
        sensor.Id = Guid.NewGuid();
        await _dataService.CreateSensorAsync(sensor);
        return CreatedAtAction(nameof(GetById), new { id = sensor.Id }, sensor);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _dataService.GetSensorByIdAsync(id);
        if (existing == null) return NotFound();
        await _dataService.DeleteSensorAsync(id);
        return NoContent();
    }
}
