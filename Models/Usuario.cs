using System;
using System.Collections.Generic;

namespace WebAPIAngular.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public int? RolId { get; set; }

    public virtual Rol? Rol { get; set; }

    public virtual ICollection<UsuarioVideojuego> UsuarioVideojuegos { get; set; } = new List<UsuarioVideojuego>();

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
}
