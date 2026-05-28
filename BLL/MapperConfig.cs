using AutoMapper;
using AutoMapper.Features;
using BLL.DTOs;
using BLL.DTOs.Auth;
using BLL.DTOs.Shared;
using DAL.EF.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Data;
using System.Numerics;

public class MapperConfig
{
    public static MapperConfiguration config = new MapperConfiguration(
        cfg =>
        {
            cfg.CreateMap<AppUser, UserDTO>().ReverseMap();
            cfg.CreateMap<Tenant, RegisterTenantDTO>().ReverseMap();
            cfg.CreateMap<AppUser, RegisterTenantDTO>().ReverseMap();
            cfg.CreateMap<AppUser, LoginDTO>().ReverseMap();
            cfg.CreateMap<AppUser, CreateUserDTO>().ReverseMap();
        },
        NullLoggerFactory.Instance
    );

    public static IMapper GetMapper()
    {
        return config.CreateMapper();
    }
}