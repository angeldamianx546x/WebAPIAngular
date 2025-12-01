using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAngular.Models
{
    public class UsuarioVideojuego
    {
        public int UsuarioId { get; set; }

        public int VideojuegoId { get; set; }

        public virtual Usuario Usuario { get; set; } = null!;

        public virtual Videojuego Videojuego { get; set; } = null!;
    }
}