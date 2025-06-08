using FloodDetection.Domain.Entities;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace FloodDetection.API.Services;

public class OracleDataService : IDataService
{
    private readonly string _connectionString;

    public OracleDataService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OracleDb") ?? string.Empty;
    }

    private OracleConnection GetConnection()
    {
        return new OracleConnection(_connectionString);
    }

    public async Task<List<Sensor>> GetAllSensoresAsync()
    {
        var sensores = new List<Sensor>();
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ID, LOCALIZACAO, TIPO, STATUS FROM SENSORES";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            sensores.Add(new Sensor
            {
                Id = Guid.Parse(reader.GetString(0)),
                Localizacao = reader.GetString(1),
                Tipo = reader.GetString(2),
                Status = reader.GetString(3)
            });
        }
        return sensores;
    }

    public async Task<Sensor?> GetSensorByIdAsync(Guid id)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ID, LOCALIZACAO, TIPO, STATUS FROM SENSORES WHERE ID = :id";
        cmd.Parameters.Add(new OracleParameter("id", id.ToString()));
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
        if (await reader.ReadAsync())
        {
            return new Sensor
            {
                Id = Guid.Parse(reader.GetString(0)),
                Localizacao = reader.GetString(1),
                Tipo = reader.GetString(2),
                Status = reader.GetString(3)
            };
        }
        return null;
    }

    public async Task CreateSensorAsync(Sensor sensor)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO SENSORES (ID, LOCALIZACAO, TIPO, STATUS) VALUES (:id, :loc, :tipo, :status)";
        cmd.Parameters.Add(new OracleParameter("id", sensor.Id.ToString()));
        cmd.Parameters.Add(new OracleParameter("loc", sensor.Localizacao));
        cmd.Parameters.Add(new OracleParameter("tipo", sensor.Tipo));
        cmd.Parameters.Add(new OracleParameter("status", sensor.Status));
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteSensorAsync(Guid id)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM SENSORES WHERE ID = :id";
        cmd.Parameters.Add(new OracleParameter("id", id.ToString()));
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<Leitura>> GetAllLeiturasAsync()
    {
        var leituras = new List<Leitura>();
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ID, SENSOR_ID, NIVEL_AGUA, VELOCIDADE_CHUVA, TIMESTAMP_LEITURA FROM LEITURAS";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            leituras.Add(new Leitura
            {
                Id = Guid.Parse(reader.GetString(0)),
                SensorId = Guid.Parse(reader.GetString(1)),
                NivelAgua = reader.GetDouble(2),
                VelocidadeChuva = reader.GetDouble(3),
                Timestamp = reader.GetDateTime(4)
            });
        }
        return leituras;
    }

    public async Task<Leitura?> GetLeituraByIdAsync(Guid id)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ID, SENSOR_ID, NIVEL_AGUA, VELOCIDADE_CHUVA, TIMESTAMP_LEITURA FROM LEITURAS WHERE ID = :id";
        cmd.Parameters.Add(new OracleParameter("id", id.ToString()));
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
        if (await reader.ReadAsync())
        {
            return new Leitura
            {
                Id = Guid.Parse(reader.GetString(0)),
                SensorId = Guid.Parse(reader.GetString(1)),
                NivelAgua = reader.GetDouble(2),
                VelocidadeChuva = reader.GetDouble(3),
                Timestamp = reader.GetDateTime(4)
            };
        }
        return null;
    }

    public async Task CreateLeituraAsync(Leitura leitura)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO LEITURAS (ID, SENSOR_ID, NIVEL_AGUA, VELOCIDADE_CHUVA, TIMESTAMP_LEITURA) VALUES (:id, :sensorId, :nivel, :velocidade, :timestamp)";
        cmd.Parameters.Add(new OracleParameter("id", leitura.Id.ToString()));
        cmd.Parameters.Add(new OracleParameter("sensorId", leitura.SensorId.ToString()));
        cmd.Parameters.Add(new OracleParameter("nivel", leitura.NivelAgua));
        cmd.Parameters.Add(new OracleParameter("velocidade", leitura.VelocidadeChuva));
        cmd.Parameters.Add(new OracleParameter("timestamp", leitura.Timestamp));
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<Alerta>> GetAllAlertasAsync()
    {
        var alertas = new List<Alerta>();
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ID, LOCAL, NIVEL_RISCO, ACAO_RECOMENDADA FROM ALERTAS";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            alertas.Add(new Alerta
            {
                Id = Guid.Parse(reader.GetString(0)),
                Local = reader.GetString(1),
                NivelRisco = reader.GetString(2),
                AcaoRecomendada = reader.GetString(3)
            });
        }
        return alertas;
    }

    public async Task<Alerta?> GetAlertaByIdAsync(Guid id)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ID, LOCAL, NIVEL_RISCO, ACAO_RECOMENDADA FROM ALERTAS WHERE ID = :id";
        cmd.Parameters.Add(new OracleParameter("id", id.ToString()));
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
        if (await reader.ReadAsync())
        {
            return new Alerta
            {
                Id = Guid.Parse(reader.GetString(0)),
                Local = reader.GetString(1),
                NivelRisco = reader.GetString(2),
                AcaoRecomendada = reader.GetString(3)
            };
        }
        return null;
    }

    public async Task CreateAlertaAsync(Alerta alerta)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO ALERTAS (ID, LOCAL, NIVEL_RISCO, ACAO_RECOMENDADA) VALUES (:id, :local, :nivel, :acao)";
        cmd.Parameters.Add(new OracleParameter("id", alerta.Id.ToString()));
        cmd.Parameters.Add(new OracleParameter("local", alerta.Local));
        cmd.Parameters.Add(new OracleParameter("nivel", alerta.NivelRisco));
        cmd.Parameters.Add(new OracleParameter("acao", alerta.AcaoRecomendada));
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<Usuario>> GetAllUsuariosAsync()
    {
        var usuarios = new List<Usuario>();
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ID, NOME, EMAIL, ROLE FROM USUARIOS";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            usuarios.Add(new Usuario
            {
                Id = Guid.Parse(reader.GetString(0)),
                Nome = reader.GetString(1),
                Email = reader.GetString(2),
                Role = reader.GetString(3)
            });
        }
        return usuarios;
    }

    public async Task<Usuario?> GetUsuarioByIdAsync(Guid id)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ID, NOME, EMAIL, ROLE FROM USUARIOS WHERE ID = :id";
        cmd.Parameters.Add(new OracleParameter("id", id.ToString()));
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
        if (await reader.ReadAsync())
        {
            return new Usuario
            {
                Id = Guid.Parse(reader.GetString(0)),
                Nome = reader.GetString(1),
                Email = reader.GetString(2),
                Role = reader.GetString(3)
            };
        }
        return null;
    }

    public async Task CreateUsuarioAsync(Usuario usuario)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO USUARIOS (ID, NOME, EMAIL, ROLE) VALUES (:id, :nome, :email, :role)";
        cmd.Parameters.Add(new OracleParameter("id", usuario.Id.ToString()));
        cmd.Parameters.Add(new OracleParameter("nome", usuario.Nome));
        cmd.Parameters.Add(new OracleParameter("email", usuario.Email));
        cmd.Parameters.Add(new OracleParameter("role", usuario.Role));
        await cmd.ExecuteNonQueryAsync();
    }
}
