using AutoMapper;
using AutoMapper.Features;
using BLL.DTOs;
using BLL.DTOs.Auth;
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
        },
        NullLoggerFactory.Instance
    );

    public static IMapper GetMapper()
    {
        return config.CreateMapper();
    }
}