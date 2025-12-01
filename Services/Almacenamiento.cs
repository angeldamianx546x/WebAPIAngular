using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAngular.wwwroot
{

    public class Almacenamiento : IAlmacenamiento
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public Almacenamiento(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> AlmacenarImagen(string contenedor, IFormFile archivo)
        {
            var extencion = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extencion}";
            var carpeta = Path.Combine(env.WebRootPath, contenedor);
            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);
            string ruta = Path.Combine(carpeta, nombreArchivo);
            using (var ms = new MemoryStream())
            {
                await archivo.CopyToAsync(ms);
                var contenido = ms.ToArray();
                await File.WriteAllBytesAsync(ruta,contenido);
            }
            var request = httpContextAccessor.HttpContext!.Request;
            var url = $"{request.Scheme}://{request.Host}";
            var archivoURL = Path.Combine(url, contenedor, nombreArchivo);
            return archivoURL;
        }

        
        public Task Eliminar(string? ruta, string contenedor)
        {
            if (string.IsNullOrWhiteSpace(ruta))
                return Task.CompletedTask;
            var nombreArchivo = Path.GetFileName(ruta);
            var directorio = Path.Combine(env.WebRootPath, contenedor, nombreArchivo);
            if(File.Exists(directorio))
                File.Delete(directorio);
            return Task.CompletedTask;
        }
    }
}