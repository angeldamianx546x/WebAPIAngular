using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAngular.wwwroot
{
    public interface IAlmacenamiento
    {
        Task<string> AlmacenarImagen(string carpeta, IFormFile archivo);
        Task Eliminar(string? ruta, string carpeta);
        async Task<string> Editar(string? ruta, string carpeta, IFormFile archivo)
        {
            await Eliminar(ruta, carpeta);
            return await AlmacenarImagen(carpeta, archivo);
        }

    }
}