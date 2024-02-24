using AutoMapper;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<UserDTO, User>();
            CreateMap<ProductDTO , Product>();  
            CreateMap <CategoriesDTO , Category>();
            CreateMap <CategoriesProductsDTO ,CategoryProduct >();
        }
    }
}
