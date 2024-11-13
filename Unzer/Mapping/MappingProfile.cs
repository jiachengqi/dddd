using System;
using AutoMapper;
using Unzer.Data;
using Unzer.Data.DTO;

namespace Unzer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CompanyDTO, Company>().ReverseMap();
            CreateMap<OwnerDTO, Owner>().ReverseMap();
        }
    }
}

