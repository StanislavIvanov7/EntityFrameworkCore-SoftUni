using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Castle.Core.Resource;
using ProductShop.Utilities;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();
            //9
            //string input = File.ReadAllText("../../../Datasets/suppliers.xml");
            //Console.WriteLine(ImportSuppliers(context, input));

            //10
            //string input = File.ReadAllText("../../../Datasets/parts.xml");
            //Console.WriteLine(ImportParts(context, input));

            //11
            //string input = File.ReadAllText("../../../Datasets/cars.xml");
            //Console.WriteLine(ImportCars(context, input));

            //12
            //string input = File.ReadAllText("../../../Datasets/customers.xml");
            //Console.WriteLine(ImportCustomers(context, input));

            //13
            //string input = File.ReadAllText("../../../Datasets/sales.xml");
            //Console.WriteLine(ImportSales(context ,input));

            //14
            //Console.WriteLine(GetCarsWithDistance(context));

            //15
            //Console.WriteLine(GetCarsFromMakeBmw(context));

            //16
            //Console.WriteLine(GetLocalSuppliers(context));

            //17
            Console.WriteLine(GetCarsWithTheirListOfParts(context));
        }
        private static Mapper GetMapper()
        {
            var cfg = new MapperConfiguration(x => x.AddProfile<CarDealerProfile>());
            return new Mapper(cfg);
        }
        //9.
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SuppliersDTO[]), new XmlRootAttribute("Suppliers"));

            using StringReader stringReader = new StringReader(inputXml);

            SuppliersDTO[] suppliersDTOs = (SuppliersDTO[])xmlSerializer.Deserialize(stringReader);

            var mapper = GetMapper();

            var suppliers = mapper.Map<Supplier[]>(suppliersDTOs);

            context.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}";

        }
        //10
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PartsDTO[]), new XmlRootAttribute("Parts"));

            using StringReader reader = new StringReader(inputXml);

            PartsDTO[] partsDTOs = (PartsDTO[])xmlSerializer.Deserialize(reader);
            var mapper = GetMapper();
            var suppliersId = context.Suppliers
                .Select(x => x.Id)
                .ToList();
            var parts = mapper.Map<Part[]>(partsDTOs.Where(x => suppliersId.Contains(x.SupplierId)));
            context.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}";
        }
        //11
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CarsDTO[]), new XmlRootAttribute("Cars"));

            using StringReader reader = new StringReader(inputXml);

            CarsDTO[] carsDTOs = (CarsDTO[])xmlSerializer.Deserialize(reader);
            var mapper = GetMapper();
            List<Car> cars = new List<Car>();

            foreach (var item in carsDTOs)
            {
                var car = mapper.Map<Car>(item);
                int[] carPartsIds = item.PartsIds
                    .Select(x => x.Id)
                    .Distinct()
                    .ToArray();
                var partCars = new List<PartCar>();

                foreach (var id in carPartsIds)
                {
                    partCars.Add(new PartCar
                    {
                        Car = car,
                        PartId = id

                    });

                }
                car.PartsCars = partCars;
                cars.Add(car);

            }
            context.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count()}";
        }
        //12
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CustomerDTO[]), new XmlRootAttribute("Customers"));

            using StringReader stringReader = new StringReader(inputXml);

            CustomerDTO[] customerDTOs = (CustomerDTO[])xmlSerializer.Deserialize(stringReader);
            var mapper = GetMapper();
            var customers = mapper.Map<Customer[]>(customerDTOs);
            context.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Count()}";
        }
        //13
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SalesDTO[]), new XmlRootAttribute("Sales"));
            using StringReader stringReader = new StringReader(inputXml);
            SalesDTO[] salesDTOs = (SalesDTO[])xmlSerializer.Deserialize(stringReader);
            var mapper = GetMapper();
            var carId = context.Cars
                .Select(x => x.Id)
                .ToList();
            var sales = mapper.Map<Sale[]>(salesDTOs.Where(x => carId.Contains(x.CarId)));
            context.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}";

        }
        //14
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            Mapper mapper = GetMapper();
            var cars = context.Cars
               .Where(x => x.TraveledDistance > 2000000)
               .OrderBy(x => x.Make)
                    .ThenBy(x => x.Model)
               .Take(10)
               .ProjectTo<CarWithDistanceDTO>(mapper.ConfigurationProvider)
               .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CarWithDistanceDTO[]), new XmlRootAttribute("cars"));

            var xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, string.Empty);


            StringBuilder stringBuilder = new StringBuilder();

            using (StringWriter sw = new StringWriter(stringBuilder))
            {
                xmlSerializer.Serialize(sw, cars, xns);
            }
            return stringBuilder.ToString().TrimEnd();
        }
        //15
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {

            var mapper = GetMapper();
            var cars = context.Cars
                .Where(x => x.Make == "BMW")
                .OrderBy(x => x.Model)
                      .ThenByDescending(x => x.TraveledDistance)
                .ProjectTo<BmwCarsDTO>(mapper.ConfigurationProvider)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(BmwCarsDTO[]), new XmlRootAttribute("cars"));
            var xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, string.Empty);

            StringBuilder stringBuilder = new StringBuilder();
            using (StringWriter sw = new StringWriter(stringBuilder))
            {
                xmlSerializer.Serialize(sw, cars, xns);
            }
            return stringBuilder.ToString().TrimEnd();
        }
        //16
        public static string GetLocalSuppliers(CarDealerContext context)
        {

            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new SuppliersExportDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count,
                })

                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SuppliersExportDTO[]), new XmlRootAttribute("suppliers"));

            var xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, string.Empty);

            StringBuilder stringBuilder = new StringBuilder();
            using (StringWriter sw = new StringWriter(stringBuilder))
            {
                xmlSerializer.Serialize(sw, suppliers, xns);
            }

            return stringBuilder.ToString().TrimEnd();
        }
        //17.
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            
       
            var carsPartsDtos = context.Cars
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .Select(c => new ExportCarPartsDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance,
                    Parts = c.PartsCars.Select(pc => new PartsDto()
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    })
                    .OrderByDescending(p => p.Price)
                    .ToArray()
                })
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarPartsDto[]), new XmlRootAttribute ("cars"));

            XmlSerializerNamespaces  xns = new XmlSerializerNamespaces();
            xns.Add (string .Empty , string .Empty);

            StringBuilder stringBuilder = new StringBuilder();

            using (StringWriter sw = new StringWriter (stringBuilder))
            {
                xmlSerializer.Serialize(sw, carsPartsDtos,xns);
            }

            return stringBuilder.ToString ().TrimEnd ();
        }
        //18.Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var xmlParser = new XmlParser();

            //Finding the Sales
            var tempDto = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SalesInfo = c.Sales.Select(s => new
                    {
                        Prices = c.IsYoungDriver
                            ? s.Car.PartsCars.Sum(p => Math.Round((double)p.Part.Price * 0.95, 2))
                            : s.Car.PartsCars.Sum(p => (double)p.Part.Price)
                    }).ToArray(),
                })
                .ToArray();

            TotalSalesByCustomerDto[] totalSalesDtos = tempDto
                .OrderByDescending(t => t.SalesInfo.Sum(s => s.Prices))
                .Select(t => new TotalSalesByCustomerDto()
                {
                    FullName = t.FullName,
                    BoughtCars = t.BoughtCars,
                    SpentMoney = t.SalesInfo.Sum(s => s.Prices).ToString("f2")
                })
                .ToArray();

            //Output
            return xmlParser.Serialize<TotalSalesByCustomerDto[]>(totalSalesDtos, "customers");

        }

        //19.Export Sales With Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var xmlParser = new XmlParser();

            //Finding the Sales
            SalesWithAppliedDiscountDto[] salesDtos = context
                .Sales
                .Select(s => new SalesWithAppliedDiscountDto()
                {
                    SingleCar = new SingleCar()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    Discount = (int)s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartsCars.Sum(p => p.Part.Price),
                    PriceWithDiscount = Math.Round((double)(s.Car.PartsCars.Sum(p => p.Part.Price) * (1 - (s.Discount / 100))), 4)
                })
                .ToArray();

            //Output
            return xmlParser.Serialize<SalesWithAppliedDiscountDto[]>(salesDtos, "sales");
        }
    }
}
