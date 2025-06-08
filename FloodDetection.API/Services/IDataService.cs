using FloodDetection.Domain.Entities;

namespace FloodDetection.API.Services;

public interface IDataService
{
    Task<List<Sensor>> GetAllSensoresAsync();
    Task<Sensor?> GetSensorByIdAsync(Guid id);
    Task CreateSensorAsync(Sensor sensor);
    Task DeleteSensorAsync(Guid id);

    Task<List<Leitura>> GetAllLeiturasAsync();
    Task<Leitura?> GetLeituraByIdAsync(Guid id);
    Task CreateLeituraAsync(Leitura leitura);

    Task<List<Alerta>> GetAllAlertasAsync();
    Task<Alerta?> GetAlertaByIdAsync(Guid id);
    Task CreateAlertaAsync(Alerta alerta);

    Task<List<Usuario>> GetAllUsuariosAsync();
    Task<Usuario?> GetUsuarioByIdAsync(Guid id);
    Task CreateUsuarioAsync(Usuario usuario);
}
