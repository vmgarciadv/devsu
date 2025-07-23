using AutoMapper;
using devsu.DTOs;
using devsu.Models;

namespace devsu.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Cliente, ClienteDto>()
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Cuenta, CuentaDto>()
                .ForMember(dest => dest.NombreCliente, opt => opt.MapFrom(src => src.Cliente.Nombre));
            CreateMap<CuentaDto, Cuenta>();
        }
    }
}