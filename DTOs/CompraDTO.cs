using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAngular.DTOs
{
    public class CompraDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int VideojuegoId { get; set; }
        public DateTime FechaCompra { get; set; }
    }
}