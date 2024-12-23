﻿using AutoMapper;
using Unzer.Data;
using Unzer.Data.DTO;

namespace Unzer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDTO>().ReverseMap();
            CreateMap<Owner, OwnerDTO>().ReverseMap();
        }
    }
}

