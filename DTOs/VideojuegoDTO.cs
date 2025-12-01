using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAngular.DTOs
{
    public class VideojuegoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public float? TamanoGb { get; set; }
        public int Descargas { get; set; }
        public string? UrlImg { get; set; }
        public float? Precio {get; set;}
    }
}