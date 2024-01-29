namespace FastFood.Core.Controllers
{
    using System;
    using System.Linq;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using FastFood.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using ViewModels.Orders;

    public class OrdersController : Controller
    {
        private readonly FastFoodContext _context;
        private readonly IMapper _mapper;

        public OrdersController(FastFoodContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Create()
        {
            var viewOrder = new CreateOrderViewModel
            {
                Items = _context.Items.Select(x => x.Name).ToList(),
                Employees = _context.Employees.Select(x => x.Name).ToList(),
            };

            return View(viewOrder);
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(CreateOrderInputModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return RedirectToAction("Error", "Home");
        //    }
        //    //var orders = _context.Orders
        //    //    .Select(x => new CreateOrderInputModel
        //    //    {
        //    //        Customer = x.Customer,
        //    //        Item = x.OrderItems
        //    //    }); ; ;
        //    var order = _mapper.Map<Order>(model);
        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("All", "Orders");
        //}

        public async Task<IActionResult> All()
        {
            var orders = await _context.Orders
                .ProjectTo<OrderAllViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return View(orders);
        }
    }
}
