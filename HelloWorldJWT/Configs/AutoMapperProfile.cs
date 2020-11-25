using AutoMapper;
using HelloWorldJWT.Entities;
using HelloWorldJWT.Models.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloWorldJWT.Configs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Person, PersonModel>();
            CreateMap<RegisterModel, Person>();
            CreateMap<UpdateModel, Person>();
        }
    }
}
