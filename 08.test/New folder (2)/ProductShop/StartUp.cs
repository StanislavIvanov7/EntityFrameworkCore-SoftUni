using AutoMapper;
using Boardgames.Utilities;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();
            //1.
            //string input = File.ReadAllText("../../../Datasets/users.xml");
            //Console.WriteLine(ImportUsers(context, input));

            ////2
            //string input2 = File.ReadAllText("../../../Datasets/products.xml");
            //Console.WriteLine(ImportProducts(context, input2));

            ////3
            //string input3 = File.ReadAllText("../../../Datasets/categories.xml");
            //Console.WriteLine(ImportCategories(context, input3));

            ////4
            //string input4 = File.ReadAllText("../../../Datasets/categories-products.xml");
            //Console.WriteLine(ImportCategoryProducts(context, input4));
            //5
            //Console.WriteLine(GetProductsInRange(context));
            //6
            //Console.WriteLine(GetSoldProducts(context));
            //7
            //Console.WriteLine(GetCategoriesByProductsCount(context));
            //8
            Console.WriteLine(GetUsersWithProducts(context));
        }

        public static Mapper GetMapper()
        {
            var cnf = new MapperConfiguration(x => x.AddProfile<ProductShopProfile>());
            return new Mapper(cnf);
        }


        //1.
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var mapper = GetMapper();
            XmlParser xmlParser = new XmlParser();
            var userDtos = xmlParser.Deserialize<UserDTO[]>(inputXml, "Users");
            var users = mapper.Map<User[]>(userDtos);

            context.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Count()}";
        }

        //2
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var mapper = GetMapper();
            var parser = new XmlParser();
            ProductDTO[] productDtos = parser.Deserialize<ProductDTO[]>(inputXml, "Products");
            var products = mapper.Map<Product[]>(productDtos);
            context.AddRange(products);
            context.SaveChanges();
            return $"Successfully imported {products.Count()}";
        }

        //3
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var mapper = GetMapper();
            var xmlParser = new XmlParser();
            CategoriesDTO[] categoriesDTOs = xmlParser.Deserialize<CategoriesDTO[]>(inputXml, "Categories");
            var catogories = mapper.Map<Category[]>(categoriesDTOs.Where(x => x.Name is not null));

            context.AddRange(catogories);
            context.SaveChanges();
            return $"Successfully imported {catogories.Count()}";
        }
        //4
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var mapper = GetMapper();
            var parser = new XmlParser();
            CategoriesProductsDTO[] categoriesProductsDTOs = parser
                .Deserialize<CategoriesProductsDTO[]>(inputXml, "CategoryProducts");
            var currectCategoriesIds = context.Categories
                .Select(x => x.Id).ToArray();

            var currectProductIds = context.Products
                .Select(x => x.Id).ToArray();


            var categoriesProducts = mapper.Map<CategoryProduct[]>(categoriesProductsDTOs
                .Where(x => currectProductIds.Contains(x.ProductId) && currectCategoriesIds.Contains(x.CategoryId)));
            context.AddRange(categoriesProducts);
            context.SaveChanges();
            return $"Successfully imported {categoriesProducts.Count()}";
        }
        // 5.
        public static string GetProductsInRange(ProductShopContext context)
        {
            //Get all products in a specified price range between 500 and 1000 (inclusive).
            //Order them by price (from lowest to highest).
            //Select only the product name, price and the full name of the buyer. Take top 10 records.

            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(x => new ExportProductsInRange
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName

                })
                .OrderBy(x => x.Price)
                   .Take(10)
                .ToArray();

            return XmlParser.Serialize(products, "Products");
        }
        //6
        public static string GetSoldProducts(ProductShopContext context)
        {
            //Get all users who have at least 1 sold item. Order them by the last name, then by the first name.
            //Select the person's first and last name. For each of the sold products, select the product's name and price.
            //Take top 5 records. 
            var users = context.Users
                .Where(x => x.ProductsSold.Count() >= 1 && x.ProductsSold.Any(ps => ps.BuyerId != null))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new ExportSoldProductsWithUsers
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(x => new ExportSoldProducts
                    {
                        Name = x.Name,
                        Price = x.Price,

                    }).ToArray()



                })

                .Take(5)
                .ToArray();
            return XmlParser.Serialize(users, "Users");
        }
        //7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            //Get all categories. For each category select its name, the number of products,
            //the average price of those products and the total revenue (total price sum)
            //of those products (regardless if they have a buyer or not). Order them by the number
            //of products (descending), then by total revenue (ascending).
            var categories = context.Categories
                .Select(x => new ExportCategories
                {
                    Name = x.Name,
                    Count = x.CategoryProducts.Count,
                    Price = x.CategoryProducts.Average(x => x.Product.Price),
                    Revenue = x.CategoryProducts.Sum(x => x.Product.Price),


                }
                )
                .OrderByDescending (x=>x.Count)
                .ThenBy(x => x.Revenue)
                .ToArray();

                return XmlParser .Serialize (categories, "Categories");
        }
        //8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersInfo = context
               .Users
               .Where(u => u.ProductsSold.Any())
               .OrderByDescending(u => u.ProductsSold.Count)
               .Select(u => new UserInfo()
               {
                   FirstName = u.FirstName,
                   LastName = u.LastName,
                   Age = u.Age,
                   SoldProducts = new SoldProductsCount()
                   {
                       Count = u.ProductsSold.Count,
                       Products = u.ProductsSold.Select(p => new SoldProduct()
                       {
                           Name = p.Name,
                           Price = p.Price
                       })
                       .OrderByDescending(p => p.Price)
                       .ToArray()
                   }
               })
               .Take(10)
               .ToArray();

            ExportUserCountDto exportUserCountDto = new ExportUserCountDto()
            {
                Count = context.Users.Count(u => u.ProductsSold.Any()),
                Users = usersInfo
            };

        
            return XmlParser.Serialize<ExportUserCountDto>(exportUserCountDto, "Users");
         
           
        }
    }
}