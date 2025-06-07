using FloodDetection.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FloodDetection.API.Controllers;

[ApiController]
[Route("api/v1/usuarios")]
public class UsuarioController : ControllerBase
{
    private static readonly List<Usuario> usuarios = new();

    [HttpGet]
    public IActionResult GetAll()
    {
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
    public IActionResult GetById(Guid id)
    {
        var usuario = usuarios.FirstOrDefault(u => u.Id == id);
        return usuario == null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    public IActionResult Create(Usuario usuario)
    {
        usuario.Id = Guid.NewGuid();
        usuarios.Add(usuario);
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }
}
