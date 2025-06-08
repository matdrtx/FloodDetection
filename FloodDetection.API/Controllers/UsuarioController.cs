using FloodDetection.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using FloodDetection.API.Services;

namespace FloodDetection.API.Controllers;

[ApiController]
[Route("api/v1/usuarios")]
public class UsuarioController : ControllerBase
{
    private readonly IDataService _dataService;

    public UsuarioController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await _dataService.GetAllUsuariosAsync();
        var result = usuarios.Select(u => new
        {
            u,
            links = new[]
            {
                new { rel = "self", href = Url.Action(nameof(GetById), new { id = u.Id }) }
            }
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var usuario = await _dataService.GetUsuarioByIdAsync(id);
        return usuario == null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Usuario usuario)
    {
        usuario.Id = Guid.NewGuid();
        await _dataService.CreateUsuarioAsync(usuario);
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }
}
