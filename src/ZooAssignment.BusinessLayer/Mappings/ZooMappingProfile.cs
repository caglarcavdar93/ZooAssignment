using AutoMapper;
using ZooAssignment.BusinessLayer.DTOs;
using ZooAssignment.DataAccessLayer.Models;

namespace ZooAssignment.BusinessLayer.Mappings
{
    public class ZooMappingProfile : Profile
    {
        public ZooMappingProfile()
        {
            CreateMap<Animal, AnimalDto>().ReverseMap();
            CreateMap<FoodPrice, FoodPriceDto>().ReverseMap();
        }
    }
}
