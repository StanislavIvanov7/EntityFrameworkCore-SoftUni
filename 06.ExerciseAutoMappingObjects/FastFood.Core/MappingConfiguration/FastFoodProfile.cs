namespace FastFood.Core.MappingConfiguration
{
    using AutoMapper;
    using FastFood.Core.ViewModels.Categories;
    using FastFood.Core.ViewModels.Employees;
    using FastFood.Core.ViewModels.Items;
    using FastFood.Core.ViewModels.Orders;
    using FastFood.Models;
    using ViewModels.Positions;

    public class FastFoodProfile : Profile
    {
        public FastFoodProfile()
        {
            //Positions
            CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            //Category
            CreateMap<CreateCategoryInputModel, Category>()
                .ForMember(c => c.Name, x => x.MapFrom(cim => cim.CategoryName));
            CreateMap <Category, CreateItemViewModel>()
                .ForMember(cvm=>cvm.CategoryId ,x=>x.MapFrom (c=>c.Id  ));
            CreateMap<Category, CategoryAllViewModel>();
            //Position 
            CreateMap<Position , RegisterEmployeeViewModel>()
                .ForMember (revm=>revm.PositionId ,x=>x.MapFrom(p=>p.Id ));

            //Employee 
            CreateMap<RegisterEmployeeInputModel, Employee>();
                
            CreateMap<Employee, EmployeesAllViewModel>()
                .ForMember(evm=>evm.Position,x=>x.MapFrom (e=>e.Position .Name));

            CreateMap<Employee, CreateOrderViewModel>()
                .ForMember(cvm => cvm.Employees, x => x.MapFrom(e => e.Name));
            // item 
            CreateMap<CreateItemInputModel, Item>();
            CreateMap<Item, ItemsAllViewModels>()
                .ForMember (ivm=>ivm.Category ,x=>x.MapFrom (i=>i.Category .Name));
            //orderItem
            CreateMap<OrderItem, CreateOrderViewModel>()
                .ForMember(cvm => cvm.Items, x => x.MapFrom(oi => oi.Item.Name));
                
            CreateMap <Order ,OrderAllViewModel > ();
              
                
                
        }
    }
}
