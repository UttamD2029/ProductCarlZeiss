using AutoMapper;
using ProductZeissApi.Model.Domain;
using ProductZeissApi.Model.DTO;

namespace ProductZeissApi.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<AddProductDTO, Product>().ReverseMap();
            CreateMap<UpdateProductDTO, Product>().ReverseMap();

        }
    }
}
