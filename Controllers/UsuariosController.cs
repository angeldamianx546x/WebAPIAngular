using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAngular.DTOs;
using WebAPIAngular.Models;
using WebAPIAngular.wwwroot;

namespace WebAPIAngular.Controllers
{
    [ApiController]
    [Route("api/Usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly GamesContext db;
        private readonly IMapper mapper;

        public UsuariosController(GamesContext db, IMapper mapper, IAlmacenamiento almacenamiento)
        {
            this.db = db;
            this.mapper = mapper;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<List<UsuarioDTO>> Get()
        {
            var usuarios = await db.Usuarios.OrderBy(x => x.Id).ToListAsync();
            return mapper.Map<List<UsuarioDTO>>(usuarios);
        }

        // GET: api/Usuarios/Buscar/5
        [HttpGet("Buscar/{id:int}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var usuario = await db.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
            if (usuario == null)
                return NotFound();

            return Ok(mapper.Map<UsuarioDTO>(usuario));
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UsuarioDTOCrear usuarioDTO)
        {
            var usuario = mapper.Map<Usuario>(usuarioDTO);
            await db.Usuarios.AddAsync(usuario);
            await db.SaveChangesAsync();
            return Ok();
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] UsuarioDTO usuarioDTO)
        {
            var existe = await db.Usuarios.AnyAsync(x => x.Id == id);
            if (!existe)
                return NotFound();

            var usuario = mapper.Map<Usuario>(usuarioDTO);
            usuario.Id = id;

            db.Update(usuario);
            await db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existe = await db.Usuarios.AnyAsync(x => x.Id == id);
            if (!existe)
                return NotFound();

            db.Remove(new Usuario { Id = id });
            await db.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Usuarios/MostrarTodo
        [HttpGet("MostrarTodo")]
        public async Task<IActionResult> MostrarTodo()
        {
            return Ok(await db.Usuarios
        .Include(x => x.Rol) // ⭐ Necesario para cargar el rol
        .Select(x => new
        {
            id = x.Id,
            nombre = x.Nombre,
            correo = x.Correo,
            rol = x.Rol.Nombre   // ⭐ Así obtienes el nombre del rol
        })
        .ToListAsync());
        }

    }
}