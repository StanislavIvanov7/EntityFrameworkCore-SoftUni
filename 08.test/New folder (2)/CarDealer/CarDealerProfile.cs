using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<SuppliersDTO, Supplier>();
            CreateMap <PartsDTO , Part>();
            CreateMap <CarsDTO , Car>();
            CreateMap <CustomerDTO , Customer>();
            CreateMap<SalesDTO ,Sale >();


            CreateMap <Car ,CarWithDistanceDTO >();
            CreateMap <Car ,BmwCarsDTO >();
           

        }
    }
}
