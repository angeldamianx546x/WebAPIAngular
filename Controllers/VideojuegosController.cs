using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAngular.DTOs;
using WebAPIAngular.Models;
using WebAPIAngular.wwwroot;

namespace WebAPIAngular.Controllers
{
    [ApiController]
    [Route("api/Videojuegos")]
    public class VideojuegosController : ControllerBase
    {
        private readonly GamesContext db;
        private readonly IMapper mapper;
        private readonly IAlmacenamiento almacenamiento;

        public VideojuegosController(GamesContext db, IMapper mapper, IAlmacenamiento almacenamiento)
        {
            this.db = db;
            this.mapper = mapper;
            this.almacenamiento = almacenamiento;
        }

        [HttpGet]
        public async Task<List<VideojuegoDTO>> Get()
        {
            var juegos = await db.Videojuegos.OrderBy(x => x.Id).ToListAsync();
            return mapper.Map<List<VideojuegoDTO>>(juegos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var juego = await db.Videojuegos.FirstOrDefaultAsync(x => x.Id == id);
            if (juego == null)
                return NotFound();

            return Ok(mapper.Map<VideojuegoDTO>(juego));
        }

        [HttpPost("crearConImagen")]
        public async Task<IActionResult> CrearVideojuego([FromForm] VideojuegoDTOCrear juegoForm)
        {
            string urlCaratula = "";

            if (juegoForm.UrlImg is not null)
            {
                urlCaratula = await SubirImagen(juegoForm.UrlImg);
            }

            var juego = new Videojuego
            {
                Nombre = juegoForm.Nombre,
                Descripcion = juegoForm.Descripcion,
                TamanoGb = juegoForm.TamanoGb,
                Precio = juegoForm.Precio,
                UrlImg = urlCaratula
            };

            await db.Videojuegos.AddAsync(juego);
            await db.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Videojuego creado correctamente",
                urlImagen = urlCaratula
            });
        }



        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existe = await db.Videojuegos.AnyAsync(x => x.Id == id);
            if (!existe)
                return NotFound();

            db.Remove(new Videojuego { Id = id });
            await db.SaveChangesAsync();
            return NoContent();
        }

        // Mostrar todos los videojuegos de un usuario (sus compras)
        [HttpGet("Usuario/{idUsuario:int}")]
        public async Task<IActionResult> JuegosDeUsuario(int idUsuario)
        {
            var juegos = await db.Compras
                .Where(x => x.Id == idUsuario)
                .Include(x => x.Videojuego)
                .Select(x => new
                {
                    x.Videojuego.Id,
                    x.Videojuego.Nombre,
                    x.Videojuego.Precio,
                    x.Videojuego.UrlImg
                })
                .ToListAsync();

            return Ok(juegos);
        }

        [HttpPost("subirImagen")]
        public async Task<string> SubirImagen(IFormFile archivo)
        {
            if (archivo == null)
                throw new Exception("No se envió ninguna imagen");

            string urlFoto = await almacenamiento.AlmacenarImagen("Caratulas", archivo);
            return urlFoto;
        }

    }
}
