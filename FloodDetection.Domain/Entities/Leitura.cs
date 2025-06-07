namespace FloodDetection.Domain.Entities;

public class Leitura
{
    public Guid Id { get; set; }
    public Guid SensorId { get; set; }
    public double NivelAgua { get; set; }
    public double VelocidadeChuva { get; set; }
    public DateTime Timestamp { get; set; }
}
