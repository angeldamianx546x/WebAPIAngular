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
    [Route("api/Compras")]
    public class ComprasController : ControllerBase
    {
        private readonly GamesContext db;

        public ComprasController(GamesContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Comprar([FromBody] CompraDTOCrear dto)
        {
            // Validar que el usuario exista
            var usuarioExiste = await db.Usuarios.AnyAsync(x => x.Id == dto.UsuarioId);
            if (!usuarioExiste)
                return BadRequest("El usuario no existe.");

            // Validar que el videojuego exista
            var juegoExiste = await db.Videojuegos.AnyAsync(x => x.Id == dto.VideojuegoId);
            if (!juegoExiste)
                return BadRequest("El videojuego no existe.");

            // Validar compra única
            var yaComprado = await db.Compras.AnyAsync(x =>
                x.UsuarioId == dto.UsuarioId &&
                x.VideojuegoId == dto.VideojuegoId);

            if (yaComprado)
                return BadRequest("Este videojuego ya fue comprado por este usuario.");

            var compra = new Compra
            {
                UsuarioId = dto.UsuarioId,
                VideojuegoId = dto.VideojuegoId,
                FechaCompra = DateTime.Now
            };

            db.Compras.Add(compra);
            await db.SaveChangesAsync();

            return Ok("Compra realizada con éxito.");
        }


        [HttpGet("Usuario/{idUsuario:int}")]
        public async Task<IActionResult> ComprasUsuario(int idUsuario)
        {
            var compras = await db.Compras
                .Where(x => x.UsuarioId == idUsuario)
                .Include(x => x.Videojuego)
                .ToListAsync();

            return Ok(compras);
        }
    }
}