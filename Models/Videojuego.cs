using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAngular.Models
{
    public class Videojuego
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public float? TamanoGb { get; set; }

        public int Descargas { get; set; }

        public string? UrlImg { get; set; }

        public DateTime CreadoEn { get; set; }
        public float? Precio {get; set;}

        public virtual ICollection<UsuarioVideojuego> UsuarioVideojuegos { get; set; } = new List<UsuarioVideojuego>();

        public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

        public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
    }
}