using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAngular.DTOs;
using WebAPIAngular.Models;

namespace WebAPIAngular.Controllers
{
    [ApiController]
    [Route("api/Comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly GamesContext db;

        public ComentariosController(GamesContext db)
        {
            this.db = db;
        }

        // GET: api/Comentarios - Listar todos los comentarios
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comentarios = await db.Comentarios
                .Include(x => x.Usuario)
                .Include(x => x.Videojuego)
                .OrderByDescending(x => x.Fecha)
                .Select(x => new
                {
                    id = x.Id,
                    usuario = x.Usuario.Nombre,
                    usuarioId = x.UsuarioId,
                    videojuego = x.Videojuego.Nombre,
                    videojuegoId = x.VideojuegoId,
                    comentario = x.ComentarioTexto,
                    fecha = x.Fecha
                })
                .ToListAsync();

            return Ok(comentarios);
        }

        // POST: api/Comentarios - Crear comentario
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ComentarioDTOCrear comentarioDTO)
        {
            // Validar que el usuario existe
            var usuarioExiste = await db.Usuarios.AnyAsync(x => x.Id == comentarioDTO.UsuarioId);
            if (!usuarioExiste)
                return BadRequest("El usuario no existe.");

            // Validar que el videojuego existe
            var juegoExiste = await db.Videojuegos.AnyAsync(x => x.Id == comentarioDTO.VideojuegoId);
            if (!juegoExiste)
                return BadRequest("El videojuego no existe.");

            var comentario = new Comentario
            {
                UsuarioId = comentarioDTO.UsuarioId,
                VideojuegoId = comentarioDTO.VideojuegoId,
                ComentarioTexto = comentarioDTO.Comentario,
                Fecha = DateTime.UtcNow
            };

            await db.Comentarios.AddAsync(comentario);
            await db.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Comentario creado exitosamente",
                comentarioId = comentario.Id,
                fecha = comentario.Fecha
            });
        }

        // GET: api/Comentarios/5 - Obtener comentario específico
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var comentario = await db.Comentarios
                .Include(x => x.Usuario)
                .Include(x => x.Videojuego)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (comentario == null)
                return NotFound();

            var resultado = new
            {
                id = comentario.Id,
                usuario = new
                {
                    id = comentario.Usuario.Id,
                    nombre = comentario.Usuario.Nombre
                },
                videojuego = new
                {
                    id = comentario.Videojuego.Id,
                    nombre = comentario.Videojuego.Nombre
                },
                comentario = comentario.ComentarioTexto,
                fecha = comentario.Fecha
            };

            return Ok(resultado);
        }

        // GET: api/Comentarios/Videojuego/5 - Comentarios de un videojuego
        [HttpGet("Videojuego/{idJuego:int}")]
        public async Task<IActionResult> ComentariosDeJuego(int idJuego)
        {
            var juegoExiste = await db.Videojuegos.AnyAsync(x => x.Id == idJuego);
            if (!juegoExiste)
                return NotFound("Videojuego no encontrado.");

            var comentarios = await db.Comentarios
                .Where(x => x.VideojuegoId == idJuego)
                .Include(x => x.Usuario)
                .OrderByDescending(x => x.Fecha)
                .Select(x => new
                {
                    id = x.Id,
                    usuario = x.Usuario.Nombre,
                    usuarioId = x.UsuarioId,
                    comentario = x.ComentarioTexto,
                    fecha = x.Fecha
                })
                .ToListAsync();

            return Ok(comentarios);
        }

        // GET: api/Comentarios/Usuario/5 - Comentarios de un usuario
        [HttpGet("Usuario/{idUsuario:int}")]
        public async Task<IActionResult> ComentariosDeUsuario(int idUsuario)
        {
            var usuarioExiste = await db.Usuarios.AnyAsync(x => x.Id == idUsuario);
            if (!usuarioExiste)
                return NotFound("Usuario no encontrado.");

            var comentarios = await db.Comentarios
                .Where(x => x.UsuarioId == idUsuario)
                .Include(x => x.Videojuego)
                .OrderByDescending(x => x.Fecha)
                .Select(x => new
                {
                    id = x.Id,
                    videojuego = x.Videojuego.Nombre,
                    videojuegoId = x.VideojuegoId,
                    comentario = x.ComentarioTexto,
                    fecha = x.Fecha
                })
                .ToListAsync();

            return Ok(comentarios);
        }

        // PUT: api/Comentarios/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] string nuevoComentario)
        {
            var comentario = await db.Comentarios.FindAsync(id);
            if (comentario == null)
                return NotFound();

            comentario.ComentarioTexto = nuevoComentario;
            await db.SaveChangesAsync();

            return Ok(new { mensaje = "Comentario actualizado" });
        }

        // DELETE: api/Comentarios/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var comentario = await db.Comentarios.FindAsync(id);
            if (comentario == null)
                return NotFound();

            db.Comentarios.Remove(comentario);
            await db.SaveChangesAsync();

            return Ok(new { mensaje = "Comentario eliminado" });
        }
    }
}