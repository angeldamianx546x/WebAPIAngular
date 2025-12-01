using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAngular.DTOs
{
    public class ComentarioDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int VideojuegoId { get; set; }
        public string Comentario { get; set; } = null!;
        public DateTime Fecha { get; set; }
    }
}