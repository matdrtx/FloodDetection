namespace FloodDetection.Domain.Entities;

public class Alerta
{
    public Guid Id { get; set; }
    public string Local { get; set; } = string.Empty;
    public string NivelRisco { get; set; } = "Baixo";
    public string AcaoRecomendada { get; set; } = string.Empty;
}
