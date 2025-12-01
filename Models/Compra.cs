using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAngular.Models
{
    public class Compra
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public int VideojuegoId { get; set; }

        public DateTime FechaCompra { get; set; }

        public virtual Usuario Usuario { get; set; } = null!;

        public virtual Videojuego Videojuego { get; set; } = null!;
    }
}