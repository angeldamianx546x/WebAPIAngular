using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAngular.DTOs
{
    public class UsuarioDTOCrear
    {
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string? Contrasena{ get; set; }
        public int RolId{get; set;}
    }
}