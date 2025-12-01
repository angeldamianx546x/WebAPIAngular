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
    [Route("api/Comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly GamesContext db;

        public ComentariosController(GamesContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Comentario comentario)
        {
            var juegoExiste = await db.Videojuegos.AnyAsync(x => x.Id == comentario.VideojuegoId);
            if (!juegoExiste)
                return BadRequest("El videojuego no existe.");

            await db.Comentarios.AddAsync(comentario);
            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("Videojuego/{idJuego:int}")]
        public async Task<IActionResult> ComentariosDeJuego(int idJuego)
        {
            var comentarios = await db.Comentarios
                .Where(x => x.VideojuegoId == idJuego)
                .Include(x => x.Usuario)
                .Select(x => new
                {
                    usuario = x.Usuario.Nombre,
                    x.ComentarioTexto,
                    x.Fecha
                })
                .ToListAsync();

            return Ok(comentarios);
        }
    }
}