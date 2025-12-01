using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAngular.Models;

namespace WebAPIAngular.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public RolDTO? Rol { get; set; }
    }
}