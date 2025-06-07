using FloodDetection.API.Controllers;
using FloodDetection.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public class SensorControllerTests
{
    [Fact]
    public void Create_ReturnsCreatedAtAction()
    {
        var controller = new SensorController();
        var sensor = new Sensor
        {
            Localizacao = "São Paulo",
            Tipo = "NivelAgua",
            Status = "Ativo"
        };

        var result = controller.Create(sensor);

        Assert.IsType<CreatedAtActionResult>(result);
    }
}
