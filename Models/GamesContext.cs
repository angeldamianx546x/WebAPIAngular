using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebAPIAngular.Models;

public partial class GamesContext : DbContext
{
    public GamesContext()
    {
    }

    public GamesContext(DbContextOptions<GamesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Rol> Roles { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Videojuego> Videojuegos { get; set; }
    public virtual DbSet<UsuarioVideojuego> UsuarioVideojuegos { get; set; }
    public virtual DbSet<Compra> Compras { get; set; }
    public virtual DbSet<Comentario> Comentarios { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseNpgsql("Host=localhost;Database=DBEscuela;Username=postgres;Password=19964");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasColumnName("nombre_rol").HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50);
            entity.Property(e => e.Correo).HasColumnName("correo").HasMaxLength(150);
            entity.Property(e => e.Contrasena).HasColumnName("contrasena").HasMaxLength(255);
            entity.Property(e => e.RolId).HasColumnName("rol_id");

            entity.HasOne(d => d.Rol)
                  .WithMany(p => p.Usuarios)
                  .HasForeignKey(d => d.RolId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Videojuego>(entity =>
        {
            entity.ToTable("videojuegos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200);
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.TamanoGb).HasColumnName("tamano_gb");
            entity.Property(e => e.Descargas).HasColumnName("descargas");
            entity.Property(e => e.UrlImg).HasColumnName("url_img");
            entity.Property(e => e.Precio).HasColumnName("precio");
            entity.Property(e => e.CreadoEn).HasColumnName("creado_en");
        });

         modelBuilder.Entity<UsuarioVideojuego>(entity =>
        {
            entity.ToTable("usuario_videojuego");

            entity.HasKey(e => new { e.UsuarioId, e.VideojuegoId });

            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            entity.Property(e => e.VideojuegoId).HasColumnName("videojuego_id");

            entity.HasOne(d => d.Usuario)
                  .WithMany(p => p.UsuarioVideojuegos)
                  .HasForeignKey(d => d.UsuarioId);

            entity.HasOne(d => d.Videojuego)
                  .WithMany(p => p.UsuarioVideojuegos)
                  .HasForeignKey(d => d.VideojuegoId);
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.ToTable("compras");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            entity.Property(e => e.VideojuegoId).HasColumnName("videojuego_id");
            entity.Property(e => e.FechaCompra).HasColumnName("fecha_compra");

            entity.HasOne(d => d.Usuario)
                  .WithMany(p => p.Compras)
                  .HasForeignKey(d => d.UsuarioId);

            entity.HasOne(d => d.Videojuego)
                  .WithMany(p => p.Compras)
                  .HasForeignKey(d => d.VideojuegoId);
        });

        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.ToTable("comentarios");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            entity.Property(e => e.VideojuegoId).HasColumnName("videojuego_id");
            entity.Property(e => e.ComentarioTexto).HasColumnName("comentario");
            entity.Property(e => e.Fecha).HasColumnName("fecha");

            entity.HasOne(d => d.Usuario)
                  .WithMany(p => p.Comentarios)
                  .HasForeignKey(d => d.UsuarioId);

            entity.HasOne(d => d.Videojuego)
                  .WithMany(p => p.Comentarios)
                  .HasForeignKey(d => d.VideojuegoId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
