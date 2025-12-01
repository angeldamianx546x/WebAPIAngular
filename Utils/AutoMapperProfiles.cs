using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WebAPIAngular.DTOs;
using WebAPIAngular.Models;

namespace WebAPIAngular.Utils
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Rol, RolDTO>().ReverseMap();

            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(
                    dest => dest.Rol,
                    opt => opt.MapFrom(src => src.Rol)
                );

            CreateMap<UsuarioDTOCrear, Usuario>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Videojuego, VideojuegoDTO>().ReverseMap();

            CreateMap<Comentario, ComentarioDTO>()
                .ForMember(dest => dest.Comentario, opt => opt.MapFrom(src => src.ComentarioTexto));

            CreateMap<ComentarioDTOCrear, Comentario>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Fecha, opt => opt.Ignore())
                .ForMember(dest => dest.ComentarioTexto, opt => opt.MapFrom(src => src.Comentario));

            CreateMap<Compra, CompraDTO>();
            CreateMap<CompraDTOCrear, Compra>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCompra, opt => opt.Ignore());

            CreateMap<UsuarioVideojuegoDTO, UsuarioVideojuego>().ReverseMap();
        }
    }
}