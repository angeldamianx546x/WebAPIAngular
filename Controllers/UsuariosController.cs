using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAngular.DTOs;
using WebAPIAngular.Models;

namespace WebAPIAngular.Controllers
{
    [ApiController]
    [Route("api/Usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly GamesContext db;
        private readonly IMapper mapper;

        public UsuariosController(GamesContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        // GET: api/Usuarios - Lista simple de usuarios
        [HttpGet]
        public async Task<List<UsuarioDTO>> Get()
        {
            var usuarios = await db.Usuarios
                .Include(x => x.Rol)
                .OrderBy(x => x.Id)
                .ToListAsync();
            return mapper.Map<List<UsuarioDTO>>(usuarios);
        }

        // GET: api/Usuarios/5 - Usuario individual
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var usuario = await db.Usuarios
                .Include(x => x.Rol)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (usuario == null)
                return NotFound();

            return Ok(mapper.Map<UsuarioDTO>(usuario));
        }

        // GET: api/Usuarios/Detalle/5 - Usuario con sus videojuegos y compras
        [HttpGet("Detalle/{id:int}")]
        public async Task<IActionResult> GetUsuarioDetalle(int id)
        {
            var usuario = await db.Usuarios
                .Include(x => x.Rol)
                .Include(x => x.Compras)
                    .ThenInclude(c => c.Videojuego)
                .Include(x => x.Comentarios)
                    .ThenInclude(c => c.Videojuego)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (usuario == null)
                return NotFound();

            var resultado = new
            {
                id = usuario.Id,
                nombre = usuario.Nombre,
                correo = usuario.Correo,
                rol = usuario.Rol != null ? new
                {
                    id = usuario.Rol.Id,
                    nombre = usuario.Rol.Nombre,
                    descripcion = usuario.Rol.Descripcion
                } : null,
                videojuegosComprados = usuario.Compras.Select(c => new
                {
                    compraId = c.Id,
                    videojuegoId = c.Videojuego.Id,
                    nombre = c.Videojuego.Nombre,
                    precio = c.Videojuego.Precio,
                    urlImg = c.Videojuego.UrlImg,
                    fechaCompra = c.FechaCompra
                }).ToList(),
                comentarios = usuario.Comentarios.Select(c => new
                {
                    comentarioId = c.Id,
                    videojuegoId = c.Videojuego.Id,
                    nombreVideojuego = c.Videojuego.Nombre,
                    comentario = c.ComentarioTexto,
                    fecha = c.Fecha
                }).ToList()
            };

            return Ok(resultado);
        }

        // POST: api/Usuarios - Crear usuario
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UsuarioDTOCrear usuarioDTO)
        {
            // Validar que el rol existe
            var rolExiste = await db.Roles.AnyAsync(x => x.Id == usuarioDTO.RolId);
            if (!rolExiste)
                return BadRequest("El rol especificado no existe.");

            var usuario = mapper.Map<Usuario>(usuarioDTO);
            await db.Usuarios.AddAsync(usuario);
            await db.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, mapper.Map<UsuarioDTO>(usuario));
        }

        // PUT: api/Usuarios/5 - Actualizar usuario
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] UsuarioDTOCrear usuarioDTO)
        {
            var usuarioExistente = await db.Usuarios.FindAsync(id);
            if (usuarioExistente == null)
                return NotFound();

            // Validar que el rol existe
            var rolExiste = await db.Roles.AnyAsync(x => x.Id == usuarioDTO.RolId);
            if (!rolExiste)
                return BadRequest("El rol especificado no existe.");

            usuarioExistente.Nombre = usuarioDTO.Nombre ?? usuarioExistente.Nombre;
            usuarioExistente.Correo = usuarioDTO.Correo ?? usuarioExistente.Correo;
            usuarioExistente.Contrasena = usuarioDTO.Contrasena ?? usuarioExistente.Contrasena;
            usuarioExistente.RolId = usuarioDTO.RolId;

            await db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await db.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            db.Usuarios.Remove(usuario);
            await db.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Usuarios/ConRol - Lista usuarios con información de rol
        [HttpGet("ConRol")]
        public async Task<IActionResult> GetUsuariosConRol()
        {
            var usuarios = await db.Usuarios
                .Include(x => x.Rol)
                .OrderBy(x => x.Id)
                .Select(x => new
                {
                    id = x.Id,
                    nombre = x.Nombre,
                    correo = x.Correo,
                    rol = x.Rol != null ? new
                    {
                        id = x.Rol.Id,
                        nombre = x.Rol.Nombre,
                        descripcion = x.Rol.Descripcion
                    } : null
                })
                .ToListAsync();

            return Ok(usuarios);
        }
    }
}