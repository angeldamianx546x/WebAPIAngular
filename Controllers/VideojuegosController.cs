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

        // GET: api/Videojuegos - Lista todos los videojuegos
        [HttpGet]
        public async Task<List<VideojuegoDTO>> Get()
        {
            var juegos = await db.Videojuegos.OrderBy(x => x.Id).ToListAsync();
            return mapper.Map<List<VideojuegoDTO>>(juegos);
        }

        // GET: api/Videojuegos/5 - Obtener un videojuego
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var juego = await db.Videojuegos.FirstOrDefaultAsync(x => x.Id == id);
            if (juego == null)
                return NotFound();

            return Ok(mapper.Map<VideojuegoDTO>(juego));
        }

        // GET: api/Videojuegos/Detalle/5 - Videojuego con comentarios
        [HttpGet("Detalle/{id:int}")]
        public async Task<IActionResult> GetDetalle(int id)
        {
            var juego = await db.Videojuegos
                .Include(x => x.Comentarios)
                    .ThenInclude(c => c.Usuario)
                .Include(x => x.Compras)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (juego == null)
                return NotFound();

            var resultado = new
            {
                id = juego.Id,
                nombre = juego.Nombre,
                descripcion = juego.Descripcion,
                tamanoGb = juego.TamanoGb,
                descargas = juego.Descargas,
                urlImg = juego.UrlImg,
                precio = juego.Precio,
                creadoEn = juego.CreadoEn,
                totalCompras = juego.Compras.Count,
                comentarios = juego.Comentarios.Select(c => new
                {
                    id = c.Id,
                    usuario = c.Usuario.Nombre,
                    usuarioId = c.UsuarioId,
                    comentario = c.ComentarioTexto,
                    fecha = c.Fecha
                }).OrderByDescending(c => c.fecha).ToList()
            };

            return Ok(resultado);
        }

        // POST: api/Videojuegos/crearConImagen - Crear videojuego con imagen
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
                UrlImg = urlCaratula,
                Descargas = juegoForm.Descargas ?? 0,
                CreadoEn = DateTime.Now
            };

            await db.Videojuegos.AddAsync(juego);
            await db.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Videojuego creado correctamente",
                id = juego.Id,
                urlImagen = urlCaratula
            });
        }

        // PUT: api/Videojuegos/5/actualizarConImagen
        [HttpPut("{id:int}/actualizarConImagen")]
        public async Task<IActionResult> ActualizarVideojuego(int id, [FromForm] VideojuegoDTOCrear juegoForm)
        {
            var juego = await db.Videojuegos.FindAsync(id);
            if (juego == null)
                return NotFound();

            // Actualizar imagen si se proporciona una nueva
            if (juegoForm.UrlImg != null)
            {
                // Eliminar imagen anterior
                await almacenamiento.Eliminar(juego.UrlImg, "Caratulas");
                // Subir nueva imagen
                juego.UrlImg = await SubirImagen(juegoForm.UrlImg);
            }

            juego.Nombre = juegoForm.Nombre;
            juego.Descripcion = juegoForm.Descripcion;
            juego.TamanoGb = juegoForm.TamanoGb;
            juego.Precio = juegoForm.Precio;

            await db.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Videojuego actualizado correctamente",
                urlImagen = juego.UrlImg
            });
        }

        // DELETE: api/Videojuegos/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var juego = await db.Videojuegos.FindAsync(id);
            if (juego == null)
                return NotFound();

            // Eliminar imagen asociada
            await almacenamiento.Eliminar(juego.UrlImg, "Caratulas");

            db.Videojuegos.Remove(juego);
            await db.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Videojuegos/Usuario/5 - Videojuegos comprados por usuario
        [HttpGet("Usuario/{idUsuario:int}")]
        public async Task<IActionResult> JuegosDeUsuario(int idUsuario)
        {
            var juegos = await db.Compras
                .Where(x => x.UsuarioId == idUsuario)
                .Include(x => x.Videojuego)
                .Select(x => new
                {
                    id = x.Videojuego.Id,
                    nombre = x.Videojuego.Nombre,
                    descripcion = x.Videojuego.Descripcion,
                    precio = x.Videojuego.Precio,
                    urlImg = x.Videojuego.UrlImg,
                    fechaCompra = x.FechaCompra
                })
                .ToListAsync();

            return Ok(juegos);
        }

        // POST: api/Videojuegos/subirImagen - Subir imagen standalone
        [HttpPost("subirImagen")]
        public async Task<IActionResult> SubirImagenEndpoint(IFormFile archivo)
        {
            if (archivo == null)
                return BadRequest("No se envió ninguna imagen");

            try
            {
                string urlFoto = await SubirImagen(archivo);
                return Ok(new { url = urlFoto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Método privado para subir imágenes
        private async Task<string> SubirImagen(IFormFile archivo)
        {
            if (archivo == null)
                throw new Exception("No se envió ninguna imagen");

            string urlFoto = await almacenamiento.AlmacenarImagen("Caratulas", archivo);
            return urlFoto;
        }
    }
}