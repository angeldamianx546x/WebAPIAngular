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

        // GET: api/Compras - Listar todas las compras
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var compras = await db.Compras
                .Include(x => x.Usuario)
                .Include(x => x.Videojuego)
                .Select(x => new
                {
                    id = x.Id,
                    usuarioId = x.UsuarioId,
                    usuario = x.Usuario.Nombre,
                    videojuegoId = x.VideojuegoId,
                    videojuego = x.Videojuego.Nombre,
                    precio = x.Videojuego.Precio,
                    fechaCompra = x.FechaCompra
                })
                .OrderByDescending(x => x.fechaCompra)
                .ToListAsync();

            return Ok(compras);
        }

        // POST: api/Compras - Realizar una compra
        [HttpPost]
        public async Task<IActionResult> Comprar([FromBody] CompraDTOCrear dto)
        {
            // Validar que el usuario exista
            var usuario = await db.Usuarios.FindAsync(dto.UsuarioId);
            if (usuario == null)
                return BadRequest("El usuario no existe.");

            // Validar que el videojuego exista
            var videojuego = await db.Videojuegos.FindAsync(dto.VideojuegoId);
            if (videojuego == null)
                return BadRequest("El videojuego no existe.");

            // Validar que no haya comprado el juego previamente
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
            
            // Incrementar contador de descargas
            videojuego.Descargas++;
            
            await db.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Compra realizada con éxito",
                compraId = compra.Id,
                fechaCompra = compra.FechaCompra
            });
        }

        // GET: api/Compras/Usuario/5 - Compras de un usuario específico
        [HttpGet("Usuario/{idUsuario:int}")]
        public async Task<IActionResult> ComprasUsuario(int idUsuario)
        {
            var usuario = await db.Usuarios.FindAsync(idUsuario);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            var compras = await db.Compras
                .Where(x => x.UsuarioId == idUsuario)
                .Include(x => x.Videojuego)
                .Select(x => new
                {
                    compraId = x.Id,
                    videojuego = new
                    {
                        id = x.Videojuego.Id,
                        nombre = x.Videojuego.Nombre,
                        descripcion = x.Videojuego.Descripcion,
                        tamanoGb = x.Videojuego.TamanoGb,
                        urlImg = x.Videojuego.UrlImg,
                        precio = x.Videojuego.Precio
                    },
                    fechaCompra = x.FechaCompra
                })
                .OrderByDescending(x => x.fechaCompra)
                .ToListAsync();

            return Ok(compras);
        }

        // GET: api/Compras/Videojuego/5 - Compras de un videojuego
        [HttpGet("Videojuego/{idVideojuego:int}")]
        public async Task<IActionResult> ComprasVideojuego(int idVideojuego)
        {
            var videojuego = await db.Videojuegos.FindAsync(idVideojuego);
            if (videojuego == null)
                return NotFound("Videojuego no encontrado.");

            var compras = await db.Compras
                .Where(x => x.VideojuegoId == idVideojuego)
                .Include(x => x.Usuario)
                .Select(x => new
                {
                    compraId = x.Id,
                    usuario = new
                    {
                        id = x.Usuario.Id,
                        nombre = x.Usuario.Nombre,
                        correo = x.Usuario.Correo
                    },
                    fechaCompra = x.FechaCompra
                })
                .OrderByDescending(x => x.fechaCompra)
                .ToListAsync();

            return Ok(compras);
        }

        // GET: api/Compras/5 - Obtener una compra específica
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCompra(int id)
        {
            var compra = await db.Compras
                .Include(x => x.Usuario)
                .Include(x => x.Videojuego)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (compra == null)
                return NotFound();

            var resultado = new
            {
                id = compra.Id,
                usuario = new
                {
                    id = compra.Usuario.Id,
                    nombre = compra.Usuario.Nombre,
                    correo = compra.Usuario.Correo
                },
                videojuego = new
                {
                    id = compra.Videojuego.Id,
                    nombre = compra.Videojuego.Nombre,
                    precio = compra.Videojuego.Precio,
                    urlImg = compra.Videojuego.UrlImg
                },
                fechaCompra = compra.FechaCompra
            };

            return Ok(resultado);
        }

        // DELETE: api/Compras/5 - Eliminar compra (reembolso)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var compra = await db.Compras
                .Include(x => x.Videojuego)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (compra == null)
                return NotFound();

            // Decrementar contador de descargas
            if (compra.Videojuego.Descargas > 0)
                compra.Videojuego.Descargas--;

            db.Compras.Remove(compra);
            await db.SaveChangesAsync();

            return Ok(new { mensaje = "Compra eliminada (reembolso procesado)" });
        }
    }
}