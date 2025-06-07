namespace FloodDetection.Domain.Entities;

public class Sensor
{
    public Guid Id { get; set; }
    public string Localizacao { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
